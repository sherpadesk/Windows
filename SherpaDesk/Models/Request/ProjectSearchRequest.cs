using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public sealed class ProjectRequest : GetRequest
    {
        [DataMember(Name = "account"), Details]
        public int AccountId { get; set; }

        [DataMember(Name = "tech"), Details]
        public int TechnicianId { get; set; }
    }
}
