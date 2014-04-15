using SherpaDesk.Models;
using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Windows.UI.Xaml;

namespace SherpaDesk.Common
{
    public class CustomStyleSelector : CalendarCellStyleSelector
    {
        public DataTemplate EventTemplate { get; set; }

        protected override void SelectStyleCore(CalendarCellStyleContext context, Telerik.UI.Xaml.Controls.Input.RadCalendar container)
        {
            if (container.DataContext != null && container.DataContext is TimesheetViewModel)
            {
                var model = container.DataContext as TimesheetViewModel;

                if (model.Timesheet != null && model.Timesheet.Any(e => e.Date.Date == context.Date.Date))
                {
                    context.CellTemplate = this.EventTemplate;
                }
            }
        }
    }

}
