using System;

namespace SherpaDesk.Common
{
    public static class DateTimeExtensions
    {
        public static string CalculateDate(this TimeSpan ts)
        {
            if (ts < TimeSpan.FromMinutes(1))
                return "just now";
            else if (ts < TimeSpan.FromMinutes(2))
                return "1 minute";
            else if (ts < TimeSpan.FromHours(1))
                return ts.Minutes.ToString() + " minutes";
            else if (ts < TimeSpan.FromHours(2))
                return "1 hour";
            else if (ts < TimeSpan.FromDays(1))
                return ts.Hours.ToString() + " hours";
            else if (ts < TimeSpan.FromDays(2))
                return "1 day";
            else if (ts < TimeSpan.FromDays(30))
                return ts.Days.ToString() + " days";
            else if (ts < TimeSpan.FromDays(60))
                return "1 month";
            else if (ts < TimeSpan.FromDays(356))
                return Convert.ToInt32(Math.Floor((decimal)(ts.Days / 30))).ToString() + " months";
            else if (ts < TimeSpan.FromDays(712))
                return "1 year";
            else
                return Convert.ToInt32(Math.Floor((decimal)(ts.Days / 356))).ToString() + " years";
        }
        public static string DaysAgo(this TimeSpan ts)
        {
            if (ts < TimeSpan.FromMinutes(1))
                return "just now";
            else if (ts < TimeSpan.FromMinutes(2))
                return "1 minute";
            else if (ts < TimeSpan.FromHours(1))
                return ts.Minutes.ToString() + " minutes";
            else if (ts < TimeSpan.FromHours(2))
                return "1 hour";
            else if (ts <= TimeSpan.FromDays(1))
                return ts.Hours.ToString() + " hours";
            else if (ts < TimeSpan.FromDays(2))
                return "1 day" + HoursToString((ts - TimeSpan.FromDays(1)).Hours);
            else
                return ts.Days.ToString() + " days" + HoursToString((ts - TimeSpan.FromDays(ts.Days)).Hours);
        }

        private static string HoursToString(int hours)
        {
            if (hours == 0)
                return string.Empty;
            else if (hours == 1)
                return " 1 hour";
            else
                return " " + hours.ToString() + " hours";
        }
    }
}
