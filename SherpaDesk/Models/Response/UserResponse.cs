using SherpaDesk.Common;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class UserResponse : ObjectBase
    {
        [DataMember(Name = "id"), Details]
        public int Id { get; set; }

        [DataMember(Name = "email"), Details]
        public string Email { get; set; }

        [DataMember(Name = "firstname"), Details]
        public string FirstName { get; set; }

        [DataMember(Name = "lastname"), Details]
        public string LastName { get; set; }

        [DataMember(Name = "type"), Details]
        public string Role { get; set; }

        [Details]
        public string FullName
        {
            get
            {
                return Helper.FullName(this.FirstName, this.LastName, this.Email);
            }
        }
    }

}
