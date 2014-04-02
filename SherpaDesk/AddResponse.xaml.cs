using SherpaDesk.Common;
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
    public sealed partial class AddResponse : SherpaDesk.Common.LayoutAwarePage
    {
        private const string ERROR_EMPTY_HOURS = "Hours should be positive number.";
        private const string ERROR_MUCH_HOURS = "Hours cannot be more then 24 hours in day.";

        public AddResponse()
        {
            this.InitializeComponent();
        }

        private void StartTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (StartTimePicker.Value.HasValue)
                StartTimeLabel.Text = StartTimePicker.Value.Value.ToString("t");
            CalculateHours();
        }

        private void EndTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (EndTimePicker.Value.HasValue)
                EndTimeLabel.Text = EndTimePicker.Value.Value.ToString("t");
            CalculateHours();
        }

        private void CalculateHours()
        {
            if (StartTimePicker.Value.HasValue && EndTimePicker.Value.HasValue)
            {
                var time = EndTimePicker.Value.Value.TimeOfDay - StartTimePicker.Value.Value.TimeOfDay;
                HoursTextBox.Text = time.TotalHours >= 0 ? String.Format("{0:0.00}", time.TotalHours) : String.Format("{0:0.00}", 24 + time.TotalHours);
            }
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            var date = DateTime.Now;
            StartTimePicker.Value = EndTimePicker.Value = date;
            StartTimeLabel.Text = date.ToString("t");
            EndTimeLabel.Text = date.ToString("t");
        }

        private void SaveButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }
    }
}
