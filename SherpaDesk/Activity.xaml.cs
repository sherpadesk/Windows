﻿using System.Linq;
using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Response;
using Windows.UI.Xaml;

namespace SherpaDesk
{
    public sealed partial class Activity : SherpaDesk.Common.LayoutAwarePage
    {
        public Activity()
        {
            this.InitializeComponent();
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var resultActivities = await connector.Func<ActivityResponse[]>(x => x.Activity);
                if (resultActivities.Status != eResponseStatus.Success)
                {
                    this.pageRoot.HandleError(resultActivities);
                    return;
                }
                var dataSource = resultActivities.Result.Select(x => new
                {
                    UserName = x.UserName,
                    Title = x.Title,
                    Note = Helper.HtmlToString(x.Note)
                }).ToList();

                if (dataSource.Count > 0)
                {
                    ActivityList.ItemsSource = dataSource;
                    ActivityList.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    ActivityList.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }

            }
        }
    }
}
