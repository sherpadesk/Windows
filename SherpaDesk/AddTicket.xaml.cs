using SherpaDesk.Common;
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
            EndUserCombo.SelectedItem = EndUserMe;
        }

        private void TechnicianMeLink_Click(object sender, RoutedEventArgs e)
        {
            TechnicianCombo.SelectedItem = TechnicianMe;
        }

        private void AlternateTechMeLink_Click(object sender, RoutedEventArgs e)
        {
            AlternateTechnicianCombo.SelectedItem = AlternateTechMe;
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            AlternateTechMe.Content = TechnicianMe.Content = EndUserMe.Content = Helper.FullName(AppSettings.Current.FirstName, AppSettings.Current.LastName);
            AlternateTechMe.Tag = TechnicianMe.Tag = EndUserMe.Tag = AppSettings.Current.Email;
            AssignToComboBox.Items.Add(new ComboBoxItem { Content = Helper.FullName(AppSettings.Current.FirstName, AppSettings.Current.LastName) });
        }

        private void CreateNewAccountPopup_Loaded(object sender, RoutedEventArgs e)
        {           
            CreateNewAccountPopup.HorizontalOffset = (Window.Current.Bounds.Width - PopupGrid.ActualWidth) / 2;
        }

        private void AccountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.AddedItems.First();
            if (selected != null)
            {
                if (((ComboBoxItem)selected).Tag != null && ((ComboBoxItem)selected).Tag.ToString() == "AddNewAccount")
                {
                    CreateNewAccountPopup.IsOpen = true;
                    MainForm.IsEnabled = false;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
