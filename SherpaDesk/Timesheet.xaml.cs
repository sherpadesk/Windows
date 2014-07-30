using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SherpaDesk
{
    public sealed partial class Timesheet : SherpaDesk.Common.LayoutAwarePage
    {
        public event EventHandler MoveScrollToRight;

        public Timesheet()
        {
            this.InitializeComponent();
            this.Model.OnDataLoading += async (object sender, TimesheetEventArgs e) => await TimesheetLoad(sender, e);
        }

        #region Handlers

        private async Task TimesheetLoad(object sender, TimesheetEventArgs e)
        {
            using (var connector = new Connector())
            {
                var result = await connector.Func<TimeSearchRequest, TimeResponse[]>(
                    x => x.Time,
                    new TimeSearchRequest
                    {
                        TechnicianId = AppSettings.Current.UserId,
                        TimeType = eTimeType.Recent,
                        StartDate = e.StartDate.AddDays(-7),
                        EndDate = e.EndDate.AddDays(7)
                    });
                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                    return;
                }

                this.Model.TimeLogList = new ObservableCollection<TimeResponse>(result.Result.ToList());

                this.TimesheetCalendar.DisplayDateStart = e.StartDate.AddYears(-3);
                this.TimesheetCalendar.DisplayDateEnd = e.EndDate.AddYears(3);
            }
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            var date = DateTime.Now;
            StartTimePicker.Value = EndTimePicker.Value = date;
            StartTimeLabel.Text = EndTimeLabel.Text = date.ToString("t");
            DateLabel.Text = date.ToString("MMMM dd, yyyy - dddd");

            await TimesheetLoad(this, new TimesheetEventArgs(this.Model));

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
                    new NameResponse { Id = AppSettings.Current.UserId, Name = Constants.TECHNICIAN_ME });
            }
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
                }
            }
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

                    //TaskTypeList.Items.Add(new ComboBoxItem
                    //{
                    //    Tag = Constants.INITIAL_ID,
                    //    Content = Constants.ADD_NEW_TASK_TYPE,
                    //    Foreground = new SolidColorBrush(Helper.HexStringToColor(Constants.CLICKABLE_COLOR))
                    //});
                }
            }
        }


        private void DateField_ValueChanged(object sender, EventArgs e)
        {
            //if (DateField.Value.HasValue)
            //{
            //    TimesheetCalendar.SelectionChanged -= TimesheetCalendar_SelectionChanged;
            //    var selectedDate = DateField.Value.Value;
            //    DateLabel.Text = DateField.Value.Value.ToString("MMMM dd, yyyy - dddd");
            //    TimesheetCalendar.SelectedDateRange = new CalendarDateRange(selectedDate, selectedDate);
            //    TimesheetCalendar.SelectionChanged += TimesheetCalendar_SelectionChanged;
            //    FillTimesheetGrid(selectedDate);
            //}
        }

        private void TimesheetCalendar_SelectionChanged(object sender, EventArgs e)
        {
            //DateField.ValueChanged -= DateField_ValueChanged;
            var selectedDate = TimesheetCalendar.SelectedDateRange.Value.StartDate.Date;
            //DateField.Value = selectedDate;
            DateLabel.Text = selectedDate.ToString("MMMM dd, yyyy - dddd");
            //DateField.ValueChanged += DateField_ValueChanged;
            FillTimesheetGrid(selectedDate);
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

        private void TimeTicketId_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var hours = decimal.Zero;
            decimal.TryParse(HoursTextBox.Text, out hours);
            //            var date = DateField.Value ?? DateTime.Now;
            var date = DateTime.Now;
            var taskType = TaskTypeList.GetSelectedValue<int>();
            using (var connector = new Connector())
            {
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
                                Date = date
                            });

                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                    return;
                }
            }

            AppSettings.Current.DefaultTaskType = taskType;

            NoteTextBox.Text = string.Empty;
            HoursTextBox.Text = "0.00";

            await TimesheetLoad(this, new TimesheetEventArgs(this.Model));

            this.FillTimesheetGrid(date);
        }
        #endregion

        #region Methods

        private void FillTimesheetGrid(DateTime date)
        {
            this.Model.CurrentDate = date;
            this.TimeLogsFrame.Navigate(typeof(TimeLogs));
            //NonTicketsGrid.ItemsSource = this.Model.NonTicketsList;
            //NonTicketsGrid.UpdateLayout();
            //NonTicketsLabel.Visibility = NonTicketsGrid.Visibility = this.Model.VisibleNonTickets;

            //TicketTimeGrid.ItemsSource = this.Model.TicketTimeList;
            //TicketTimeGrid.UpdateLayout();
            //TicketTimeLabel.Visibility = TicketTimeGrid.Visibility = this.Model.VisibleTicketTime;

            //if ((this.Model.VisibleNonTickets & this.Model.VisibleTicketTime) == Windows.UI.Xaml.Visibility.Visible)
            //{
            //    TimesheetGrids.Visibility = Visibility.Visible;
            //    if (MoveScrollToRight != null)
            //    {
            //        MoveScrollToRight(this, new EventArgs());
            //    }
            //}
            //else
            //    TimesheetGrids.Visibility = Visibility.Collapsed;
        }

        private void CalculateHours()
        {
            if (StartTimePicker.Value.HasValue && EndTimePicker.Value.HasValue)
            {
                var time = EndTimePicker.Value.Value.TimeOfDay - StartTimePicker.Value.Value.TimeOfDay;
                HoursTextBox.Text = time.TotalHours >= 0 ? String.Format("{0:0.00}", time.TotalHours) : String.Format("{0:0.00}", 24 + time.TotalHours);
            }
        }
        #endregion

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
