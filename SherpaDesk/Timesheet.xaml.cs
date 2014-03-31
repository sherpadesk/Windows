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
        IList<TimeResponse> _timeLogList = null;
        public Timesheet()
        {
            this.InitializeComponent();
            _timeLogList = new List<TimeResponse>();
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            var currentDate = DateTime.Now.Date;
            TimesheetCalendar.SelectedDateRange = new Telerik.UI.Xaml.Controls.Input.CalendarDateRange(currentDate, currentDate);
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

        private void FillTimesheetGrid(DateTime selectedDate)
        {
            TimesheetGridView.ItemsSource = _timeLogList.Where(x => x.Date.Date == selectedDate).ToList();
            TimesheetGridView.Visibility = TimesheetGridView.Items.Count > 0 ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
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
                var currentDate = DateTime.Now;
                TimesheetCalendar.DisplayDateStart = new DateTime(currentDate.Year, currentDate.Month, 1);
            }
        }
    }
}
