using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class LoginRequest : ObjectBase
    {
        private const string ERROR_EMPTY_USERNAME = "Username is required field.";
        private const string ERROR_EMPTY_PASSWORD = "Password is required field.";

        [Required(ErrorMessage = ERROR_EMPTY_USERNAME)]
        [DataMember(Name = "username"), Details]
        public string Username { get; set; }

        [Required(ErrorMessage = ERROR_EMPTY_PASSWORD)]
        [DataMember(Name = "password"), Details("*****")]
        public string Password { get; set; }
    }


}
