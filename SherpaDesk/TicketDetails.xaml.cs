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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace SherpaDesk
{
    public sealed partial class TicketDetails : SherpaDesk.Common.LayoutAwarePage
    {
        private string _ticketKey;

        public TicketDetails()
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
//            SubjectDecorate.Height = SubjectLabel.ActualHeight;
            using (var connector = new Connector())
            {
                var resultTicket = await connector.Func<KeyRequest, TicketDetailsResponse>("tickets", new KeyRequest(_ticketKey));

                if (resultTicket.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTicket);
                    return;
                }

                var ticket = resultTicket.Result;
                TicketDetailsList.Items.Add(ticket);
                TicketDetailsList.Items.Add(ticket);
                TicketDetailsList.Items.Add(ticket);
                //SubjectLabel.Text = ticket.Subject;
                //EndUserLabel.Text = ticket.UserFullName;
                //InitialPostLabel.Text = Helper.HtmlToString(ticket.InitialPost);
                //WorkpadLabel.Text = Helper.HtmlToString(ticket.Workpad);

                //var resultFiles = await connector.Func<KeyRequest, FileResponse[]>("files", new KeyRequest("?ticket=", _ticketKey));

                //if (resultTicket.Status != eResponseStatus.Success)
                //{
                //    this.HandleError(resultTicket);
                //    return;
                //}
                //if (resultFiles.Result != null && resultFiles.Result.Length > 0)
                //{
                //    AttachedView.ItemsSource = resultFiles.Result.Select(file => new
                //    {
                //        FileName = file.Name,
                //        Image = new BitmapImage(new Uri(file.Url, UriKind.Absolute))
                //    }).ToList();
                //    FilesLabel.Visibility = AttachedView.Visibility = AttachedPages.Visibility = Windows.UI.Xaml.Visibility.Visible;
                //}
            }
        }

        private void AddResponseButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ResponseFrame.Navigate(typeof(AddResponse), _ticketKey);
        }
    }
}
