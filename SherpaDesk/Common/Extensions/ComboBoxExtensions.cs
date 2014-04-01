using SherpaDesk.Models.Response;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace SherpaDesk.Common
{
    public static class ComboBoxExtensions
    {
        public static void FillData(this ComboBox comboBox, IEnumerable<IKeyName> list, params IKeyName[] args)
        {
            comboBox.Items.Clear();
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
        }

        public static T GetSelectedValue<T>(this ComboBox comboBox)
        {
            if (comboBox.SelectedIndex > -1)
            {
                return (T)((ComboBoxItem)comboBox.Items[comboBox.SelectedIndex]).Tag;
            }
            else
                return default(T);
        }

        public static string GetSelectedText(this ComboBox comboBox)
        {
            if (comboBox.SelectedIndex > -1)
            {
                return (string)((ComboBoxItem)comboBox.Items[comboBox.SelectedIndex]).Content;
            }
            else
                return string.Empty;
        }

        public static void SetSelectedValue(this ComboBox comboBox, object value)
        {
            if (value == null)
                return;
            int index = 0;
            foreach (var item in comboBox.Items)
            {
                if (value.Equals(((ComboBoxItem)item).Tag))
                {
                    comboBox.SelectedIndex = index;
                    return;
                }
                index++;
            }
        }

    }
}
