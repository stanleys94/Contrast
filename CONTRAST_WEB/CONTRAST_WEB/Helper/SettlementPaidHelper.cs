using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.Models
{
    public class SettlementPaidHelper
    {
        public vw_summary_settlement Summary { get; set; }
        public float MealSettlement { get; set; }
        public float HotelSettlement { get; set; }
        public float TicketSettlement { get; set; }
        public DateTime? StartSettlement { get; set; }
        public DateTime? EndSettlement { get; set; }
    }
}