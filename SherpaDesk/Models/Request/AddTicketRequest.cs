using SherpaDesk.Common;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class AddTicketRequest : PostRequest
    {
        private const string ERROR_EMPTY_TECHNICIAN_ID = "Please select a Technician.#TechnicianList";
        private const string ERROR_EMPTY_USER_ID = "Please select a End User.#EndUserList";
        private const string ERROR_EMPTY_ACCOUNT_ID = "Please select a Account.#AccountList";
        private const string ERROR_EMPTY_CLASS_ID = "Please select a Class.#ClassList";
        private const string ERROR_EMPTY_SUBJECT = "Please enter a Subject.#SubjectTextbox";
        
        [DataMember(Name = "status"), Details]
        public string Status { get; set; }

        [DataMember(Name = "subject"), Details]
        [Required(ErrorMessage = ERROR_EMPTY_SUBJECT)]
        public string Name { get; set; }

        [DataMember(Name = "initial_post"), Details]
        public string Comment { get; set; }

        [DataMember(Name = "class_id"), Details]
        [IntRequired(ErrorMessage = ERROR_EMPTY_CLASS_ID)]
        public int ClassId { get; set; }

        [DataMember(Name = "account_id"), Details]
        [IntRequired(ErrorMessage = ERROR_EMPTY_ACCOUNT_ID)]
        public int AccountId { get; set; }

        [DataMember(Name = "user_id"), Details]
        [IntRequired(ErrorMessage = ERROR_EMPTY_USER_ID)]
        public int UserId { get; set; }

        [DataMember(Name = "tech_id"), Details]
        [IntRequired(ErrorMessage = ERROR_EMPTY_TECHNICIAN_ID)]
        public int TechnicianId { get; set; }
    }
}
