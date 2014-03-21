using System;
using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SherpaDesk
{
    public sealed partial class Timesheet : SherpaDesk.Common.LayoutAwarePage
    {
        public Timesheet()
        {
            this.InitializeComponent();
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var result = await connector.Operation<TimeSearchRequest, TimeResponse[]>(
                    "time",
                    new TimeSearchRequest
                    {
                        TechnicianId = AppSettings.Current.UserId
                    });
                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                }
                foreach (var time in result.Result)
                {
                    TimesheetGridView.Items.Add(
                        new GridViewItem
                        {
                            Content = string.Format("{0}: {1} hours{2}{3}", time.Date.ToString("D"), time.Hours.ToString("F"), Environment.NewLine, time.Note)
                        });
                }
            }
        }
    }
}
