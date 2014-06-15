using System;
using SherpaDesk.Common;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class NoteResponse : ObjectBase
    {
        [DataMember(Name = "id"), Details]
        public int PostId { get; set; }

        [DataMember(Name = "ticket_key"), Details]
        public string TicketKey { get; set; }

        [DataMember(Name = "user_id"), Details]
        public int UserId { get; set; }

        [DataMember(Name = "user_email"), Details]
        public string Email { get; set; }

        [DataMember(Name = "user_firstname"), Details]
        public string FirstName { get; set; }

        [DataMember(Name = "user_lastname"), Details]
        public string LastName { get; set; }

        [Details]
        public string FullName
        {
            get
            {
                return Helper.FullName(this.FirstName, this.LastName, this.Email);
            }
        }

        [DataMember(Name = "record_date")]
        protected string _responseDate;

        [Details]
        public string ResponseDateText
        {
            get
            {
                var days = (DateTime.Now - this.ResponseDate).DaysAgo();
                return !string.IsNullOrEmpty(days) ? string.Format("{0} old", days) : string.Empty;
            }
        }

        [Details]
        public DateTime ResponseDate { get; set; }

        [DataMember(Name = "log_type"), Details]
        public string NoteType { get; set; }

        [DataMember(Name = "note"), Details]
        public string NoteText { get; set; }

        [DataMember(Name = "ticket_time_id"), Details]
        public int TicketTimeId { get; set; }

        [DataMember(Name = "sent_to"), Details]
        public string SentTo { get; set; }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            DateTime date;
            if (DateTime.TryParse(this._responseDate, out date))
                this.ResponseDate = date.ToLocalTime();
        }
    }

}
