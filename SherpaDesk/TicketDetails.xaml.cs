﻿using SherpaDesk.Common;
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
        private string _ticketKey;
        private int _techId;
        private IList<StorageFile> _attachment = null;
        private byte confirmType = 0;

        public event EventHandler UpdatePage;

        public TicketDetails()
        {
            this.InitializeComponent();
            _attachment = new List<StorageFile>();
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

                var resultClasses = await connector.Func<UserRequest, ClassResponse[]>(x => x.Classes, new UserRequest { UserId = AppSettings.Current.UserId });
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
                    NoteText = Helper.HtmlToString(x.NoteText)
                }).Where(x => x.NoteType != eNoteType.InitialPost.Details()).ToList();
                TicketDetailsList.ItemsSource = resultView;
            }
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
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
                            TechnicianId = AppSettings.Current.UserId,
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

        protected async override void UpdatedPage(object sender, EventArgs e)
        {
            await LoadPage();
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

        private void StartTimePicker_ValueChanged(object sender, EventArgs e)
        {
            CalculateHours();
        }

        private void EndTimePicker_ValueChanged(object sender, EventArgs e)
        {
            CalculateHours();
        }

        private void CalculateHours()
        {
            if (StartTimePicker.Value.HasValue && EndTimePicker.Value.HasValue)
            {
                var time = EndTimePicker.Value.Value.TimeOfDay - StartTimePicker.Value.Value.TimeOfDay;
                HoursTextBox.Text = time.TotalHours >= 0 ? String.Format("{0:0.00}", time.TotalHours) : String.Format("{0:0.00}", 24 + time.TotalHours);
            }
        }

        private void SaveTransferButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GridTicketDetails.Visibility =
            GridAddResponse.Visibility =
            Windows.UI.Xaml.Visibility.Visible;

            GridTicketDetailsTransfer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
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

        private void ShowConfirm()
        {
            if (confirmType == 1)
            {
                ConfirmMessage.Text = "Are you sure you want to close this ticket?";
                ConfirmYesLabel.Text = "Yes, Close It";
            }
            else
            {
                ConfirmMessage.Text = "Are you sure you want to delete this ticket?";
                ConfirmYesLabel.Text = "Yes, Delete It";
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

        private void CloseButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            confirmType = 1;
            ShowConfirm();
        }

        private void DeleteButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            confirmType = 2;
            ShowConfirm();
        }

        private async void ReplyGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await PostResponse();
        }

        private void ClassCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ClassList.IsEnabled = ClassCheckBox.IsChecked.Value;
        }

        private void TechnicianCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            TechnicianList.IsEnabled = TechnicianCheckbox.IsChecked.Value;
        }

        private async void ConfirmYes_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HideConfirm();
            using (var connector = new Connector())
            {
                if (confirmType == 1)
                {
                    var result = await connector.Action<CloseTicketRequest>(x => x.Tickets,
                            new CloseTicketRequest(_ticketKey));

                    if (result.Status != eResponseStatus.Success)
                    {
                        this.HandleError(result);
                        return;
                    }
                }
                else
                {
                    var result = await connector.Action<DeleteRequest>(x => x.Tickets,
                            new DeleteRequest(_ticketKey));

                    if (result.Status != eResponseStatus.Success)
                    {
                        this.HandleError(result);
                        return;
                    }
                }
            }
            this.pageRoot.MainPage(page =>
            {
                page.WorkDetailsFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                ((WorkList)page.WorkListFrame.Content).FullUpdate();
            });
        }

        private void ConfirmNo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HideConfirm();
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
    }
}
