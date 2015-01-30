using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class DeleteTimeRequest : DeleteRequest
    {
        public DeleteTimeRequest(int timeId, bool isProject)
            : base(timeId.ToString())
        {
            IsProjectLog = isProject;
        }

        [DataMember(Name = "is_project_log"), Details]
        public bool IsProjectLog { get; set; }
    }
}
