using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SherpaDesk
{
    public sealed partial class Info : SherpaDesk.Common.LayoutAwarePage
    {
        public Info()
        {
            this.InitializeComponent();
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var result = await connector.Func<KeyRequest, TicketCountsResponse>(
                    "tickets", new KeyRequest("counts"));
                if (result.Status == eResponseStatus.Success)
                {
                    NewMessagesCount.Text = result.Result.NewMessages.ToString();
                    OpenTicketsCount.Text = result.Result.OpenAsTech.ToString();
                    OpenAsEndUserCount.Text = result.Result.OpenAsUser.ToString();
                    OnHoldCount.Text = result.Result.OnHold.ToString();
                    //TODO: make awaiting count of tickets
                    WaitingCount.Text = result.Result.NewMessages.ToString();
                    //FollowUpDatesCount.Text = result.Result.Reminder.ToString();
                }
                else
                    this.pageRoot.HandleError(result);
            }
        }

        private void NewMessagesTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(WorkList), eWorkListType.NewMessages);
            scrollViewer.ChangeView(0, new double?(), new float?());
        }

        private void OpenTicketsTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(WorkList), eWorkListType.Open);
            scrollViewer.ChangeView(0, new double?(), new float?());
        }

        private void OpenAsEndUserTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(WorkList), eWorkListType.OpenAsEndUser);
            scrollViewer.ChangeView(0, new double?(), new float?());
        }

        private void OnHoldTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(WorkList), eWorkListType.OnHold);
            scrollViewer.ChangeView(0, new double?(), new float?());
        }

        //private void FollowUpDatesTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        //{
        //    this.LeftFrame.Navigate(typeof(FollowUpDates));
        //    scrollViewer.ChangeView(0, new double?(), new float?());
        //}

        private void WaitingTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(WorkList), eWorkListType.AwaitingResponse);
            scrollViewer.ChangeView(0, new double?(), new float?());
        }

        private void AddTicketTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.RightFrame.Navigate(typeof(AddTicket));
            scrollViewer.ChangeView(2000, new double?(), new float?());
        }

        private void AddTimeTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.RightFrame.Navigate(typeof(AddTime));
            scrollViewer.ChangeView(2000, new double?(), new float?());
        }

        private void TimesheetTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.RightFrame.Navigate(typeof(Timesheet));
            scrollViewer.ChangeView(2000, new double?(), new float?());
        }
    }
}
