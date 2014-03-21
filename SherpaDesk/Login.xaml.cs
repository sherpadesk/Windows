using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SherpaDesk
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        public Login()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void SignIn(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                AppSettings.Current.Clear();

                // authentication
                var resultLogin = await connector.Operation<LoginRequest, LoginResponse>(
                        "login",
                        new LoginRequest
                        {
                            Email = UserNameTextbox.Text,
                            Password = PasswordTextBox.Password
                        });

                if (resultLogin.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultLogin);
                    return;
                }
                if (string.IsNullOrEmpty(resultLogin.Result.ApiToken))
                {
                    MessageDialog dialog = new MessageDialog("Invalid API Token", "Error");
                    await dialog.ShowAsync();
                    return;
                }

                AppSettings.Current.ApiToken = resultLogin.Result.ApiToken;
                AppSettings.Current.Email = UserNameTextbox.Text;

                // load organization and instance info
                var resultOrg = await connector.Operation<OrganizationResponse[]>(
                        "organizations");
                
                if (resultOrg.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultOrg);
                    return;
                }

                var org = resultOrg.Result.DefaultIfEmpty(new OrganizationResponse()).First();
                AppSettings.Current.OrganizationKey = org.Key;
                AppSettings.Current.OrganizationName = org.Name;

                var instance = org.Instances.DefaultIfEmpty(new InstanceResponse()).First();
                
                AppSettings.Current.InstanceKey = instance.Key;
                AppSettings.Current.InstanceName = instance.Name;

                //load user info
                var resultUser = await connector.Operation<UserSearchRequest, UserResponse[]>(
                    "users",
                    new UserSearchRequest { Email = UserNameTextbox.Text });

                if (resultUser.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultUser);
                    return;
                }
                if (resultUser.Result == null || resultUser.Result.Length == 0)
                {
                    MessageDialog dialog = new MessageDialog("Cannot found useer", "Error");
                    await dialog.ShowAsync();
                    return;
                }
                var user = resultUser.Result.First();

                AppSettings.Current.UserId = user.Id;
                AppSettings.Current.FirstName = user.FirstName;
                AppSettings.Current.LastName = user.LastName;
                AppSettings.Current.Role = user.Role;

                // redirect to main page
                this.Frame.Navigate(typeof(MainPage));

            }
        }
    }
}
