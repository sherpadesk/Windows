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

namespace SherpaDesk
{
    public sealed partial class MainPage : Page
    {
        private const string AVATAR_URL_FORMAT = "https://www.gravatar.com/avatar/{0}?s=40";
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void MyProfileMenu_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.Navigate(typeof(UpdateProfile)); 
        }

        private async void LogOutMenu_Click(object sender, RoutedEventArgs e)
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.MainFrame.Navigate(typeof(Info));
            this.LoginNameButton.Content = Helper.FullName(AppSettings.Current.FirstName, AppSettings.Current.LastName);
            this.Avatar.Source = new BitmapImage(
                new Uri(string.Format(AVATAR_URL_FORMAT,
                    Helper.GetMD5(AppSettings.Current.Email)),
                    UriKind.Absolute));
        }
    }
}
