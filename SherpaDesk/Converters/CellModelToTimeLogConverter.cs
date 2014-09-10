using System;
using System.Linq;
using Windows.UI.Xaml.Data;
using SherpaDesk.Models;
using SherpaDesk.Models.ViewModels;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar;

namespace SherpaDesk.Converters
{
    public class CellModelToTimeLogConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cellModel = value as CalendarCellModel;
            
            if (cellModel == null) return null;

            var calendar = cellModel.Presenter as RadCalendar;

            if (calendar == null || calendar.DisplayMode != CalendarDisplayMode.MonthView) return cellModel.Label;

            var model = calendar.DataContext as TimesheetViewModel;
                    
            if (model == null || model.Timesheet == null) return cellModel.Label;

            var eventInfo = model.Timesheet.FirstOrDefault(e => e.Date == cellModel.Date);
                
            return eventInfo != null ? eventInfo.Text : cellModel.Label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
