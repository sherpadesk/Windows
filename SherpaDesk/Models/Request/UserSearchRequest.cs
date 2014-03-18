using System;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public sealed class UserSearchRequest : SearchRequest
    {
        [DataMember(Name = "lastname"), Details]
        public string LastName { get; set; }

        [DataMember(Name = "firstname"), Details]
        public string FirstName { get; set; }

        [DataMember(Name = "email"), Details]
        public string Email { get; set; }

        [DataMember(Name = "id"), Details]
        public int Id { get; set; }
    }
}
