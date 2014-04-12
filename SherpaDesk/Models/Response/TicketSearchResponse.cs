using SherpaDesk.Common;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class TicketSearchResponse : TicketBaseResponse, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember(Name = "id"), Details]
        public int TicketId { get; set; }

        [DataMember(Name = "is_new_user_post"), Details]
        public bool NewUserPost { get; set; }

        [DataMember(Name = "is_new_tech_post"), Details]
        public bool NewTechPost { get; set; }

        [DataMember(Name = "technician_firstname"), Details]
        public string TechnicianFirstName { get; set; }

        [DataMember(Name = "technician_lastname"), Details]
        public string TechnicianLastName { get; set; }

        [IgnoreDataMember]
        public string TechnicianFullName { get { return Helper.FullName(this.TechnicianFirstName, this.TechnicianLastName, this.TechnicianEmail); } }

        [DataMember(Name = "technician_email"), Details]
        public string TechnicianEmail { get; set; }

        [Details, IgnoreDataMember]
        public string DaysOld { get; set; }

        [IgnoreDataMember]
        private bool _checked = false;

        [IgnoreDataMember]
        public bool IsChecked
        {
            get { return _checked; }
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this,
                              new PropertyChangedEventArgs("IsChecked"));
                    }
                }
            }
        }

        [OnDeserialized]
        protected new void OnDeserialized(StreamingContext context)
        {
            if (this.СreatedTime != DateTime.MinValue)
                this.DaysOld = (DateTime.Now - this.СreatedTime).CalculateTime();
        }

        //		[{\"key\":\"sujtm8\",\"created_time\":\"2014-03-25T20:36:00.0000000\",\"number\":4,\"is_new_user_post\":true,\"is_new_tech_post\":false,\"prefix\":\"\",\"subject\":\"second ticket\",\"user_firstname\":\"Organization\",\"user_lastname\":\"Administrator\",\"user_email\":\"alexey.gavrilov@gmail.com\",\"technician_firstname\":\"Artem\",\"technician_lastname\":\"Korzhavin\",\"technician_email\":\"livehex@gmail.com\",\"account_id\":-1,\"account_name\":\"Yoshkar\",\"location_id\":null,\"location_name\":\"\",\"account_location_id\":null,\"account_location_name\":\"\",\"priority_name\":\"\",\"level_name\":\"\",\"status\":\"Open\",\"creation_category_id\":null,\"creation_category_name\":\"\",\"class_id\":11341,\"class_name\":\"General Question\",\"id\":99076},
        //{\"key\":\"e5g1mr\",\"created_time\":\"2014-03-25T20:29:00.0000000\",\"number\":3,\"is_new_user_post\":true,\"is_new_tech_post\":false,\"prefix\":\"\",\"subject\":\"first ticket from windows 8.1\",\"user_firstname\":\"Organization\",\"user_lastname\":\"Administrator\",\"user_email\":\"alexey.gavrilov@gmail.com\",\"technician_firstname\":\"Artem\",\"technician_lastname\":\"Korzhavin\",\"technician_email\":\"livehex@gmail.com\",\"account_id\":-1,\"account_name\":\"Yoshkar\",\"location_id\":null,\"location_name\":\"\",\"account_location_id\":null,\"account_location_name\":\"\",\"priority_name\":\"\",\"level_name\":\"\",\"status\":\"Open\",\"creation_category_id\":null,\"creation_category_name\":\"\",\"class_id\":11342,\"class_name\":\"Technology Support\",\"id\":99072}]"	string

    }
}
