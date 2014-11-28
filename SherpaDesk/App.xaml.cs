﻿using SherpaDesk.Common;
using SherpaDesk.Extensions;
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
using Windows.Foundation.Diagnostics;
using Windows.Storage;
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
        private const string APPLICATION_FRAME = "SherpaDeskFrame";
        private const string SESSION_NAME = "SherpaDeskSession";
        private const string CHANNEL_NAME = "SherpaDeskChannel";

        public static async void ShowStandartMessage(string message, eErrorType title)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                try
                {
                    var md = new MessageDialog(message, title.Details());
                    await md.ShowAsync();
                }
                catch (UnauthorizedAccessException) { }
            }).AsTask();

            await WriteLog(message, title);

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

        public static async Task WriteLog(string message, eErrorType type, Exception e = null)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                if (type == eErrorType.Error || type == eErrorType.FailedOperation || type == eErrorType.InternalError || type == eErrorType.Message)
                {
                    try
                    {

                        LoggingLevel level = type == eErrorType.Message ? LoggingLevel.Information : LoggingLevel.Error;

                        var channel = new LoggingChannel(CHANNEL_NAME);
                        var session = new LoggingSession(SESSION_NAME);

                        session.AddLoggingChannel(channel, level);

                        channel.LogMessage(message, level);
                        if (e != null)
                            channel.LogMessage(e.ToString(), LoggingLevel.Critical);

                        await session.SaveToFileAsync(ApplicationData.Current.LocalFolder, "Errors.etl");
                    }
                    catch (UnauthorizedAccessException) { }
                }
            }).AsTask();
        }

        public static async void ShowErrorMessage(string message, eErrorType title, Exception e = null)
        {
            await WriteLog(message, title, e);

            var flyout = new Error.Flyout(message, title);

            await flyout.ShowAsync();

        }

        public static async void ShowErrorMessage(Response response, eErrorType title)
        {
            await WriteLog(response.ToString(), title);

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
            ShowErrorMessage(e.Message, eErrorType.Error, e.Exception);
            e.Handled = true;
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
