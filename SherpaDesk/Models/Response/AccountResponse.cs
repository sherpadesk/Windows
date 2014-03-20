using System.Runtime.Serialization;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class AccountResponse : NameResponse
    {
        [DataMember(Name = "ticket_counts"), Details]
        public TicketCountsSimpleResponse TicketCounts { get; set; }

    }
}
