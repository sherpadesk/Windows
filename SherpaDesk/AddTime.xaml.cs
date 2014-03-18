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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SherpaDesk
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
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

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
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
                    Name = TECHNICIAN_COMBOBOXITEM_NAME + AppSettings.Current.UserId,
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
                        Name = ACCOUNT_COMBOBOXITEM_NAME + account.Id.ToString(),
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
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}
