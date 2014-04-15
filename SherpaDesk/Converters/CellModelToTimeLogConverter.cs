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
            var calendar = cellModel.Presenter as RadCalendar;
            
            if (calendar.DisplayMode == CalendarDisplayMode.MonthView)
            {
                var model = calendar.DataContext as TimesheetViewModel;
                if (model.Timesheet != null)
                {
                    var eventInfo = model.Timesheet.Where(e => e.Date == cellModel.Date).FirstOrDefault();
                    if (eventInfo != null)
                    {
                        return eventInfo.Text;
                    }
                }
            }
            return cellModel.Label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
