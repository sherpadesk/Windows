using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SherpaDesk
{
    public sealed partial class WorkList : SherpaDesk.Common.LayoutAwarePage
    {
        private WorkListEnum workType;
        public WorkList()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            workType = (WorkListEnum)e.Parameter;
            switch (workType)
            {
                case WorkListEnum.Open:
                    pageTitle.Text = "Open Tickets";
                    break;
                case WorkListEnum.OnHold:
                    pageTitle.Text = "On Hold";
                    break;
                case WorkListEnum.NewMessages:
                    pageTitle.Text = "New Messages";
                    break;
                case WorkListEnum.OpenAsEndUser:
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
                switch (workType)
                {
                    case WorkListEnum.Open:
                        request = new TicketSearchRequest
                        {
                            UserId = AppSettings.Current.UserId,
                            Status = "Open"
                        };
                        break;
                    case WorkListEnum.OnHold:
                        request = new TicketSearchRequest
                        {
                            UserId = AppSettings.Current.UserId,
                            Status = "OnHold"
                        };
                        break;
                    case WorkListEnum.NewMessages:
                        request = new TicketSearchRequest
                        {
                            UserId = AppSettings.Current.UserId
                        };
                        break;
                    case WorkListEnum.OpenAsEndUser:
                        request = new TicketSearchRequest
                        {                            
                            UserId = AppSettings.Current.UserId
                        }; break;
                }

                var result = await connector.Func<TicketSearchRequest, TicketResponse[]>("tickets", request);
                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                    return;
                }
                ItemsGrid.ItemsSource = result.Result.ToList();
            }
        }
    }
}
