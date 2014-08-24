using System.Linq;
using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Response;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;

namespace SherpaDesk
{
    public sealed partial class TimeLogs : SherpaDesk.Common.LayoutAwarePage
    {
        private ObservableCollection<TimeResponse> list;

        public TimeLogs()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            list = (ObservableCollection<TimeResponse>)e.Parameter;
            UpdateGrid(list);
            base.OnNavigatedTo(e);
        }

        public void UpdateGrid(ObservableCollection<TimeResponse> list)
        {
            TicketTimeGrid.ItemsSource = list;
            TicketTimeGrid.UpdateLayout();
        }
    }
}
