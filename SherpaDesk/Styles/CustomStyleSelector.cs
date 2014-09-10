using System.Linq;
using Windows.UI.Xaml;
using SherpaDesk.Models.ViewModels;
using Telerik.UI.Xaml.Controls.Input.Calendar;

namespace SherpaDesk.Styles
{
    public class CustomStyleSelector : CalendarCellStyleSelector
    {
        public DataTemplate EventTemplate { get; set; }
        public DataTemplate YearTemplate { get; set; }

        protected override void SelectStyleCore(CalendarCellStyleContext context, Telerik.UI.Xaml.Controls.Input.RadCalendar container)
        {
            if (container.DataContext is TimesheetViewModel)
            {
                var model = container.DataContext as TimesheetViewModel;

                if (model.Timesheet == null) return;
                
                switch (container.DisplayMode)
                {
                    case Telerik.UI.Xaml.Controls.Input.CalendarDisplayMode.MonthView:
                        if (model.Timesheet.Any(e => e.Date.Date == context.Date.Date))
                        {
                            context.CellTemplate = this.EventTemplate;
                        }
                        break;
                    case Telerik.UI.Xaml.Controls.Input.CalendarDisplayMode.YearView:
                        context.CellTemplate = this.YearTemplate;
                        break;
                }
            }
        }
    }

}
