using SherpaDesk.Common;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class AttachAltTechRequest : PostRequest, IPath
    {
        public AttachAltTechRequest(string key, int techId)
        {
            this.Path = "/" + key;
            this.Action = "add_tech";
            this.TechnicianId = techId;
        }

        [Details]
        public string Path { get; set; }

        [DataMember(Name = "action"), Details]
        public string Action { get; private set; }

        [DataMember(Name = "tech_id"), Details]
        public int TechnicianId { get; set; }
    }
}
