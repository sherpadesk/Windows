using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class TaskTypeRequest : PostRequest
    {
        [DataMember(Name = "key"), Details]
        public string Key { get; set; }

        [DataMember(Name = "ticket"), Details]
        public string Ticket { get; set; }

        [DataMember(Name = "project"), Details]
        public int ProjectId { get; set; }

        [DataMember(Name = "account"), Details]
        public int AccountId { get; set; }
    }
}
