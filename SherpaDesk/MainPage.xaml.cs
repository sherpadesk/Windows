using SherpaDesk.Common;
using SherpaDesk.Extensions;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System.Linq;
using System.Reflection;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace SherpaDesk
{
    public sealed partial class MainPage : Page
    {
        private const string AVATAR_URL_FORMAT = "https://www.gravatar.com/avatar/{0}?s=40";
        private const string USER_INFO_FORMAT_WITH_INSTANCE = "{0} {1}";
        private const string USER_INFO_FORMAT_SINGLE = "{0}";

        private CoreCursor _cursor;

        public ScrollViewer ScrollViewer { get { return scrollViewer; } }

        public Frame WorkDetailsFrame { get { return workDetailsFrame; } }

        public Frame WorkListFrame { get { return workListFrame; } }

        public Frame TimeSheetFrame { get { return timesheetFrame; } }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void MyProfileMenu_Click(object sender, RoutedEventArgs e)
        {
            this.infoFrame.Navigate(typeof(UpdateProfile));
        }

        public void ShowFullScreenImage(ImageSource image)
        {
            ImageFull.Source = image;
            FullscreenPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        public void StartProgress()
        {
            progressRing.IsActive = true;
            _cursor = Window.Current.CoreWindow.PointerCursor.Type != Windows.UI.Core.CoreCursorType.Wait ?
                Window.Current.CoreWindow.PointerCursor :
                new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Wait, 2);
            Window.Current.CoreWindow.IsInputEnabled = false;
        }

        public void StopProgress()
        {
            progressRing.IsActive = false;
            Window.Current.CoreWindow.PointerCursor = _cursor ?? new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
            Window.Current.CoreWindow.IsInputEnabled = true;
        }

        public async void UpdateInfo()
        {
            Info info = this.infoFrame.Content as Info;
            if (info != null)
            {
                await info.RefreshData();
            }
        }

        private async void LogOutMenu_Click(object sender, RoutedEventArgs e)
        {
            if (AppSettings.Current.Single)
            {
                if (await App.ConfirmMessage())
                {
                    AppSettings.Current.Clear();
                    this.Frame.Navigate(typeof(Login));
                }
            }
            else
            {
                this.Frame.Navigate(typeof(Organization));
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {

                //load user info
                var resultUser = await connector.Func<UserSearchRequest, UserResponse[]>(
                    x => x.Users,
                    new UserSearchRequest { Email = AppSettings.Current.Email });

                if (resultUser.Status == eResponseStatus.Success)
                {
                    if (resultUser.Result.Length == 0)
                        throw new InternalException("User not found", eErrorType.InvalidOutputData);

                    var user = resultUser.Result.First();

                    AppSettings.Current.AddUser(
                        user.Id.IsNull("Invalid user identifier"),
                        user.FirstName,
                        user.LastName,
                        user.Role.IsNull("Invalid role"));

                    //                    this.LoginNameButton.Content = string.Format("{0} {1}", user.FirstName, user.LastName);
                    ////                    this.LoginNameButton.Content = string.Format(AppSettings.Current.Single ? USER_INFO_FORMAT_SINGLE : USER_INFO_FORMAT_WITH_INSTANCE, Helper.FullName(user.FirstName, user.LastName, user.Email), AppSettings.Current.OrganizationName, AppSettings.Current.InstanceName);
                    //                    OrgName.Text = string.Format(AppSettings.Current.Single ? USER_INFO_FORMAT_SINGLE : USER_INFO_FORMAT_WITH_INSTANCE, AppSettings.Current.OrganizationName, AppSettings.Current.InstanceName);
                    //                    this.Avatar.Source = new BitmapImage(
                    //                        new Uri(string.Format(AVATAR_URL_FORMAT,
                    //                            Helper.GetMD5(user.Email)),
                    //                            UriKind.Absolute));

                    //                    this.timesheetFrame.Navigate(typeof(Timesheet));
                    this.workListFrame.Navigate(typeof(WorkList), eWorkListType.Open);
                    this.infoFrame.Navigate(typeof(Info));
                }
                else
                {
                    this.HandleError(resultUser);

                    AppSettings.Current.Clear();
                    this.Frame.Navigate(typeof(Login));
                }
            }
#if DEBUG
            BuildNumber.Visibility = Visibility.Visible;
            BuildNumber.Text = this.GetType().GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
#else
            BuildNumber.Visibility = Visibility.Collapsed;
#endif
        }



        private void FullscreenPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FullscreenPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}
