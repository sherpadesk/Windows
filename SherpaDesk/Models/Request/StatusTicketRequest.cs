using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public abstract class StatusTicketRequest : PutRequest
    {
        public StatusTicketRequest(string key, string status)
            : base(key)
        {
            this.Status = status;
        }

        [DataMember(Name = "status"), Details]
        public string Status { get; private set; }

        [DataMember(Name = "note_text"), Details]
        public string Note { get; set; }

    }
}
