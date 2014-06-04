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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SherpaDesk
{
    public sealed partial class Transfer : SherpaDesk.Common.LayoutAwarePage, IChildPage
    {
        private string _ticketKey;

        public event EventHandler UpdatePage;

        public Transfer()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _ticketKey = (string)e.Parameter;
            base.OnNavigatedTo(e);
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var resultTechnicians = await connector.Func<TechniciansRequest, UserResponse[]>(x => x.Technicians, new TechniciansRequest());

                if (resultTechnicians.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTechnicians);
                    return;
                }

                if (resultTechnicians.Result.Length < SearchRequest.DEFAULT_PAGE_COUNT)
                {
                    TechnicianList.FillData(
                        resultTechnicians.Result.Select(user => new NameResponse { Id = user.Id, Name = Helper.FullName(user.FirstName, user.LastName, user.Email) }),
                        new NameResponse { Id = -1, Name = "Let the system choose." });
                }
                else
                {
                    TechnicianList.AutoComplete(x => x.Search(false));
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

        private async void SubmitTransferButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var transferResult = await connector.Action<TransferRequest>(x => x.Tickets, new TransferRequest(_ticketKey)
                {
                    Note = DescritionTextbox.Text,
                    KeepAttached = KeepMeCheckBox.IsChecked ?? false,
                    ClassId = TransferToClassCheckBox.IsChecked ?? false ? ClassList.GetSelectedValue<int>() : 0,
                    TechnicianId = TransferToTechCheckBox.IsChecked ?? false ? TechnicianList.GetSelectedValue<int>() : 0
                });

                if (transferResult.Status != eResponseStatus.Success)
                {
                    this.HandleError(transferResult);
                    return;
                }

                if (MakeMeAlternateCheckBox.IsChecked ?? false)
                {
                    var attachAltTechResult = await connector.Action<AttachAltTechRequest>(x => x.Tickets,
                        new AttachAltTechRequest(_ticketKey, AppSettings.Current.UserId));

                    if (attachAltTechResult.Status != eResponseStatus.Success)
                    {
                        this.HandleError(attachAltTechResult);
                        return;
                    }
                }

                if (UpdatePage != null)
                {
                    UpdatePage(this, EventArgs.Empty);
                }
                ((Frame)this.Parent).Navigate(typeof(Empty));
                
                App.ExternalAction(x =>
                        x.UpdateInfo());
            }
        }

        private void TechnicianMeLink_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.SetMe(this.TechnicianList);
        }
    }
}
