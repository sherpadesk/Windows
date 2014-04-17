using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SherpaDesk
{
    public sealed partial class Transfer : SherpaDesk.Common.LayoutAwarePage
    {
        private string _ticketKey;

        public Transfer()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _ticketKey = (string)e.Parameter;
            base.OnNavigatedTo(e);
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var resultTechnicians = await connector.Func<TechniciansRequest, UserResponse[]>(x => x.Technicians, new TechniciansRequest());

                if (resultTechnicians.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTechnicians);
                    return;
                }
            }
        }

        private void SubmitTransferButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }

        private void TechnicianMeLink_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }
    }
}
