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

//                AddResponseButton.Visibility = Windows.UI.Xaml.Visibility.Visible;

                _techId = ticket.TechnicianId ?? AppSettings.Current.UserId;

                //SubjectLabel.Text = ticket.Subject;
                //EndUserLabel.Text = ticket.UserFullName;
                //InitialPostLabel.Text = Helper.HtmlToString(ticket.InitialPost);
                //WorkpadLabel.Text = Helper.HtmlToString(ticket.Workpad);

                var resultFiles = await connector.Func<KeyRequest, FileResponse[]>(x => x.Files, new KeyRequest("?ticket=", _ticketKey));

                if (resultTicket.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTicket);
                    return;
                }
                if (resultFiles.Result != null && resultFiles.Result.Length > 0)
                {
                    //AttachedView.ItemsSource = resultFiles.Result.Select(file => new AttachmentModel
                    //{
                    //    FileName = file.Name,
                    //    Image = new BitmapImage(new Uri(file.Url, UriKind.Absolute))
                    //}).ToList();
                    //FilesLabel.Visibility = AttachedView.Visibility = AttachedPages.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                await FillResponses();
            }

        }
        private async Task FillResponses()
        {
            using (var connector = new Connector())
            {
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
                //TicketDetailsList.ItemsSource = null; // For Visual Effect
                //TicketDetailsList.ItemsSource = resultView;
            }
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPage();
            this.MainPage(page => page.ScrollViewer.ChangeView(Constants.WIDTH_MAX_RIGHT, null, null));
        }

        private void AddResponseButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //ResponseFrame.Navigated -= ChildPage_Navigated;
            //ResponseFrame.Navigated += ChildPage_Navigated;
            //ResponseFrame.Navigate(typeof(AddResponse), _ticketKey);
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

        private void TransferMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //ResponseFrame.Navigated -= ChildPage_Navigated;
            //ResponseFrame.Navigated += ChildPage_Navigated;
            //ResponseFrame.Navigate(typeof(Transfer), new KeyValuePair<string, int>(_ticketKey, _techId));
        }

        private void AttachedView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //App.ExternalAction(x =>
            //    x.ShowFullScreenImage(((AttachmentModel)AttachedView.SelectedItem).Image));
        }
    }
}
