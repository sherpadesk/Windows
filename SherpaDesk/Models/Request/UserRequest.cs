using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class UserRequest : GetRequest
    {
        [DataMember(Name = "user"), Details]
        public int UserId { get; set; }
    }
}
