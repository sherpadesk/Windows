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
    public sealed partial class OpenTickets : SherpaDesk.Common.LayoutAwarePage
    {
        public OpenTickets()
        {
            this.InitializeComponent();
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var result = await connector.Func<TicketSearchRequest, TicketResponse[]>(
                    "tickets",
                    new TicketSearchRequest
                    {
                        UserId = AppSettings.Current.UserId
                    });
                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                    return;
                }
                itemListView.ItemsSource = result.Result.ToList();
            }
        }
    }
}
