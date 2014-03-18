using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public abstract class SearchRequest : ObjectBase
    {
        [DataMember(Name = "query"), Details]
        public string Query { get; set; }

        [DataMember(Name = "limit"), Details]
        public int PageCount { get; set; }

        [DataMember(Name = "page"), Details]
        public int PageIndex { get; set; }

        [DataMember(Name = "sort_order"), Details]
        public string SortDirection { get; set; }

        [DataMember(Name = "sort_by"), Details]
        public string SortBy { get; set; }

        [Details]
        public DateTime StartDate { get; set; }

        [Details]
        public DateTime EndDate { get; set; }

        [DataMember(Name = "start_date", EmitDefaultValue = false)]
        private string _startDate;

        [DataMember(Name = "end_date", EmitDefaultValue = false)]
        private string _endDate;

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            this._startDate = this.StartDate.ToString();
            this._endDate = this.EndDate.ToString();
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            this._endDate = this._startDate = DateTime.MinValue.ToString();
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            DateTime date;
            if (DateTime.TryParse(this._startDate, out date))
                this.StartDate = date;
            if (DateTime.TryParse(this._endDate, out date))
                this.EndDate = date;
        }

    }
}
