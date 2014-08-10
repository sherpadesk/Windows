using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System.Linq;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace SherpaDesk
{
    public sealed partial class WorkList : SherpaDesk.Common.LayoutAwarePage
    {
        private eWorkListType _workType;

        public WorkList()
        {
            this.InitializeComponent();
        }

        void viewModel_CommandExecuted(object sender, EventArgs e)
        {
            //DetailsFrame.Navigated -= ChildPage_Navigated;
            //DetailsFrame.Navigated += ChildPage_Navigated;
            //DetailsFrame.Navigate(typeof(TicketDetails), sender.ToString());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                _workType = (eWorkListType)e.Parameter;
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
            }
            base.OnNavigatedTo(e);
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            this.Model.DataLoading += UpdatedPage;
            var viewModel = this.DataContext as WorkListPageViewModel;
            viewModel.CommandExecuted += viewModel_CommandExecuted;

            await LoadStatInfo();
            await Load();
        }

        protected async override void UpdatedPage(object sender, EventArgs e)
        {
            await Load();
        }

        //private void ItemsGrid_SelectionChanged(object sender, Telerik.UI.Xaml.Controls.Grid.DataGridSelectionChangedEventArgs e)
        //{
        //    foreach (var ticket in e.AddedItems)
        //    {
        //        ((TicketSearchResponse)ticket).IsChecked = true;
        //    }
        //    foreach (var ticket in e.RemovedItems)
        //    {
        //        ((TicketSearchResponse)ticket).IsChecked = false;
        //    }
        //}

        private async void ConfirmMenu_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (await App.ConfirmMessage())
            {
            }
        }

        private async void CloseMenu_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (!this.Model.Data.Any(x=>x.IsChecked))
            {
                App.ShowErrorMessage("No Items selected", eErrorType.Warning);
                return;
            }
            if (await App.ConfirmMessage())
            {
                using (var connector = new Connector())
                {
                    foreach (var ticket in this.Model.Data.ToList())
                    {
                        if (ticket.IsChecked)
                        {
                            var result = await connector.Action<CloseTicketRequest>(x => x.Tickets,
                                    new CloseTicketRequest(ticket.TicketKey));

                            if (result.Status != eResponseStatus.Success)
                            {
                                this.HandleError(result);
                                return;
                            }
                        }
                    }
                }
                await this.Load();

                App.ExternalAction(x => x.UpdateInfo());
            }
        }

        private void HeaderGridCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            this.Model.SelectAll(true);
            ItemsGrid.SelectAll();
        }

        private void HeaderGridCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Model.SelectAll(false);
            ItemsGrid.SelectedItems.Clear();
        }

        private void PageNext_Click(object sender, RoutedEventArgs e)
        {
            this.Model.PageNext();
        }

        private void PagePrev_Click(object sender, RoutedEventArgs e)
        {
            this.Model.PagePrev();
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

        public async Task Load()
        {
            using (var connector = new Connector())
            {
                TicketSearchRequest request = new TicketSearchRequest { PageIndex = this.Model.PageIndex };
                switch (_workType)
                {
                    case eWorkListType.Open:
                        request.Status = eTicketStatus.Open;
                        request.Role = eRoles.Technician;
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
                }
                else
                {
                    Style style = Resources["WorkListPaging"] as Style;
                    var list = result.Result.ToList();
                    //if (list.Count == 0)
                    //{
                    //    style.SetValue(Grid.VisibilityProperty, Visibility.Collapsed);
                    //}
                    //else
                    //{
                    //    style.SetValue(Grid.VisibilityProperty, Visibility.Visible);
                    //}
                    this.Model.Data = new WorkListViewData(result.Result.ToList());
                }
            }
        }

        private void GridCheckbox_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private async void OpenTicketsButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            _workType = eWorkListType.Open;
            await Load();
        }

        private async void AsEndUserButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            _workType = eWorkListType.OpenAsEndUser;
            await Load();
        }

        private async void OnHoldButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            _workType = eWorkListType.OnHold;
            await Load();
        }

        private async void FollowUpDatesButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            _workType = eWorkListType.NewMessages;
            await Load();
        }

        private void ItemsGrid_SelectionChanged(object sender, Telerik.UI.Xaml.Controls.Grid.DataGridSelectionChangedEventArgs e)
        {
            this.pageRoot.MainPage(page =>
                page.WorkDetailsFrame.Navigate(typeof(TicketDetails), ((TicketSearchResponse)e.AddedItems.First()).TicketKey));
        }
    }
}
