using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.Models
{
    public class InvoiceHelper
    {
        public vw_invoice_actualcost_new invoice { get; set; }
        public tb_m_employee loged_employee { get; set; }
        public List<Departure_City> from { get; set; }
    }

    public class Departure_City
    {
        public string City { get; set; }
        public int Destination { get; set; }
    }
}
