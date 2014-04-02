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

namespace SherpaDesk
{
    public sealed partial class Timesheet : SherpaDesk.Common.LayoutAwarePage
    {
        IList<TimeResponse> _timeLogList = null;
        public Timesheet()
        {
            this.InitializeComponent();
            _timeLogList = new List<TimeResponse>();
        }

        #region Handlers

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            var date = DateTime.Now;
            DateField.Value = date;
            DateLabel.Text = date.ToString("MMMM dd, yyyy - dddd");
            StartTimePicker.Value = EndTimePicker.Value = date;
            StartTimeLabel.Text = date.ToString("t");
            EndTimeLabel.Text = date.ToString("t");
            var currentDate = DateTime.Now.Date;
            //TimesheetCalendar.SelectedDateRange = new Telerik.UI.Xaml.Controls.Input.CalendarDateRange(currentDate, currentDate);
            DateField.Value = currentDate;
            DateLabel.Text = currentDate.ToString("MMMM dd, yyyy - dddd");
            TimesheetCalendar.SelectionChanged += TimesheetCalendar_SelectionChanged;
            DateField.ValueChanged += DateField_ValueChanged;
            FillTimesheetGrid(currentDate);
        }

        private void DateField_ValueChanged(object sender, EventArgs e)
        {
            if (DateField.Value.HasValue)
            {
                TimesheetCalendar.SelectionChanged -= TimesheetCalendar_SelectionChanged;
                var selectedDate = DateField.Value.Value;
                DateLabel.Text = DateField.Value.Value.ToString("MMMM dd, yyyy - dddd");
                TimesheetCalendar.DisplayDateStart = new DateTime(selectedDate.Year, selectedDate.Month, 1);
                TimesheetCalendar.SelectedDateRange = new Telerik.UI.Xaml.Controls.Input.CalendarDateRange(selectedDate, selectedDate);
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

        private void TimesheetCalendar_Loaded(object sender, RoutedEventArgs e)
        {
            var now = DateTime.Now;
            TimesheetCalendar.DisplayDateStart = new DateTime(now.Year - 3, 1, 1);
            this.LoadTimesheet(
                new DateTime(now.Year, now.Month, 1),
                new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month)));

        }
        #endregion

        #region Methods

        private async void LoadTimesheet(DateTime startDate, DateTime endDate)
        {
            using (var connector = new Connector())
            {
                var result = await connector.Func<TimeSearchRequest, TimeResponse[]>(
                    "time",
                    new TimeSearchRequest
                    {
                        TechnicianId = AppSettings.Current.UserId,
                        TimeType = eTimeType.Recent,
                        StartDate = startDate,
                        EndDate = endDate
                    });
                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                    return;
                }

                _timeLogList = result.Result.ToList();

                TimesheetCalendar.DataContext = _timeLogList
                    .GroupBy(x => x.Date.Date)
                    .Select(time => new CalendarCell
                    {
                        Date = time.Key,
                        Text = time.Sum(x => x.Hours).ToString("F")
                    }).ToList();
                // This is refreshing grid
                TimesheetCalendar.DisplayDateStart = new DateTime(startDate.Year - 3, 1, 1);
            }
        }

        private void FillTimesheetGrid(DateTime selectedDate)
        {
            var nonTicketsList = _timeLogList
                .Where(x => x.Date.Date == selectedDate && x.TicketId == 0)
                .ToList();
            NonTicketsGrid.ItemsSource = nonTicketsList;
            NonTicketsGrid.Visibility = nonTicketsList.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

            var ticketTimeList = _timeLogList
                .Where(x => x.Date.Date == selectedDate && x.TicketId > 0)
                .ToList();
            TicketTimeGrid.ItemsSource = ticketTimeList;
            TicketTimeGrid.Visibility = ticketTimeList.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

            if (ticketTimeList.Count > 0 && nonTicketsList.Count > 0)
            {
                TimesheetGrids.Visibility = Visibility.Visible;
                //TODO: move screen to right
            }
            else
                TimesheetGrids.Visibility = Visibility.Visible;
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

        private void TimesheetCalendar_CurrentDateChanged(object sender, EventArgs e)
        {
            this.LoadTimesheet(
                TimesheetCalendar.DisplayDateStart,
                TimesheetCalendar.DisplayDateEnd);
        }
    }
}
