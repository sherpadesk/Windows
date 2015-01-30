using SherpaDesk.Interfaces;
using System.Globalization;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class UpdateTimeRequest : AddTimeRequest, IPath
    {
        public UpdateTimeRequest(string ticketKey, int timeId)
        {
            this.Path = "/" + timeId.ToString(CultureInfo.InvariantCulture);
            this.TicketKey = ticketKey;
            this.TicketTimeId = timeId;
            this.IsProjectTime = false;
        }

        public UpdateTimeRequest(int projectId, int timeId)
        {
            this.Path = "/" + timeId.ToString();
            this.ProjectId = projectId;
            this.ProjectTimeId = timeId;
            this.IsProjectTime = true;
        }

        public override eRequestType Type
        {
            get
            {
                return eRequestType.PUT;
            }
        }

        [DataMember(Name = "project_time_id"), Details]
        public int ProjectTimeId { get; set; }

        [DataMember(Name = "ticket_time_id"), Details]
        public int TicketTimeId { get; set; }

        public string Path { get; set; }
    }
}
