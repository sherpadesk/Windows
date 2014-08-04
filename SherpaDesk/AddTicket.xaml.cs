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
using Windows.UI.Xaml.Media.Animation;

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


        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            SelectedAlternateTechnicianList.Items.Clear();

            using (var connector = new Connector())
            {
                // users
                var resultUsers = await connector.Func<UserResponse[]>(x => x.Users);

                if (resultUsers.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultUsers);
                    return;
                }

                // technicians
                var resultTechnicians = await connector.Func<UserResponse[]>(x => x.Technicians);

                if (resultTechnicians.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTechnicians);
                    return;
                }


                if (resultUsers.Result.Length < SearchRequest.DEFAULT_PAGE_COUNT)
                {
                    EndUserList.FillData(
                        resultUsers.Result.Select(user => new NameResponse { Id = user.Id, Name = user.FullName }),
                        new NameResponse { Id = AppSettings.Current.UserId, Name = Constants.USER_ME });
                }
                else
                {
                    EndUserList.AutoComplete(
                        x => x.Search(false),
                        k => this.EndUserList_SelectionChanged(
                            this.EndUserList,
                            new SelectionChangedEventArgs(new object[0].ToList(), (new object[1] { new ComboBoxItem() { Tag = k.Key, Content = k.Name } }).ToList())));

                    // accounts
                    var resultAccounts = await connector.Func<AccountSearchRequest, AccountResponse[]>(x => x.Accounts,
                        new AccountSearchRequest { UserId = AppSettings.Current.UserId, PageCount = SearchRequest.MAX_PAGE_COUNT });

                    if (resultAccounts.Status != eResponseStatus.Success)
                    {
                        this.HandleError(resultAccounts);
                        return;
                    }

                    AccountList.FillData(resultAccounts.Result.AsEnumerable());
                }

                if (resultTechnicians.Result.Length < SearchRequest.MAX_PAGE_COUNT)
                {
                    TechnicianList.FillData(
                        resultTechnicians.Result.Select(user => new NameResponse { Id = user.Id, Name = Helper.FullName(user.FirstName, user.LastName, user.Email, true) }),
                        new NameResponse { Id = -1, Name = "Let the system choose." },
                        new NameResponse { Id = AppSettings.Current.UserId, Name = Constants.TECHNICIAN_ME });

                    AlternateTechnicianList.FillData(
                        resultTechnicians.Result.Select(user => new NameResponse { Id = user.Id, Name = Helper.FullName(user.FirstName, user.LastName, user.Email, true) }),
                        NameResponse.Empty,
                        new NameResponse { Id = AppSettings.Current.UserId, Name = Constants.TECHNICIAN_ME });

                }
                else
                {
                    TechnicianList.AutoComplete(x => x.Search(false));
                    AlternateTechnicianList.AutoComplete(
                        x => x.Search(false),
                        k => this.AlternateTechnicianList_SelectionChanged(
                            this.AlternateTechnicianList,
                            new SelectionChangedEventArgs(new object[0].ToList(), (new object[1] { new ComboBoxItem { Tag = k.Key, Content = k.Name } }).ToList())));
                }

                var resultClasses = await connector.Func<UserRequest, ClassResponse[]>(x => x.Classes, new UserRequest { UserId = AppSettings.Current.UserId });

                if (resultClasses.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultClasses);
                    return;
                }

                ClassList.FillData(resultClasses.Result.AsEnumerable());
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
                //SelectedFilesList.Text = "Picked photos: ";
                //List<string> fileNames = new List<string>();
                //foreach (var file in files)
                //{
                //    fileNames.Add(file.Name);
                //    _attachment.Add(file);
                //}
                //SelectedFilesList.Text += string.Join(", ", fileNames.ToArray());
            }
            else
            {
                //SelectedFilesList.Text = string.Empty;
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
                        this.HandleError(resultAccounts);
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
            using (var connector = new Connector())
            {
                var resultAddTicket = await connector.Func<AddTicketRequest, AddTicketResponse>(
                    x => x.Tickets,
                    new AddTicketRequest
                    {
                        AccountId = AccountList.GetSelectedValue<int>(),
                        ClassId = ClassList.GetSelectedValue<int>(),
                        UserId = EndUserList.GetSelectedValue<int>(),
                        TechnicianId = TechnicianList.GetSelectedValue<int>(),
                        Name = SubjectTextbox.Text,
                        Status = eTicketStatus.Open.Details(),
                        Comment = DescritionTextbox.Text
                    });
                if (resultAddTicket.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultAddTicket);
                    return;
                }
                foreach (var item in SelectedAlternateTechnicianList.Items)
                {
                    if (((CheckBox)item).IsChecked ?? false)
                    {
                        var attachAltTechResult = await connector.Action<AttachAltTechRequest>(x => x.Tickets,
                            new AttachAltTechRequest(resultAddTicket.Result.TicketKey, (int)((CheckBox)item).Tag));

                        if (attachAltTechResult.Status != eResponseStatus.Success)
                        {
                            this.HandleError(attachAltTechResult);
                            return;
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
                            this.HandleError(resultUploadFile);
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

                var scrollViewer = (ScrollViewer)((Frame)this.pageRoot.Parent).FindName("scrollViewer");
                var leftFrame = (Frame)((Frame)this.pageRoot.Parent).FindName("LeftFrame");

                leftFrame.Navigate(typeof(WorkList), eWorkListType.Open);
                scrollViewer.ChangeView(0, new double?(), new float?());

                App.ExternalAction(x => x.UpdateInfo());
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

        private void CreateUserButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NewUserGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;            
        }

        private void AddUserLink_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NewUserGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void AddUserArrow_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ((Image)sender).Opacity = 0.7;
        }

        private void AddUserArrow_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ((Image)sender).Opacity = 1;
        }
    }
}
