﻿using SherpaDesk.Common;
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
            this.Model.DataLoading += UpdatedPage;
            var viewModel = this.DataContext as WorkListPageViewModel;
            viewModel.CommandExecuted += viewModel_CommandExecuted;
        }

        void viewModel_CommandExecuted(object sender, EventArgs e)
        {
            DetailsFrame.Navigated -= ChildPage_Navigated;
            DetailsFrame.Navigated += ChildPage_Navigated;
            DetailsFrame.Navigate(typeof(TicketDetails), sender.ToString());
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
            await Load();
        }

        protected async override void UpdatedPage(object sender, EventArgs e)
        {
            await Load();
        }

        private void ItemsGrid_SelectionChanged(object sender, Telerik.UI.Xaml.Controls.Grid.DataGridSelectionChangedEventArgs e)
        {
            foreach (var ticket in e.AddedItems)
            {
                ((TicketSearchResponse)ticket).IsChecked = true;
            }
            foreach (var ticket in e.RemovedItems)
            {
                ((TicketSearchResponse)ticket).IsChecked = false;
            }
        }

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

        private void GridTicketId_Click(object sender, RoutedEventArgs e)
        {
            DetailsFrame.Navigated -= ChildPage_Navigated;
            DetailsFrame.Navigated += ChildPage_Navigated;
            DetailsFrame.Navigate(typeof(TicketDetails), ((Button)sender).Tag.ToString());
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
                    this.Model.Data = new ObservableCollection<TicketSearchResponse>(result.Result.ToList());
                }
            }
        }

        private void GridCheckbox_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
