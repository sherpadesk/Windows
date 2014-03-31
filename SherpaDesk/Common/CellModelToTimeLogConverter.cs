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
    public class CellModelToTimeLogConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cellModel = value as CalendarCellModel;

            // Get a reference to the calendar container
            var calendar = cellModel.Presenter as RadCalendar;

            // Then you can get a reference to its DataContext (i.e. the page view model that holds the event information)
            var events = calendar.DataContext as IList<CalendarCell>;

            // return custom label for event cells
            var eventInfo = events.Where(e => e.Date == cellModel.Date).FirstOrDefault();
            if (eventInfo != null)
            {
                return eventInfo.Text;
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
