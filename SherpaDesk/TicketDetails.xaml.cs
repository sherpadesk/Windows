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
    public sealed partial class TicketDetails : SherpaDesk.Common.LayoutAwarePage
    {
        public TicketDetails()
        {
            this.InitializeComponent();
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            SubjectDecorate.Height = SubjectLabel.ActualHeight;            
        }

        private void ToggleWorkpad()
        {
            if (WorkpadEditBox.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
            {
                WorkpadLabel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                WorkpadEditBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
                WorkpadSaveButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                WorkpadCancelButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                WorkpadButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                WorkpadLabel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                WorkpadEditBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                WorkpadSaveButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                WorkpadCancelButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                WorkpadButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        private void WorkpadButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ToggleWorkpad();
        }

        private void WorkpadSaveButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ToggleWorkpad();
        }

        private void WorkpadCancelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ToggleWorkpad();
        }
    }
}
