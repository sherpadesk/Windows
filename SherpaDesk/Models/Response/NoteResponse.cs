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

        //		[
        //  {\"id\":20971,\"ticket_key\":\"cpszh2\",\"user_id\":7512,\"user_email\":\"alexey.gavrilov@micajah.com\",\"user_firstname\":\"Alexey\",\"user_lastname\":\"Gavrilov\",\"record_date\":\"2014-07-21T17:47:00.0000000\",\"log_type\":\"Response\",\"note\":\"test 2\",\"ticket_time_id\":0,\"sent_to\":\"\",\"is_waiting\":null,\"sla_used\":0},
        //  {\"id\":20970,\"ticket_key\":\"cpszh2\",\"user_id\":7512,\"user_email\":\"alexey.gavrilov@micajah.com\",\"user_firstname\":\"Alexey\",\"user_lastname\":\"Gavrilov\",\"record_date\":\"2014-07-21T17:43:00.0000000\",\"log_type\":\"Response\",\"note\":\"test response\",\"ticket_time_id\":0,\"sent_to\":\"\",\"is_waiting\":null,\"sla_used\":0},
        //  {\"id\":20901,\"ticket_key\":\"cpszh2\",\"user_id\":7512,\"user_email\":\"alexey.gavrilov@micajah.com\",\"user_firstname\":\"Alexey\",\"user_lastname\":\"Gavrilov\",\"record_date\":\"2014-06-15T12:03:00.0000000\",\"log_type\":\"Initial Post\",\"note\":\"test 15<br><br>Following file was  uploaded: .\",\"ticket_time_id\":0,\"sent_to\":\"\",\"is_waiting\":null,\"sla_used\":0}]"	string

    }

}
