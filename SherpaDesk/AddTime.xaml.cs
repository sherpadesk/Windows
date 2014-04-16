﻿using SherpaDesk.Common;
using SherpaDesk.Models;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SherpaDesk
{
    public sealed partial class AddTime : SherpaDesk.Common.LayoutAwarePage
    {
        public AddTime()
        {
            this.InitializeComponent();
        }

        private async void Refresh()
        {
            var date = DateTime.Now;
            DateField.Value = date;
            DateLabel.Text = date.ToString("MMMM dd, yyyy - dddd");
            StartTimePicker.Value = EndTimePicker.Value = date;
            StartTimeLabel.Text = date.ToString("t");
            EndTimeLabel.Text = date.ToString("t");
            using (var connector = new Connector())
            {
                // types
                var resultTaskType = await connector.Func<TaskTypeRequest, NameResponse[]>(
                    x => x.TaskTypes,
                    new TaskTypeRequest());

                if (resultTaskType.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultTaskType);
                    return;
                }

                TaskTypeList.FillData(resultTaskType.Result.AsEnumerable());

                //TaskTypeList.Items.Add(new ComboBoxItem
                //{
                //    Tag = Constants.INITIAL_ID,
                //    Content = Constants.ADD_NEW_TASK_TYPE,
                //    Foreground = new SolidColorBrush(Helper.HexStringToColor(Constants.CLICKABLE_COLOR))
                //});

                // technician
                var resultUsers = await connector.Func<UserResponse[]>(x => x.Users);

                if (resultUsers.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultUsers);
                    return;
                }

                TechnicianList.FillData(
                    resultUsers.Result.Select(user => new NameResponse { Id = user.Id, Name = Helper.FullName(user.FirstName, user.LastName, user.Email) }),
                    new NameResponse { Id = AppSettings.Current.UserId, Name = Constants.TECHNICIAN_ME });

                // projects
                var resultProjects = await connector.Func<ProjectResponse[]>(x => x.Projects);

                if (resultProjects.Status != eResponseStatus.Success)
                {
                    this.HandleError(resultProjects);
                    return;
                }

                ProjectList.FillData(resultProjects.Result.AsEnumerable());

                //AccountList.Items.Add(new ComboBoxItem
                //{
                //    Tag = Constants.INITIAL_ID,
                //    Content = Constants.ADD_NEW_ACCOUNT,
                //    Foreground = new SolidColorBrush(Helper.HexStringToColor(Constants.CLICKABLE_COLOR))
                //});
            }
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var connector = new Connector())
            {
                var hours = decimal.Zero;
                decimal.TryParse(HoursTextBox.Text, out hours);
                var result = await connector.Action<AddTimeRequest>(
                    x => x.Time,
                    new AddTimeRequest
                    {
                        AccountId = -1,
                        ProjectId = ProjectList.GetSelectedValue<int>(-1),
                        TaskTypeId = TaskTypeList.GetSelectedValue<int>(),
                        TechnicianId = TechnicianList.GetSelectedValue<int>(),
                        Billable = BillableBox.IsChecked.HasValue ? BillableBox.IsChecked.Value : false,
                        Hours = hours,
                        Note = NoteTextBox.Text, Date = DateField.Value ?? DateTime.Now
                    });

                if (result.Status != eResponseStatus.Success)
                {
                    this.HandleError(result);
                }
                else
                {
                    ((Frame)this.Parent).Navigate(typeof(Timesheet));
                }
            }
        }

        private void CalculateHours()
        {
            if (StartTimePicker.Value.HasValue && EndTimePicker.Value.HasValue)
            {
                var time = EndTimePicker.Value.Value.TimeOfDay - StartTimePicker.Value.Value.TimeOfDay;
                HoursTextBox.Text = time.TotalHours >= 0 ? String.Format("{0:0.00}", time.TotalHours) : String.Format("{0:0.00}", 24 + time.TotalHours);
            }
        }

        private void DateField_ValueChanged(object sender, EventArgs e)
        {
            DateLabel.Text = DateField.Value.Value.ToString("MMMM dd, yyyy - dddd");
        }

        private void StartTimePicker_ValueChanged(object sender, EventArgs e)
        {
            StartTimeLabel.Text = StartTimePicker.Value.Value.ToString("t");
            CalculateHours();
        }

        private void EndTimePicker_ValueChanged(object sender, EventArgs e)
        {
            EndTimeLabel.Text = EndTimePicker.Value.Value.ToString("t");
            CalculateHours();
        }
    }
}
