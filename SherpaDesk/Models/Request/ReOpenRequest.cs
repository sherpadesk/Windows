using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class ReOpenRequest : StatusTicketRequest
    {
        public ReOpenRequest(string key)
            : base(key, "open")
        {
        }
    }
}
