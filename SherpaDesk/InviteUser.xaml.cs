using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using SocialEbola.Lib.PopupHelpers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        private async void InviteButton_Click(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var resultAddUser = await connector.Func<AddUserRequest, UserResponse>(
                    x => x.Users, new AddUserRequest
                    {
                        LastName = LastNameTextbox.Text,
                        FirstName = FirstNameTextbox.Text,
                        Email = EmailTextbox.Text,
                        AccountId = _flyout.AccountId,
                        LocationId = _flyout.LocationId
                    });
                if (resultAddUser.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultAddUser);
                }
                else
                {
                    _flyout.OnCreated(resultAddUser.Result);

                    await _flyout.CloseAsync();
                }
            }
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
            public event EventHandler<UserResponse> Created;

            public Flyout(int accountId, int locationId = 0)
            {
                this.AccountId = accountId;
                this.LocationId = locationId;
            }

            public void OnCreated(UserResponse u)
            {
                if (this.Created != null)
                {
                    Created(this, u);
                }
            }

            public int AccountId { get; private set; }

            public int LocationId { get; private set; }

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
