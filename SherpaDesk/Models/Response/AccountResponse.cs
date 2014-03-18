using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class AccountResponse : NameResponse
    {
        [DataMember(Name = "ticket_counts"), Details]
        public TicketCountsSimpleResponse TicketCounts { get; set; }

    }
}
