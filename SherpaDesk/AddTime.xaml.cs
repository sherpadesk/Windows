using SherpaDesk.Common;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SherpaDesk
{
    public sealed partial class AddTime : SherpaDesk.Common.LayoutAwarePage
    {
        private const string TASKTYPE_COMBOBOXITEM_NAME = "TaskTypeItem_";
        private const string TECHNICIAN_COMBOBOXITEM_NAME = "TechnicianItem_";
        private const string ACCOUNT_COMBOBOXITEM_NAME = "AccountItem_";
        private const string ADD_NEW_TASK_TYPE = "Add New Task Type...";
        private const string ADD_NEW_ACCOUNT = "Add New Account...";
        private const string TECHNICIAN_ME = "Technician Me";
        private const string CLICKABLE_COLOR = "FF00A10A";
        private const int INITIAL_ID = 0;
        public AddTime()
        {
            this.InitializeComponent();
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            DateLabel.Text = DateTime.Now.ToString("MMMM dd, yyyy - dddd");
            StartTime.Time = EndTime.Time = DateTime.Now.TimeOfDay;
            using (var connector = new Connector())
            {
                // types
                var resultTaskType = await connector.Operation<TaskTypeRequest, NameResponse[]>(
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
                        Name = TASKTYPE_COMBOBOXITEM_NAME + taskType.Id.ToString(),
                        Content = taskType.Name
                    });
                }
                TaskTypeList.Items.Add(new ComboBoxItem
                {
                    Name = TASKTYPE_COMBOBOXITEM_NAME + INITIAL_ID.ToString(),
                    Content = ADD_NEW_TASK_TYPE,
                    Foreground = new SolidColorBrush(Helper.HexStringToColor(CLICKABLE_COLOR))
                });

                // technician
                var resultUsers = await connector.Operation<UserSearchRequest, UserResponse[]>(
                    "users",
                    new UserSearchRequest());

                if (resultUsers.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultUsers);
                    return;
                }
                TechnicianList.Items.Clear();
                TechnicianList.Items.Add(new ComboBoxItem
                {
                    Name = TECHNICIAN_COMBOBOXITEM_NAME + AppSettings.Current.UserId, // Please use .Tag property for UserId
                    Content = TECHNICIAN_ME
                });
                foreach (var user in resultUsers.Result)
                {
                    if (user.Id != AppSettings.Current.UserId)
                    {
                        TechnicianList.Items.Add(new ComboBoxItem
                        {
                            Name = TECHNICIAN_COMBOBOXITEM_NAME + user.Id.ToString(),
                            Content = Helper.FullName(user.FirstName, user.LastName)
                        });
                    }
                }

                // accounts
                var resultAccounts = await connector.Operation<AccountSearchRequest, AccountResponse[]>(
                    "accounts",
                    new AccountSearchRequest());

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
                        Name = ACCOUNT_COMBOBOXITEM_NAME + account.Id.ToString(), // Please use .Tag property for account.Id
                        Content = account.Name
                    });
                }
                AccountList.Items.Add(new ComboBoxItem
                {
                    Name = ACCOUNT_COMBOBOXITEM_NAME + INITIAL_ID.ToString(),
                    Content = ADD_NEW_ACCOUNT,
                    Foreground = new SolidColorBrush(Helper.HexStringToColor(CLICKABLE_COLOR))
                });
            }
        }
    }
}
