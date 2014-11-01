using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SherpaDesk.Common;
using SherpaDesk.Interfaces;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using Telerik.UI.Xaml.Controls.Input;

namespace SherpaDesk.Extensions
{
    public static class ComboBoxExtensions
    {
        public const string NONE = "None";

        public static void AutoComplete(this ComboBox comboBox, Func<RadAutoCompleteBox, Task> searchFunc, Action<IKeyName> selectedFunc = null)
        {
            var grid = comboBox.ParentGrid();
            if (grid == null) return;

            comboBox.Visibility = Visibility.Collapsed;

            var searchBox = new RadAutoCompleteBox
            {
                Name = comboBox.Name + "_Text",
                Watermark = "Search",
                Width = comboBox.ActualWidth,
                Height = comboBox.ActualHeight,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 18,
                FilterComparisonMode = StringComparison.CurrentCultureIgnoreCase,
                FilterMode = AutoCompleteBoxFilterMode.Contains,
                FilterDelay = TimeSpan.FromSeconds(1),
                FilterStartThreshold = 2,
                IsDropDownOpen = true,
                BorderBrush = comboBox.BorderBrush,
                AutosuggestFirstItem = false,
                CompositeMode = Windows.UI.Xaml.Media.ElementCompositeMode.SourceOver,
                Visibility = Visibility.Visible,
                ItemTemplate =
                    (DataTemplate)
                        ((ResourceDictionary)Application.Current.Resources["CommonResources"])["AutoCompleteItemTemplate"]
            };
            searchBox.TextChanged += async (obj, args) =>
                {
                    await searchFunc((RadAutoCompleteBox)obj);
                };
            searchBox.SelectionChanged += (sender, e) =>
                {
                    var tb = e.AddedItems.FirstOrDefault() as IKeyName;
                    if (tb != null)
                    {
                        searchBox.Tag = tb.Key;
                        if (selectedFunc != null)
                        {
                            selectedFunc(tb);
                        }
                    }
                };
            searchBox.Margin = new Thickness(comboBox.Margin.Left, comboBox.Margin.Top, comboBox.Margin.Right, comboBox.Margin.Bottom);
            searchBox.Padding = new Thickness(8, 5, 0, 0);

            Grid.SetRow(searchBox, Grid.GetRow(comboBox));
            Grid.SetColumn(searchBox, Grid.GetColumn(comboBox));

            grid.Children.Add(searchBox);

            //comboBox.Items.Clear();
            //comboBox.Items.Add(searchBox);
            //comboBox.DropDownOpened += (object sender, object e) => 
            //{
            //    searchBox.Focus(FocusState.Pointer);
            //};
        }

        public async static Task Search(this RadAutoCompleteBox searchBox, bool tech)
        {
            using (var connector = new Connector())
            {
                searchBox.FilterMemberPath = "Name";
                if (searchBox.Text.Trim().Length > 2)
                {
                    var result = await connector.Func<SearchRequest, UserResponse[]>(x => tech ? x.Technicians : x.Users,
                        tech ? ((SearchRequest)new TechniciansRequest { Query = searchBox.Text }) : new UserSearchRequest { Query = searchBox.Text });

                    if (result.Status != eResponseStatus.Success)
                    {
                        App.ShowErrorMessage(result.Message, eErrorType.FailedOperation);
                    }
                    else
                    {
                        searchBox.ItemsSource = result.Result.Select(user => new NameResponse { Id = user.Id, Name = user.FullName }).ToList();
                    }
                }
            }
        }

        public static void FillData(this ComboBox comboBox, IEnumerable<IKeyName> list, params IKeyName[] args)
        {
            if (comboBox.Items != null)
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
                    if (args.All(x => x.Key != kv.Key))
                    {
                        comboBox.Items.Add(new ComboBoxItem
                        {
                            Tag = kv.Key,
                            Content = kv.Name
                        });
                    }
                }
                if (comboBox.Items.Count == 0)
                {
                    comboBox.Items.Add(new ComboBoxItem
                    {
                        Tag = 0,
                        Content = NONE
                    });
                }
            }
            comboBox.SelectedIndex = 0;
        }

        public static T GetSelectedValue<T>(this ComboBox comboBox, T defaultValue = default(T))
        {
            if (comboBox.Visibility == Visibility.Visible)
            {
                if (comboBox.SelectedIndex <= -1) return defaultValue;

                if (comboBox.Items == null) return defaultValue;

                var item = comboBox.Items[comboBox.SelectedIndex];

                var comboBoxItem = item as ComboBoxItem;

                return comboBoxItem != null ? (T)comboBoxItem.Tag : defaultValue;
            }

            var textBox = comboBox.ParentGrid().FindName(comboBox.Name + "_Text") as RadAutoCompleteBox;

            return textBox != null && textBox.Tag != null ? (T)textBox.Tag : defaultValue;
        }

        public static string GetSelectedText(this ComboBox comboBox)
        {
            if (comboBox.SelectedIndex <= -1) return string.Empty;

            if (comboBox.Items == null) return string.Empty;

            var item = comboBox.Items[comboBox.SelectedIndex];

            var comboBoxItem = item as ComboBoxItem;

            return comboBoxItem != null ? (string)comboBoxItem.Content : string.Empty;
        }

        public static void SetSelectedValue(this ComboBox comboBox, object value)
        {
            if (value == null) return;
            var index = 0;

            if (comboBox.Items == null) return;

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

        public static void SetDefaultValue(this ComboBox comboBox)
        {
            if (comboBox != null) comboBox.SelectedIndex = comboBox.Items != null && comboBox.Items.Count > 0 ? 0 : -1;
        }

        public static void SetSelectedValueByName(this ComboBox comboBox, object value)
        {
            if (value == null) return;

            var index = 0;

            if (comboBox.Items != null)
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
