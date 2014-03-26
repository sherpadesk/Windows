using SherpaDesk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SherpaDesk.Common
{
    public static class EnumExtentions
    {
        public static String Name(this Enum item)
        {
            return Enum.GetName(item.GetType(), item);
        }

        public static String Description(this Enum item)
        {
            String result = item.ToString();

            Type type = item.GetType();

            FieldInfo memInfo = type.GetRuntimeField(item.ToString());

            if (memInfo != null)
            {
                var attrs = memInfo.GetCustomAttributes<DetailsAttribute>().ToArray();

                if (attrs != null && attrs.Length > 0)
                {
                    result = ((DetailsAttribute) attrs[0]).Text;
                }
            }

            return result;
        }

        public static List<T> GetAllItems<T>() where T : struct
        {
            var result = new List<T>();

            foreach (T item in Enum.GetValues(typeof (T)))
            {
                result.Add(item);
            }

            return result;
        }
    }
}
