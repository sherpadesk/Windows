using System;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class TimeResponse : ObjectBase
    {
        [DataMember(Name = "time_id"), Details]
        public int TimeId { get; set; }

        [DataMember(Name = "user_id"), Details]
        public int UserId { get; set; }

        [DataMember(Name = "user_name"), Details]
        public string UserName { get; set; }

        [DataMember(Name = "task_type_id"), Details]
        public int TaskTypeId { get; set; }

        [DataMember(Name = "task_type"), Details]
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

        [Details]
        public string StartStopTime
        {
            get
            {
                if (this.StartTime != DateTime.MinValue && this.StopTime != DateTime.MinValue)
                {
                    return string.Format("{0:t}-{1:t}", this.StartTime, this.StopTime);
                }
                else
                    return string.Empty;
            }
        }

        [DataMember(Name = "hours"), Details]
        public decimal Hours { get; set; }

        [DataMember(Name = "ticket_id"), Details]
        public int TicketId { get; set; }

        [DataMember(Name = "ticket_number"), Details]
        public int TicketNumber { get; set; }

        [DataMember(Name = "billable"), Details]
        public bool Billable { get; set; }

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
        //{\"time_id\":8332,\"project_name\":\"\",\"user_name\":\"Alexey Gavrilov\",\"user_email\":\"alexey.gavrilov@gmail.com\",\"note\":\"test\",\"date\":\"02 Apr 2014\",\"hours\":1.7500,\"fb_id\":0,\"is_project_log\":true,\"ticket_id\":0,\"task_type_id\":4261,\"task_type\":\"Desktop Support\",\"project_id\":0,\"account_id\":-1,\"ticket_number\":0,\"account_name\":\"Yoshkar\",\"ticket_subject\":\"\",\"invoice_id\":0,\"billable\":true,\"invoice_pseudo_id\":\"      \"},
        //{\"time_id\":8331,\"project_name\":\"\",\"user_name\":\"Alexey Gavrilov\",\"user_email\":\"alexey.gavrilov@gmail.com\",\"note\":\"timelog without ticket\",\"date\":\"02 Apr 2014\",\"hours\":2.2200,\"fb_id\":0,\"is_project_log\":true,\"ticket_id\":0,\"task_type_id\":4260,\"task_type\":\"Consulting\",\"project_id\":0,\"account_id\":-1,\"ticket_number\":0,\"account_name\":\"Yoshkar\",\"ticket_subject\":\"\",\"invoice_id\":0,\"billable\":true,\"invoice_pseudo_id\":\"      \"},
        //{\"time_id\":61780,\"project_name\":\"\",\"user_name\":\"Alexey Gavrilov\",\"user_email\":\"alexey.gavrilov@gmail.com\",\"note\":\"first time log for ticket\",\"date\":\"02 Apr 2014\",\"hours\":3.3300,\"fb_id\":0,\"is_project_log\":false,\"ticket_id\":95286,\"task_type_id\":4263,\"task_type\":\"Project Management\",\"project_id\":0,\"account_id\":-1,\"ticket_number\":1,\"account_name\":\"Yoshkar\",\"ticket_subject\":\"Test Ticket\",\"invoice_id\":0,\"billable\":true,\"invoice_pseudo_id\":\"      \"},
        //{\"time_id\":8310,\"project_name\":\"\",\"user_name\":\"Alexey Gavrilov\",\"user_email\":\"alexey.gavrilov@gmail.com\",\"note\":\"rwer\",\"date\":\"01 Apr 2014\",\"hours\":2.0000,\"fb_id\":0,\"is_project_log\":true,\"ticket_id\":0,\"task_type_id\":4261,\"task_type\":\"Desktop Support\",\"project_id\":0,\"account_id\":-1,\"ticket_number\":0,\"account_name\":\"Yoshkar\",\"ticket_subject\":\"\",\"invoice_id\":0,\"billable\":true,\"invoice_pseudo_id\":\"      \"},
        //{\"time_id\":8120,\"project_name\":\"\",\"user_name\":\"Alexey Gavrilov\",\"user_email\":\"alexey.gavrilov@gmail.com\",\"note\":\"very very test\",\"date\":\"21 Mar 2014\",\"hours\":4.4400,\"fb_id\":0,\"is_project_log\":true,\"ticket_id\":0,\"task_type_id\":4262,\"task_type\":\"Network Support\",\"project_id\":0,\"account_id\":-1,\"ticket_number\":0,\"account_name\":\"Yoshkar\",\"ticket_subject\":\"\",\"invoice_id\":0,\"billable\":true,\"invoice_pseudo_id\":\"      \"},
        //{\"time_id\":8112,\"project_name\":\"\",\"user_name\":\"Alexey Gavrilov\",\"user_email\":\"alexey.gavrilov@gmail.com\",\"note\":\"huge test\",\"date\":\"21 Mar 2014\",\"hours\":4.0000,\"fb_id\":0,\"is_project_log\":true,\"ticket_id\":0,\"task_type_id\":4262,\"task_type\":\"Network Support\",\"project_id\":0,\"account_id\":7912,\"ticket_number\":0,\"account_name\":\"Llama Feed Store\",\"ticket_subject\":\"\",\"invoice_id\":0,\"billable\":true,\"invoice_pseudo_id\":\"      \"},
        //{\"time_id\":8092,\"project_name\":\"\",\"user_name\":\"Alexey Gavrilov\",\"user_email\":\"alexey.gavrilov@gmail.com\",\"note\":\"test\",\"date\":\"20 Mar 2014\",\"hours\":2.0000,\"fb_id\":0,\"is_project_log\":true,\"ticket_id\":0,\"task_type_id\":4260,\"task_type\":\"Consulting\",\"project_id\":0,\"account_id\":-1,\"ticket_number\":0,\"account_name\":\"Yoshkar\",\"ticket_subject\":\"\",\"invoice_id\":0,\"billable\":true,\"invoice_pseudo_id\":\"      \"}

        //"[{\"time_id\":5258,\"project_name\":\"Windows 8.1\",\"user_name\":\"Alexey Gavrilov\",\"user_email\":\"alexey.gavrilov@micajah.com\",\"note\":\"test\",\"date\":\"2014-10-26T00:00:00.0000000\",\"stop_time\":null,\"start_time\":null,\"hours\":1.0000,\"fb_id\":0,\"is_project_log\":true,\"ticket_id\":0,\"task_type_id\":1039,\"task_type\":\"Vacation\",\"project_id\":20,\"account_id\":-1,\"ticket_number\":0,\"account_name\":\"bigWebApps Windows Store\",\"ticket_subject\":\"\",\"invoice_id\":0,\"billable\":true,\"invoice_pseudo_id\":\"      \",\"qb_id\":0}]"

    }
}
