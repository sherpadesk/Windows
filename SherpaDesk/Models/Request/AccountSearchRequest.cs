using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public sealed class AccountSearchRequest : SearchRequest
    {
        [DataMember(Name = "user"), Details]
        public int UserId { get; set; }

    }
}
