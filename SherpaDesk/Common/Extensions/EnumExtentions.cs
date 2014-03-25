using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SherpaDesk.Common
{
    //public class EnumExtentions
    //{
    //            public static String Name(this Enum item)
    //    {
    //        return Enum.GetName(item.GetType(), item);
    //    }

    //    public static String Description(this Enum item)
    //    {
    //        String result = item.ToString();

    //        Type type = item.GetType();

    //        MemberInfo[] memInfo = type.GetMember(item.ToString());

    //        if (memInfo != null && memInfo.Length > 0)
    //        {
    //            Object[] attrs = memInfo[0].GetCustomAttributes(typeof (DescriptionAttribute), false);

    //            if (attrs != null && attrs.Length > 0)
    //            {
    //                result = ((DescriptionAttribute) attrs[0]).Description;
    //            }
    //        }

    //        return result;
    //    }

    //    public static List<T> GetAllItems<T>() where T : struct
    //    {
    //        var result = new List<T>();

    //        foreach (T item in Enum.GetValues(typeof (T)))
    //        {
    //            result.Add(item);
    //        }

    //        return result;
    //    }
    //}

    //}
}
