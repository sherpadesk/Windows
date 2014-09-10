using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SherpaDesk.Models;

namespace SherpaDesk.Extensions
{
    public static class EnumExtentions
    {
        public static String Name(this Enum item)
        {
            return Enum.GetName(item.GetType(), item);
        }

        public static String Details(this Enum item)
        {
            var result = item.ToString();

            var type = item.GetType();

            var memInfo = type.GetRuntimeField(item.ToString());

            if (memInfo != null)
            {
                var attrs = memInfo.GetCustomAttributes<DetailsAttribute>().ToArray();

                if (attrs != null && attrs.Length > 0)
                {
                    result = attrs[0].Text;
                }
            }

            return result;
        }

        public static List<T> GetAllItems<T>() where T : struct
        {
            return Enum.GetValues(typeof (T)).Cast<T>().ToList();
        }
    }
}
