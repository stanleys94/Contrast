using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.Models

{
    public class PaymentProposalHelper
    {
        public vw_payment_proposal Entity { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Name { get; set; }
        public int No_Reg { get; set; }
    }
}