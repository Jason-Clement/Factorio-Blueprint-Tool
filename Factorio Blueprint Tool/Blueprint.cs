using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using zlib;

namespace Factorio_Blueprint_Tool
{
    public class Blueprint
    {
        private Blueprint() { }

        public static string Decode(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) return "";
            var enc = new UTF8Encoding();
            return enc.GetString(Decompress(Convert.FromBase64String(base64.Substring(1))));
        }
        
        public static string Encode(string json)
        {
            var enc = new UTF8Encoding();
            return "0" + Convert.ToBase64String(Compress(enc.GetBytes(json)));
        }

        private static byte[] Compress(byte[] inData)
        {
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream, zlibConst.Z_DEFAULT_COMPRESSION))
            using (Stream inMemoryStream = new MemoryStream(inData))
            {
                CopyStream(inMemoryStream, outZStream);
                outZStream.finish();
                return outMemoryStream.ToArray();
            }
        }

        private static byte[] Decompress(byte[] inData)
        {
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream))
            using (Stream inMemoryStream = new MemoryStream(inData))
            {
                CopyStream(inMemoryStream, outZStream);
                outZStream.finish();
                return outMemoryStream.ToArray();
            }
        }

        private static void CopyStream(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[2000];
            int len;
            while ((len = input.Read(buffer, 0, 2000)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }

        public static string Mirror(string json, bool vertical)
        {
            var o = JObject.Parse(json);
            var entities = o["blueprint"]["entities"];
            foreach (var entity in entities)
            {
                if (entity["direction"] == null)
                    entity["direction"] = 0;
                int d = entity["direction"].Value<int>();
                switch (d)
                {
                    case 0: d = vertical ? 4 : 0; break;
                    case 4: d = vertical ? 0 : 4; break;
                    case 2: d = vertical ? 2 : 6; break;
                    case 6: d = vertical ? 6 : 2; break;
                }
                entity["direction"] = d;

                if (vertical)
                {
                    entity["position"]["y"] = entity["position"]["y"].Value<int>() * -1;
                }
                else
                {
                    entity["position"]["x"] = entity["position"]["x"].Value<int>() * -1;
                }
            }
            return o.ToString(Formatting.None);
        }

        public static string ReplaceItems(string json, Dictionary<string, string> itemsToReplace, out string results)
        {
            var o = JObject.Parse(json);
            var entities = o["blueprint"]["entities"];
            var resultsD = new Dictionary<string, int>();
            foreach (var entity in entities)
            {
                foreach (var key in itemsToReplace.Keys)
                {
                    if (entity["name"].Value<string>() == key)
                    {
                        entity["name"] = itemsToReplace[key];
                        if (!resultsD.ContainsKey(key)) resultsD.Add(key, 0);
                        resultsD[key]++;
                    }
                }
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("The following items were replaced:");
            sb.AppendLine();
            foreach (var key in resultsD.Keys)
            {
                sb.AppendLine(key + ": " + resultsD[key].ToString());
            }
            results = sb.ToString();

            return o.ToString(Formatting.None);
        }

    }
}
