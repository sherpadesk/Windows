using System;
using System.Linq;
using SherpaDesk.Common;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
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
                var resultLogin = await connector.Operation<LoginRequest, LoginResponse>(
                        "login",
                        new LoginRequest
                        {
                            Username = UserNameTextbox.Text,
                            Password = PasswordTextBox.Password
                        });

                if (resultLogin.Status == eResponseStatus.Success)
                {
                    if (!string.IsNullOrEmpty(resultLogin.Data.ApiToken))
                    {
                        AppSettings.Current.ApiToken = resultLogin.Data.ApiToken;
                        AppSettings.Current.Username = UserNameTextbox.Text;

                        var resultOrg = await connector.Operation<OrganizationResponse[]>(
                                "organizations");

                        if (resultOrg.Status == eResponseStatus.Success)
                        {
                            var org = resultOrg.Data.DefaultIfEmpty(new OrganizationResponse()).First();
                            AppSettings.Current.OrganizationKey = org.Key;
                            AppSettings.Current.InstanceKey = org.Instances.DefaultIfEmpty(new InstanceResponse()).First().Key;
                            this.Frame.Navigate(typeof(MainPage));
                        }
                        else
                        {
                            this.HandleError(resultOrg);
                        }
                    }
                    else
                    {
                        MessageDialog dialog = new MessageDialog("Invalid API Token", "Error");
                        await dialog.ShowAsync();
                    }
                }
                else
                {
                    this.HandleError(resultLogin);
                }
            }
        }
    }
}
