using SherpaDesk.Models;

namespace SherpaDesk.Common
{
    public interface IRequestType
    {
        eRequestType Type { get; }

        bool IsEmpty { get; }
    }

}
