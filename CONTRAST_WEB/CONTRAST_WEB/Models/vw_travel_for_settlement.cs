//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CONTRAST_WEB.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class vw_travel_for_settlement
    {
        public string group_code { get; set; }
        public int id_request { get; set; }
        public Nullable<int> no_reg { get; set; }
        public string emp_name { get; set; }
        public Nullable<int> id_destination_city { get; set; }
        public string destination_name { get; set; }
        public int total_ticket { get; set; }
        public int total_hotel { get; set; }
        public int total_meal { get; set; }
        public int total_winter { get; set; }
        public int total_preparation { get; set; }
        public int total_laundry { get; set; }
        public int total_miscellaneous { get; set; }
        public int total_transportation { get; set; }
        public Nullable<int> grand_total_settlement { get; set; }
        public Nullable<System.DateTime> start_date { get; set; }
        public Nullable<System.DateTime> end_date { get; set; }

        public string login_id { get; set; }
        public string final_status { get; set; }
        public string comment { get; set; }
        public string process_reject { get; set; }
    }
}