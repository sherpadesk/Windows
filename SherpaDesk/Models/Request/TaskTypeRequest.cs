using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class TaskTypeRequest : ObjectBase
    {
        [DataMember(Name = "key"), Details]
        public string Key { get; set; }

        [DataMember(Name = "ticket"), Details]
        public string Ticket { get; set; }

        [DataMember(Name = "project"), Details]
        public int ProjectId { get; set; }
    }
}
