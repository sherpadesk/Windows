using SherpaDesk.Common;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class TransferRequest : ActionTicketRequest
    {
        public TransferRequest(string key) : base(key, "transfer") { }

        [DataMember(Name = "keep_attached"), Details]
        public bool KeepAttached { get; set; }

        [DataMember(Name = "tech_id"), Details]
        [IntRequired(ErrorMessage = AddTicketRequest.ERROR_EMPTY_TECHNICIAN_ID)]
        public int TechnicianId { get; set; }

        [DataMember(Name = "class_id"), Details]
        [IntRequired(ErrorMessage = AddTicketRequest.ERROR_EMPTY_CLASS_ID)]
        public int ClassId { get; set; }

    }
}
