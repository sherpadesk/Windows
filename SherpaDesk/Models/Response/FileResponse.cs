using System;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class FileResponse : ObjectBase
    {
        [DataMember(Name = "id"), Details]
        public string Id { get; set; }

        [DataMember(Name = "name"), Details]
        public string Name { get; set; }

        [DataMember(Name = "url"), Details]
        public string Url { get; set; }

        [DataMember(Name = "date")]
        protected string _date { get; set; }

        [Details]
        public DateTime Date { get; set; }

        [DataMember(Name = "size"), Details]
        public long Size { get; set; }

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            DateTime date;
            if (DateTime.TryParse(this._date, out date))
            {
                this.Date = date.ToLocalTime();
            }
        }

    }
}
