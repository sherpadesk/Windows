using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class AddTicketRequest : PostRequest
    {
        [DataMember(Name = "status"), Details]
        public string Status { get; set; }

        [DataMember(Name = "subject"), Details]
        public string Name { get; set; }

        [DataMember(Name = "initial_post"), Details]
        public string Comment { get; set; }

        [DataMember(Name = "class_id"), Details]
        public int ClassId { get; set; }

        [DataMember(Name = "account_id"), Details]
        public int AccountId { get; set; }

        [DataMember(Name = "user_id"), Details]
        public int UserId { get; set; }

        [DataMember(Name = "tech_id"), Details]
        public int TechnicianId { get; set; }
    }
}
