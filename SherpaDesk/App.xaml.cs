using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace SherpaDesk
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private const string APPLICATION_FRAME = "appFrame";

        public static async void ShowStandartMessage(string message, eErrorType title)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    var md = new MessageDialog(message, title.Details());
                    await md.ShowAsync();
                }
                catch (UnauthorizedAccessException) { }
            });
        }
        public static async void LogOut()
        {
            if (await App.ConfirmMessage())
            {
                AppSettings.Current.Clear();
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame != null)
                {
                    rootFrame.Navigate(typeof(Login));
                }
            }
        }

        public static async void ShowErrorMessage(string message, eErrorType title)
        {
            var flyout = new Error.Flyout(message, title);
            await flyout.ShowAsync();
        }

        public static async void ShowErrorMessage(Response response, eErrorType title)
        {
            var flyout = new Error.Flyout(response, title);
            await flyout.ShowAsync();
        }

        public static async Task<bool> ConfirmMessage()
        {
            MessageDialog dialog = new MessageDialog("Are you sure?");
            dialog.Commands.Add(new UICommand { Label = "Ok", Id = "ok" });
            dialog.Commands.Add(new UICommand { Label = "Cancel", Id = "cancel" });
            var confirmResult = await dialog.ShowAsync();
            return confirmResult.Id.ToString() == "ok";
        }

        public static void ExternalAction(Action<MainPage> action)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
            {
                MainPage page = rootFrame.Content as MainPage;
                if (page != null)
                {
                    action(page);
                }
            }
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            //this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ShowErrorMessage(e.Message, eErrorType.Error);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    this.DebugSettings.EnableFrameRateCounter = true;
            //}

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                SherpaDesk.Common.SuspensionManager.RegisterFrame(rootFrame, APPLICATION_FRAME);

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    await SherpaDesk.Common.SuspensionManager.RestoreAsync();
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!string.IsNullOrEmpty(AppSettings.Current.ApiToken) &&
                    !string.IsNullOrEmpty(AppSettings.Current.OrganizationKey) &&
                    !string.IsNullOrEmpty(AppSettings.Current.InstanceKey))
                {
                    rootFrame.Navigate(typeof(MainPage));
                }
                else
                {
                    rootFrame.Navigate(typeof(Login), args.Arguments);
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            await SherpaDesk.Common.SuspensionManager.SaveAsync();

            deferral.Complete();
        }
    }
}
