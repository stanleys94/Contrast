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
    
    public partial class tb_r_record_payment_proposal
    {
        public int id_data { get; set; }
        public string vendor_code { get; set; }
        public string currency { get; set; }
        public Nullable<int> total_amount { get; set; }
        public string beneficiary_name { get; set; }
        public string account_number { get; set; }
        public string employee_name { get; set; }
        public string refference { get; set; }
        public string generate_by { get; set; }
        public Nullable<System.DateTime> generate_date { get; set; }
    }
}