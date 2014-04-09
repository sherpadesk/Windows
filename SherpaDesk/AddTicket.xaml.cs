using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SherpaDesk
{
    public sealed partial class AddTicket : SherpaDesk.Common.LayoutAwarePage
    {
        private IList<StorageFile> _attachment = null;
        public AddTicket()
        {
            this.InitializeComponent();
            _attachment = new List<StorageFile>();
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
            _attachment.Clear();
            if (files != null && files.Count > 0)
            {
                SelectedFilesList.Text = "Picked photos: ";
                List<string> fileNames = new List<string>();
                foreach (var file in files)
                {
                    fileNames.Add(file.Name);
                    _attachment.Add(file);
                }
                SelectedFilesList.Text += string.Join(", ", fileNames.ToArray());
            }
            else
            {
                SelectedFilesList.Text = string.Empty;
            }
        }

        private void SetMe(ComboBox combobox)
        {
            if (combobox.Visibility == Visibility.Visible)
                combobox.SetSelectedValue(AppSettings.Current.UserId);
            else
            {
                var textBox = this.FindName(combobox.Name + "_Text") as RadAutoCompleteBox;
                if (textBox != null)
                {
                    textBox.Text = Helper.FullName(AppSettings.Current.FirstName, AppSettings.Current.LastName, AppSettings.Current.Email);
                    textBox.Tag = AppSettings.Current.UserId;
                }
            }
        }

        private void EndUserMeLink_Click(object sender, RoutedEventArgs e)
        {
            this.SetMe(this.EndUserList);
        }

        private void TechnicianMeLink_Click(object sender, RoutedEventArgs e)
        {
            this.SetMe(this.TechnicianList);
        }

        private void AlternateTechMeLink_Click(object sender, RoutedEventArgs e)
        {
            this.SetMe(this.AlternateTechnicianList);
        }

        private async void SearchUsers(object obj, TextChangedEventArgs args)
        {
            using (var inсonn = new Connector())
            {
                RadAutoCompleteBox searchBox = (RadAutoCompleteBox)obj;

                searchBox.FilterMemberPath = "Name";
                if (searchBox.Text.Length > 1)
                {
                    var result = await inсonn.Func<UserSearchRequest, UserResponse[]>("users",
                        new UserSearchRequest { Query = searchBox.Text });

                    if (result.Status != eResponseStatus.Success)
                    {
                        this.HandleError(result);
                        return;
                    }
                    searchBox.ItemsSource = result.Result.Select(user => new NameResponse { Id = user.Id, Name = user.FullName }).ToList();
                }
            }
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                // users
                var resultUsers = await connector.Func<UserSearchRequest, UserResponse[]>("users", new UserSearchRequest());

                if (resultUsers.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultUsers);
                    return;
                }

                if (resultUsers.Result.Length < SearchRequest.DEFAULT_PAGE_COUNT)
                {
                    EndUserList.FillData(
                        resultUsers.Result.Select(user => new NameResponse { Id = user.Id, Name = user.FullName }),
                        new NameResponse { Id = AppSettings.Current.UserId, Name = Constants.USER_ME });

                    TechnicianList.FillData(
                        resultUsers.Result.Select(user => new NameResponse { Id = user.Id, Name = Helper.FullName(user.FirstName, user.LastName, user.Email) }),
                        new NameResponse { Id = -1, Name = "Let the system choose." },
                        new NameResponse { Id = AppSettings.Current.UserId, Name = Constants.TECHNICIAN_ME });

                    AlternateTechnicianList.FillData(
                        resultUsers.Result.Select(user => new NameResponse { Id = user.Id, Name = Helper.FullName(user.FirstName, user.LastName, user.Email) }),
                        new NameResponse { Id = AppSettings.Current.UserId, Name = Constants.TECHNICIAN_ME });

                }
                else
                {
                    EndUserList.AutoComplete(this.SearchUsers);
                    TechnicianList.AutoComplete(this.SearchUsers);
                    AlternateTechnicianList.AutoComplete(this.SearchUsers);
                }

                // accounts
                var resultAccounts = await connector.Func<AccountResponse[]>("accounts");

                if (resultAccounts.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultAccounts);
                    return;
                }

                AccountList.FillData(resultAccounts.Result.AsEnumerable());

                var resultClasses = await connector.Func<UserRequest, ClassResponse[]>("classes", new UserRequest { UserId = AppSettings.Current.UserId });

                if (resultClasses.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultUsers);
                    return;
                }

                ClassList.FillData(resultClasses.Result.AsEnumerable());
            }
        }

        private async void SaveButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var resultAddTicket = await connector.Func<AddTicketRequest, AddTicketResponse>(
                    "tickets",
                    new AddTicketRequest
                    {
                        AccountId = AccountList.GetSelectedValue<int>(),
                        ClassId = ClassList.GetSelectedValue<int>(),
                        UserId = EndUserList.GetSelectedValue<int>(),
                        TechnicianId = TechnicianList.GetSelectedValue<int>(),
                        Name = SubjectTextbox.Text,
                        Status = StatusList.GetSelectedValue<string>(),
                        Comment = DescritionTextbox.Text
                    });
                if (resultAddTicket.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultAddTicket);
                    return;
                }
                if (_attachment.Count > 0)
                {
                    using (FileRequest fileRequest = new FileRequest("?ticket=" + resultAddTicket.Result.TicketKey))
                    {
                        foreach (var file in _attachment)
                        {
                            fileRequest.Add(file);
                        }
                        var resultUploadFile = await connector.Action<FileRequest>("files", fileRequest);
                        if (resultUploadFile.Status != eResponseStatus.Success)
                        {
                            this.HandleError(resultUploadFile);
                            return;
                        }
                    }
                }
                // if all okay to clear form
                
                _attachment.Clear();
                DescritionTextbox.Text = SubjectTextbox.Text = string.Empty;

                var scrollViewer = (ScrollViewer)((Frame)this.pageRoot.Parent).FindName("scrollViewer");
                var leftFrame = (Frame)((Frame)this.pageRoot.Parent).FindName("LeftFrame");

                leftFrame.Navigate(typeof(WorkList), eWorkListType.Open);
                scrollViewer.ChangeView(0, new double?(), new float?());
            }
        }
    }
}
