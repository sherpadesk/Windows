using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class PlaceOnHoldRequest : PutRequest
    {
        public PlaceOnHoldRequest(string key)
            : base(key)
        {
            this.Status = "onhold";
        }

        [DataMember(Name = "status"), Details]
        public string Status { get; private set; }

        [DataMember(Name = "note_text"), Details]
        public string Note { get; set; }

    }
}
