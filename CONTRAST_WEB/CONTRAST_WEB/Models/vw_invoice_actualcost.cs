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
    
    public partial class vw_invoice_actualcost
    {
        public int id_actualcost { get; set; }
        public string group_code { get; set; }
        public int id_request { get; set; }
        public int no_reg { get; set; }
        public string name { get; set; }
        public string jenis_transaksi { get; set; }
        public string wbs_no { get; set; }
        public string cost_center { get; set; }
        public string qty { get; set; }
        public int amount { get; set; }
        public string tax { get; set; }
        public string vendor_code { get; set; }
        public string vendor_name { get; set; }
        public string invoice_number { get; set; }
        public string tax_invoice_number { get; set; }
        public Nullable<int> amount_total { get; set; }
        public Nullable<int> id_destination_city { get; set; }
        public string destination_name { get; set; }
        public Nullable<System.DateTime> start_date { get; set; }
        public Nullable<System.DateTime> end_date { get; set; }
        public Nullable<System.DateTime> create_date { get; set; }
        public string employee_input { get; set; }
    }
}