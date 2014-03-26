using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

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
                            Status = "open"
                        };
                        break;
                    case eWorkListType.OnHold:
                        request = new TicketSearchRequest
                        {
                            Status = "on_hold"
                        };
                        break;
                    case eWorkListType.NewMessages:
                        request = new TicketSearchRequest
                        {
                            UserId = AppSettings.Current.UserId
                        };
                        break;
                    case eWorkListType.OpenAsEndUser:
                        request = new TicketSearchRequest
                        {
                            UserId = AppSettings.Current.UserId,
                            Role = "user"
                        }; break;
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
            if (e.AddedItems.Count() > 0 && e.AddedItems.First() is TicketResponse)
            {
                var ticket = (TicketResponse)e.AddedItems.First();
//                DetailsFrame.Navigate(typeof(TicketDetails), ticket.TicketKey);
            }
        }
    }
}
