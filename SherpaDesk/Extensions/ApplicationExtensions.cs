using SherpaDesk.Common;
using SherpaDesk.Models;

namespace SherpaDesk.Extensions
{
    public static class ApplicationExtensions
    {
        public static T IsNull<T>(this T obj, string message)
        {
            if ((typeof(T) == typeof(string)
                && string.IsNullOrEmpty(obj as string))
                || obj.Equals(default(T)))
            {
                throw new InternalException(message, eErrorType.InvalidOutputData);
            }
            return obj;
        }

    }
}
