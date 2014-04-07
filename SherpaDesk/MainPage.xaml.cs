﻿using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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

        private CoreCursor _cursor;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void MyProfileMenu_Click(object sender, RoutedEventArgs e)
        {
            this.MainFrame.Navigate(typeof(UpdateProfile));
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

        private async void LogOutMenu_Click(object sender, RoutedEventArgs e)
        {
            if (AppSettings.Current.Single)
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
                    "users",
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

                    this.LoginNameButton.Content = Helper.FullName(user.FirstName, user.LastName, user.Email);

                    this.Avatar.Source = new BitmapImage(
                        new Uri(string.Format(AVATAR_URL_FORMAT,
                            Helper.GetMD5(user.Email)),
                            UriKind.Absolute));

                    this.MainFrame.Navigate(typeof(Info));
                }
                else
                {
                    this.HandleError(resultUser);

                    AppSettings.Current.Clear();
                    this.Frame.Navigate(typeof(Login));
                }
            }

        }
    }
}
