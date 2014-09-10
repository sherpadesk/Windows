using System.Net.Http;
using SherpaDesk.Models;

namespace SherpaDesk.Interfaces
{
    public interface IRequestType
    {
        eRequestType Type { get; }

        bool IsEmpty { get; }

        HttpContent GetContent();
    }

}
