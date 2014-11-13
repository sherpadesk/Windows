using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public class ConfigResponse : ObjectBase
    {
        //is_onhold_status =false is_time_tracking=true is_freshbooks=false freshbooks_url=https://micajah3.freshbooks.com` is_parts_tracking=false is_project_tracking=true is_unassigned_queue=true is_location_tracking=true is_waiting_on_response=true is_invoice=true is_payments=true is_expenses=true is_class_tracking=true is_travel_costs=true is_priorities_general=true is_confirmation_tracking=true is_ticket_levels=true is_account_manager=true is_require_ticket_initial_post=false is_ticket_require_closure_note=true is_asset_tracking=true timezone_offset=-5 timezone_name=Eastern Standard Time currency=$ businessday_length=540
        [DataMember(Name = "is_onhold_status"), Details]
        public bool OnHoldStatus { get; set; }

        [DataMember(Name = "is_time_tracking"), Details]
        public bool TimeTracking { get; set; }

        [DataMember(Name = "timezone_offset"), Details]
        public int TimezoneOffset { get; set; }

        [DataMember(Name = "timezone_name"), Details]
        public string TimezoneName { get; set; }

        [DataMember(Name = "is_project_tracking"), Details]
        public bool ProjectTracking { get; set; }
        
        [DataMember(Name = "is_unassigned_queue"), Details]
        public bool UnassignedQueue { get; set; }
        
        [DataMember(Name = "is_location_tracking"), Details]
        public bool LocationTracking { get; set; }
        
        [DataMember(Name = "is_waiting_on_response"), Details]
        public bool WaitingOnResponse { get; set; }

        [DataMember(Name = "is_class_tracking"), Details]
        public bool ClassTracking { get; set; }
        
        [DataMember(Name = "is_confirmation_tracking"), Details]
        public bool ConfirmationTracking { get; set; }
        
        [DataMember(Name = "is_ticket_levels"), Details]
        public bool TicketLevels { get; set; }
        
        [DataMember(Name = "is_account_manager"), Details]
        public bool AccountManager { get; set; }
        
        [DataMember(Name = "is_require_ticket_initial_post"), Details]
        public bool RequireTicketInitialPost { get; set; }
        
        [DataMember(Name = "is_ticket_require_closure_note"), Details]
        public bool RequireTicketClosureNote { get; set; }
        
        //assets { unique 1_caption, unique 2_caption, unique 3_caption, unique 4_caption, unique 5_caption, unique 6_caption, unique 7_caption }

        [DataMember(Name = "user"), Details]
        public ConfigUserResponse User { get; set; }
        //user { login_id = 4f843b46db68g6879ja9a9841bd2c316 user_id=3 email=jon@me.com firstname=Jon lastname=Key is_techoradmin=true is_useworkdaystimer=true }
    }

    [DataContract]
    public class ConfigUserResponse : ObjectBase
    {
        [DataMember(Name = "login_id"), Details]
        public string LoginId { get; set; }

        [DataMember(Name = "user_id"), Details]
        public int Id { get; set; }

        [DataMember(Name = "email"), Details]
        public string Email { get; set; }

        [DataMember(Name = "firstname"), Details]
        public string FirstName { get; set; }

        [DataMember(Name = "lastname"), Details]
        public string LastName { get; set; }

        [DataMember(Name = "is_techoradmin"), Details]
        public bool TechOrAdmin { get; set; }

        [DataMember(Name = "is_useworkdaystimer"), Details]
        public bool UseWorkdaysTimer { get; set; }

    }
}
