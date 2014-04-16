using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class PlaceOnHoldRequest : StatusTicketRequest
    {
        public PlaceOnHoldRequest(string key)
            : base(key, "onhold")
        {
        }
    }
}
