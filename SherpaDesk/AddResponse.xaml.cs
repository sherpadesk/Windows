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
    public sealed partial class AddResponse : SherpaDesk.Common.LayoutAwarePage
    {
        public event EventHandler UpdateTicketDetailsEvent;

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
                    "task_types",
                    new TaskTypeRequest());

                if (resultTaskType.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTaskType);
                    return;
                }

                TaskTypeList.FillData(resultTaskType.Result.AsEnumerable());

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
                filepickButton.Content = "Select Files";
            }
        }


        private async void SaveButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                decimal hours;
                decimal.TryParse(HoursTextBox.Text, out hours);
                if (hours > decimal.Zero)
                {
                    var resultAddTime = await connector.Action<AddTimeRequest>(
                        "time",
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
                else if (!string.IsNullOrEmpty(CommentsTextbox.Text))
                {
                    var resultNote = await connector.Action<AddNoteRequest>("posts", new AddNoteRequest
                    {
                        TicketKey = _ticketKey,
                        Note = CommentsTextbox.Text
                    });
                    if (resultNote.Status != eResponseStatus.Success)
                    {
                        this.HandleError(resultNote);
                        return;
                    }
                }

                if (HoldBox.IsChecked ?? false)
                {
                    var resultOnHold = await connector.Action<PlaceOnHoldRequest>("tickets",
                        new PlaceOnHoldRequest(_ticketKey) { Note = CommentsTextbox.Text });

                    if (resultOnHold.Status != eResponseStatus.Success)
                    {
                        this.HandleError(resultOnHold);
                    }
                }

                if (WaitingBox.IsChecked ?? false)
                {
                    var resultWait = await connector.Action<WaitingOnPostRequest>("tickets",
                        new WaitingOnPostRequest(_ticketKey) { Note = CommentsTextbox.Text });

                    if (resultWait.Status != eResponseStatus.Success)
                    {
                        this.HandleError(resultWait);
                    }

                }
                if (_attachment.Count > 0)
                {
                    using (FileRequest fileRequest = new FileRequest("?ticket=" + _ticketKey))
                    {
                        foreach (var file in _attachment)
                        {
                            await fileRequest.Add(file);
                        }
                        var resultUploadFile = await connector.Action<FileRequest>("files", fileRequest);
                        if (resultUploadFile.Status != eResponseStatus.Success)
                        {
                            this.HandleError(resultUploadFile);
                            return;
                        }
                    }
                }
                if (UpdateTicketDetailsEvent != null)
                {
                    UpdateTicketDetailsEvent(this, new EventArgs());
                }
                ((Frame)this.Parent).Navigate(typeof(Empty));
            }
        }
    }
}
