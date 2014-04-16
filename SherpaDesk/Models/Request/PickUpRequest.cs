using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class PickUpRequest : ActionTicketRequest
    {
        public PickUpRequest(string key) : base(key, "pickup") { }
    }
}
