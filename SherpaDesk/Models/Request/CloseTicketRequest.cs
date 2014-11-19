using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class CloseTicketRequest : StatusTicketRequest
    {
        public CloseTicketRequest(string key)
            : base(key, "closed")
        {
        }

        [DataMember(Name = "is_send_notifications"), Details]
        public bool SendNotification { get; set; }

        [DataMember(Name = "resolved"), Details]
        public bool Resolved { get; set; }

        [DataMember(Name = "confirmed"), Details]
        public bool Confirmed { get; set; }

        [DataMember(Name = "confirm_note"), Details]
        public string ConfirmNote { get; set; }


    }
}
