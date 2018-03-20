using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.Models
{
    public class ActualCostHelper
    {
        public tb_r_travel_request TravelRequest { get; set; }
        public tb_r_travel_actualcost ActualCost { get; set; }
        public int amount2 { get; set; }
        public string login_id { get; set; }
        public string vendor2 { get; set; }
        public string employee_name;
        public string destination_name;

        public string tax2{ get; set; }
        public string invoice2 { get; set; }
        public string tax_invoice_number2 { get; set; }

        public string comment { get; set; }
    }   
}