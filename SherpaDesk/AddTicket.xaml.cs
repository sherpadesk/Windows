using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SherpaDesk
{
    public sealed partial class AddTicket : SherpaDesk.Common.LayoutAwarePage
    {
        public AddTicket()
        {
            this.InitializeComponent();
        }

        private async void filepickButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            var files = await openPicker.PickMultipleFilesAsync();
            if (files != null && files.Count > 0)
            {
                SelectedFilesList.Text = "Picked photos: ";
                List<string> fileNames = new List<string>();
                foreach (var file in files)
                {
                    fileNames.Add(file.Name);
                }
                SelectedFilesList.Text += string.Join(", ", fileNames.ToArray());
            }
            else
            {
                SelectedFilesList.Text = string.Empty;
            }
        }

        private void EndUserMeLink_Click(object sender, RoutedEventArgs e)
        {
            EndUserList.SetSelectedValue(AppSettings.Current.UserId);
        }

        private void TechnicianMeLink_Click(object sender, RoutedEventArgs e)
        {
            TechnicianList.SetSelectedValue(AppSettings.Current.UserId);
        }

        private void AlternateTechMeLink_Click(object sender, RoutedEventArgs e)
        {
            AlternateTechnicianList.SetSelectedValue(AppSettings.Current.UserId);
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                // users
                var resultUsers = await connector.Func<UserResponse[]>("users");

                if (resultUsers.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultUsers);
                    return;
                }

                TechnicianList.FillData(
                    resultUsers.Result.Select(user => new NameResponse { Id = user.Id, Name = Helper.FullName(user.FirstName, user.LastName) }),
                    new NameResponse { Id = -1, Name = "Let the system choose." },
                    new NameResponse { Id = AppSettings.Current.UserId, Name = Constants.TECHNICIAN_ME });

                AlternateTechnicianList.FillData(
                    resultUsers.Result.Select(user => new NameResponse { Id = user.Id, Name = Helper.FullName(user.FirstName, user.LastName) }),
                    new NameResponse { Id = AppSettings.Current.UserId, Name = Constants.TECHNICIAN_ME });

                EndUserList.FillData(
                    resultUsers.Result.Select(user => new NameResponse { Id = user.Id, Name = Helper.FullName(user.FirstName, user.LastName) }),
                    new NameResponse { Id = AppSettings.Current.UserId, Name = Constants.USER_ME });

                EndUserList.Items.Add(new ComboBoxItem
                {
                    Tag = Constants.INITIAL_ID,
                    Content = Constants.ADD_NEW_USER,
                    Foreground = new SolidColorBrush(Helper.HexStringToColor(Constants.CLICKABLE_COLOR))
                });

                // accounts
                var resultAccounts = await connector.Func<AccountResponse[]>("accounts");

                if (resultAccounts.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultAccounts);
                    return;
                }

                AccountList.FillData(resultAccounts.Result.AsEnumerable());

                AccountList.Items.Add(new ComboBoxItem
                {
                    Tag = Constants.INITIAL_ID,
                    Content = Constants.ADD_NEW_ACCOUNT,
                    Foreground = new SolidColorBrush(Helper.HexStringToColor(Constants.CLICKABLE_COLOR))
                });
                //classes

                var resultClasses = await connector.Func<UserRequest, ClassResponse[]>("classes", new UserRequest { UserId = AppSettings.Current.UserId });

                if (resultClasses.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultUsers);
                    return;
                }

                ClassList.FillData(resultClasses.Result.AsEnumerable());
            }
        }

        private void SaveButton_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
