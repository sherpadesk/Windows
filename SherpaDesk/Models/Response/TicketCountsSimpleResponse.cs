using System.Runtime.Serialization;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class TicketCountsSimpleResponse : ObjectBase
    {
        [DataMember(Name = "open"), Details]
        public int Open { get; set; }

        [DataMember(Name = "closed"), Details]
        public int Closed { get; set; }

        [DataMember(Name = "scheduled"), Details]
        public int Scheduled { get; set; }

        [DataMember(Name = "followups"), Details]
        public int FollowUps { get; set; }
    }
}
