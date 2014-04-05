using SherpaDesk.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class WaitingOnPostRequest : PostRequest, IPath
    {
        public WaitingOnPostRequest(string ticketKey)
        {
            this.Path = "/" + ticketKey;
            this.IsWaitingOnPost = true;
        }
        [DataMember(Name = "note_text"), Details]
        public string Note { get; set; }

        [DataMember(Name = "is_waiting_on_post"), Details]
        public bool IsWaitingOnPost { get; private set; }

        [Details]
        public string Path { get; set; }
    }
}
