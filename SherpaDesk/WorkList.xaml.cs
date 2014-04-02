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

namespace SherpaDesk
{
    public sealed partial class WorkList : SherpaDesk.Common.LayoutAwarePage
    {
        private eWorkListType _workType;
        public WorkList()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
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
            base.OnNavigatedTo(e);
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                TicketSearchRequest request = null;
                switch (_workType)
                {
                    case eWorkListType.Open:
                        request = new TicketSearchRequest
                        {
                            Status = eTicketStatus.Open,
                            Role = eRoles.Technician
                        };
                        break;
                    case eWorkListType.OnHold:
                        request = new TicketSearchRequest
                        {
                            Status = eTicketStatus.OnHold
                        };
                        break;
                    case eWorkListType.NewMessages:
                        request = new TicketSearchRequest
                        {
                            Role = eRoles.Technician, 
                            Status = eTicketStatus.NewMessages
                        };
                        break;
                    case eWorkListType.OpenAsEndUser:
                        request = new TicketSearchRequest
                        {
                            Role = eRoles.EndUser
                        };
                        break;
                    case eWorkListType.AwaitingResponse:
                        request = new TicketSearchRequest
                        {
                            Role = eRoles.EndUser,
                            Status = eTicketStatus.Waiting
                        };
                        break;
                }

                var result = await connector.Func<TicketSearchRequest, TicketSearchResponse[]>("tickets", request);
                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                    return;
                }
                ItemsGrid.ItemsSource = result.Result.ToList();
            }
        }

        private void ItemsGrid_SelectionChanged(object sender, Telerik.UI.Xaml.Controls.Grid.DataGridSelectionChangedEventArgs e)
        {
            var ticket = e.AddedItems.FirstOrDefault() as TicketSearchResponse;
            if (ticket != null)
                DetailsFrame.Navigate(typeof(TicketDetails), ticket.TicketKey);

        }

        private void GridCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (ItemsGrid.ItemsSource is IList<TicketSearchResponse>)
            {
                int ticketId; int.TryParse(((CheckBox)sender).Tag.ToString(), out ticketId);
                var item = ((IList<TicketSearchResponse>)ItemsGrid.ItemsSource).FirstOrDefault(x => x.TicketNumber == ticketId);
                if (item != null)
                {
                    ItemsGrid.SelectedItems.Add(item);
                }
            }
        }

        private void GridCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ItemsGrid.ItemsSource is IList<TicketSearchResponse>)
            {
                int ticketId; int.TryParse(((CheckBox)sender).Tag.ToString(), out ticketId);
                var item = ((IList<TicketSearchResponse>)ItemsGrid.ItemsSource).FirstOrDefault(x => x.TicketNumber == ticketId);
                if (item != null)
                {
                    ItemsGrid.SelectedItems.Remove(item);
                }
            }
        }

        private void MarkReadMenu_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //            ItemsGrid.SelectedItems has checked items
        }

        private void CloseMenu_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
        }

        private void DeleteMenu_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
        }

        private void GridTicketId_Click(object sender, RoutedEventArgs e)
        {
            DetailsFrame.Navigate(typeof(TicketDetails), ((Button)sender).Tag.ToString());
        }

        private void HeaderGridCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            ItemsGrid.SelectedItems.Clear();
            foreach (var item in ((IList<TicketSearchResponse>)ItemsGrid.ItemsSource))
            {
                item.IsChecked = true;
                ItemsGrid.SelectedItems.Add(item);
            }
        }

        private void HeaderGridCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            ItemsGrid.SelectedItems.Clear();
            foreach (var item in ((IList<TicketSearchResponse>)ItemsGrid.ItemsSource))
            {
                item.IsChecked = false;
            }
        }
    }
}
