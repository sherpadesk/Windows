using SherpaDesk.Common;
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
    public sealed partial class Info : SherpaDesk.Common.LayoutAwarePage
    {
        public Info()
        {
            this.InitializeComponent();
        }

        public async void Refresh()
        {
            using (var connector = new Connector())
            {
                var result = await connector.Operation<TicketCountsResponse>(
                    "tickets/counts");
                if (result.Status == eResponseStatus.Success)
                {
                    NewMessagesLink.Content = result.Result.NewMessages;
                    OpenTicketsLink.Content = result.Result.AllOpen;
                    OpenAsEndUserLink.Content = result.Result.OpenAsUser;
                    OnHoldLink.Content = result.Result.OnHold;
                    FollowUpDatesLink.Content = result.Result.Reminder;
                }
                else
                    this.pageRoot.HandleError(result);
            }
        }

        private void NewMessagesLink_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NewMessages));
        }

        private void OpenTicketsLink_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(OpenTickets));
        }

        private void OpenAsEndUserLink_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(OpenAsEndUser));
        }

        private void OnHoldLink_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(OnHold));
        }

        private void FollowUpDatesLink_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FollowUpDates));
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            this.Refresh();
        }
    }
}
