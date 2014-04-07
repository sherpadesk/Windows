using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public abstract class SearchRequest : GetRequest
    {
        public const int DEFAULT_PAGE_COUNT = 20;
        public const int DEFAULT_PAGE_INDEX = 1;

        protected SearchRequest()
        {
            this.PageCount = DEFAULT_PAGE_COUNT;
            //this.PageIndex = DEFAULT_PAGE_INDEX;
        }

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
        protected string _startDate;

        [DataMember(Name = "end_date", EmitDefaultValue = false)]
        protected string _endDate;

        [OnSerializing]
        protected virtual void OnSerializing(StreamingContext context)
        {
            if (this.StartDate != DateTime.MinValue)
                this._startDate = this.StartDate.ToString("yyyy-MM-dd");
            if (this.EndDate != DateTime.MinValue)
                this._endDate = this.EndDate.ToString("yyyy-MM-dd");
        }

        [OnDeserializing]
        protected virtual void OnDeserializing(StreamingContext context)
        {
            this._endDate = this._startDate = DateTime.MinValue.ToString();
        }

        [OnDeserialized]
        protected virtual void OnDeserialized(StreamingContext context)
        {
            DateTime date;
            if (DateTime.TryParse(this._startDate, out date))
                this.StartDate = date.ToLocalTime();
            if (DateTime.TryParse(this._endDate, out date))
                this.EndDate = date.ToLocalTime();
        }

    }
}
