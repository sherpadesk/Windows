﻿using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace SherpaDesk
{
    public sealed partial class TicketDetails : SherpaDesk.Common.LayoutAwarePage
    {
        private string _ticketKey;

        public event EventHandler UpdateTicketListEvent;

        public TicketDetails()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _ticketKey = (string)e.Parameter;
            base.OnNavigatedTo(e);
        }

        private async void FillResponses()
        {
            using (var connector = new Connector())
            {
                var resultNotes = await connector.Func<NoteSearchRequest, NoteResponse[]>("tickets", new NoteSearchRequest(_ticketKey));
                if (resultNotes.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultNotes);
                    return;
                }

                var resultView = resultNotes.Result.Select(x => new
                {
                    x.FullName,
                    x.ResponseDateText,
                    x.NoteType,
                    NoteText = Helper.HtmlToString(x.NoteText)
                }).Where(x => x.NoteType != eNoteType.InitialPost.Details()).ToList(); 
                TicketDetailsList.ItemsSource = null; // For Visual Effect
                TicketDetailsList.ItemsSource = resultView;
            }
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
//            SubjectDecorate.Height = SubjectLabel.ActualHeight;
            using (var connector = new Connector())
            {
                var resultTicket = await connector.Func<KeyRequest, TicketDetailsResponse>("tickets", new KeyRequest(_ticketKey));

                if (resultTicket.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTicket);
                    return;
                }
                FillResponses();
                var ticket = resultTicket.Result;

                AddResponseButton.Visibility = Windows.UI.Xaml.Visibility.Visible;

                SubjectLabel.Text = ticket.Subject;
                EndUserLabel.Text = ticket.UserFullName;
                InitialPostLabel.Text = Helper.HtmlToString(ticket.InitialPost);
                WorkpadLabel.Text = Helper.HtmlToString(ticket.Workpad);

                var resultFiles = await connector.Func<KeyRequest, FileResponse[]>("files", new KeyRequest("?ticket=", _ticketKey));

                if (resultTicket.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTicket);
                    return;
                }
                if (resultFiles.Result != null && resultFiles.Result.Length > 0)
                {
                    AttachedView.ItemsSource = resultFiles.Result.Select(file => new
                    {
                        FileName = file.Name,
                        Image = new BitmapImage(new Uri(file.Url, UriKind.Absolute))
                    }).ToList();
                    FilesLabel.Visibility = AttachedView.Visibility = AttachedPages.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
        }

        private void AddResponseButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ResponseFrame.Navigated += ResponseFrame_Navigated;
            ResponseFrame.Navigate(typeof(AddResponse), _ticketKey);
        }

        private void ResponseFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (((ContentControl)sender).Content is AddResponse)
            {
                ((AddResponse)((ContentControl)sender).Content).UpdateTicketDetailsEvent -= UpdateTicketDetails;
                ((AddResponse)((ContentControl)sender).Content).UpdateTicketDetailsEvent += UpdateTicketDetails;
            }
        }

        private void UpdateTicketDetails(object sender, EventArgs e)
        {
            FillResponses();
        }

        private async void CloseMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //TODO: make confirm window
            using (var connector = new Connector())
            {
                var result = await connector.Action<CloseTicketRequest>("tickets",
                        new CloseTicketRequest(_ticketKey));

                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                    return;
                }
            }
            ((Frame)this.Parent).Navigate(typeof(Empty));
            if (this.UpdateTicketListEvent != null)
            {
                this.UpdateTicketListEvent(this, EventArgs.Empty);
            }
            App.ExternalAction(x => x.UpdateInfo());
        }

        private void DeleteMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
