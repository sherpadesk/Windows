using SherpaDesk.Common;
using SherpaDesk.Extensions;
using SherpaDesk.Models;
using SherpaDesk.Models.Response;
using SocialEbola.Lib.PopupHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        public async void Opened()
        {
            TitleText.Text = _flyout.Title.Details();
            MessageText.Text = _flyout.Message;
            SendReport.Visibility = SaveLog.Visibility = _flyout.CanSendReport ? Visibility.Visible : Visibility.Collapsed;
        }

        public void SetParent(PopupHelper parent)
        {
            _flyout = (Flyout)parent;
        }

        public class Flyout : PopupHelper<Error>
        {
            private Response _response;

            public bool CanSendReport { get; private set; }

            public eErrorType Title { get; private set; }

            public string Message { get; private set; }

            public Flyout(string message, eErrorType title)
            {
                this.Title = title;
                this.CanSendReport = title != eErrorType.InvalidInputData && title != eErrorType.Warning;
                this.Message = message;
            }

            public Flyout(Response response, eErrorType title)
                : this(response.Message, title)
            {
                this._response = response;
            }
            public override PopupSettings Settings
            {
                get
                {
                    return new PopupSettings(TimeSpan.FromMilliseconds(PopupSettings.DefaultDelayMs), 1, 0, PopupAnimation.OverlayFade, true);
                }
            }

            public string GetReportText()
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(Environment.NewLine);

                sb.Append(Environment.NewLine);

                sb.Append(" ----------------------------- User Details ----------------------------- ");

                sb.Append(Environment.NewLine);

                sb.AppendFormat("User: {0}{1}",
                    Helper.FullName(AppSettings.Current.Configuration.User.FirstName, AppSettings.Current.Configuration.User.LastName, AppSettings.Current.Configuration.User.Email, true),
                    Environment.NewLine);

                sb.AppendFormat("Organization: {0}{1}",
                    AppSettings.Current.Single ? AppSettings.Current.OrganizationName : AppSettings.Current.OrganizationName + " - " + AppSettings.Current.InstanceName,
                    Environment.NewLine);

                sb.AppendFormat("Date: {0}{1}",
                    DateTime.Now.ToString(CultureInfo.CurrentUICulture),
                    Environment.NewLine);

                sb.Append(Environment.NewLine);

                sb.Append(" ----------------------------- Error Details ----------------------------- ");

                sb.Append(Environment.NewLine);

                if (_response != null)
                {
                    foreach (var msg in _response.Messages)
                    {
                        sb.Append(msg);
                        sb.Append(Environment.NewLine);
                    }
                }
                else
                {
                    sb.Append(this.Message);
                    sb.Append(Environment.NewLine);
                }

                //sb.Append(" ------------------------------------------------------------------------- ");

                //sb.Append(Environment.NewLine);

                //sb.AppendFormat("Please attach Errors.log file to email from: {0}", ApplicationData.Current.LocalFolder.Path);

                //sb.Append(Environment.NewLine);

                //sb.Append("Thanks.");

                //sb.Append(Environment.NewLine);

                return sb.ToString();
            }

        }

        private async void CloseError_Click(object sender, RoutedEventArgs e)
        {
            await _flyout.CloseAsync();
        }

        private async void SendReport_Click(object sender, RoutedEventArgs e)
        {
            var mailto = new Uri(string.Format("mailto:?to={0}&subject=The Error Report from SherpaDesk App&body={1}",
                AppSettings.Current.SupportEmail,
                Uri.EscapeDataString(_flyout.GetReportText())));

            await Launcher.LaunchUriAsync(mailto);

            await _flyout.CloseAsync();
        }

        private async void SaveLog_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add(".etl Log", new List<string> { ".etl" });
            picker.DefaultFileExtension = ".etl";
            picker.SuggestedFileName = "Serpadesk";
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            var file = await picker.PickSaveFileAsync();
            var log = await ApplicationData.Current.LocalFolder.GetFileAsync("Errors.etl");
            if (file != null && log != null)
            {
                CachedFileManager.DeferUpdates(file);
                var buffer = await FileIO.ReadBufferAsync(log);
                await FileIO.WriteBufferAsync(file, buffer);
                await CachedFileManager.CompleteUpdatesAsync(file);
            }
        }

    }
}
