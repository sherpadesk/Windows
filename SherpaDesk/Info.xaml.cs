using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Response;
using Windows.UI.Xaml;

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
