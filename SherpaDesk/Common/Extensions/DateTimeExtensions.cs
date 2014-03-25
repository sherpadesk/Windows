using System;

namespace SherpaDesk.Common
{
    public static class DateTimeExtensions
    {
        public static string CalculateTime(this TimeSpan ts)
        {
            string result = string.Empty;
            if (ts.Minutes > 0)
                result = string.Concat(ts.Minutes, "m");
            if (ts.Hours > 0)
                result = string.Concat(ts.Hours, "h", " ", result);
            if (ts.Days > 0)
                result = string.Concat(ts.Days, "d", " ", result);
            return result.Trim();
        }
    }
}
