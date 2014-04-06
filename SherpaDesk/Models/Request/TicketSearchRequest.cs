using SherpaDesk.Common;
using System.Runtime.Serialization;

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
        protected string _role { get; set; }

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
                _status += eTicketStatus.Open.Details() + ",";
            if (this.Status.HasFlag(eTicketStatus.Closed))
                _status += eTicketStatus.Closed.Details() + ",";
            if (this.Status.HasFlag(eTicketStatus.OnHold))
                _status += eTicketStatus.OnHold.Details() + ",";
            if (this.Status.HasFlag(eTicketStatus.Waiting))
                _status += eTicketStatus.Waiting.Details() + ",";
            if (this.Status.HasFlag(eTicketStatus.NewMessages))
                _status += eTicketStatus.NewMessages.Details() + ",";
            _status = _status.Trim(',');

            if (this.Role == eRoles.All)
                _role = eRoles.All.Details();
            else
            {
                _role = string.Empty;
                if (this.Role.HasFlag(eRoles.EndUser))
                    _role += eRoles.EndUser.Details() + ",";
                if (this.Role.HasFlag(eRoles.Technician))
                    _role += eRoles.Technician.Details() + ",";
                if (this.Role.HasFlag(eRoles.AltTechnician))
                    _role += eRoles.AltTechnician.Details() + ",";
                _role = _role.Trim(',');
            }
        }
    }
}
