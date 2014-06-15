using SherpaDesk.Models;
using SherpaDesk.Models.Response;
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
using SherpaDesk.Common;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SherpaDesk
{
    public sealed partial class Error : UserControl, IPopupControl
    {
        private Flyout _flyout;

        public Error()
        {
            this.InitializeComponent();
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
            TitleText.Text = _flyout.Title.Details();
            MessageText.Text = _flyout.Response.Message;
            SendReport.Visibility = _flyout.Title != eErrorType.FailedOperation && _flyout.Title != eErrorType.Warning ?
                Visibility.Visible : Visibility.Collapsed;
        }

        public class Flyout : PopupHelper<Error>
        {
            public eErrorType Title { get; private set; }
            public Response Response { get; private set; }

            public Flyout(Response response, eErrorType title)
            {
                this.Title = title;
                this.Response = response;
            }
            public override PopupSettings Settings
            {
                get
                {
                    return new PopupSettings(TimeSpan.FromMilliseconds(PopupSettings.DefaultDelayMs), 1, 0, PopupAnimation.OverlayFade, true);
                }
            }


        }

        private async void CloseError_Click(object sender, RoutedEventArgs e)
        {
            await _flyout.CloseAsync();
        }

        private void SendReport_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
