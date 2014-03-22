using System;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class TimeResponse : ObjectBase
    {
        [DataMember(Name = "project_time_id"), Details]
        public int TimeId { get; set; }

        [DataMember(Name = "user_id"), Details]
        public int UserId { get; set; }

        [DataMember(Name = "name"), Details]
        public string UserName { get; set; }

        [DataMember(Name = "task_type_id"), Details]
        public int TaskTypeId { get; set; }

        [DataMember(Name = "task_type_name"), Details]
        public string TaskTypeName { get; set; }

        [DataMember(Name = "project_id"), Details]
        public int ProjectId { get; set; }

        [DataMember(Name = "project_name"), Details]
        public string ProjectName { get; set; }

        [DataMember(Name = "account_id"), Details]
        public int AccountId { get; set; }

        [DataMember(Name = "account_name"), Details]
        public string AccountName { get; set; }

        [DataMember(Name = "note"), Details]
        public string Note { get; set; }

        [DataMember(Name = "date", EmitDefaultValue = false)]
        private string _date;
        
        [Details]
        public DateTime Date { get; set; }

        [DataMember(Name = "start_time")]
        private string _startTime;

        [Details]
        public DateTime StartTime { get; set; }

        [DataMember(Name = "stop_time")]
        private string _stopTime;

        [Details]
        public DateTime StopTime { get; set; }

        [DataMember(Name = "hours"), Details]
        public decimal Hours { get; set; }

        [DataMember(Name = "fb_time_entry_id"), Details]
        public int FB_TimeId { get; set; }

        [DataMember(Name = "fb_task_type_id"), Details]
        public int FB_TaskTypeId { get; set; }

        [DataMember(Name = "fb_staff_id"), Details]
        public int FB_StaffId { get; set; }

        [DataMember(Name = "fb_client_id"), Details]
        public int FB_ClientId { get; set; }

        [DataMember(Name = "fb_project_id"), Details]
        public int FB_ProjectId { get; set; }

        [DataMember(Name = "fb_default_project_id"), Details]
        public int FB_DefaultProjectId { get; set; }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            DateTime date;
            if (DateTime.TryParse(this._date, out date))
                this.Date = date;
            if (DateTime.TryParse(this._startTime, out date))
                this.StartTime = date;
            if (DateTime.TryParse(this._stopTime, out date))
                this.StopTime = date;
        }

    }
}
