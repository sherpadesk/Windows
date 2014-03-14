using System.Runtime.Serialization;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class LoginResponse : ObjectBase
    {
        [DataMember(Name = "api_token"), Details("********")]
        public string ApiToken { get; set; }
    }
}
