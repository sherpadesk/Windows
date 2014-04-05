using System.Runtime.Serialization;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class ProjectResponse : NameResponse
    {
        [DataMember(Name = "account_id"), Details]
        public int AccountId { get; set; }

        [DataMember(Name = "account_name"), Details]
        public string AccountName { get; set; }

        [DataMember(Name = "open_tickets"), Details]
        public int OpenTickets { get; set; }

        [DataMember(Name = "closed_tickets"), Details]
        public int ClosedTickets { get; set; }

        [DataMember(Name = "complete"), Details]
        public int Complete { get; set; }

        [DataMember(Name = "logged_hours"), Details]
        public decimal LoggedHours { get; set; }

        [DataMember(Name = "remaining_hours"), Details]
        public decimal RemainingHours { get; set; }

        [DataMember(Name = "client_manager"), Details]
        public string Manager { get; set; }

        //\\"priority\":null,\"priority_name\":\"\"
    }
}
