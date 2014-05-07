using System;
using System.Runtime.Serialization;
using SherpaDesk.Common;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class ActivityResponse : NameResponse
    {
        [DataMember(Name = "ticket_id"), Details]
        public int TicketId { get; set; }

        [DataMember(Name = "user_id"), Details]
        public int UserId { get; set; }

        [DataMember(Name = "friendly_id"), Details]
        public string FriendlyId { get; set; }

        [DataMember(Name = "user_name"), Details]
        public string UserName { get; set; }

        [DataMember(Name = "user_email"), Details]
        public string UserEmail { get; set; }

        [DataMember(Name = "object"), Details]
        public string Object { get; set; }

        [DataMember(Name = "title"), Details]
        public string Title { get; set; }


        [DataMember(Name = "note"), Details]
        public string Note { get; set; }

        [Details, IgnoreDataMember]
        public DateTime Date { get; set; }

        [DataMember(Name = "date")]
        protected string _date;

        [Details, IgnoreDataMember]
        public string DaysOld { get; set; }

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            DateTime date;
            if (DateTime.TryParse(this._date, out date))
            {
                this.Date = date.ToLocalTime();
                this.DaysOld = (DateTime.Now - this.Date).CalculateDate();
            }
        }

        public override string ToString()
        {
            return string.Concat(this.UserName, Environment.NewLine, this.Title, Environment.NewLine, Helper.HtmlToString(this.Note), Environment.NewLine, this.DaysOld);
        }
    }

    //		{\"id\":99072,\"ticket_id\":99072,\"user_id\":33833,\"friendly_id\":\"3\",\"user_name\":\"Artem Korzhavin\",\"user_email\":\"livehex@gmail.com\",\"date\":\"2014-04-22T18:40:00.0000000\",\"object\":\"ticket\",\"title\":\"Response\",\"note\":\"qwdqwdqw test\"},
    //      {\"id\":112349,\"ticket_id\":112349,\"user_id\":33829,\"friendly_id\":\"24\",\"user_name\":\"Alexey Gavrilov\",\"user_email\":\"alexey.gavrilov@gmail.com\",\"date\":\"2014-04-22T09:31:00.0000000\",\"object\":\"ticket\",\"title\":\"File was uploaded \",\"note\":\"Following file was  uploaded: .\"},


}
