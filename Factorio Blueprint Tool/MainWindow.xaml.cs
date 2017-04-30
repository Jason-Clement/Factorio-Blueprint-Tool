using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Factorio_Blueprint_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                InputBox.Text = Clipboard.GetText().Trim();
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(InputBox.Text);
        }

        private void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OutputBox.Text = Blueprint.Decode(InputBox.Text);
            }
            catch (Exception ex)
            {
                OutputBox.Text = ex.Message;
            }
        }

        private void EncodeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InputBox.Text = Blueprint.Encode(OutputBox.Text);
            }
            catch (Exception ex)
            {
                InputBox.Text = ex.Message;
            }
        }

        private void HMirrorButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputBox.Text)) return;
            try
            {
                InputBox.Text = Blueprint.Encode(Blueprint.Mirror(Blueprint.Decode(InputBox.Text), false));
                OutputBox.Text = "Successfully mirrored vertically";
            }
            catch (Exception ex)
            {
                OutputBox.Text = ex.Message;
            }
        }

        private void VMirrorButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputBox.Text)) return;
            try
            {
                InputBox.Text = Blueprint.Encode(Blueprint.Mirror(Blueprint.Decode(InputBox.Text), true));
                OutputBox.Text = "Successfully mirrored vertically";
            }
            catch (Exception ex)
            {
                OutputBox.Text = ex.Message;
            }
        }

        private Dictionary<string, Dictionary<string, string>> ItemsToReplace = new Dictionary<string, Dictionary<string, string>>()
        {
            { "transport-belt", new Dictionary<string, string>() {
                { "fast-transport-belt", "transport-belt" },
                { "express-transport-belt", "transport-belt" },
                { "fast-underground-belt", "underground-belt" },
                { "express-underground-belt", "underground-belt" },
                { "fast-splitter", "splitter" },
                { "express-splitter", "splitter" }, } },
            { "fast-transport-belt", new Dictionary<string, string>() {
                { "transport-belt", "fast-transport-belt" },
                { "express-transport-belt", "fast-transport-belt" },
                { "underground-belt", "fast-underground-belt" },
                { "express-underground-belt", "fast-underground-belt" },
                { "splitter", "fast-splitter" },
                { "express-splitter", "fast-splitter" }, } },
            { "express-transport-belt", new Dictionary<string, string>() {
                { "transport-belt", "express-transport-belt" },
                { "fast-transport-belt", "express-transport-belt" },
                { "underground-belt", "express-underground-belt" },
                { "fast-underground-belt", "express-underground-belt" },
                { "splitter", "express-splitter" },
                { "fast-splitter", "express-splitter" }, } },
            { "assembling-machine-1", new Dictionary<string, string>() {
                { "assembling-machine-2", "assembling-machine-1" },
                { "assembling-machine-3", "assembling-machine-1" }, } },
            { "assembling-machine-2", new Dictionary<string, string>() {
                { "assembling-machine-1", "assembling-machine-2" },
                { "assembling-machine-3", "assembling-machine-2" }, } },
            { "assembling-machine-3", new Dictionary<string, string>() {
                { "assembling-machine-1", "assembling-machine-3" },
                { "assembling-machine-2", "assembling-machine-3" }, } },
            { "stone-furnace", new Dictionary<string, string>() {
                { "steel-furnace", "stone-furnace" }, } },
            { "steel-furnace", new Dictionary<string, string>() {
                { "stone-furnace", "steel-furnace" }, } },
            { "inserter", new Dictionary<string, string>() {
                { "fast-inserter", "inserter" },
                { "stack-inserter", "inserter" }, } },
            { "fast-inserter", new Dictionary<string, string>() {
                { "inserter", "fast-inserter" },
                { "stack-inserter", "fast-inserter" }, } },
            { "stack-inserter", new Dictionary<string, string>() {
                { "inserter", "stack-inserter" },
                { "fast-inserter", "stack-inserter" }, } },
            { "filter-inserter", new Dictionary<string, string>() {
                { "stack-filter-inserter", "filter-inserter" }, } },
            { "stack-filter-inserter", new Dictionary<string, string>() {
                { "filter-inserter", "stack-filter-inserter" }, } },
        };

        private void ItemButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = System.IO.Path.GetFileNameWithoutExtension((button.Content as Image).Source.ToString());
            try
            {
                var s = Blueprint.Decode(InputBox.Text);
                if (ItemsToReplace.ContainsKey(item))
                {
                    string results;
                    InputBox.Text = Blueprint.Encode(Blueprint.ReplaceItems(s, ItemsToReplace[item], out results));
                    OutputBox.Text = results;
                }
            }
            catch (Exception ex)
            {
                OutputBox.Text = ex.Message;
            }
        }
    }
}
