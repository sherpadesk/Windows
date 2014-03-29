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

namespace SherpaDesk
{
    public sealed partial class Timesheet : SherpaDesk.Common.LayoutAwarePage
    {
        public Timesheet()
        {
            this.InitializeComponent();
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            var currentDate = DateTime.Now;
            TimesheetCalendar.SelectedDateRange = new Telerik.UI.Xaml.Controls.Input.CalendarDateRange(currentDate, currentDate);
            DateField.Value = currentDate;
            DateLabel.Text = currentDate.ToString("MMMM dd, yyyy - dddd");
            TimesheetCalendar.SelectionChanged += TimesheetCalendar_SelectionChanged;
            DateField.ValueChanged += DateField_ValueChanged;
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
            }
        }

        private void TimesheetCalendar_SelectionChanged(object sender, EventArgs e)
        {
            DateField.ValueChanged -= DateField_ValueChanged;
            DateField.Value = TimesheetCalendar.SelectedDateRange.Value.StartDate;
            DateLabel.Text = DateField.Value.Value.ToString("MMMM dd, yyyy - dddd");
            DateField.ValueChanged += DateField_ValueChanged;
        }

        private async void TimesheetCalendar_Loaded(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var result = await connector.Func<TimeSearchRequest, TimeResponse[]>(
                    "time",
                    new TimeSearchRequest
                    {
                        TechnicianId = AppSettings.Current.UserId
                    });
                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                }

                TimesheetCalendar.DataContext = result.Result.Select(time => new TimeLog
                {
                    Date = time.Date,
                    Text = time.Hours.ToString("F")
                }).ToList();
                var currentDate = DateTime.Now;
                TimesheetCalendar.DisplayDateStart = new DateTime(currentDate.Year, currentDate.Month, 1);
            }
        }
    }
}
