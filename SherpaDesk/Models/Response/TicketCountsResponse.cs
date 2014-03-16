using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class TicketCountsResponse : ObjectBase
    {
        [DataMember(Name = "new_messages"), Details]
        public int NewMessages { get; set; }

        [DataMember(Name = "open_all"), Details]
        public int AllOpen { get; set; }

        [DataMember(Name = "open_as_user"), Details]
        public int OpenAsUser { get; set; }

        [DataMember(Name = "open_as_tech"), Details]
        public int OpenAsTech { get; set; }

        [DataMember(Name = "open_as_alttech"), Details]
        public int OpenAsAltTech { get; set; }

        [DataMember(Name = "onhold"), Details]
        public int OnHold { get; set; }

        [DataMember(Name = "reminder"), Details]
        public int Reminder { get; set; }

        [DataMember(Name = "parts_on_order"), Details]
        public int PartsOnOrder { get; set; }

        [DataMember(Name = "unconfirmed"), Details]
        public int Uncomfirmed { get; set; }
    }
}
