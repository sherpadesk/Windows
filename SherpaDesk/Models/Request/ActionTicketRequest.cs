using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public abstract class ActionTicketRequest : PutRequest
    {
        protected ActionTicketRequest(string key, string action) : 
            base(key)
        {
            this.Action = action;
        }


        [DataMember(Name = "action"), Details]
        public string Action { get; private set; }

        [DataMember(Name = "note_text"), Details]
        public string Note { get; set; }

    }
}
