﻿using SherpaDesk.Common;
using System;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Response
{
    [DataContract]
    public abstract class TicketBaseResponse : ObjectBase
    {
        [DataMember(Name = "key"), Details]
        public string TicketKey { get; set; }

        [DataMember(Name = "created_time")]
        protected string _createdTime;

        [Details]
        public DateTime СreatedTime { get; set; }

        [DataMember(Name = "number"), Details]
        public int TicketId { get; set; }

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

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            DateTime date;
            if (DateTime.TryParse(this._createdTime, out date))
            {
                this.СreatedTime = date.ToLocalTime();
            }
        }
        //		{\"users\":[{\"user_id\":33829,\"user_fullname\":\"Organization Administrator\",\"is_primary\":true,\"start_date\":\"2014-03-25T20:35:46.1030000\",\"stop_date\":null,\"id\":229322}],\"technicians\":[{\"user_id\":33833,\"user_fullname\":\"Artem Korzhavin\",\"is_primary\":true,\"start_date\":\"2014-03-25T20:35:46.1230000\",\"stop_date\":null,\"id\":229323}],\"ticketlogs\":[{\"ticket_key\":null,\"user_id\":33829,\"user_email\":\"alexey.gavrilov@gmail.com\",\"user_firstname\":\"Organization\",\"user_lastname\":\"Administrator\",\"record_date\":\"2014-03-25T20:36:00.0000000\",\"log_type\":\"Initial Post\",\"note\":\"test\",\"ticket_time_id\":0,\"sent_to\":\"\",\"id\":266552}],\"assets\":[],\"attachments\":null,\"classes\":[{\"name\":\"General Question\",\"id\":11341,\"parent_id\":0,\"hierarchy_level\":0,\"sub\":null,\"is_lastchild\":true,\"is_restrict_to_techs\":false,\"is_active\":true}],\"key\":\"sujtm8\",\"created_time\":\"2014-03-25T20:36:00.0000000\",\"closed_time\":null,\"request_completion_date\":null,\"is_waiting_on_response\":false,\"waiting_date\":null,\"waiting_minutes\":0,\"followup_date\":null,\"sla_complete_date\":null,\"sla_response_date\":null,\"confirmed_date\":null,\"next_step_date\":null,\"updated_time\":\"2014-03-25T20:36:00.0000000\",\"organization_key\":\"ecfa4c95436242f3baeb2495f9a885c2\",\"department_key\":3952,\"is_deleted\":false,\"user_id\":33829,\"user_title\":\"\",\"user_firstname\":\"Organization\",\"user_lastname\":\"Administrator\",\"user_email\":\"alexey.gavrilov@gmail.com\",\"tech_id\":33833,\"tech_firstname\":\"Artem\",\"tech_lastname\":\"Korzhavin\",\"tech_email\":\"livehex@gmail.com\",\"priority\":0,\"priority_name\":\"\",\"priority_id\":0,\"user_created_id\":33829,\"user_created_firstname\":\"Organization\",\"user_created_lastname\":\"Administrator\",\"user_created_email\":\"alexey.gavrilov@gmail.com\",\"status\":\"Open\",\"location_id\":0,\"location_name\":\"\",\"class_id\":11341,\"class_name\":\"General Question\",\"project_id\":0,\"project_name\":\"\",\"serial_number\":\"\",\"folder_id\":0,\"folder_path\":\"\",\"creation_category_id\":0,\"creation_category_name\":\"\",\"subject\":\"second ticket\",\"note\":\"\",\"number\":4,\"prefix\":\"\",\"customfields_xml\":\"\",\"parts_cost\":0.0000,\"labor_cost\":0.0000,\"total_time_in_minutes\":0,\"misc_cost\":0.0000,\"travel_cost\":0.0000,\"request_completion_note\":\"\",\"followup_note\":\"\",\"initial_response\":false,\"sla_complete_used\":0,\"sla_response_used\":0,\"level\":0,\"level_name\":\"\",\"is_via_email_parser\":false,\"account_id\":-1,\"account_name\":\"Yoshkar\",\"account_location_id\":0,\"account_location_name\":\"\",\"resolution_category_id\":0,\"resolution_category_name\":\"\",\"is_resolved\":false,\"confirmed_by_name\":\"\",\"is_confirmed\":false,\"confirmed_note\":\"\",\"support_group_id\":0,\"support_group_name\":\"\",\"is_handle_by_callcentre\":false,\"submission_category\":\"\",\"is_user_inactive\":false,\"next_step\":\"\",\"total_hours\":0,\"remaining_hours\":-79228162514264337593543950335,\"estimated_time\":0.0000,\"workpad\":\"\",\"scheduled_ticket_id\":0,\"kb\":false,\"kb_type\":0,\"kb_publish_level\":0,\"kb_search_desc\":\"\",\"kb_alternate_id\":\"\",\"kb_helpful_count\":0,\"kb_portal_alias\":\"General Question\",\"initial_post\":\"test\",\"is_sent_notification_email\":true,\"email_cc\":\"\",\"related_tickets_count\":0,\"daysold_in_minutes\":0,\"tech_type\":\"Administrator\",\"BillRate\":0}"	string

    }
}
