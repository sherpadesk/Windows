using SherpaDesk.Common;
using System;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class TicketResponse : ObjectBase
    {
        [DataMember(Name = "key"), Details]
        public string TicketKey { get; set; }

        [DataMember(Name = "created_time")]
        private string _createdTime;

        [Details]
        public DateTime СreatedTime { get; set; }

        [DataMember(Name = "number"), Details]
        public int Number { get; set; }

        [DataMember(Name = "is_new_user_post"), Details]
        public bool NewUserPost { get; set; }

        [DataMember(Name = "is_new_tech_post"), Details]
        public bool NewTechPost { get; set; }

        [DataMember(Name = "prefix"), Details]
        public string Prefix { get; set; }

        [DataMember(Name = "subject"), Details]
        public string Subject { get; set; }

        [DataMember(Name = "user_firstname"), Details]
        public string UserFirstName { get; set; }

        [DataMember(Name = "user_lastname"), Details]
        public string UserLastName { get; set; }

        public string UserFullName { get { return Helper.FullName(this.UserFirstName, this.UserLastName); } }

        [DataMember(Name = "user_email"), Details]
        public string UserEmail { get; set; }

        [DataMember(Name = "technician_firstname"), Details]
        public string TechnicianFirstName { get; set; }

        [DataMember(Name = "technician_lastname"), Details]
        public string TechnicianLastName { get; set; }

        public string TechnicianFullName { get { return Helper.FullName(this.TechnicianFirstName, this.TechnicianLastName); } }

        [DataMember(Name = "technician_email"), Details]
        public string TechnicianEmail { get; set; }

        [DataMember(Name = "account_id"), Details]
        public int AccountId { get; set; }

        [DataMember(Name = "account_name"), Details]
        public string AccountName { get; set; }

        [DataMember(Name = "location_id"), Details]
        public int? LocationId { get; set; }

        [DataMember(Name = "location_name"), Details]
        public string LocationName { get; set; }

        [DataMember(Name = "account_location_id"), Details]
        public int? AccountLocationId { get; set; }

        [DataMember(Name = "account_location_name"), Details]
        public string AccountLocationName { get; set; }

        [DataMember(Name = "priority_name"), Details]
        public string PriorityName { get; set; }

        [DataMember(Name = "level_name"), Details]
        public string LevelName { get; set; }

        [DataMember(Name = "status"), Details]
        public string Status { get; set; }

        [DataMember(Name = "creation_category_id"), Details]
        public int? CreationCategoryId { get; set; }

        [DataMember(Name = "creation_category_name"), Details]
        public string CreationCategoryName { get; set; }

        [DataMember(Name = "class_id"), Details]
        public int ClassId { get; set; }

        [DataMember(Name = "class_name"), Details]
        public string ClassName { get; set; }

        [DataMember(Name = "id"), Details]
        public int TicketId { get; set; }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            DateTime date;
            if (DateTime.TryParse(this._createdTime, out date))
                this.СreatedTime = date;
        }

    }
}
