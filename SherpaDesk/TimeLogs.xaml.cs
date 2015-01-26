using SherpaDesk.Common;
using SherpaDesk.Extensions;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using SherpaDesk.Models.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SherpaDesk
{
    public sealed partial class TimeLogs : SherpaDesk.Common.LayoutAwarePage
    {
        public TimeLogs()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.DataContext = new TimeLogModel((ObservableCollection<TimeResponse>)e.Parameter);

            base.OnNavigatedTo(e);
        }

        public void UpdateGrid(ObservableCollection<TimeResponse> list)
        {
            ((TimeLogModel)DataContext).List = list;

            TicketTimeGrid.UpdateLayout();

            EditTimeGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void StartTimePicker_ValueChanged(object sender, EventArgs e)
        {
            StartTimeLabel.Text = StartTimePicker.Value.Value.ToString("t");
            CalculateHours();
        }

        private void CalculateHours()
        {
            if (StartTimePicker.Value.HasValue && EndTimePicker.Value.HasValue)
            {
                var time = EndTimePicker.Value.Value.TimeOfDay - StartTimePicker.Value.Value.TimeOfDay;
                HoursTextBox.Text = time.TotalHours >= 0 ? String.Format("{0:0.00}", time.TotalHours) : String.Format("{0:0.00}", 24 + time.TotalHours);
            }
        }

        private void EndTimePicker_ValueChanged(object sender, EventArgs e)
        {
            EndTimeLabel.Text = EndTimePicker.Value.Value.ToString("t");
            CalculateHours();
        }

        private async void AccountList_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            ComboBoxItem item = e.AddedItems.FirstOrDefault() as ComboBoxItem;
            if (item != null)
            {
                using (var connector = new Connector())
                {
                    // accounts
                    var resultProjects = await connector.Func<ProjectRequest, ProjectResponse[]>(x => x.Projects,
                        new ProjectRequest { AccountId = (int)item.Tag });

                    if (resultProjects.Status != eResponseStatus.Success)
                    {
                        await this.HandleError(resultProjects);
                        return;
                    }

                    if (resultProjects.Result.Length > 0)
                    {
                        ProjectLabel.Visibility = ProjectList.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        ProjectList.FillData(resultProjects.Result.AsEnumerable());
                    }
                    else
                    {
                        ProjectLabel.Visibility = ProjectList.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                        var resultTaskType = await connector.Func<TaskTypeRequest, NameResponse[]>(
                            x => x.TaskTypes, new TaskTypeRequest { AccountId = (int)item.Tag });

                        if (resultTaskType.Status != eResponseStatus.Success)
                        {
                            await this.HandleError(resultTaskType);
                            return;
                        }

                        TaskTypeList.FillData(resultTaskType.Result.AsEnumerable());

                        if (AppSettings.Current.DefaultTaskType != 0)
                        {
                            TaskTypeList.SetSelectedValue(AppSettings.Current.DefaultTaskType);
                        }
                    }
                    if (((TimeLogModel)DataContext).IsEdit)
                    {
                        ProjectList.SetSelectedValue(((TimeLogModel)DataContext).Time.ProjectId);
                        TaskTypeList.SetSelectedValue(((TimeLogModel)DataContext).Time.TaskTypeId);
                        AccountList.IsHitTestVisible = true;
                        ProjectList.IsHitTestVisible = true;
                        TechnicianList.IsHitTestVisible = true;
                        TaskTypeList.IsHitTestVisible = true;
                        ((TimeLogModel)DataContext).IsEdit = false;
                    }
                }
            }
        }

        private async void EditButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var time = ((Windows.UI.Xaml.FrameworkElement)(sender)).DataContext as TimeResponse;

            ((TimeLogModel)DataContext).Time = time;

            BillableBox.IsChecked = time.Billable;

            NoteTextBox.Text = time.Note;

            if (time.StartTime.HasValue)
            {
                StartTimePicker.Value = time.StartTime.Value;
                StartTimeLabel.Text = time.StartTime.Value.ToString("t");
            }
            else
            {
                StartTimePicker.Value = null;
                StartTimeLabel.Text = string.Empty;
            }
            if (time.StopTime.HasValue)
            {
                EndTimePicker.Value = time.StopTime.Value;
                EndTimeLabel.Text = time.StopTime.Value.ToString("t");
            }
            else
            {
                EndTimePicker.Value = null;
                EndTimeLabel.Text = string.Empty;
            }

            HoursTextBox.Text = time.Hours.ToString("0.00");

            EditTimeGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;

            using (var connector = new Connector())
            {
                // technician
                var resultTechnicians = await connector.Func<UserResponse[]>(x => x.Technicians);

                if (resultTechnicians.Status != eResponseStatus.Success)
                {
                    await this.HandleError(resultTechnicians);
                    return;
                }
                TechnicianList.FillData(
                    resultTechnicians.Result.Select(user => new NameResponse
                    {
                        Id = user.Id,
                        Name = Helper.FullName(user.FirstName, user.LastName, user.Email, true)
                    }));
                ((TimeLogModel)DataContext).IsEdit = true;
                AccountList.IsHitTestVisible = false;
                ProjectList.IsHitTestVisible = false;
                TechnicianList.IsHitTestVisible = false;
                TaskTypeList.IsHitTestVisible = false;
                TechnicianList.SetSelectedValue(time.UserId);
            }
        }

        public async Task RefreshGrid(DateTime date)
        {
            using (var connector = new Connector())
            {
                var result = await connector.Func<TimeSearchRequest, TimeResponse[]>(
                            x => x.Time,
                            new TimeSearchRequest
                            {
                                TechnicianId = AppSettings.Current.Configuration.User.Id,
                                TimeType = eTimeType.Recent,
                                StartDate = date,
                                EndDate = date
                            });

                if (result.Status != eResponseStatus.Success)
                {
                    await this.HandleError(result);
                    return;
                }

                UpdateGrid(new ObservableCollection<TimeResponse>(result.Result.Where(x => x.Date.Date == date.Date).ToList()));

                await App.ExternalAction(async x => await x.UpdateTimesheet(date.AddDays(date.Day * -1), DateTime.Now));
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
                        await this.HandleError(result);
                    }
                    else
                    {
                        await RefreshGrid(time.Date);
                    }
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var hours = decimal.Zero;
                decimal.TryParse(HoursTextBox.Text, out hours);

                var time = ((TimeLogModel)this.DataContext).Time;

                UpdateTimeRequest request = time.TicketId > 0
                    ? new UpdateTimeRequest(time.TicketId.ToString(), time.TimeId)
                    : new UpdateTimeRequest(time.ProjectId, time.TimeId);
                request.AccountId = AccountList.GetSelectedValue<int>();
                request.Billable = BillableBox.IsChecked.HasValue ? BillableBox.IsChecked.Value : false;
                request.Date = time.Date;
                request.Hours = hours;
                request.Note = NoteTextBox.Text;
                request.ProjectId = ProjectList.GetSelectedValue<int>();
                request.TaskTypeId = TaskTypeList.GetSelectedValue<int>();
                request.TechnicianId = TechnicianList.GetSelectedValue<int>();
                request.StartDate = StartTimePicker.Value;
                request.StopDate = EndTimePicker.Value;

                var result = await connector.Action<UpdateTimeRequest>(x => x.Time, request);

                if (result.Status != eResponseStatus.Success)
                {
                    await this.HandleError(result);
                }
                else
                {
                    await RefreshGrid(request.Date);
                }
            }
        }

        private void SubmitButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 0.9;
        }

        private void SubmitButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 1;
        }

        private void CancelButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            EditTimeGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void TechnicianList_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            ComboBoxItem item = e.AddedItems.FirstOrDefault() as ComboBoxItem;
            if (item != null)
            {
                using (var connector = new Connector())
                {
                    // accounts
                    var resultAccounts = await connector.Func<AccountSearchRequest, AccountResponse[]>(x => x.Accounts, new AccountSearchRequest
                    {
                        UserId = (int)item.Tag,
                        PageCount = SearchRequest.MAX_PAGE_COUNT
                    });

                    if (resultAccounts.Status != eResponseStatus.Success)
                    {
                        await this.HandleError(resultAccounts);
                        return;
                    }

                    AccountList.FillData(resultAccounts.Result.AsEnumerable());
                    if (((TimeLogModel)DataContext).IsEdit)
                    {
                        AccountList.SetSelectedValue(((TimeLogModel)DataContext).Time.AccountId);
                    }
                }
            }
        }

        private async void ProjectList_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            ComboBoxItem item = e.AddedItems.FirstOrDefault() as ComboBoxItem;
            if (item != null)
            {
                using (var connector = new Connector())
                {
                    // types
                    TaskTypeRequest request = new TaskTypeRequest();
                    int projectId = (int)item.Tag;
                    if (projectId > 0)
                    {
                        request.ProjectId = projectId;
                    }
                    else
                    {
                        request.AccountId = AccountList.GetSelectedValue<int>(-1);
                    }

                    var resultTaskType = await connector.Func<TaskTypeRequest, NameResponse[]>(
                        x => x.TaskTypes, request);

                    if (resultTaskType.Status != eResponseStatus.Success)
                    {
                        await this.HandleError(resultTaskType);
                        return;
                    }

                    TaskTypeList.FillData(resultTaskType.Result.AsEnumerable());

                    if (AppSettings.Current.DefaultTaskType != 0)
                    {
                        TaskTypeList.SetSelectedValue(AppSettings.Current.DefaultTaskType);
                    }
                }
            }

        }

        private void CancelButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((TextBlock)sender).Opacity = 0.6;
        }

        private void CancelButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((TextBlock)sender).Opacity = 1;
        }
    }
}
