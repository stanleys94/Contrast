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
    
    public partial class tb_r_travel_request_comment
    {
        public long id_comment { get; set; }
        public string group_code { get; set; }
        public Nullable<int> no_reg_comment { get; set; }
        public string name { get; set; }
        public string comment { get; set; }
        public Nullable<bool> read_flag { get; set; }
        public Nullable<int> no_reg_comment_to { get; set; }
        public string name_to { get; set; }
        public Nullable<System.DateTime> create_date { get; set; }
    }
}
