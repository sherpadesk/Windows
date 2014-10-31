using System.Linq;
using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Response;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using SherpaDesk.Models.Request;
using SherpaDesk.Extensions;
using System.Threading.Tasks;

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

        private async void EditButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var timeId = (int)((Windows.UI.Xaml.FrameworkElement)(sender)).Tag;
            // TODO: этот блок надо перенести туда где будет присходить редактирование
            var time = list.First(); // данные надо будет получить с формы
            using (var connector = new Connector())
            {
                UpdateTimeRequest request = time.TicketId > 0
                    ? new UpdateTimeRequest(time.TicketId.ToString(), timeId)
                    : new UpdateTimeRequest(time.ProjectId, timeId);
                request.AccountId = time.AccountId;
                request.Billable = time.Billable;
                request.Date = time.Date;
                request.Hours = time.Hours;
                request.Note = time.Note + "1";
                request.TaskTypeId = time.TaskTypeId;
                request.TechnicianId = AppSettings.Current.UserId;

                var result = await connector.Action<UpdateTimeRequest>(x => x.Time, request);

                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                }
                else
                {
                    await RefreshGrid();
                }

            }
        }

        public async Task RefreshGrid()
        {
            using (var connector = new Connector())
            {
                var result = await connector.Func<TimeSearchRequest, TimeResponse[]>(
                            x => x.Time,
                            new TimeSearchRequest
                            {
                                TechnicianId = AppSettings.Current.UserId,
                                TimeType = eTimeType.Recent,
                                StartDate = DateTime.Now.AddDays(-7),
                                EndDate = DateTime.Now.AddDays(7)
                            });

                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                    return;
                }

                UpdateGrid(new ObservableCollection<TimeResponse>(result.Result.ToList()));
            }
        }

        private async void DeleteButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (await App.ConfirmMessage())
            {
                var time = (TimeResponse)((Windows.UI.Xaml.FrameworkElement)sender).DataContext;

                using (var connector = new Connector())
                {
                    var result = await connector.Action<DeleteTimeRequest>(x => x.Time, new DeleteTimeRequest(time.TimeId, time.ProjectId > 0));

                    if (result.Status != eResponseStatus.Success)
                    {
                        this.HandleError(result);
                    }
                    else
                    {
                        await RefreshGrid();
                    }
                }
            }
        }
    }
}
