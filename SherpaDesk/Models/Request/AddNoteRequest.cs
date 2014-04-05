using SherpaDesk.Common;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class AddNoteRequest : PostRequest
    {
        [DataMember(Name = "ticket"), Details]
        public string TicketKey { get; set; }

        [DataMember(Name = "note_text"), Details]
        public string Note { get; set; }

    }
}
