using SherpaDesk.Common;
using SherpaDesk.Extensions;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Linq;
using System.Threading.Tasks;
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


        public async Task RefreshData()
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

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            this.ActivityFrame.Navigate(typeof(Activity));
            await this.RefreshData();
            //this.MainPage(page => page.ScrollViewer.ChangeView(Constants.WIDTH_TIMESHEET, null, null));
        }

        private void OpenWorkList(eWorkListType type)
        {
            this.MainPage(page =>
            {
                page.WorkListFrame.Navigate(typeof(WorkList), type);
                page.ScrollViewer.ChangeView(Constants.WIDTH_TIMESHEET + Constants.WIDTH_INFO, null, null);
            });
        }

        private void NewMessagesTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            OpenWorkList(eWorkListType.NewMessages);
        }

        private void OpenTicketsTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            OpenWorkList(eWorkListType.Open);
        }

        private void OpenAsEndUserTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            OpenWorkList(eWorkListType.OpenAsEndUser);
        }

        private void OnHoldTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            OpenWorkList(eWorkListType.OnHold);
        }

        private void WaitingTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            OpenWorkList(eWorkListType.AwaitingResponse);
        }

        private void TimeButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.MainPage(page =>
            {
                page.TimeSheetFrame.Navigate(typeof(Timesheet));
                page.TimeSheetFrame.Visibility = Windows.UI.Xaml.Visibility.Visible;
                page.ScrollViewer.ChangeView(0, null, null);
            });
        }

        private void TicketButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.MainPage(page =>
            {
                page.TimeSheetFrame.Navigate(typeof(AddTicket));
                page.TimeSheetFrame.Visibility = Windows.UI.Xaml.Visibility.Visible;
                page.ScrollViewer.ChangeView(0, null, null);
            });
        }

        private void LogOutMenu_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            App.LogOut();
        }

        private void SherpaDeskLink_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Windows.System.Launcher.LaunchUriAsync(new Uri("http://www.sherpadesk.com"));
        }
    }
}
