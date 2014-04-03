using SherpaDesk.Models.Response;
using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml.Controls;

namespace SherpaDesk.Common
{
    public static class ComboBoxExtensions
    {
        public static void FillData(this ComboBox comboBox, IEnumerable<IKeyName> list, params IKeyName[] args)
        {
            var searchBox = new RadAutoCompleteBox
            {
                Watermark = "Search",
                ItemsSource = list.Select(x => x.Name),
                Width = comboBox.ActualWidth - 30,
                Height = comboBox.ActualHeight,
                FontSize = 18,
                FilterComparisonMode = System.StringComparison.CurrentCultureIgnoreCase,
                FilterMode = AutoCompleteBoxFilterMode.Contains,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center
            };
            searchBox.SelectionChanged += searchBox_SelectionChanged;
            comboBox.Items.Clear();
            comboBox.Items.Add(searchBox);
            foreach (var kv in args)
            {
                comboBox.Items.Add(new ComboBoxItem
                {
                    Tag = kv.Key,
                    Content = kv.Name
                });
            }
            foreach (var kv in list)
            {
                if (!args.Any(x => x.Key == kv.Key))
                {
                    comboBox.Items.Add(new ComboBoxItem
                    {
                        Tag = kv.Key,
                        Content = kv.Name
                    });
                }
            }
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }

        static void searchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selected = e.AddedItems.First().ToString();
                ((Windows.UI.Xaml.Controls.ComboBox)((Control)sender).Parent).SetSelectedValueByName(selected);
                ((RadAutoCompleteBox)sender).Text = string.Empty;
            }
        }

        public static T GetSelectedValue<T>(this ComboBox comboBox)
        {
            if (comboBox.SelectedIndex > -1)
            {
                var item = comboBox.Items[comboBox.SelectedIndex];
                if (item is ComboBoxItem)
                {
                    return (T)((ComboBoxItem)item).Tag;
                }
            }
            return default(T);
        }

        public static string GetSelectedText(this ComboBox comboBox)
        {
            if (comboBox.SelectedIndex > -1)
            {
                var item = comboBox.Items[comboBox.SelectedIndex];
                if (item is ComboBoxItem)
                {
                    return (string)((ComboBoxItem)item).Content;
                }
            }
            return string.Empty;
        }

        public static void SetSelectedValue(this ComboBox comboBox, object value)
        {
            if (value == null)
                return;
            int index = 0;
            foreach (var item in comboBox.Items)
            {
                if (item is ComboBoxItem && value.Equals(((ComboBoxItem)item).Tag))
                {
                    comboBox.SelectedIndex = index;
                    return;
                }
                index++;
            }
        }

        public static void SetSelectedValueByName(this ComboBox comboBox, object value)
        {
            if (value == null)
                return;
            int index = 0;
            foreach (var item in comboBox.Items)
            {
                if (item is ComboBoxItem && value.Equals(((ComboBoxItem)item).Content))
                {
                    comboBox.SelectedIndex = index;
                    return;
                }
                index++;
            }
        }
    }
}
