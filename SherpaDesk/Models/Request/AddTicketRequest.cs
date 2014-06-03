using SherpaDesk.Common;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    [DataContract]
    public class AddTicketRequest : PostRequest
    {
        internal const string ERROR_EMPTY_TECHNICIAN_ID = "Please select a Technician.#TechnicianList";
        internal const string ERROR_EMPTY_USER_ID = "Please select a End User.#EndUserList";
        internal const string ERROR_EMPTY_ACCOUNT_ID = "Please select a Account.#AccountList";
        internal const string ERROR_EMPTY_CLASS_ID = "Please select a Class.#ClassList";
        internal const string ERROR_EMPTY_SUBJECT = "Please enter a Subject.#SubjectTextbox";

        [DataMember(Name = "status"), Details]
        public string Status { get; set; }

        [DataMember(Name = "subject"), Details]
        [Required(ErrorMessage = ERROR_EMPTY_SUBJECT)]
        public string Name { get; set; }

        [DataMember(Name = "initial_post"), Details]
        public string Comment { get; set; }

        [DataMember(Name = "class_id"), Details]
        [IntRequired(ErrorMessage = ERROR_EMPTY_CLASS_ID)]
        public int ClassId { get; set; }

        [DataMember(Name = "account_id"), Details]
        [IntRequired(ErrorMessage = ERROR_EMPTY_ACCOUNT_ID)]
        public int AccountId { get; set; }

        [DataMember(Name = "user_id"), Details]
        [IntRequired(ErrorMessage = ERROR_EMPTY_USER_ID)]
        public int UserId { get; set; }

        [DataMember(Name = "tech_id"), Details]
        [IntRequired(ErrorMessage = ERROR_EMPTY_TECHNICIAN_ID)]
        public int TechnicianId { get; set; }

        //[DataMember(Name = "technicians"), Details]
        //public AssigmentUser[] Technicians { get; set; }
    }

    //[DataContract]
    //public class AssigmentUser : ObjectBase
    //{
    //    public AssigmentUser(int userId)
    //    {
    //        this.UserId = userId.ToString();
    //    }

    //    public AssigmentUser(int userId, bool alt_tech)
    //    {
    //        this.UserId = userId.ToString();
    //        if (alt_tech)
    //            this.Primary = bool.TrueString;
    //    }

    //    [DataMember(Name = "user_id"), Details]
    //    public string UserId { get; set; }

    //    [DataMember(Name = "user_fullname"), Details]
    //    public string UserName { get; set;}

    //    [DataMember(Name = "is_primary"), Details]
    //    public string Primary { get; set; }

    //    [DataMember(Name = "start_date"), Details]
    //    protected string _startDate;

    //    [DataMember(Name = "stop_date"), Details]
    //    protected string _stopDate;
    //}
}
