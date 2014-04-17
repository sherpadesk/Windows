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
            string status = string.Empty, role = string.Empty;
            if (this.Status.HasFlag(eTicketStatus.Open))
                status += eTicketStatus.Open.Details() + ",";
            if (this.Status.HasFlag(eTicketStatus.Closed))
                status += eTicketStatus.Closed.Details() + ",";
            if (this.Status.HasFlag(eTicketStatus.OnHold))
                status += eTicketStatus.OnHold.Details() + ",";
            if (this.Status.HasFlag(eTicketStatus.Waiting))
                status += eTicketStatus.Waiting.Details() + ",";
            if (this.Status.HasFlag(eTicketStatus.NewMessages))
                status += eTicketStatus.NewMessages.Details() + ",";
            status = status.Trim(',');

            if (this.Role == eRoles.All)
                role = eRoles.All.Details();
            else
            {
                role = string.Empty;
                if (this.Role.HasFlag(eRoles.EndUser))
                    role += eRoles.EndUser.Details() + ",";
                if (this.Role.HasFlag(eRoles.Technician))
                    role += eRoles.Technician.Details() + ",";
                if (this.Role.HasFlag(eRoles.AltTechnician))
                    role += eRoles.AltTechnician.Details() + ",";
                role = role.Trim(',');
            }

            if (!string.IsNullOrEmpty(role))
                _role = role;
            if (!string.IsNullOrEmpty(status))
                _status = status;
        }
    }
}
