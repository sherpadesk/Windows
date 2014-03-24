﻿using SherpaDesk.Common;
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
                var result = await connector.Func<TicketCountsResponse>(
                    "tickets/counts");
                if (result.Status == eResponseStatus.Success)
                {
                    NewMessagesCount.Text = result.Result.NewMessages.ToString();
                    OpenTicketsCount.Text = result.Result.AllOpen.ToString();
                    OpenAsEndUserCount.Text = result.Result.OpenAsUser.ToString();
                    OnHoldCount.Text = result.Result.OnHold.ToString();
                    FollowUpDatesCount.Text = result.Result.Reminder.ToString();
                }
                else
                    this.pageRoot.HandleError(result);
            }
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            this.Refresh();
        }

        private void NewMessagesTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(NewMessages));
            scrollViewer.ChangeView(0, new double?(), new float?());
        }

        private void OpenTicketsTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(OpenTickets));
            scrollViewer.ChangeView(0, new double?(), new float?());
        }

        private void OpenAsEndUserTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(OpenAsEndUser));
            scrollViewer.ChangeView(0, new double?(), new float?());
        }

        private void OnHoldTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(OnHold));
            scrollViewer.ChangeView(0, new double?(), new float?());
        }

        private void FollowUpDatesTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(FollowUpDates));
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

        private void EndOfDayTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.RightFrame.Navigate(typeof(EndOfDay));
            scrollViewer.ChangeView(2000, new double?(), new float?());
        }
    }
}
