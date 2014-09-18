using SherpaDesk.Common;
using SherpaDesk.Extensions;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Core.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace SherpaDesk
{
    public sealed partial class WorkList : SherpaDesk.Common.LayoutAwarePage
    {
        private eWorkListType _workType = eWorkListType.Open;

        private IncrementalLoadingCollection<TicketSearchResponse> _data;

        private uint _pageSize = SearchRequest.DEFAULT_PAGE_COUNT;

        private uint _pageIndex = SearchRequest.DEFAULT_PAGE_INDEX;

        public WorkList()
        {
            this.InitializeComponent();
        }

        private async Task<IList<TicketSearchResponse>> Load()
        {
            using (var connector = new Connector())
            {
                TicketSearchRequest request = new TicketSearchRequest { PageIndex = (int)_pageIndex, PageCount = (int)_pageSize };
                switch (_workType)
                {
                    case eWorkListType.Open:
                        request.Status = eTicketStatus.Open;
                        //request.Role = eRoles.Technician;
                        break;
                    case eWorkListType.OnHold:
                        request.Status = eTicketStatus.OnHold;
                        break;
                    case eWorkListType.NewMessages:
                        request.Status = eTicketStatus.NewMessages;
                        request.Role = eRoles.Technician;
                        break;
                    case eWorkListType.OpenAsEndUser:
                        request.Status = eTicketStatus.Open | eTicketStatus.OnHold;
                        request.Role = eRoles.EndUser;
                        break;
                    case eWorkListType.AwaitingResponse:
                        request.Status = eTicketStatus.Waiting;
                        break;
                }

                var result = await connector.Func<TicketSearchRequest, TicketSearchResponse[]>(x => x.Tickets, request);
                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                    return new TicketSearchResponse[0].ToList();
                }
                else
                {
                    var list = result.Result;

                    _pageIndex++;

                    return list.ToList();
                }
            }

        }

        public async Task LoadStatInfo()
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

                OpenTicketsCount.Text = resultCounts.Result.AllOpen > 0 ? resultCounts.Result.AllOpen.ToString() : string.Empty;
                AsEndUserCount.Text = resultCounts.Result.OpenAsUser > 0 ? resultCounts.Result.OpenAsUser.ToString() : string.Empty;
                OnHoldCount.Text = resultCounts.Result.OnHold > 0 ? resultCounts.Result.OnHold.ToString() : string.Empty;

            }
        }

        private async void FillData(eWorkListType workType)
        {
            _pageIndex = SearchRequest.DEFAULT_PAGE_INDEX;

            _workType = workType;

            switch (_workType)
            {
                case eWorkListType.Open:
                    pageTitle.Text = "Open Tickets";
                    break;
                case eWorkListType.OnHold:
                    pageTitle.Text = "On Hold";
                    break;
                case eWorkListType.NewMessages:
                    pageTitle.Text = "New Messages";
                    break;
                case eWorkListType.OpenAsEndUser:
                    pageTitle.Text = "Open As End User";
                    break;
                case eWorkListType.AwaitingResponse:
                    pageTitle.Text = "Awaiting Response";
                    break;
            }
            _data = new IncrementalLoadingCollection<TicketSearchResponse>(async count =>
            {
                try
                {
                    return await Load();
                }
                catch
                {
                    return Enumerable.Empty<TicketSearchResponse>();
                }
            }) { BatchSize = _pageSize };

            try
            {
                var result = await Load();

                foreach (var item in result)
                {
                    _data.Add(item);
                }
            }
            catch
            {
            }

            ItemsGrid.ItemsSource = _data;
        }

        public void FullUpdate()
        {
            FillData(_workType);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                FillData((eWorkListType)e.Parameter);
            }
            base.OnNavigatedTo(e);
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStatInfo();
        }


        private void OpenTicketsButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FillData(eWorkListType.Open);
        }

        private void AsEndUserButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FillData(eWorkListType.OpenAsEndUser);
        }

        private void OnHoldButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FillData(eWorkListType.OnHold);
        }

        private void FollowUpDatesButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FillData(eWorkListType.NewMessages);
        }

        private void ItemsGrid_SelectionChanged(object sender, Telerik.UI.Xaml.Controls.Grid.DataGridSelectionChangedEventArgs e)
        {
            this.pageRoot.MainPage(page =>
            {
                page.WorkDetailsFrame.Visibility = Windows.UI.Xaml.Visibility.Visible;
                page.ScrollViewer.ChangeView(Constants.WIDTH_MAX_RIGHT, null, null);
                page.WorkDetailsFrame.Navigate(typeof(TicketDetails), ((TicketSearchResponse)e.AddedItems.First()).TicketKey);
            });
        }
    }
}
