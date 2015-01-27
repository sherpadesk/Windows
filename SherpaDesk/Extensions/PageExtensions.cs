using SherpaDesk.Models;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SherpaDesk.Extensions
{
    public static class PageExtensions
    {
        private const string TOOL_TIP_NAME = "ToolTip";

        public static async Task HandleValidators(this UserControl page, params string[] messages)
        {
            string messageWithoutControl = string.Empty;
            IDictionary<string, string> controls = new Dictionary<string, string>();
            foreach (var message in messages)
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
                var control = page.FindName(kv.Key) as FrameworkElement;
                if (control != null)
                {

                    var toolTip = page.FindName(kv.Key + TOOL_TIP_NAME) as ToolTip;
                    var grid = control.ParentGrid();
                    if (toolTip == null)
                    {
                        toolTip = new ToolTip
                        {
                            Name = kv.Key + TOOL_TIP_NAME,
                            Content = kv.Value,
                            Foreground = new SolidColorBrush(Colors.Black),
                            BorderThickness = new Thickness(0, 0, 0, 0),
                            Background = new SolidColorBrush(Color.FromArgb(230, 242, 108, 108)),
                            FontSize = 16,
                            Height = control.Height - 10,
                            Width = control.Width - 40,
                            PlacementTarget = control
                        };
                        if (grid == null)
                            ToolTipService.SetToolTip(control, toolTip);
                        else
                        {
                            toolTip.Margin = new Thickness(control.Margin.Left, control.Margin.Top + 10, control.Margin.Right, control.Margin.Bottom);
                            Grid.SetRow(toolTip, Grid.GetRow(control));
                            Grid.SetRowSpan(toolTip, Grid.GetRowSpan(control));
                            Grid.SetColumn(toolTip, Grid.GetColumn(control));
                            Grid.SetColumnSpan(toolTip, Grid.GetColumnSpan(control));
                            grid.Children.Add(toolTip);
                        }
                    }
                    else
                    {
                        toolTip.Content = kv.Value;
                    }
                    toolTip.Visibility = Visibility.Visible;
                    var closing = new RoutedEventHandler((sender, e) =>
                    {
                        toolTip.Visibility = Visibility.Collapsed;
                    });
                    control.Unloaded += closing;
                    control.GotFocus += closing;
                    toolTip.PointerPressed += (sender, e) =>
                    {
                        toolTip.Visibility = Visibility.Collapsed;
                        var control1 = control as Control;
                        if (control1 != null)
                            control1.Focus(FocusState.Pointer);
                    };
                }
            }
            if (!string.IsNullOrEmpty(messageWithoutControl))
            {
                await App.ShowStandartMessage(messageWithoutControl, eErrorType.InvalidInputData);
            }
        }

        public static async Task HandleError(this UserControl page, Response response)
        {
            App.ExternalAction(x => x.StopProgress());

            if (response.Status == eResponseStatus.Invalid)
            {
                await page.HandleValidators(response.Messages.ToArray());
            }
            else if (response.Status == eResponseStatus.Fail)
            {
                await App.ShowErrorMessage(response.Message, eErrorType.Warning);
            }
            else if (response.Status == eResponseStatus.Error)
            {
                await App.ShowErrorMessage(response.Message, eErrorType.Error);
            }
        }


        //public static async void Handle(this Page page, Action handling)
        //{
        //string title = string.Empty, message = string.Empty;
        //try
        //{
        //    handling();
        //}
        //catch (InternalException e)
        //{
        //    title = e.Title;
        //    message = e.Message;
        //}
        //catch (Exception e)
        //{
        //    title = eExceptionType.Error.Details();
        //    message = e.Message;
        //}
        //if (!string.IsNullOrEmpty(message))
        //{
        //    await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        //        {
        //            var md = new MessageDialog(message, title);
        //            await md.ShowAsync();
        //        });
        //}
        //Task.Factory
        //    .StartNew(handling)
        //    .ContinueWith(antecedent =>
        //    {
        //        if (antecedent.Status == TaskStatus.Faulted)
        //        {
        //            string title = "Error";
        //            if (antecedent.Exception.InnerException is InternalException)
        //                title = ((InternalException)antecedent.Exception.InnerException).Title;

        //            var task = CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        //            {
        //                var md = new MessageDialog(antecedent.Exception.InnerException.Message, title);
        //                await md.ShowAsync();
        //            }).AsTask();
        //            task.Start();
        //            task.Wait();
        //        }
        //    }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
        //}
    }
}
