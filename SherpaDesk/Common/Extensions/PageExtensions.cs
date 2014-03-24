﻿using SherpaDesk.Models;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
                            toolTip.Content = kv.Value;
                            toolTip.Foreground = new SolidColorBrush(Colors.Black);
                            toolTip.BorderThickness = new Thickness(0, 0, 0, 0);
                            toolTip.Background = new SolidColorBrush(Color.FromArgb(230, 242, 108, 108));
                            toolTip.FontSize = 20;
                            toolTip.HorizontalContentAlignment = HorizontalAlignment.Right;
                            toolTip.Height -= 5;
                            toolTip.Width = control.Width;
                            toolTip.Placement = Windows.UI.Xaml.Controls.Primitives.PlacementMode.Top;
                            toolTip.VerticalOffset = -control.Height;
                            ToolTipService.SetToolTip(control, toolTip);
                        }
                        else
                        {
                            toolTip.Content = kv.Value;
                        }
                        toolTip.IsOpen = true;
                        control.Unloaded += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
                        {
                            toolTip.IsOpen = false;
                            toolTip.Content = string.Empty;
                            toolTip.Background = new SolidColorBrush(Colors.Transparent);
                        });
                        control.GotFocus += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
                        {
                            toolTip.IsOpen = false;
                            toolTip.Content = string.Empty;
                            toolTip.Background = new SolidColorBrush(Colors.Transparent);
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
