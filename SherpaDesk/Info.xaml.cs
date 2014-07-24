﻿using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Linq;
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


        public async void RefreshData()
        {
            using (var connector = new Connector())
            {
                var resultCounts = await connector.Func<KeyRequest, TicketCountsResponse>(
                    x => x.Tickets, new KeyRequest("counts"));
                if (resultCounts.Status != eResponseStatus.Success)
                {
                    this.pageRoot.HandleError(resultCounts);
                    return;
                }
                OpenCount.Text = resultCounts.Result.AllOpen.ToString();
                OpenAsTechCount.Text = resultCounts.Result.OpenAsTech.ToString();
                OpenAsEndUserCount.Text = resultCounts.Result.OpenAsUser.ToString();
                OpenAsAltTechCount.Text = resultCounts.Result.OpenAsAltTech.ToString();

                var resultStat = await connector.Func<SearchRequest, AccountStatResponse[]>(
                    x => x.Accounts, new SearchRequest("account_statistics.ticket_counts.open>0"));
                if (resultCounts.Status != eResponseStatus.Success)
                {
                    this.pageRoot.HandleError(resultCounts);
                    return;
                }
                StatInfoList.ItemsSource = resultStat
                    .Result
                    .Where(x => x.StatInfo.TicketCounts.Open > 0)
                    .Select(x => new
                    {
                        AccountName = x.Name,
                        OpenTickets = x.StatInfo.TicketCounts.Open,
                        UninvoicedTimes = x.StatInfo.TimeLogs,
                        UnInvoicedExpenses = x.StatInfo.Invoices
                    });
            }
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {

            this.ActivityFrame.Navigate(typeof(Activity));
            this.RightFrame.Navigate(typeof(WorkList), eWorkListType.Open);
            this.RefreshData();
        }

        private void NewMessagesTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(WorkList), eWorkListType.NewMessages);
            //            scrollViewer.ChangeView(0, new double?(), new float?());
        }

        private void OpenTicketsTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(WorkList), eWorkListType.Open);
            //            scrollViewer.ChangeView(0, new double?(), new float?());
        }

        private void OpenAsEndUserTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(WorkList), eWorkListType.OpenAsEndUser);
            //            scrollViewer.ChangeView(0, new double?(), new float?());
        }

        private void OnHoldTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(WorkList), eWorkListType.OnHold);
            //            scrollViewer.ChangeView(0, new double?(), new float?());
        }

        //private void FollowUpDatesTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        //{
        //    this.LeftFrame.Navigate(typeof(FollowUpDates));
        //    scrollViewer.ChangeView(0, new double?(), new float?());
        //}

        private void WaitingTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(WorkList), eWorkListType.AwaitingResponse);
            //            scrollViewer.ChangeView(0, new double?(), new float?());
        }

        private void AddTicketTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.RightFrame.Navigate(typeof(AddTicket));
            //            scrollViewer.ChangeView(20000, new double?(), new float?());
        }

        private void AddTimeTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.RightFrame.Navigate(typeof(AddTime));
            //            scrollViewer.ChangeView(20000, new double?(), new float?());
        }

        private void TimesheetTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.RightFrame.Loaded += RightFrame_Loaded;
            this.RightFrame.Navigate(typeof(Timesheet));
            //            scrollViewer.ChangeView(20000, new double?(), new float?());
        }

        void RightFrame_Loaded(object sender, RoutedEventArgs e)
        {
            if (((ContentControl)sender).Content is Timesheet)
            {
                ((Timesheet)((ContentControl)sender).Content).MoveScrollToRight += TimeSheetClicked;
            }
            this.RightFrame.Loaded -= RightFrame_Loaded;
        }

        void TimeSheetClicked(object sender, EventArgs e)
        {
            //            scrollViewer.ChangeView(20000, new double?(), new float?());
        }

        private void TimeButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.LeftFrame.Navigate(typeof(Timesheet));
        }
    }
}
