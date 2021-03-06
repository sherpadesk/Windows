﻿using SherpaDesk.Common;
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
using Windows.UI.Xaml.Navigation;

namespace SherpaDesk
{
    public sealed partial class AddResponse : SherpaDesk.Common.LayoutAwarePage, IChildPage
    {
        public event EventHandler UpdatePage;

        private const string ERROR_EMPTY_HOURS = "Hours should be positive number.";
        private const string ERROR_MUCH_HOURS = "Hours cannot be more then 24 hours in day.";
        private IList<StorageFile> _attachment = new List<StorageFile>();
        private string _ticketKey;
        public AddResponse()
        {
            this.InitializeComponent();
            _attachment = new List<StorageFile>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _ticketKey = (string)e.Parameter;
            base.OnNavigatedTo(e);
        }

        private void StartTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (StartTimePicker.Value.HasValue)
                StartTimeLabel.Text = StartTimePicker.Value.Value.ToString("t");
            CalculateHours();
        }

        private void EndTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (EndTimePicker.Value.HasValue)
                EndTimeLabel.Text = EndTimePicker.Value.Value.ToString("t");
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

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            var date = DateTime.Now;
            StartTimePicker.Value = EndTimePicker.Value = date;
            StartTimeLabel.Text = date.ToString("t");
            EndTimeLabel.Text = date.ToString("t");
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

                var resultTicket = await connector.Func<KeyRequest, TicketDetailsResponse>(x => x.Tickets, new KeyRequest(_ticketKey));
                if (resultTicket.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTicket);
                    return;
                }
                var ticket = resultTicket.Result;
                if (ticket.Status == eTicketStatus.OnHold.ToString())
                {
                    SaveAndReopenButton.Visibility = SaveDoNotReopenLink.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    SaveButton.Visibility = HoldBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
        }

        private async void filepickButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
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
                filepickButton.Content = String.Format("Attached Files: {0}", files.Count);
                foreach (var file in files)
                {
                    _attachment.Add(file);
                }
            }
            else
            {
                filepickButton.Content = "Attach Files";
            }
        }

        public async Task PostResponse(bool hold, bool reopen)
        {
            using (var connector = new Connector())
            {
                bool statusUpdated = false;
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
                if (hold)
                {
                    var resultOnHold = await connector.Action<PlaceOnHoldRequest>(x => x.Tickets,
                        new PlaceOnHoldRequest(_ticketKey) { Note = CommentsTextbox.Text });

                    if (resultOnHold.Status != eResponseStatus.Success)
                    {
                        this.HandleError(resultOnHold);
                    }
                    statusUpdated = true;
                }
                if (reopen)
                {
                    var resultReOpen = await connector.Action<ReOpenRequest>(x => x.Tickets,
                        new ReOpenRequest(_ticketKey) { Note = CommentsTextbox.Text });

                    if (resultReOpen.Status != eResponseStatus.Success)
                    {
                        this.HandleError(resultReOpen);
                    }
                    statusUpdated = true;
                }


                if (!string.IsNullOrEmpty(CommentsTextbox.Text))
                {
                    if (WaitingBox.IsChecked ?? false)
                    {
                        var resultWait = await connector.Action<WaitingOnPostRequest>(x => x.Tickets,
                            new WaitingOnPostRequest(_ticketKey)
                            {
                                Note = CommentsTextbox.Text
                            });

                        if (resultWait.Status != eResponseStatus.Success)
                        {
                            this.HandleError(resultWait);
                        }
                    }
                    else if (hours == decimal.Zero && !statusUpdated)
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
                if (UpdatePage != null)
                {
                    UpdatePage(this, EventArgs.Empty);
                }
                if (this.Parent != null)
                {
                    ((Frame)this.Parent).Navigate(typeof(Empty));
                }
                if (statusUpdated)
                {
                    App.ExternalAction(x =>
                        x.UpdateInfo());
                }
            }
        }

        private async void SaveButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await PostResponse(HoldBox.IsChecked ?? false, false);
        }

        private async void SaveAndReopenButton_Click(object sender, TappedRoutedEventArgs e)
        {
            await PostResponse(false, true);
        }

        private async void SaveDoNotReopenLink_Click(object sender, TappedRoutedEventArgs e)
        {
            await PostResponse(false, false);
        }
    }
}
