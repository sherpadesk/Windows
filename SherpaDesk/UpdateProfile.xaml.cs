using SherpaDesk.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SherpaDesk
{
    public sealed partial class UpdateProfile : SherpaDesk.Common.LayoutAwarePage
    {
        public UpdateProfile()
        {
            this.InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Info));   
        }

        private void SaveCloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Info));  
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Text = AppSettings.Current.Email;
            FirstNameTextBox.Text = AppSettings.Current.FirstName;
            LastNameTextBox.Text = AppSettings.Current.LastName;
        }
    }
}
