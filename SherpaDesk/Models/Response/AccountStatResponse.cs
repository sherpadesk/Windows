using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class AccountStatResponse : NameResponse
    {
        [DataMember(Name = "account_statistics"), Details]
        public AccountStatistics StatInfo { get; set; }

    }

    [DataContract]
    public class AccountStatistics : ObjectBase
    {

        [DataMember(Name = "ticket_counts"), Details]
        public TicketCountsSimpleResponse TicketCounts { get; set; }

        [DataMember(Name = "timelogs"), Details]
        public int TimeLogs { get; set; }

        [DataMember(Name = "invoices"), Details]
        public int Invoices { get; set; }

    }
}

//\"id\":-1,
//\"name\":\"bigWebApps Windows Store\",
//\"account_statistics\":
//    \"ticket_counts\":
//        \"open\":4,
//        \"closed\":16,
//        \"scheduled\":0,
//        \"followups\":0,
//    \"timelogs\":9,
//    \"invoices\":0
//\"id\":27,
//\"name\":\"AlexeyG\",
//\"account_statistics\": 
//    \"ticket_counts\":  
//        \"open\":2,
//        \"closed\":1,
//        \"scheduled\":0,
//        \"followups\":0},
//    \"timelogs\":5,
//    \"invoices\":0
