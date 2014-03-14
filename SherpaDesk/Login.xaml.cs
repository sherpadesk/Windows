using SherpaDesk.Common;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
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
        }

        private async void SignIn(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var result = await connector.Operation<LoginRequest, LoginResponse>(
                    "login",
                    new LoginRequest
                    {
                        Username = UserNameTextbox.Text,
                        Password = PasswordTextBox.Password
                    });

                if (result.Status == eResponseStatus.Success)
                {
                    if (!string.IsNullOrEmpty(result.Data.ApiToken))
                    {
                        this.Frame.Navigate(typeof(MainPage));
                    }
                    else
                    {
                        //TODO: show : "Invalid API Token" error; 
                    }
                }
                else if (result.Status == eResponseStatus.Invalid)
                {
                    //TODO: show validation warnings on display form result.Messages
                }
                else if(result.Status == eResponseStatus.Fail)
                {
                    //TODO: show the sherpadesk.com error message on display form result.Message
                }
                else
                {
                    //TODO: show internal error on display form result.Message
                }
            }
        }
    }
}
