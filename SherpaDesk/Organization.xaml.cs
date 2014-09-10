using SherpaDesk.Common;
using SherpaDesk.Extensions;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SherpaDesk
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Organization : Page
    {
        private IList<OrganizationResponse> _organizationList = null;
        private IList<InstanceResponse> _instanceList = null;
        public Organization()
        {
            this.InitializeComponent();
            _organizationList = new List<OrganizationResponse>();
            _instanceList = new List<InstanceResponse>();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            if (_organizationList.Count == 0)
            {
                AppSettings.Current.OrganizationKey =
                    AppSettings.Current.OrganizationName =
                        AppSettings.Current.InstanceKey =
                            AppSettings.Current.InstanceName = string.Empty;

                using (var connector = new Connector())
                {
                    // load organization and instance info
                    var resultOrg = await connector.Func<OrganizationResponse[]>(
                            x => x.Organizations);

                    if (resultOrg.Status != eResponseStatus.Success)
                    {
                        this.HandleError(resultOrg);
                        return;
                    }
                    _organizationList = resultOrg.Result.ToList();
                }
            }
            if (_organizationList.Count == 0)
            {
                App.ShowErrorMessage("Cannot found organization", eErrorType.Error);
                return;
            }
            OrganizationList.FillData(_organizationList);
            OrganizationList.SelectedIndex = 0;
        }

        private void OrganizationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var orgKey = OrganizationList.GetSelectedValue<string>();
            if (!string.IsNullOrEmpty(orgKey))
            {
                var org = _organizationList.FirstOrDefault(x => x.Key == orgKey);
                if (org != null)
                {
                    _instanceList = org.Instances.ToList();
                    InstanceList.FillData(_instanceList);
                    InstanceList.SelectedIndex = 0;
                }
            }
        }

        private void LogoffButton_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.Current.Clear();
            this.Frame.Navigate(typeof(Login));
        }

        private void SelectOrgButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var orgKey = OrganizationList.GetSelectedValue<string>();
            if (!string.IsNullOrEmpty(orgKey))
            {
                string instKey = string.Empty, instName = string.Empty;
                if (InstanceList.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                {
                    var instance = _instanceList.FirstOrDefault();
                    if (instance != null)
                    {
                        instKey = instance.Key;
                        instName = instance.Name;
                    }
                }
                else
                {
                    instKey = InstanceList.GetSelectedValue<string>();
                    instName = InstanceList.GetSelectedText();
                }
                if (!string.IsNullOrEmpty(instKey))
                {
                    AppSettings.Current.AddOrganization(
                            orgKey,
                            OrganizationList.GetSelectedText(),
                            instKey,
                            instName,
                            _organizationList.IsSingle());

                    this.Frame.Navigate(typeof(MainPage));
                }
                else
                {
                    App.ShowErrorMessage("No selected instance", eErrorType.Error);
                }
            }
            else
            {
                App.ShowErrorMessage("No selected organization", eErrorType.Error);
            }
        }

        private void SignInButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 0.9;
        }

        private void SignInButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 1;
        }

        private void RegisterButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 0.9;
        }

        private void RegisterButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ((Button)sender).Opacity = 1;
        }
    }
}
