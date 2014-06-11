using SocialEbola.Lib.PopupHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SherpaDesk
{
    public sealed partial class InviteUser : UserControl, IPopupControl
    {
        private Flyout _flyout;

        public InviteUser()
        {
            this.InitializeComponent();
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            await _flyout.CloseAsync();
        }

        private void InviteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public void Closed(CloseAction closeAction)
        {
            
        }

        public void Opened()
        {
            
        }

        public void SetParent(PopupHelper parent)
        {
            _flyout = (Flyout)parent;
        }

        public class Flyout : PopupHelper<InviteUser>
        {
            public override PopupSettings Settings
            {
                get
                {
                    return PopupSettings.Flyout;
                }
            }
        }


    }
}
