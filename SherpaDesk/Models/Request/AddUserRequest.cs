using SherpaDesk.Common;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class AddUserRequest : PostRequest
    {
        private const string ERROR_EMPTY_LAST_NAME = "Last name is required field.#LastNameTextbox";
        private const string ERROR_EMPTY_FIRST_NAME = "First name is required field.#FirstNameTextbox";
        private const string ERROR_EMPTY_EMAIL = "Email is required field.#EmailTextbox";
        private const string ERROR_INVALID_EMAIL = "Email has a incorrect format.#EmailTextbox";
        private const string ERROR_EMPTY_ACCOUNT_ID = "Account idetifier is mandatory parameter.";
        private const string ERROR_EMPTY_LOCATION_ID = "Location idetifier is mandatory parameter.";

        [Required(ErrorMessage = ERROR_EMPTY_LAST_NAME)]
        [DataMember(Name = "lastname"), Details]
        public string LastName { get; set; }

        [Required(ErrorMessage = ERROR_EMPTY_FIRST_NAME)]
        [DataMember(Name = "firstname"), Details]
        public string FirstName { get; set; }

        [Required(ErrorMessage = ERROR_EMPTY_EMAIL)]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = ERROR_INVALID_EMAIL)]
        [DataMember(Name = "email"), Details]
        public string Email { get; set; }

        [IntRequired(ErrorMessage = ERROR_EMPTY_ACCOUNT_ID)]
        [DataMember(Name = "account"), Details]
        public int AccountId { get; set; }

        [IntRequired(ErrorMessage = ERROR_EMPTY_LOCATION_ID)]
        [DataMember(Name = "location"), Details]
        public int LocationId { get; set; }

    }
}
