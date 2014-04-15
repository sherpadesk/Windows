using SherpaDesk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Windows.UI.Xaml.Data;

namespace SherpaDesk.Common
{
    public class CellModelToTimeLogYearConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cellModel = value as CalendarCellModel;

            // Get a reference to the calendar container
            var calendar = cellModel.Presenter as RadCalendar;

            // Then you can get a reference to its DataContext (i.e. the page view model that holds the event information)
            var model = calendar.DataContext as TimesheetViewModel;
            if (model.Timesheet != null)
            {
                // return custom label for event cells
                var eventInfo = model.Timesheet.Where(e => e.Date.Month == cellModel.Date.Month && e.Date.Year == cellModel.Date.Year).Sum(x => x.Value).ToString("F");
                if (!string.IsNullOrEmpty(eventInfo))
                {
                    return eventInfo;
                }
            }
            // return default label for regular cells
            return cellModel.Label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
