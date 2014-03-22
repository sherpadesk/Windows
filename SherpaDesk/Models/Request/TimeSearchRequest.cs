using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class TimeSearchRequest : SearchRequest
    {
        [DataMember(Name = "task_type_id"), Details]
        public int TaskTypeId { get; set; }

        [DataMember(Name = "ticket_time_id"), Details]
        public int TicketTimeId { get; set; }

        [DataMember(Name = "project_time_id"), Details]
        public int ProjectTimeId { get; set; }

        [DataMember(Name = "tech"), Details]
        public int TechnicianId { get; set; }

        [DataMember(Name = "account"), Details]
        public int AccountId { get; set; }

        [DataMember(Name = "type"), Details]
        public eTimeType TimeType { get; set; }

        [DataMember(Name = "project"), Details]
        public int ProjectId { get; set; }

        [DataMember(Name = "ticket"), Details]
        public string TicketKey { get; set; }

        [DataMember(Name = "key"), Details]
        public string TimeKey { get; set; }
    }
}
