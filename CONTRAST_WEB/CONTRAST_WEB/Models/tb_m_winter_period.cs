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
    
    public partial class tb_m_winter_period
    {
        public int id_winter_period { get; set; }
        public int month_id { get; set; }
        public string month_desc { get; set; }
        public bool winter_flag { get; set; }
        public Nullable<System.DateTime> start_date { get; set; }
        public Nullable<System.DateTime> end_date { get; set; }
        public Nullable<bool> active_flag { get; set; }
    }
}
