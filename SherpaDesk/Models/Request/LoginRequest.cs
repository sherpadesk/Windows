﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public sealed class LoginRequest : ObjectBase
    {
        private const string ERROR_EMPTY_USERNAME = "Username is required field.";
        private const string ERROR_INVALID_EMAIL = "Username should has a email format.";
        private const string ERROR_EMPTY_PASSWORD = "Password is required field.";

        [Required(ErrorMessage = ERROR_EMPTY_USERNAME)]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = ERROR_INVALID_EMAIL)]
        [DataMember(Name = "username"), Details]
        public string Email { get; set; }

        [Required(ErrorMessage = ERROR_EMPTY_PASSWORD)]
        [DataMember(Name = "password"), Details("*****")]
        public string Password { get; set; }
    }


}
