using SherpaDesk.Common;
using SherpaDesk.Extensions;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Linq;
using Windows.UI.Core;
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
#if !DEBUG
            ModeBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
#endif
            base.OnNavigatedTo(e);
        }

        private async void SignIn(object sender, RoutedEventArgs e)
        {
            AppSettings.Current.Clear();

            AppSettings.Current.Beta = ModeBox.IsChecked ?? false;

            this.StartProgress();

            using (var connector = new Connector())
            {
                // authentication
                var resultLogin = await connector.Func<LoginRequest, LoginResponse>(
                        x => x.Login,
                        new LoginRequest
                        {
                            Email = UserNameTextbox.Text,
                            Password = PasswordTextBox.Password
                        });

                if (resultLogin.Status != eResponseStatus.Success)
                {
                    this.StopProgress();
                    
                    await this.HandleError(resultLogin);
                    
                    return;
                }

                AppSettings.Current.AddToken(
                    resultLogin.Result.ApiToken.IsNull("Invalid API Token"));

                // load organization and instance info
                var resultOrg = await connector.Func<OrganizationResponse[]>(
                        x => x.Organizations);

                this.StopProgress();

                if (resultOrg.Status != eResponseStatus.Success)
                {
                    await this.HandleError(resultOrg);
                }
                else
                {
                    var orgList = resultOrg.Result.ToList();

                    if (orgList.IsSingle())
                    {
                        var org = orgList.First();
                        var instance = org.Instances.First();
                        AppSettings.Current.AddOrganization(
                            org.Key.IsNull("Invalid Organiation Key"),
                            org.Name,
                            instance.Key.IsNull("Invalid Instance Key"),
                            instance.Name,
                            true);

                        // redirect to main page
                        this.Frame.Navigate(typeof(MainPage));
                    }
                    else
                    {
                        this.Frame.Navigate(typeof(Organization));
                    }
                }
            }
        }

        private void UserNameTextbox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Tab:
                    PasswordTextBox.Focus(FocusState.Keyboard);
                    break;
                case Windows.System.VirtualKey.Enter:
                    SignIn(this, new RoutedEventArgs());
                    break;
            }
        }

        private void PasswordTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Enter:
                    SignIn(this, new RoutedEventArgs());
                    break;
            }
        }

        private CoreCursor _cursor;

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

        private async void RegisterButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("http://www.sherpadesk.com/start-your-climb/"));
        }

        private void SignInButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 0.9;
        }

        private void SignInButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 1;
        }

        private void RegisterButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 0.9;
        }

        private void RegisterButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 1;
        }
    }
}
