using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SherpaDesk.Common;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class TicketSearchRequest : SearchRequest
    {
        [DataMember(Name = "user"), Details]
        public int UserId { get; set; }

        [DataMember(Name = "location"), Details]
        public string Location { get; set; }

        [DataMember(Name = "account"), Details]
        public string Account { get; set; }

        [DataMember(Name = "Class"), Details]
        public string Class { get; set; }

        [DataMember(Name = "role")]
        public string _role { get; set; }

        [DataMember(Name = "status")]
        protected string _status;

        [Details]
        public eRoles Role { get; set; }

        [Details]
        public eTicketStatus Status { get; set; }

        [OnSerializing]
        internal void OnSerializing(StreamingContext context)
        {
            _status = string.Empty;
            if (this.Status.HasFlag(eTicketStatus.Open))
                _status += eTicketStatus.Open.Description() + ",";
            if (this.Status.HasFlag(eTicketStatus.Closed))
                _status += eTicketStatus.Closed.Description() + ",";
            if (this.Status.HasFlag(eTicketStatus.OnHold))
                _status += eTicketStatus.OnHold.Description() + ",";
            if (this.Status.HasFlag(eTicketStatus.Waiting))
                _status += eTicketStatus.Waiting.Description() + ",";
            _status = _status.Trim(',');

            _role = string.Empty;
            if (this.Role.HasFlag(eRoles.EndUser))
                _role += eRoles.EndUser.Description() + ",";
            if (this.Role.HasFlag(eRoles.Technician))
                _role += eRoles.Technician.Description() + ",";
            if (this.Role.HasFlag(eRoles.AltTechnician))
                _role += eRoles.AltTechnician.Description() + ",";

            _role = _role.Trim(',');
        }
    }
}
