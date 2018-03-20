using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.Models
{
    public class TravelStatusHelper
    {
        public vw_request_summary travel_request { get; set; }
        public tb_m_employee employee_info { get; set; }
        public List<tb_r_travel_request_participant> participants { get; set; }

        public DateTime?[] tstart_date { get; set; }
        public DateTime?[] tend_date { get; set; }
        public int?[] tid_destination_city { get; set; }
        public bool?[] toverseas_flag { get; set; }

    }
}