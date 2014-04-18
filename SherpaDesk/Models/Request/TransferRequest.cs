using SherpaDesk.Common;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class TransferRequest : ActionTicketRequest, IValidatableObject
    {
        public TransferRequest(string key) : base(key, "transfer") { }

        [DataMember(Name = "keep_attached"), Details]
        public bool KeepAttached { get; set; }

        [DataMember(Name = "tech_id"), Details]
        public int TechnicianId { get; set; }

        [DataMember(Name = "class_id"), Details]
        public int ClassId { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.TechnicianId == 0 && this.ClassId == 0)
            {
                return (new ValidationResult[2] { new ValidationResult(AddTicketRequest.ERROR_EMPTY_TECHNICIAN_ID), new ValidationResult(AddTicketRequest.ERROR_EMPTY_CLASS_ID) }).AsEnumerable();
            }
            else return (new ValidationResult[0]).AsEnumerable();
        }
    }
}
