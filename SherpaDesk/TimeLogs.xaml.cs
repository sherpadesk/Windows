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
using Windows.UI.Xaml.Controls;

namespace SherpaDesk
{
    public sealed partial class TimeLogs : SherpaDesk.Common.LayoutAwarePage
    {
        private ObservableCollection<TimeResponse> list;
        private TimeResponse selectedTime;
        private bool isEdit;

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
                        this.HandleError(resultProjects);
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
                            this.HandleError(resultTaskType);
                            return;
                        }

                        TaskTypeList.FillData(resultTaskType.Result.AsEnumerable());

                        if (AppSettings.Current.DefaultTaskType != 0)
                        {
                            TaskTypeList.SetSelectedValue(AppSettings.Current.DefaultTaskType);
                        }
                    }
                    if (isEdit)
                    {
                        ProjectList.SetSelectedValue(selectedTime.ProjectId);
                        TaskTypeList.SetSelectedValue(selectedTime.TaskTypeId);
                        AccountList.IsHitTestVisible = true;
                        ProjectList.IsHitTestVisible = true;
                        TechnicianList.IsHitTestVisible = true;
                        TaskTypeList.IsHitTestVisible = true;
                        isEdit = false;
                    }
                }
            }
        }

        private async void EditButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            selectedTime = ((Windows.UI.Xaml.FrameworkElement)(sender)).DataContext as TimeResponse;
            BillableBox.IsChecked = selectedTime.Billable;
            NoteTextBox.Text = selectedTime.Note;
            StartTimePicker.Value = selectedTime.StartTime;
            StartTimeLabel.Text = StartTimePicker.Value.Value.ToString("t");
            EndTimePicker.Value = selectedTime.StopTime;
            EndTimeLabel.Text = EndTimePicker.Value.Value.ToString("t");
            HoursTextBox.Text = selectedTime.Hours.ToString("0.00");
            EditTimeGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            using (var connector = new Connector())
            {
                // technician
                var resultTechnicians = await connector.Func<UserResponse[]>(x => x.Technicians);

                if (resultTechnicians.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTechnicians);
                    return;
                }
                TechnicianList.FillData(
                resultTechnicians.Result.Select(user => new NameResponse { Id = user.Id, Name = Helper.FullName(user.FirstName, user.LastName, user.Email, true) }),
                new NameResponse { Id = AppSettings.Current.Configuration.User.Id, Name = Constants.TECHNICIAN_ME });
                isEdit = true;
                AccountList.IsHitTestVisible = false;
                ProjectList.IsHitTestVisible = false;
                TechnicianList.IsHitTestVisible = false;
                TaskTypeList.IsHitTestVisible = false;
                TechnicianList.SetSelectedValue(selectedTime.UserId);
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
                    this.HandleError(result);
                    return;
                }

                App.ExternalAction(x => x.UpdateTimesheet(date.AddDays(date.Day*-1), DateTime.Now));

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

                UpdateTimeRequest request = selectedTime.TicketId > 0
                    ? new UpdateTimeRequest(selectedTime.TicketId.ToString(), selectedTime.TimeId)
                    : new UpdateTimeRequest(selectedTime.ProjectId, selectedTime.TimeId);
                request.AccountId = AccountList.GetSelectedValue<int>();
                request.Billable = BillableBox.IsChecked.HasValue ? BillableBox.IsChecked.Value : false;
                request.Date = selectedTime.Date;
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
                    this.HandleError(result);
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
                        this.HandleError(resultAccounts);
                        return;
                    }

                    AccountList.FillData(resultAccounts.Result.AsEnumerable());
                    if (isEdit)
                    {
                        AccountList.SetSelectedValue(selectedTime.AccountId);
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
                        this.HandleError(resultTaskType);
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
