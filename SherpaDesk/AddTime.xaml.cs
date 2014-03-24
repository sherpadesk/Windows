using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SherpaDesk
{
    public sealed partial class AddTime : SherpaDesk.Common.LayoutAwarePage
    {
        private const string ADD_NEW_TASK_TYPE = "Add New Task Type...";
        private const string ADD_NEW_ACCOUNT = "Add New Account...";
        private const string TECHNICIAN_ME = "Technician Me";
        private const string CLICKABLE_COLOR = "FF00A10A";
        private const int INITIAL_ID = 0;
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
                TaskTypeList.Items.Clear();
                foreach (var taskType in resultTaskType.Result)
                {
                    TaskTypeList.Items.Add(new ComboBoxItem
                    {
                        Tag = taskType.Id,
                        Content = taskType.Name
                    });
                }
                TaskTypeList.Items.Add(new ComboBoxItem
                {
                    Tag = INITIAL_ID,
                    Content = ADD_NEW_TASK_TYPE,
                    Foreground = new SolidColorBrush(Helper.HexStringToColor(CLICKABLE_COLOR))
                });

                // technician
                var resultUsers = await connector.Func<UserResponse[]>("users");

                if (resultUsers.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultUsers);
                    return;
                }
                TechnicianList.Items.Clear();
                TechnicianList.Items.Add(new ComboBoxItem
                {
                    Tag = AppSettings.Current.UserId,
                    Content = TECHNICIAN_ME
                });
                foreach (var user in resultUsers.Result)
                {
                    if (user.Id != AppSettings.Current.UserId)
                    {
                        TechnicianList.Items.Add(new ComboBoxItem
                        {
                            Tag = user.Id,
                            Content = Helper.FullName(user.FirstName, user.LastName)
                        });
                    }
                }

                // accounts
                var resultAccounts = await connector.Func<AccountResponse[]>("accounts");

                if (resultAccounts.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultAccounts);
                    return;
                }

                AccountList.Items.Clear();
                foreach (var account in resultAccounts.Result)
                {
                    AccountList.Items.Add(new ComboBoxItem
                    {
                        Tag = account.Id,
                        Content = account.Name
                    });
                }
                AccountList.Items.Add(new ComboBoxItem
                {
                    Tag = INITIAL_ID,
                    Content = ADD_NEW_ACCOUNT,
                    Foreground = new SolidColorBrush(Helper.HexStringToColor(CLICKABLE_COLOR))
                });
            }
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private int GetSelectedValue(ComboBox comboBox)
        {
            if (comboBox.SelectedIndex > -1)
            {
                return (int)((ComboBoxItem)comboBox.Items[comboBox.SelectedIndex]).Tag;
            }
            else
                return 0;
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
                        AccountId = this.GetSelectedValue(AccountList),
                        TaskTypeId = this.GetSelectedValue(TaskTypeList),
                        TechnicianId = this.GetSelectedValue(TechnicianList),
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
            HoursTextBox.Text = time.TotalHours >= 0 ? String.Format("{0:0.00}", time.TotalHours) : String.Format("{0:0.00}", 24+time.TotalHours);
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
