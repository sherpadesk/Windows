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
        public AddTime()
        {
            this.InitializeComponent();
        }

        private async void Refresh()
        {
            DateField.Value = DateTime.Now;
            StartTimePicker.Time = EndTimePicker.Time = DateTime.Now.TimeOfDay;
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
                
                TaskTypeList.Items.Add(new ComboBoxItem
                {
                    Tag = Constants.INITIAL_ID,
                    Content = Constants.ADD_NEW_TASK_TYPE,
                    Foreground = new SolidColorBrush(Helper.HexStringToColor(Constants.CLICKABLE_COLOR))
                });

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

                AccountList.Items.Add(new ComboBoxItem
                {
                    Tag = Constants.INITIAL_ID,
                    Content = Constants.ADD_NEW_ACCOUNT,
                    Foreground = new SolidColorBrush(Helper.HexStringToColor(Constants.CLICKABLE_COLOR))
                });
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

        private void CalculateHours()
        {
            var time = EndTimePicker.Time - StartTimePicker.Time;
            HoursTextBox.Text = time.TotalHours >= 0 ? String.Format("{0:0.00}", time.TotalHours) : String.Format("{0:0.00}", 24 + time.TotalHours);
        }

        private void StartTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            CalculateHours();
        }

        private void EndTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            CalculateHours();
        }
    }
}
