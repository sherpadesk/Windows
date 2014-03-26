using SherpaDesk.Models;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;


namespace SherpaDesk.Common
{
    public static class PageExtensions
    {
        private const string TOOL_TIP_NAME = "ToolTip";
        public static async void HandleError(this Page page, Response response)
        {
            MessageDialog dialog = new MessageDialog(response.Message);
            if (response.Status == eResponseStatus.Invalid)
            {
                string messageWithoutControl = string.Empty;
                IDictionary<string, string> controls = new Dictionary<string, string>();
                foreach (var message in response.Messages)
                {
                    string[] keyPair = message.Split('#');
                    if (keyPair.Length == 2)
                    {
                        if (controls.ContainsKey(keyPair[1]))
                        {
                            controls[keyPair[1]] += Environment.NewLine + keyPair[0];
                        }
                        else
                        {
                            controls.Add(keyPair[1], keyPair[0]);
                        }
                    }
                    else
                    {
                        messageWithoutControl += message + Environment.NewLine;
                    }
                }
                foreach (var kv in controls)
                {
                    Control control = page.FindName(kv.Key) as Control;
                    if (control != null)
                    {

                        ToolTip toolTip = page.FindName(kv.Key + TOOL_TIP_NAME) as ToolTip;
                        if (toolTip == null)
                        {
                            toolTip = new ToolTip();
                            toolTip.Name = kv.Key + TOOL_TIP_NAME;
                            toolTip.Content = kv.Value;
                            toolTip.Foreground = new SolidColorBrush(Colors.Black);
                            toolTip.BorderThickness = new Thickness(0, 0, 0, 0);
                            toolTip.Background = new SolidColorBrush(Color.FromArgb(255, 242, 108, 108));
                            toolTip.Height = 40;
                            toolTip.Width = control.Width;
                            toolTip.FontSize = 16;
                            toolTip.FontWeight = FontWeights.Bold;
                            toolTip.Placement = Windows.UI.Xaml.Controls.Primitives.PlacementMode.Bottom;
                            toolTip.UseLayoutRounding = true;
                            toolTip.PlacementTarget = control;
                            toolTip.HorizontalAlignment = control.HorizontalAlignment;
                            toolTip.VerticalAlignment = control.VerticalAlignment;
                            var grid = control.ParentGrid();
                            if (grid == null)
                                ToolTipService.SetToolTip(control, toolTip);
                            else
                            {
                                var margin = control.Margin;
                                toolTip.Margin = margin;                                
                                Grid.SetRow(toolTip, Grid.GetRow(control));
                                Grid.SetColumn(toolTip, Grid.GetColumn(control));
                                grid.Children.Add(toolTip);
                            }
                        }
                        else
                        {
                            toolTip.Content = kv.Value;
                        }
                        toolTip.Visibility = Visibility.Visible;
                        var closing = new RoutedEventHandler((object sender, RoutedEventArgs e) =>
                        {
                            toolTip.Visibility = Visibility.Collapsed;
                        });
                        control.Unloaded += closing;
                        control.GotFocus += closing;
                        toolTip.PointerPressed += new PointerEventHandler((object sender, PointerRoutedEventArgs e) =>
                            {
                                toolTip.Visibility = Visibility.Collapsed;
                                control.Focus(FocusState.Pointer);
                            });
                    }
                }
                if (!string.IsNullOrEmpty(messageWithoutControl))
                {
                    dialog.Title = "Invalid input data";
                    dialog.Content = messageWithoutControl;
                    await dialog.ShowAsync();
                }
            }
            else if (response.Status == eResponseStatus.Fail)
            {
                dialog.Title = "Failed Operation";
                await dialog.ShowAsync();
            }
            else if (response.Status == eResponseStatus.Error)
            {
                //TODO: show the complex dialog with internal error message and descriptions from response.Messagess
                // It can has a possibility to send response object by email
                dialog.Title = "Internal Error";
                await dialog.ShowAsync();
            }
        }
    }
}
