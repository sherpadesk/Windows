using SherpaDesk.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class AddTimeRequest : PostRequest
    {
        private const string ERROR_EMPTY_TECHNICIAN_ID = "Please select a Technician.#TechnicianList";
        private const string ERROR_EMPTY_ACCOUNT_ID = "Please select a Account.#AccountList";
        private const string ERROR_EMPTY_TASK_TYPE_ID = "Please select a Task Type.#TaskTypeList";
        private const string ERROR_EMPTY_HOURS = "Hours should be positive number.#ErrorHours";
        private const string ERROR_MUCH_HOURS = "Hours cannot be more then 24 hours in day.#ErrorHours";

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

        [DataMember(Name = "date", EmitDefaultValue = false)]
        protected string _date;

        [Details]
        public DateTime Date { get; set; }

        [DataMember(Name = "start_date", EmitDefaultValue = false)]
        protected string _startDate;

        [Details]
        public DateTime? StartDate { get; set; }

        [DataMember(Name = "stop_date", EmitDefaultValue = false)]
        protected string _stopDate;

        [Details]
        public DateTime? StopDate { get; set; }

        [DataMember(Name = "is_project_time"), Details]
        public bool IsProjectTime { get; set; }

        [DataMember(Name = "complete"), Details]
        public string Complete { get; set; }

        [OnSerializing]
        protected void OnSerializing(StreamingContext context)
        {
            if (this.Date != DateTime.MinValue)
                this._date = this.Date.ToString("yyyy-MM-dd");
            if (this.StartDate.HasValue)
                this._startDate = this.StartDate.Value.ToString("yyyy-MM-dd hh:mm:00");
            if (this.StopDate.HasValue)
                this._stopDate = this.StopDate.Value.ToString("yyyy-MM-dd hh:mm:00");
        }
    }

    [DataContract]
    public class UpdateTimeRequest : AddTimeRequest
    {
        public UpdateTimeRequest(string ticketKey, int timeId)
        {
            this.TicketKey = ticketKey;
            this.TicketTimeId = timeId;
            this.IsProjectTime = false;
        }

        public UpdateTimeRequest(int projectId, int timeId)
        {
            this.ProjectId = projectId;
            this.ProjectTimeId = timeId;
            this.IsProjectTime = true;
        }

        public override eRequestType Type
        {
            get
            {
                return eRequestType.PUT;
            }
        }

        [DataMember(Name = "project_time_id"), Details]
        public int ProjectTimeId { get; set; }

        [DataMember(Name = "ticket_time_id"), Details]
        public int TicketTimeId { get; set; }
    }

    [DataContract]
    public class DeleteTimeRequest : DeleteRequest
    {
        public DeleteTimeRequest(int timeId, bool isProject)
            : base(timeId.ToString())
        {
            IsProjectLog = isProject;
        }

        [DataMember(Name = "is_project_log"), Details]
        public bool IsProjectLog { get; set; }
    }
}
