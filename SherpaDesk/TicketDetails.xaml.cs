using SherpaDesk.Common;
using SherpaDesk.Extensions;
using SherpaDesk.Interfaces;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using SherpaDesk.Models.ViewModels;

namespace SherpaDesk
{
    public sealed partial class TicketDetails : SherpaDesk.Common.LayoutAwarePage, IChildPage
    {
        #region Members & Ctor

        private string _ticketKey;
        private int _techId;
        private IList<StorageFile> _attachment = null;
        private eTicketAction _actionType = eTicketAction.None;

        public event EventHandler UpdatePage;

        public TicketDetails()
        {
            this.InitializeComponent();
            _attachment = new List<StorageFile>();
        }
        #endregion

        #region Data Methods
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

                if (ticket.EstimatedTime.HasValue && ticket.EstimatedTime.Value > 0 && ticket.TotalHours.HasValue)
                {
                    var percentageComplete = Math.Round(100 * ticket.TotalHours.Value / ticket.EstimatedTime.Value);
                    var chartData = new List<ChartDataModel>();
                    chartData.Add(new ChartDataModel { Value = Convert.ToDouble(100 - percentageComplete) });
                    chartData.Add(new ChartDataModel { Value = Convert.ToDouble(percentageComplete) });
                    detailsChart.Series[0].ItemsSource = chartData;
                    detailsChartTransfer.Series[0].ItemsSource = chartData;
                    PercentageCompleteTransferLabel.Text = PercentageCompleteLabel.Text = string.Format("{0}%", percentageComplete.ToString());
                    CompleteLabel.Visibility = detailsChart.Visibility = detailsChartTransfer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    CompleteLabel.Visibility = detailsChart.Visibility = detailsChartTransfer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    PercentageCompleteTransferLabel.Text = PercentageCompleteLabel.Text = string.Empty;
                }

                _techId = ticket.TechnicianId ?? AppSettings.Current.Configuration.User.Id;

                TicketNumber.Text = ticket.TicketNumber.ToString();
                SubjectLabel.Text = ticket.Subject;
                EndUserLabel.Text = ticket.UserFullName;
                TicketDetailsAvatar.Source = new BitmapImage(new Uri(string.Format("http://www.gravatar.com/avatar/{0}?d=mm&s=200", Helper.GetMD5(ticket.UserEmail))));

                TicketDescription.Text = Helper.HtmlToString(ticket.InitialPost);
                СreatedTime.Text = ticket.СreatedTimeText;

                TicketTransferAvatar.Source = new BitmapImage(new Uri(string.Format("http://www.gravatar.com/avatar/{0}?d=mm&s=100", Helper.GetMD5(ticket.UserEmail))));
                TicketNumberTransfer.Text = ticket.TicketNumber.ToString();
                SubjectLabelTransfer.Text = ticket.Subject;
                EndUserLabelTransfer.Text = ticket.UserFullName;
                СreatedTimeTransfer.Text = ticket.СreatedTimeText;

                var resultClasses = await connector.Func<UserRequest, ClassResponse[]>(x => x.Classes, new UserRequest { UserId = AppSettings.Current.Configuration.User.Id });
                if (resultClasses.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultClasses);
                    return;
                }
                ClassList.FillData(resultClasses.Result.AsEnumerable());

                var resultTechnicians = await connector.Func<UserResponse[]>(x => x.Technicians);

                if (resultTechnicians.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTechnicians);
                    return;
                }
                if (resultTechnicians.Result.Length < SearchRequest.MAX_PAGE_COUNT)
                {
                    TechnicianList.FillData(
                        resultTechnicians.Result.Select(user => new NameResponse { Id = user.Id, Name = Helper.FullName(user.FirstName, user.LastName, user.Email, true) }),
                        new NameResponse { Id = -1, Name = "Let the system choose." });
                }
                else
                {
                    TechnicianList.AutoComplete(x => x.Search(false));
                }

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
                    NoteText = Helper.HtmlToString(x.NoteText),
                    Avatar = string.Format("http://www.gravatar.com/avatar/{0}?d=mm&s=125", Helper.GetMD5(x.Email))
                }).Where(x => x.NoteType != eNoteType.InitialPost.Details()).ToList();
                TicketDetailsList.ItemsSource = resultView;
            }
        }

        public async Task PostResponse()
        {
            using (var connector = new Connector())
            {
                int? postId = null;
                decimal hours;
                decimal.TryParse(HoursTextBox.Text, out hours);
                if (hours > decimal.Zero)
                {
                    var resultAddTime = await connector.Action<AddTimeRequest>(
                        x => x.Time,
                        new AddTimeRequest
                        {
                            TicketKey = _ticketKey,
                            AccountId = -1,
                            ProjectId = -1,
                            TaskTypeId = TaskTypeList.GetSelectedValue<int>(),
                            TechnicianId = AppSettings.Current.Configuration.User.Id,
                            Billable = Billable.SelectedIndex == 0 ? true : false,
                            Hours = hours,
                            Note = CommentsTextbox.Text,
                            Date = DateTime.Now
                        });
                    if (resultAddTime.Status != eResponseStatus.Success)
                    {
                        this.HandleError(resultAddTime);
                        return;
                    }
                }
                else
                {
                    var resultNote = await connector.Func<AddNoteRequest, NoteResponse[]>(x => x.Posts,
                        new AddNoteRequest
                        {
                            TicketKey = _ticketKey,
                            Note = CommentsTextbox.Text
                        });
                    if (resultNote.Status != eResponseStatus.Success)
                    {
                        this.HandleError(resultNote);
                        return;
                    }
                    postId = resultNote.Result.First().PostId;
                }

                if (_attachment.Count > 0)
                {
                    using (FileRequest fileRequest = FileRequest.Create(_ticketKey, postId))
                    {
                        foreach (var file in _attachment)
                        {
                            await fileRequest.Add(file);
                        }
                        var resultUploadFile = await connector.Action<FileRequest>(x => x.Files, fileRequest);
                        if (resultUploadFile.Status != eResponseStatus.Success)
                        {
                            this.HandleError(resultUploadFile);
                            return;
                        }
                    }
                }
                StartTimePicker.Value = EndTimePicker.Value = new DateTime?();
                HoursTextBox.Text = "0.00";
                CommentsTextbox.Text = string.Empty;
                await LoadPage();
            }
        }

        private async void DoActionOnTicket()
        {
            Response result = null;
            using (var connector = new Connector())
            {
                switch (_actionType)
                {
                    case eTicketAction.Close:
                        result = await connector.Action<CloseTicketRequest>(x => x.Tickets,
                                new CloseTicketRequest(_ticketKey));

                        break;
                    case eTicketAction.Delete:
                        result = await connector.Action<DeleteRequest>(x => x.Tickets,
                                new DeleteRequest(_ticketKey));
                        break;
                    case eTicketAction.PlaceOnHold:
                        result = await connector.Action<PlaceOnHoldRequest>(x => x.Tickets,
                            new PlaceOnHoldRequest(_ticketKey) { Note = OnHoldTextbox.Text });
                        break;
                    case eTicketAction.PickUp:
                        result = await connector.Action<PickUpRequest>(x => x.Tickets,
                            new PickUpRequest(_ticketKey));
                        break;
                }
            }
            if (result != null)
            {
                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                }
                else
                {
                    this.pageRoot.MainPage(page =>
                    {
                        page.WorkDetailsFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        ((WorkList)page.WorkListFrame.Content).FullUpdate();

                    });
                }
            }
        }

        private void CalculateHours()
        {
            if (StartTimePicker.Value.HasValue && EndTimePicker.Value.HasValue)
            {
                var time = EndTimePicker.Value.Value.TimeOfDay - StartTimePicker.Value.Value.TimeOfDay;
                HoursTextBox.Text = time.TotalHours >= 0 ? String.Format("{0:0.00}", time.TotalHours) : String.Format("{0:0.00}", 24 + time.TotalHours);
            }
        }

        private void ShowConfirm()
        {
            if (_actionType == eTicketAction.Close)
            {
                ConfirmMessage.Text = "Are you sure you want to close this ticket?";
                ConfirmYesLabel.Text = "Yes, Close It";
            }
            else if (_actionType == eTicketAction.Delete)
            {
                ConfirmMessage.Text = "Are you sure you want to delete this ticket?";
                ConfirmYesLabel.Text = "Yes, Delete It";
            }
            else if (_actionType == eTicketAction.PickUp)
            {
                ConfirmMessage.Text = "Are you sure you want to pick up this ticket?";
                ConfirmYesLabel.Text = "Yes, Pick Up It";
            }
            grid.IsHitTestVisible = false;
            ConfirmPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            BlackScreen.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void HideConfirm()
        {
            grid.IsHitTestVisible = true;
            ConfirmPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            BlackScreen.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void ShowOnHold()
        {
            OnHoldTextbox.Text = string.Empty;
            grid.IsHitTestVisible = false;
            PlaceOnHoldPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            BlackScreen.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void HideOnHold()
        {
            grid.IsHitTestVisible = true;
            PlaceOnHoldPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            BlackScreen.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        #endregion

        #region Main Handlers
        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            this.pageRoot.MainPage(page =>
            {
                page.ScrollViewer.ChangeView((page.ScrollViewer.ScrollableWidth - this.Frame.ActualWidth + 300), null, null);
            });
            await LoadPage();
            using (var connector = new Connector())
            {
                var resultTaskType = await connector.Func<TaskTypeRequest, NameResponse[]>(
                    x => x.TaskTypes,
                    new TaskTypeRequest());

                if (resultTaskType.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTaskType);
                    return;
                }
                TaskTypeList.FillData(resultTaskType.Result.AsEnumerable());
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _ticketKey = (string)e.Parameter;
            base.OnNavigatedTo(e);
        }

        protected async override void UpdatedPage(object sender, EventArgs e)
        {
            await LoadPage();
        }
        #endregion

        #region Action Handlers
        private void AttachedView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.ExternalAction(x =>
                x.ShowFullScreenImage(((Image)sender).Source));
        }

        private async void SaveTransferButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var transferResult = await connector.Action<TransferRequest>(x => x.Tickets, new TransferRequest(_ticketKey)
                {
                    Note = NotesTextbox.Text,
                    KeepAttached = KeepMeCheckBox.IsChecked ?? false,
                    ClassId = ClassCheckBox.IsChecked ?? false ? ClassList.GetSelectedValue<int>() : 0,
                    TechnicianId = TechnicianList.GetSelectedValue<int>(AppSettings.Current.Configuration.User.Id)
                });

                if (transferResult.Status != eResponseStatus.Success)
                {
                    this.HandleError(transferResult);
                    return;
                }

                if (MakeMeAlternateCheckBox.IsChecked ?? false)
                {
                    var attachAltTechResult = await connector.Action<AttachAltTechRequest>(x => x.Tickets,
                        new AttachAltTechRequest(_ticketKey, AppSettings.Current.Configuration.User.Id));

                    if (attachAltTechResult.Status != eResponseStatus.Success)
                    {
                        this.HandleError(attachAltTechResult);
                        return;
                    }
                }
            }

            GridTicketDetails.Visibility =
            GridAddResponse.Visibility =
            Windows.UI.Xaml.Visibility.Visible;

            GridTicketDetailsTransfer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            this.pageRoot.MainPage(page =>
            {
                page.WorkDetailsFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                ((WorkList)page.WorkListFrame.Content).FullUpdate();

            });
        }

        private async void attachButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            var files = await openPicker.PickMultipleFilesAsync();
            _attachment.Clear();
            if (files != null && files.Count > 0)
            {
                SelectedFilesList.Text = "Picked photos: ";
                List<string> fileNames = new List<string>();
                foreach (var file in files)
                {
                    fileNames.Add(file.Name);
                    _attachment.Add(file);
                }
                SelectedFilesList.Text += string.Join(", ", fileNames.ToArray());
            }
            else
            {
                SelectedFilesList.Text = string.Empty;
            }
        }

        private void CloseButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _actionType = eTicketAction.Close;
            ShowConfirm();
        }

        private void PickupButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _actionType = eTicketAction.PickUp;
            ShowConfirm();
        }

        private void DeleteButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _actionType = eTicketAction.Delete;
            ShowConfirm();
        }

        private async void ReplyGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await PostResponse();
        }

        private void ConfirmYes_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HideConfirm();
            DoActionOnTicket();
        }

        private void ConfirmNo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HideConfirm();
        }

        private void ConfirmYesOnHold_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HideOnHold();
            DoActionOnTicket();
        }

        private void ConfirmNoOnHold_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HideOnHold();
        }

        private void PlaceOnHoldButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _actionType = eTicketAction.PlaceOnHold;
            ShowOnHold();
        }

        #endregion

        #region Visual Handlers
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

        private void StartTimePicker_ValueChanged(object sender, EventArgs e)
        {
            CalculateHours();
        }

        private void EndTimePicker_ValueChanged(object sender, EventArgs e)
        {
            CalculateHours();
        }

        private void ClassCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ClassList.IsEnabled = ClassCheckBox.IsChecked.Value;
        }

        private void TechnicianCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            TechnicianList.IsEnabled = TechnicianCheckbox.IsChecked.Value;
        }

        private void ConfirmYes_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ConfirmYes.Background.Opacity = 0.9;
        }

        private void ConfirmYes_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ConfirmYes.Background.Opacity = 1;
        }

        private void ConfirmNo_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ConfirmNo.Background.Opacity = 0.9;
        }

        private void ConfirmNo_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ConfirmNo.Background.Opacity = 1;
        }

        private void ConfirmNoOnHold_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ConfirmNoOnHold.Background.Opacity = 0.9;
        }

        private void ConfirmNoOnHold_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ConfirmNoOnHold.Background.Opacity = 1;
        }

        private void ConfirmYesOnHold_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ConfirmYesOnHold.Background.Opacity = 0.9;
        }

        private void ConfirmYesOnHold_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ConfirmYesOnHold.Background.Opacity = 1;
        }
        #endregion

        private void RemoveMeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            KeepMeCheckBox.IsChecked = false;
        }

        private void KeepMeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RemoveMeCheckBox.IsChecked = false;
        }
    }
    public enum eTicketAction
    {
        None = 0,
        Close,
        ReOpen,
        PickUp,
        PlaceOnHold,
        Delete
    }
}
