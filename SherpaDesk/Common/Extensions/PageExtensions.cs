using SherpaDesk.Models;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;


namespace SherpaDesk.Common
{
    public static class PageExtensions
    {
        private const string TOOL_TIP_NAME = "ToolTip";
        public static void HandleError(this Page page, Response response)
        {
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
                    FrameworkElement control = page.FindName(kv.Key) as FrameworkElement;
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
                            toolTip.Background = new SolidColorBrush(Color.FromArgb(230, 242, 108, 108));
                            toolTip.FontSize = 16;
                            toolTip.Height = control.Height - 10;
                            toolTip.Width = control.Width;
                            toolTip.PlacementTarget = control;
                            var grid = control.ParentGrid();
                            if (grid == null)
                                ToolTipService.SetToolTip(control, toolTip);
                            else
                            {
                                toolTip.Margin = new Thickness(control.Margin.Left, control.Margin.Top + 10, control.Margin.Right, control.Margin.Bottom);
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
                            if (control is Control)
                                ((Control)control).Focus(FocusState.Pointer);
                        });
                    }
                }
                if (!string.IsNullOrEmpty(messageWithoutControl))
                {
                    App.ShowErrorMessage(messageWithoutControl, eErrorType.InvalidInputData);
                }
            }
            else if (response.Status == eResponseStatus.Fail)
            {
                App.ShowErrorMessage(response.Message, eErrorType.FailedOperation);
            }
            else if (response.Status == eResponseStatus.Error)
            {
                //TODO: show the complex dialog with internal error message and descriptions from response.Messagess
                // It can has a possibility to send response object by email
                App.ShowErrorMessage(response.Message, eErrorType.InternalError);
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
