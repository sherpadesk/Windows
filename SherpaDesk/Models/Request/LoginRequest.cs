using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public sealed class LoginRequest : PostRequest
    {
        private const string ERROR_EMPTY_USERNAME = "Username is required field.#UserNameTextbox";
        private const string ERROR_INVALID_EMAIL = "Username should has a email format.#UserNameTextbox";
        private const string ERROR_EMPTY_PASSWORD = "Password is required field.#PasswordTextBox";

        [Required(ErrorMessage = ERROR_EMPTY_USERNAME)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = ERROR_INVALID_EMAIL)]
        [DataMember(Name = "username"), Details]
        public string Email { get; set; }

        [Required(ErrorMessage = ERROR_EMPTY_PASSWORD)]
        [DataMember(Name = "password"), Details("*****")]
        public string Password { get; set; }
    }


}
