using SherpaDesk.Models;
using System.Net.Http;

namespace SherpaDesk.Common
{
    public interface IRequestType
    {
        eRequestType Type { get; }

        bool IsEmpty { get; }

        HttpContent GetContent();
    }

}
