using SherpaDesk.Common;
using SherpaDesk.Extensions;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SherpaDesk
{
    public sealed partial class AddTime : SherpaDesk.Common.LayoutAwarePage
    {
        public AddTime()
        {
            this.InitializeComponent();
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            var date = DateTime.Now;
            DateField.Value = date;
            DateLabel.Text = date.ToString("MMMM dd, yyyy - dddd");
            StartTimePicker.Value = EndTimePicker.Value = date;
            StartTimeLabel.Text = date.ToString("t");
            EndTimeLabel.Text = date.ToString("t");
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
            }
        }

        private async void TechnicianList_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                }
            }
        }


        private async void AccountList_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                        Grid.SetColumn(TaskTypeList, 3);
                        Grid.SetColumn(TaskTypeLabel, 3);
                        ProjectList.FillData(resultProjects.Result.AsEnumerable());
                    }
                    else
                    {
                        ProjectLabel.Visibility = ProjectList.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        Grid.SetColumn(TaskTypeList, 2);
                        Grid.SetColumn(TaskTypeLabel, 2);
                        
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
                }
            }
        }

        private async void ProjectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

                    //TaskTypeList.Items.Add(new ComboBoxItem
                    //{
                    //    Tag = Constants.INITIAL_ID,
                    //    Content = Constants.ADD_NEW_TASK_TYPE,
                    //    Foreground = new SolidColorBrush(Helper.HexStringToColor(Constants.CLICKABLE_COLOR))
                    //});
                }
            }
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var hours = decimal.Zero;
                decimal.TryParse(HoursTextBox.Text, out hours);
                int taskType = TaskTypeList.GetSelectedValue<int>();
                var result = await connector.Action<AddTimeRequest>(
                    x => x.Time,
                    new AddTimeRequest
                    {
                        AccountId = AccountList.GetSelectedValue<int>(-1),
                        ProjectId = ProjectList.GetSelectedValue<int>(-1),
                        TaskTypeId = taskType,
                        TechnicianId = TechnicianList.GetSelectedValue<int>(),
                        Billable = BillableBox.IsChecked.HasValue ? BillableBox.IsChecked.Value : false,
                        Hours = hours,
                        Note = NoteTextBox.Text,
                        Date = DateField.Value ?? DateTime.Now
                    });

                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                }
                else
                {
                    AppSettings.Current.DefaultTaskType = taskType;
                    ((Frame)this.Parent).Navigate(typeof(Timesheet));
                }
            }
        }

        private void CalculateHours()
        {
            if (StartTimePicker.Value.HasValue && EndTimePicker.Value.HasValue)
            {
                var time = EndTimePicker.Value.Value.TimeOfDay - StartTimePicker.Value.Value.TimeOfDay;
                HoursTextBox.Text = time.TotalHours >= 0 ? String.Format("{0:0.00}", time.TotalHours) : String.Format("{0:0.00}", 24 + time.TotalHours);
            }
        }

        private void DateField_ValueChanged(object sender, EventArgs e)
        {
            DateLabel.Text = DateField.Value.Value.ToString("MMMM dd, yyyy - dddd");
        }

        private void StartTimePicker_ValueChanged(object sender, EventArgs e)
        {
            StartTimeLabel.Text = StartTimePicker.Value.Value.ToString("t");
            CalculateHours();
        }

        private void EndTimePicker_ValueChanged(object sender, EventArgs e)
        {
            EndTimeLabel.Text = EndTimePicker.Value.Value.ToString("t");
            CalculateHours();
        }

    }
}
