using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using CONTRAST_WEB.CustomValidator;

namespace CONTRAST_WEB.Models
{
    public class TravelRequestHelper
    {
        public tb_r_travel_request travel_request { get; set; }
        public tb_m_employee employee_info { get; set; }
        public tb_m_vendor_employee v_employee_info { get; set; }
        public tb_r_travel_dit travel_dit { get; set; }

        public List<tb_r_travel_request_participant> participants { get; set; }


        public DateTime?[] tstart_date { get; set; }
        
        public DateTime?[] tend_date { get; set; }

        //[DateValidator]
        //[DataType(DataType.Date)]
        //public DateTime? tstart_date0 { get; set; }

        [DateValidator]
        [DataType(DataType.Date)]
        public DateTime? tstart_date0
        {
            get
            {
                if (tstart_date0D == null || tstart_date0T == null) return null;

                return tstart_date0D.Value.Date + tstart_date0T.Value.TimeOfDay;
            }
             
        }

        //[DateValidator2]
        //[DataType(DataType.Date)]
        //public DateTime? tstart_date1 { get; set; }

        [DateValidator2]
        [DataType(DataType.Date)]
        public DateTime? tstart_date1
        {
            get
            {
                if (tstart_date1D == null || tstart_date1T == null) return null;

                return tstart_date1D.Value.Date + tstart_date1T.Value.TimeOfDay;
            }
        }

        //[DateValidator3]
        //[DataType(DataType.Date)]
        //public DateTime? tstart_date2 { get; set; }

        [DateValidator3]
        [DataType(DataType.Date)]
        public DateTime? tstart_date2
        {
            get
            {
                if (tstart_date2D == null || tstart_date2T == null) return null;

                return tstart_date2D.Value.Date + tstart_date2T.Value.TimeOfDay;
            }
        }


        [DateValidator]
        public DateTime? tend_date0
        {
            get
            {
                if (tend_date0D == null || tend_date0T == null) return null;

                return tend_date0D.Value.Date + tend_date0T.Value.TimeOfDay;
            }
        }

        [DateValidator2]
        public DateTime? tend_date1
        {
            get
            {
                if (tend_date1D == null || tend_date1T == null) return null;

                return tend_date1D.Value.Date + tend_date1T.Value.TimeOfDay;
            }
        }

        [DateValidator3]
        public DateTime? tend_date2
        {
            get
            {
                if (tend_date2D == null || tend_date2T == null) return null;

                return tend_date2D.Value.Date + tend_date2T.Value.TimeOfDay;
            }
        }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true), Display(Name = "End Date")]
        public DateTime? tend_date0D { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true), Display(Name = "End Time")]
        public DateTime? tend_date0T { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true), Display(Name = "End Date")]
        public DateTime? tend_date1D { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true), Display(Name = "End Time")]
        public DateTime? tend_date1T { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true), Display(Name = "End Date")]
        public DateTime? tend_date2D { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true), Display(Name = "End Time")]
        public DateTime? tend_date2T { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true), Display(Name = "start Date")]
        public DateTime? tstart_date0D { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true), Display(Name = "start Time")]
        public DateTime? tstart_date0T { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true), Display(Name = "start Date")]
        public DateTime? tstart_date1D { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true), Display(Name = "start Time")]
        public DateTime? tstart_date1T { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true), Display(Name = "start Date")]
        public DateTime? tstart_date2D { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true), Display(Name = "start Time")]
        public DateTime? tstart_date2T { get; set; }

        //[DateValidator2]
        //public DateTime? tend_date1 { get; set; }
        //[DateValidator3]
        //public DateTime? tend_date2 { get; set; }

        public int?[] tid_destination_city { get; set; }
        public bool?[] tair_ticket_flag { get; set; }
        public bool?[] toverseas_flag { get; set; }

        [Required(ErrorMessage = "No bank Account registered,contact finance division")]
        public string tbankaccount { get; set; }

        [Required(ErrorMessage = "No bank name registered,contact finance division")]
        public string tbankname { get; set; }

        [Required(ErrorMessage = "Invalid data : This field cannot be empty")]
        public string tpurpose { get; set; }

        [Required(ErrorMessage = "Invalid data : This field cannot be empty")]
        [StringLength(199, MinimumLength = 1, ErrorMessage = "Minimum Character Allowed is 1, Maximum is 200")]
        public string treason { get; set; }

        [Required(ErrorMessage = "Invalid data : This field cannot be empty")]
        public int tactivity { get; set; }

        [Required(ErrorMessage = "WBS data for this account is empty, contact finance division")]
        public int twbsnumber { get; set; }

        [ParticipantValidator]
        public string tparticipant { get; set; }

        [Required(ErrorMessage = "Travel document must be attached !")]
        public HttpPostedFileBase generaldoc_file { get; set; }
        public HttpPostedFileBase itinerary_file { get; set; }
        public HttpPostedFileBase invitation_file { get; set; }

        public HttpPostedFileBase proposaldoc_file { get; set; }

        public bool special_employee_flag { get; set; }

        public TravelRequestHelper() { }

        public string destination_string { get; set; }
        public string bossname_string { get; set; }
        public string bank_name_string { get; set; }
        public string bank_account_string { get; set; }


        public int total_days_validate { get; set; }
        public int total_meal_validate { get; set; }
        public float total_hotel_validate { get; set; }
        public float total_allowance_validate { get; set; }
        public float total_ticket_validate { get; set; }
        
    }


}