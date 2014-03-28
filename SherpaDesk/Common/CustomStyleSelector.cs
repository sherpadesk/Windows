using SherpaDesk.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Windows.UI.Xaml;

namespace SherpaDesk.Common
{
    public class CustomStyleSelector : CalendarCellStyleSelector
    {
        public DataTemplate EventTemplate { get; set; }

        protected override void SelectStyleCore(CalendarCellStyleContext context, Telerik.UI.Xaml.Controls.Input.RadCalendar container)
        {
            if (container.DataContext != null && container.DataContext is IList<TimeLog>)
            {
                var events = container.DataContext as IList<TimeLog>;

                if (events.Any(e => e.Date.Date == context.Date.Date))
                {
                    context.CellTemplate = this.EventTemplate;
                }
            }
        }
    }

}
