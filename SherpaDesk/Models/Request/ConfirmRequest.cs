using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class ConfirmRequest : ActionTicketRequest
    {
        public ConfirmRequest(string key) : base(key, "confirm") { }
    }
}
