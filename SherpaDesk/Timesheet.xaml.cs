using System;
using System.Linq;
using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Input.Calendar.Commands;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Telerik.UI.Xaml.Controls.Input;

namespace SherpaDesk
{
    public sealed partial class Timesheet : SherpaDesk.Common.LayoutAwarePage
    {
        public event EventHandler MoveScrollToRight;

        public Timesheet()
        {
            this.InitializeComponent();
            this.Model.OnDataLoading += async (object sender, TimesheetEventArgs e) => await TimesheetLoad(sender, e);
        }

        #region Handlers

        private async Task TimesheetLoad(object sender, TimesheetEventArgs e)
        {
            using (var connector = new Connector())
            {
                var result = await connector.Func<TimeSearchRequest, TimeResponse[]>(
                    "time",
                    new TimeSearchRequest
                    {
                        TechnicianId = AppSettings.Current.UserId,
                        TimeType = eTimeType.Recent,
                        StartDate = e.StartDate.AddDays(-7),
                        EndDate = e.EndDate.AddDays(7)
                    });
                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                    return;
                }

                this.Model.TimeLogList = new ObservableCollection<TimeResponse>(result.Result.ToList());

                this.TimesheetCalendar.DisplayDateStart = e.StartDate.AddYears(-3);
                this.TimesheetCalendar.DisplayDateEnd = e.EndDate.AddYears(3);
            }
        }

        //private void TimesheetSelectedDate(object sender, TimesheetEventArgs e)
        //{
        //    if ((this.Model.VisibleNonTickets & this.Model.VisibleTicketTime) == Windows.UI.Xaml.Visibility.Visible)
        //    {
        //        TimesheetGrids.Visibility = Visibility.Visible;
        //        if (MoveScrollToRight != null)
        //        {
        //            MoveScrollToRight(this, new EventArgs());
        //        }
        //    }
        //    else
        //        TimesheetGrids.Visibility = Visibility.Collapsed;
        //}

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            var date = DateTime.Now;
            StartTimePicker.Value = EndTimePicker.Value = date;
            StartTimeLabel.Text = EndTimeLabel.Text = date.ToString("t");
            DateLabel.Text = date.ToString("MMMM dd, yyyy - dddd");

            using (var connector = new Connector())
            {
                // types
                var resultTaskType = await connector.Func<TaskTypeRequest, NameResponse[]>(
                    "task_types",
                    new TaskTypeRequest());
                if (resultTaskType.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTaskType);
                    return;
                }
                TaskTypeList.FillData(resultTaskType.Result.AsEnumerable());

                // technician
                var resultUsers = await connector.Func<UserResponse[]>("users");
                if (resultUsers.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultUsers);
                    return;
                }
                TechnicianList.FillData(
                    resultUsers.Result.Select(user => new NameResponse { Id = user.Id, Name = Helper.FullName(user.FirstName, user.LastName, user.Email) }),
                    new NameResponse
                    {
                        Id = AppSettings.Current.UserId,
                        Name = Constants.TECHNICIAN_ME
                    });

                // projects
                var resultProjects = await connector.Func<ProjectResponse[]>("projects");
                if (resultProjects.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultProjects);
                    return;
                }
                ProjectList.FillData(resultProjects.Result.AsEnumerable());
            }
        }

        private void DateField_ValueChanged(object sender, EventArgs e)
        {
            if (DateField.Value.HasValue)
            {
                TimesheetCalendar.SelectionChanged -= TimesheetCalendar_SelectionChanged;
                var selectedDate = DateField.Value.Value;
                DateLabel.Text = DateField.Value.Value.ToString("MMMM dd, yyyy - dddd");
                TimesheetCalendar.SelectedDateRange = new CalendarDateRange(selectedDate, selectedDate);
                TimesheetCalendar.SelectionChanged += TimesheetCalendar_SelectionChanged;
                FillTimesheetGrid(selectedDate);
            }
        }

        private void TimesheetCalendar_SelectionChanged(object sender, EventArgs e)
        {
            DateField.ValueChanged -= DateField_ValueChanged;
            var selectedDate = TimesheetCalendar.SelectedDateRange.Value.StartDate.Date;
            DateField.Value = selectedDate;
            DateLabel.Text = selectedDate.ToString("MMMM dd, yyyy - dddd");
            DateField.ValueChanged += DateField_ValueChanged;
            FillTimesheetGrid(selectedDate);
        }

        private void StartTimePicker_ValueChanged(object sender, EventArgs e)
        {
            StartTimeLabel.Text = StartTimePicker.Value.Value.ToString("t");
            CalculateHours();
        }

        private void EndTimePicker_ValueChanged(object sender, EventArgs e)
        {
            EndTimeLabel.Text = EndTimePicker.Value.Value.ToString("t");
            CalculateHours();
        }

        private void TimeTicketId_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var hours = decimal.Zero;
            decimal.TryParse(HoursTextBox.Text, out hours);
            var date = DateField.Value ?? DateTime.Now;

            using (var connector = new Connector())
            {
                var result = await connector.Action<AddTimeRequest>(
                            "time",
                            new AddTimeRequest
                            {
                                AccountId = -1,
                                ProjectId = ProjectList.GetSelectedValue<int>(-1),
                                TaskTypeId = TaskTypeList.GetSelectedValue<int>(),
                                TechnicianId = TechnicianList.GetSelectedValue<int>(),
                                Billable = BillableBox.IsChecked.HasValue ? BillableBox.IsChecked.Value : false,
                                Hours = hours,
                                Note = NoteTextBox.Text,
                                Date = date
                            });

                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                    return;
                }
            }
            TaskTypeList.SelectedIndex = -1;
            NoteTextBox.Text = string.Empty;
            HoursTextBox.Text = "0.00";

            await TimesheetLoad(this, new TimesheetEventArgs(this.Model));

            this.FillTimesheetGrid(date);
        }
        #endregion

        #region Methods

        private void FillTimesheetGrid(DateTime date)
        {
            this.Model.CurrentDate = date;
            NonTicketsGrid.ItemsSource = this.Model.NonTicketsList;
            NonTicketsGrid.UpdateLayout();
            NonTicketsLabel.Visibility = NonTicketsGrid.Visibility = this.Model.VisibleNonTickets;

            TicketTimeGrid.ItemsSource = this.Model.TicketTimeList;
            TicketTimeGrid.UpdateLayout();
            TicketTimeLabel.Visibility = TicketTimeGrid.Visibility = this.Model.VisibleTicketTime;
            
            if ((this.Model.VisibleNonTickets & this.Model.VisibleTicketTime) == Windows.UI.Xaml.Visibility.Visible)
            {
                TimesheetGrids.Visibility = Visibility.Visible;
                if (MoveScrollToRight != null)
                {
                    MoveScrollToRight(this, new EventArgs());
                }
            }
            else
                TimesheetGrids.Visibility = Visibility.Collapsed;
        }

        private void CalculateHours()
        {
            if (StartTimePicker.Value.HasValue && EndTimePicker.Value.HasValue)
            {
                var time = EndTimePicker.Value.Value.TimeOfDay - StartTimePicker.Value.Value.TimeOfDay;
                HoursTextBox.Text = time.TotalHours >= 0 ? String.Format("{0:0.00}", time.TotalHours) : String.Format("{0:0.00}", 24 + time.TotalHours);
            }
        }
        #endregion

    }
}
