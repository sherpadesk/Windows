using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SherpaDesk
{
    public sealed partial class TicketDetails : SherpaDesk.Common.LayoutAwarePage
    {
        private string _ticketKey;
        public TicketDetails()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _ticketKey = (string)e.Parameter;
            base.OnNavigatedTo(e);
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            SubjectDecorate.Height = SubjectLabel.ActualHeight;
            using (var connector = new Connector())
            {
                var result = await connector.Func<KeyRequest, TicketDetailsResponse>("tickets", new KeyRequest(_ticketKey));

                if (result.Status == eResponseStatus.Success)
                {
                    var ticket = result.Result;
                    SubjectLabel.Text = ticket.Subject;
                    EndUserLabel.Text = ticket.UserFullName;
                    InitialPostLabel.Text = ticket.InitialPost;
                    WorkpadLabel.Text = Windows.Data.Html.HtmlUtilities.ConvertToText(ticket.Workpad);
                }
                else
                {
                    this.HandleError(result);
                }
            }
        }

        private void ToggleWorkpad()
        {
            if (WorkpadEditBox.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
            {
                WorkpadLabel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                WorkpadEditBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
                WorkpadSaveButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                WorkpadCancelButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                WorkpadButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                WorkpadLabel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                WorkpadEditBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                WorkpadSaveButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                WorkpadCancelButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                WorkpadButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        private void WorkpadButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ToggleWorkpad();
        }

        private void WorkpadSaveButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ToggleWorkpad();
        }

        private void WorkpadCancelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ToggleWorkpad();
        }
    }
}
