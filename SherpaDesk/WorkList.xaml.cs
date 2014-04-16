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

namespace SherpaDesk
{
    public sealed partial class WorkList : SherpaDesk.Common.LayoutAwarePage
    {
        private eWorkListType _workType;

        public WorkList()
        {
            this.InitializeComponent();
            this.Model.DataLoading += Load;
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

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            Load(this.Model, EventArgs.Empty);
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

        private void MarkReadMenu_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }

        private void CloseMenu_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
        }

        private void DeleteMenu_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
        }

        private void GridTicketId_Click(object sender, RoutedEventArgs e)
        {
            DetailsFrame.Navigated += (object s, NavigationEventArgs a) =>
            {
                if (((ContentControl)s).Content is TicketDetails)
                {
                    ((TicketDetails)((ContentControl)s).Content).UpdateTicketListEvent -= Load;
                    ((TicketDetails)((ContentControl)s).Content).UpdateTicketListEvent += Load;
                }
            };
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

        public async void Load(object sender, EventArgs e)
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
                        request.Role = eRoles.Technician;
                        break;
                    case eWorkListType.NewMessages:
                        request.Role = eRoles.Technician;
                        request.Status = eTicketStatus.NewMessages;
                        break;
                    case eWorkListType.OpenAsEndUser:
                        request.Status = eTicketStatus.Open;
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

    }
}
