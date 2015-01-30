using SherpaDesk.Common;
using SherpaDesk.Extensions;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SherpaDesk
{
    public sealed partial class AddTicket : SherpaDesk.Common.LayoutAwarePage
    {
        private ObservableCollection<StorageFile> _attachment = null;

        public AddTicket()
        {
            this.InitializeComponent();
            _attachment = new ObservableCollection<StorageFile>();
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            SelectedAlternateTechnicianList.Items.Clear();

            using (var connector = new Connector())
            {
                if (AppSettings.Current.Configuration.User.TechOrAdmin)
                {
                    // users
                    EndUserList.Visibility = Visibility.Visible;
                    EndUserText.Visibility = Visibility.Collapsed;
                    EndUserMeLink.Visibility = Visibility.Visible;
                    AccountText.Visibility = AppSettings.Current.Configuration.AccountManager ? Visibility.Visible : Visibility.Collapsed;
                    AccountList.Visibility = AppSettings.Current.Configuration.AccountManager ? Visibility.Visible : Visibility.Collapsed;
                    TechnicianMeLink.Visibility = Visibility.Visible;
                    AlternateTechnicianText.Visibility = Visibility.Visible;
                    AlternateTechnicianList.Visibility = Visibility.Visible;
                    AlternateTechMeLink.Visibility = Visibility.Visible;

                    var resultUsers = await connector.Func<UserResponse[]>(x => x.Users);

                    if (resultUsers.Status != eResponseStatus.Success)
                    {
                        await this.HandleError(resultUsers);
                        return;
                    }

                    if (resultUsers.Result.Length < SearchRequest.DEFAULT_PAGE_COUNT)
                    {
                        EndUserList.FillData(
                            resultUsers.Result.Select(user => new NameResponse { Id = user.Id, Name = user.FullName }),
                            new NameResponse { Id = AppSettings.Current.Configuration.User.Id, Name = Constants.USER_ME });
                    }
                    else
                    {
                        EndUserList.AutoComplete(
                            x => x.Search(false),
                            k => this.EndUserList_SelectionChanged(
                                this.EndUserList,
                                new SelectionChangedEventArgs(new object[0].ToList(), (new object[1] { new ComboBoxItem() { Tag = k.Key, Content = k.Name } }).ToList())));

                        if (AppSettings.Current.Configuration.AccountManager)
                        {
                            // accounts
                            var resultAccounts = await connector.Func<AccountSearchRequest, AccountResponse[]>(x => x.Accounts,
                                new AccountSearchRequest { UserId = AppSettings.Current.Configuration.User.Id, PageCount = SearchRequest.MAX_PAGE_COUNT });

                            if (resultAccounts.Status != eResponseStatus.Success)
                            {
                                await this.HandleError(resultAccounts);
                                return;
                            }

                            AccountList.FillData(resultAccounts.Result.AsEnumerable());
                        }
                    }
                }
                else
                {
                    EndUserList.Visibility = Visibility.Collapsed;
                    EndUserText.Visibility = Visibility.Visible;
                    EndUserMeLink.Visibility = Visibility.Collapsed;
                    TechnicianMeLink.Visibility = Visibility.Collapsed;
                    AccountText.Visibility = Visibility.Collapsed;
                    AccountList.Visibility = Visibility.Collapsed;
                    AlternateTechnicianText.Visibility = Visibility.Collapsed;
                    AlternateTechnicianList.Visibility = Visibility.Collapsed;
                    AlternateTechMeLink.Visibility = Visibility.Collapsed;
                    AddUserArrow.Visibility = Visibility.Collapsed;
                    AddUserLink.Visibility = Visibility.Collapsed;

                    EndUserText.Text = Helper.FullName(AppSettings.Current.Configuration.User.FirstName, AppSettings.Current.Configuration.User.LastName, AppSettings.Current.Configuration.User.Email);
                }

                // technicians
                var resultTechnicians = await connector.Func<UserResponse[]>(x => x.Technicians);

                if (resultTechnicians.Status != eResponseStatus.Success)
                {
                    await this.HandleError(resultTechnicians);
                    return;
                }

                if (resultTechnicians.Result.Length < SearchRequest.MAX_PAGE_COUNT)
                {
                    TechnicianList.FillData(
                        resultTechnicians.Result.Select(user => new NameResponse { Id = user.Id, Name = Helper.FullName(user.FirstName, user.LastName, user.Email, true) }),
                        new NameResponse { Id = 0, Name = "Route via Class (default)" });

                    if (AppSettings.Current.Configuration.User.TechOrAdmin)
                    {
                        AlternateTechnicianList.FillData(
                            resultTechnicians.Result.Select(user => new NameResponse { Id = user.Id, Name = Helper.FullName(user.FirstName, user.LastName, user.Email, true) }),
                            NameResponse.Empty,
                            new NameResponse { Id = AppSettings.Current.Configuration.User.Id, Name = Constants.TECHNICIAN_ME });
                    }
                }
                else
                {
                    TechnicianList.AutoComplete(x => x.Search(false));

                    if (AppSettings.Current.Configuration.User.TechOrAdmin)
                    {
                        AlternateTechnicianList.AutoComplete(
                            x => x.Search(false),
                            k => this.AlternateTechnicianList_SelectionChanged(
                                this.AlternateTechnicianList,
                                new SelectionChangedEventArgs(new object[0].ToList(), (new object[1] { new ComboBoxItem { Tag = k.Key, Content = k.Name } }).ToList())));
                    }
                }

                if (AppSettings.Current.Configuration.ClassTracking)
                {
                    var resultClasses = await connector.Func<UserRequest, ClassResponse[]>(x => x.Classes, new UserRequest { UserId = AppSettings.Current.Configuration.User.Id });

                    if (resultClasses.Status != eResponseStatus.Success)
                    {
                        await this.HandleError(resultClasses);
                        return;
                    }

                    ClassList.FillData(resultClasses.Result.AsEnumerable());
                }
            }
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
                foreach (var file in files)
                {
                    _attachment.Add(file);
                }
                selectedFilesList.ItemsSource = _attachment;
            }
            else
            {
                selectedFilesList.Items.Clear();
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

        private async void EndUserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = e.AddedItems.FirstOrDefault() as ComboBoxItem;
            if (selectedItem != null && (int)selectedItem.Tag > 0)
            {
                using (var connector = new Connector())
                {
                    // accounts
                    var resultAccounts = await connector.Func<AccountSearchRequest, AccountResponse[]>(x => x.Accounts,
                        new AccountSearchRequest { UserId = (int)selectedItem.Tag, PageCount = SearchRequest.MAX_PAGE_COUNT });

                    if (resultAccounts.Status != eResponseStatus.Success)
                    {
                        await this.HandleError(resultAccounts);
                        return;
                    }

                    AccountList.FillData(resultAccounts.Result.AsEnumerable());
                }
            }
        }

        private void AlternateTechnicianList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = e.AddedItems.FirstOrDefault() as ComboBoxItem;
            if (selectedItem != null && (int)selectedItem.Tag > 0)
            {
                if (!SelectedAlternateTechnicianList.Items.Any(x => ((int)((CheckBox)x).Tag == (int)selectedItem.Tag)))
                {
                    SelectedAlternateTechnicianList.Items.Insert(0, this.CreateCheckBox(selectedItem));
                }
            }
        }

        private CheckBox CreateCheckBox(ComboBoxItem selectedItem)
        {
            CheckBox result = new CheckBox { IsChecked = true, Content = selectedItem.Content, Tag = selectedItem.Tag };
            result.Unchecked += CheckBox_UnChecked;
            return result;
        }

        private void CheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < SelectedAlternateTechnicianList.Items.Count; i++)
            {
                if ((int)((CheckBox)SelectedAlternateTechnicianList.Items[i]).Tag == (int)((CheckBox)sender).Tag)
                {
                    SelectedAlternateTechnicianList.Items.RemoveAt(i);
                }
            }
        }

        private async void SaveButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (AppSettings.Current.Configuration.RequireTicketInitialPost && string.IsNullOrWhiteSpace(DescritionTextbox.Text))
            {
                await this.HandleValidators("Initial Post is required for current configuration.#DescritionTextbox");
                return;
            }
            using (var connector = new Connector())
            {
                var resultAddTicket = await connector.Func<AddTicketRequest, AddTicketResponse>(
                    x => x.Tickets,
                    new AddTicketRequest
                    {
                        AccountId = AppSettings.Current.Configuration.User.TechOrAdmin && AppSettings.Current.Configuration.AccountManager ? AccountList.GetSelectedValue<int>() : 0,
                        ClassId = AppSettings.Current.Configuration.ClassTracking ? ClassList.GetSelectedValue<int>() : 0,
                        UserId = AppSettings.Current.Configuration.User.TechOrAdmin
                            ? EndUserList.GetSelectedValue<int>()
                            : AppSettings.Current.Configuration.User.Id,
                        TechnicianId = TechnicianList.GetSelectedValue<int>(),
                        Name = SubjectTextbox.Text,
                        Status = eTicketStatus.Open.Details(),
                        Comment = DescritionTextbox.Text
                    });
                if (resultAddTicket.Status != eResponseStatus.Success)
                {
                    await this.HandleError(resultAddTicket);
                    return;
                }

                if (AppSettings.Current.Configuration.User.TechOrAdmin)
                {
                    foreach (var item in SelectedAlternateTechnicianList.Items)
                    {
                        if (((CheckBox)item).IsChecked ?? false)
                        {
                            var attachAltTechResult = await connector.Action<AttachAltTechRequest>(x => x.Tickets,
                                new AttachAltTechRequest(resultAddTicket.Result.TicketKey, (int)((CheckBox)item).Tag));

                            if (attachAltTechResult.Status != eResponseStatus.Success)
                            {
                                await this.HandleError(attachAltTechResult);
                                return;
                            }
                        }
                    }
                }
                if (_attachment.Count > 0)
                {
                    int? postId = null;
                    if (resultAddTicket.Result.Posts != null && resultAddTicket.Result.Posts.Length > 0)
                        postId = resultAddTicket.Result.Posts[0].PostId;
                    using (FileRequest fileRequest = FileRequest.Create(resultAddTicket.Result.TicketKey, postId))
                    {
                        foreach (var file in _attachment)
                        {
                            await fileRequest.Add(file);
                        }
                        var resultUploadFile = await connector.Action<FileRequest>(x => x.Files, fileRequest);
                        if (resultUploadFile.Status != eResponseStatus.Success)
                        {
                            await this.HandleError(resultUploadFile);
                            return;
                        }
                    }
                }
                // if all okay to clear form

                DescritionTextbox.Text = SubjectTextbox.Text = string.Empty;

                EndUserList.SetDefaultValue();
                AccountList.SetDefaultValue();
                ClassList.SetDefaultValue();
                AccountList.SetDefaultValue();
                TechnicianList.SetDefaultValue();
                AlternateTechnicianList.SetDefaultValue();

                SelectedAlternateTechnicianList.Items.Clear();
                _attachment.Clear();

                this.MainPage(page =>
                {
                    page.WorkListFrame.Navigate(typeof(WorkList), eWorkListType.Open);
                    page.ScrollViewer.ChangeView(Constants.WIDTH_TIMESHEET + Constants.WIDTH_INFO, null, null);
                });

                await App.ExternalAction(x => x.UpdateInfo());
            }
        }

        private void SaveButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 0.9;
        }

        private void SaveButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 1;
        }

        private void CreateUserButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 0.9;
        }

        private void CreateUserButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 1;
        }

        private async void CreateUserButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var resultAddUser = await connector.Func<AddUserRequest, UserResponse>(
                    x => x.Users, new AddUserRequest
                    {
                        LastName = LastNameTextbox.Text,
                        FirstName = FirstNameTextbox.Text,
                        Email = EmailTextbox.Text,
                        AccountId = AccountList.GetSelectedValue<int>(),
                        LocationId = 0
                    });
                if (resultAddUser.Status != eResponseStatus.Success)
                {
                    await this.HandleError(resultAddUser);
                }
                else
                {
                    NewUserGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    await App.ShowStandardMessage("A user account was created", eErrorType.NoTitle);
                }
            }
        }

        private void AddUserLink_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FirstNameTextbox.Text = LastNameTextbox.Text = EmailTextbox.Text = string.Empty;
            NewUserGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.MainPage(page =>
            {
                page.ScrollViewer.ChangeView(AddTicketGrid.ActualWidth + 700 - Window.Current.Bounds.Width, null, null);
            });
        }

        private void AddUserArrow_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ((Image)sender).Opacity = 0.7;
        }

        private void AddUserArrow_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ((Image)sender).Opacity = 1;
        }

        private void CancelButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            NewUserGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void CancelButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((TextBlock)sender).Opacity = 0.6;
        }

        private void CancelButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((TextBlock)sender).Opacity = 1;
        }

        private void RemoveAttachmentButton_Click(object sender, RoutedEventArgs e)
        {
            var attachment = _attachment.FirstOrDefault(x => x.Name == ((Windows.UI.Xaml.Controls.Primitives.ButtonBase)(sender)).Tag.ToString());
            if (attachment != null)
            {
                _attachment.Remove(attachment);
                selectedFilesList.Focus(FocusState.Pointer);
            }
        }
    }
}
