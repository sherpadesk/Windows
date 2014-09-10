
using SherpaDesk.Common;
using System.Runtime.Serialization;
using SherpaDesk.Interfaces;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class NoteSearchRequest : SearchRequest, IPath
    {
        public NoteSearchRequest(string ticketKey)
        {
            this.Path = "/" + ticketKey + "/posts";
        }
        [DataMember(Name = "is_waiting_on_response"), Details]
        public bool IsWaiting { get; set; }

        [DataMember(Name = "ticket"), Details]
        public string TicketKey { get; set; }

        public string Path { get; set; }

    }
}
