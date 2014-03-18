using SherpaDesk.Common;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SherpaDesk
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string FULL_NAME_FORMAT = "{1}, {0}";
        private const string AVATAR_URL_FORMAT = "https://www.gravatar.com/avatar/{0}?s=40";
        public MainPage()
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
            this.MainFrame.Navigate(typeof(Info));
            this.LoginNameButton.Content = string.Format(FULL_NAME_FORMAT, AppSettings.Current.FirstName, AppSettings.Current.LastName);
            this.Avatar.Source = new BitmapImage(
                new Uri(string.Format(AVATAR_URL_FORMAT, 
                    SherpaDesk.Common.Extensions.GetMD5(AppSettings.Current.Email)), 
                    UriKind.Absolute));

            base.OnNavigatedTo(e);
        }

        private void AddTimeClick(object sender, RoutedEventArgs e)
        {
            this.MainFrame.Navigate(typeof(AddTime));
        }

        private void AddTicketClick(object sender, RoutedEventArgs e)
        {
            this.MainFrame.Navigate(typeof(AddTicket));
        }

        private async void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog = new MessageDialog("Are you sure?");
            dialog.Commands.Add(new UICommand { Label = "Ok", Id = "ok" });
            dialog.Commands.Add(new UICommand { Label = "Cancel", Id = "cancel" });
            var confirmResult = await dialog.ShowAsync();
            if (confirmResult.Id.ToString() == "ok")
            {
                AppSettings.Current.Clear();
                this.Frame.Navigate(typeof(Login));
            }
        }

        private void LoginNameButton_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.Navigate(typeof(UpdateProfile));            
        }
    }
}
