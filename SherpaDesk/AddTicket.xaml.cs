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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SherpaDesk
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class AddTicket : SherpaDesk.Common.LayoutAwarePage
    {
        public AddTicket()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private async void filepickButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                filepickButton.Content = "Picked photo: " + file.Name;
            }
            else
            {
                filepickButton.Content = "Select Files";
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
            AlternateTechMe.Content = TechnicianMe.Content = EndUserMe.Content = string.Format("{0}, {1}", AppSettings.Current.LastName, AppSettings.Current.FirstName);
            AlternateTechMe.Tag = TechnicianMe.Tag = EndUserMe.Tag = AppSettings.Current.Email;

            AssignToComboBox.Items.Add(new ComboBoxItem { Content = string.Format("{0}, {1}", AppSettings.Current.LastName, AppSettings.Current.FirstName) });
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
    }
}
