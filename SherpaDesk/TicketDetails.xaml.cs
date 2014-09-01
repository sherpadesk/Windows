using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace SherpaDesk
{
    public sealed partial class TicketDetails : SherpaDesk.Common.LayoutAwarePage, IChildPage
    {
        private string _ticketKey;
        private int _techId;

        public event EventHandler UpdatePage;

        public TicketDetails()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _ticketKey = (string)e.Parameter;
            base.OnNavigatedTo(e);
        }

        private async Task LoadPage()
        {
            using (var connector = new Connector())
            {
                var resultTicket = await connector.Func<KeyRequest, TicketDetailsResponse>(x => x.Tickets, new KeyRequest(_ticketKey));

                if (resultTicket.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTicket);
                    return;
                }
                var ticket = resultTicket.Result;

                _techId = ticket.TechnicianId ?? AppSettings.Current.UserId;

                TicketNumber.Text = ticket.TicketNumber.ToString();
                SubjectLabel.Text = ticket.Subject;
                EndUserLabel.Text = ticket.UserFullName;
                TicketDescription.Text = Helper.HtmlToString(ticket.InitialPost);
                СreatedTime.Text = ticket.СreatedTimeText;

                TicketNumberTransfer.Text = ticket.TicketNumber.ToString();
                SubjectLabelTransfer.Text = ticket.Subject;
                EndUserLabelTransfer.Text = ticket.UserFullName;
                СreatedTimeTransfer.Text = ticket.СreatedTimeText;

                var resultFiles = await connector.Func<KeyRequest, FileResponse[]>(x => x.Files, new KeyRequest("?ticket=", _ticketKey));

                if (resultTicket.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTicket);
                    return;
                }
                if (resultFiles.Result != null && resultFiles.Result.Length > 0)
                {
                    AttachmentsList.ItemsSource = resultFiles.Result.Select(file => new AttachmentModel
                    {
                        FileName = file.Name,
                        ImageUrl = file.Url
                    }).ToList();
                    GridAttachments.Visibility = AttachmentsTitle.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    GridAttachments.Visibility = AttachmentsTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                await FillResponses();
            }

        }
        private async Task FillResponses()
        {
            using (var connector = new Connector())
            {
                var chartData = new List<ChartDataModel>();
                chartData.Add(new ChartDataModel { Value = 25 });
                chartData.Add(new ChartDataModel { Value = 75 });
                detailsChart.Series[0].ItemsSource = chartData;
                detailsChartTransfer.Series[0].ItemsSource = chartData;
                ChartGridTransfer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                ChartGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
                var resultNotes = await connector.Func<NoteSearchRequest, NoteResponse[]>(x => x.Tickets, new NoteSearchRequest(_ticketKey));
                if (resultNotes.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultNotes);
                    return;
                }

                var resultView = resultNotes.Result.Select(x => new
                {
                    x.FullName,
                    x.ResponseDateText,
                    x.NoteType,
                    NoteText = Helper.HtmlToString(x.NoteText)
                }).Where(x => x.NoteType != eNoteType.InitialPost.Details()).ToList();
                TicketDetailsList.ItemsSource = resultView;
            }
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPage();
        }

        protected async override void UpdatedPage(object sender, EventArgs e)
        {
            await LoadPage();
        }

        private async void CloseMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (await App.ConfirmMessage())
            {
                using (var connector = new Connector())
                {
                    var result = await connector.Action<CloseTicketRequest>(x => x.Tickets,
                            new CloseTicketRequest(_ticketKey));

                    if (result.Status != eResponseStatus.Success)
                    {
                        this.HandleError(result);
                        return;
                    }
                }
                ((Frame)this.Parent).Navigate(typeof(Empty));
                if (this.UpdatePage != null)
                {
                    this.UpdatePage(this, EventArgs.Empty);
                }
                App.ExternalAction(x => x.UpdateInfo());
            }
        }

        private void AttachedView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.ExternalAction(x =>
                x.ShowFullScreenImage(((Image)sender).Source));
        }

        private void ReplyGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ReplyGrid.Background.Opacity = 0.9;
        }

        private void ReplyGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ReplyGrid.Background.Opacity = 1;
        }

        private void SaveTransferButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 0.9;
        }

        private void SaveTransferButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 1;
        }
        private void TransferButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GridTicketDetails.Visibility =
            GridAddResponse.Visibility =
            Windows.UI.Xaml.Visibility.Collapsed;

            GridTicketDetailsTransfer.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void CancelTransferButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            GridTicketDetails.Visibility =
            GridAddResponse.Visibility =
            Windows.UI.Xaml.Visibility.Visible;

            GridTicketDetailsTransfer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void CancelTransferButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((TextBlock)sender).Opacity = 0.6;
        }

        private void CancelTransferButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((TextBlock)sender).Opacity = 1;
        }

        private void SaveTransferButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GridTicketDetails.Visibility =
            GridAddResponse.Visibility =
            Windows.UI.Xaml.Visibility.Visible;

            GridTicketDetailsTransfer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}
