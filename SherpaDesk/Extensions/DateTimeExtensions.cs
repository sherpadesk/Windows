using System;

namespace SherpaDesk.Extensions
{
    public static class DateTimeExtensions
    {
        public static string CalculateDate(this TimeSpan ts)
        {
            if (ts < TimeSpan.FromMinutes(1))
                return "just now";
            if (ts < TimeSpan.FromMinutes(2))
                return "1 minute";
            if (ts < TimeSpan.FromHours(1))
                return ts.Minutes + " minutes";
            if (ts < TimeSpan.FromHours(2))
                return "1 hour";
            if (ts < TimeSpan.FromDays(1))
                return ts.Hours + " hours";
            if (ts < TimeSpan.FromDays(2))
                return "1 day";
            if (ts < TimeSpan.FromDays(30))
                return ts.Days + " days";
            if (ts < TimeSpan.FromDays(60))
                return "1 month";
            if (ts < TimeSpan.FromDays(356))
                // ReSharper disable once PossibleLossOfFraction
                return Convert.ToInt32(Math.Floor((decimal)(ts.Days / 30))) + " months";
            if (ts < TimeSpan.FromDays(712))
                return "1 year";

            // ReSharper disable once PossibleLossOfFraction
            return Convert.ToInt32(Math.Floor((decimal)(ts.Days / 356))) + " years";
        }
        public static string DaysAgo(this TimeSpan ts)
        {
            if (ts < TimeSpan.FromMinutes(1))
                return "just now";
            if (ts < TimeSpan.FromMinutes(2))
                return "1 minute";
            if (ts < TimeSpan.FromHours(1))
                return ts.Minutes + " minutes";
            if (ts < TimeSpan.FromHours(2))
                return "1 hour";
            if (ts <= TimeSpan.FromDays(1))
                return ts.Hours + " hours";
            if (ts < TimeSpan.FromDays(2))
                return "1 day" + HoursToString((ts - TimeSpan.FromDays(1)).Hours);

            return ts.Days + " days" + HoursToString((ts - TimeSpan.FromDays(ts.Days)).Hours);
        }

        private static string HoursToString(int hours)
        {
            switch (hours)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return " 1 hour";
                default:
                    return " " + hours + " hours";
            }
        }
    }
}
