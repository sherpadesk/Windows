using SherpaDesk.Common;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class AddTimeRequest : ObjectBase
    {
        private const string ERROR_EMPTY_TECHNICIAN_ID = "Please select a Technician.#TechnicianList";
        private const string ERROR_EMPTY_ACCOUNT_ID = "Please select a Account.#AccountList";
        private const string ERROR_EMPTY_TASK_TYPE_ID = "Please select a Task Type.#TaskTypeList";
        private const string ERROR_EMPTY_HOURS = "Hours should be positive number.#HoursTextBox";
        private const string ERROR_MUCH_HOURS = "Hours cannot be more then 24 hours in day.#HoursTextBox";
        
        
        [DataMember(Name = "ticket_key"), Details]
        public string TicketKey { get; set; }

        [DataMember(Name = "task_type_id"), Details]
        [IntRequired(ErrorMessage = ERROR_EMPTY_TASK_TYPE_ID)]
        public int TaskTypeId { get; set; }

        [DataMember(Name = "note_text"), Details]
        public string Note { get; set; }

        [IntRequired(ErrorMessage = ERROR_EMPTY_HOURS)]
        [Range(0.01, 24.0, ErrorMessage = ERROR_MUCH_HOURS)]
        [DataMember(Name = "hours"), Details]
        public decimal Hours { get; set; }

        [DataMember(Name = "is_billable"), Details]
        public bool Billable { get; set; }

        [DataMember(Name = "project_id"), Details]
        public int ProjectId { get; set; }

        [IntRequired(ErrorMessage = ERROR_EMPTY_ACCOUNT_ID)]
        [DataMember(Name = "account_id"), Details]
        public int AccountId { get; set; }

        [IntRequired(ErrorMessage = ERROR_EMPTY_TECHNICIAN_ID)]
        [DataMember(Name = "tech_id"), Details]
        public int TechnicianId { get; set; }
    }
}
