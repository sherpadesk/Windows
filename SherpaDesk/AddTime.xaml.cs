using SherpaDesk.Common;
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
        private const string ERROR_EMPTY_HOURS = "Hours should be positive number.";
        private const string ERROR_MUCH_HOURS = "Hours cannot be more then 24 hours in day.";

        public AddTime()
        {
            this.InitializeComponent();
        }

        private async void Refresh()
        {
            var date = DateTime.Now;
            DateField.Value = date;
            DateLabel.Text = date.ToString("MMMM dd, yyyy - dddd");
            StartTimePicker.Value = EndTimePicker.Value = date;
            StartTimeLabel.Text = date.ToString("t");
            EndTimeLabel.Text = date.ToString("t");
            using (var connector = new Connector())
            {
                // types
                var resultTaskType = await connector.Func<TaskTypeRequest, NameResponse[]>(
                    "task_types",
                    new TaskTypeRequest());

                if (resultTaskType.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTaskType);
                    return;
                }

                TaskTypeList.FillData(resultTaskType.Result.AsEnumerable());
                
                //TaskTypeList.Items.Add(new ComboBoxItem
                //{
                //    Tag = Constants.INITIAL_ID,
                //    Content = Constants.ADD_NEW_TASK_TYPE,
                //    Foreground = new SolidColorBrush(Helper.HexStringToColor(Constants.CLICKABLE_COLOR))
                //});

                // technician
                var resultUsers = await connector.Func<UserResponse[]>("users");

                if (resultUsers.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultUsers);
                    return;
                }
                
                TechnicianList.FillData(
                    resultUsers.Result.Select(user => new NameResponse { Id = user.Id, Name = Helper.FullName(user.FirstName, user.LastName) }),
                    new NameResponse { Id = AppSettings.Current.UserId, Name = Constants.TECHNICIAN_ME });

                // accounts
                var resultAccounts = await connector.Func<AccountResponse[]>("accounts");

                if (resultAccounts.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultAccounts);
                    return;
                }

                AccountList.FillData(resultAccounts.Result.AsEnumerable());

                //AccountList.Items.Add(new ComboBoxItem
                //{
                //    Tag = Constants.INITIAL_ID,
                //    Content = Constants.ADD_NEW_ACCOUNT,
                //    Foreground = new SolidColorBrush(Helper.HexStringToColor(Constants.CLICKABLE_COLOR))
                //});
            }
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var hours = decimal.Zero;
                decimal.TryParse(HoursTextBox.Text, out hours);
                if (hours == 0)
                {
                    ErrorHours.Text = ERROR_EMPTY_HOURS;
                }
                else if (hours > 24)
                {
                    ErrorHours.Text = ERROR_MUCH_HOURS;
                }
                else
                {
                    ErrorHours.Text = string.Empty;
                    var result = await connector.Action<AddTimeRequest>(
                        "time",
                        new AddTimeRequest
                        {
                            AccountId = AccountList.GetSelectedValue<int>(),
                            TaskTypeId = TaskTypeList.GetSelectedValue<int>(),
                            TechnicianId = TechnicianList.GetSelectedValue<int>(),
                            Billable = BillableBox.IsChecked.HasValue ? BillableBox.IsChecked.Value : false,
                            Hours = hours,
                            Note = NoteTextBox.Text
                        });

                    if (result.Status != eResponseStatus.Success)
                    {
                        this.HandleError(result);
                    }
                    else
                    {
                        ((Frame)this.Parent).Navigate(typeof(Timesheet));
                    }
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
