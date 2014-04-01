using SherpaDesk.Models;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SherpaDesk.Common
{
    public static class ApplicationExtensions
    {
        public static T IsNull<T>(this T obj, string message)
        {
            if ((typeof(T).Equals(typeof(string))
                && string.IsNullOrEmpty(obj as string))
                || obj.Equals(default(T)))
            {
                throw new InternalException(message, eErrorType.InvalidOutputData);
            }
            return obj;
        }

    }
}
