using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class TicketSearchRequest : SearchRequest
    {
        [DataMember(Name = "user"), Details]
        public int UserId { get; set; }

        [DataMember(Name = "location"), Details]
        public string Location { get; set; }

        [DataMember(Name = "account"), Details]
        public string Account { get; set; }

        [DataMember(Name = "Class"), Details]
        public string Class { get; set; }

        [DataMember(Name = "role"), Details]
        public string Role { get; set; }

        [DataMember(Name = "status"), Details]
        public string Status { get; set; }

    }
}
