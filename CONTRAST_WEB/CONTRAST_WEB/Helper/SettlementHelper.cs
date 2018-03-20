using CONTRAST_WEB.CustomValidator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.Models
{
    public class SettlementHelper
    {
        public vw_travel_for_settlement TravelRequest { get; set; }
      
        
        public float MealSettlement{get;set;}
        public float PreparationSettlement{get;set;}
        public float HotelSettlement{get;set;}
        public float TicketSettlement{get;set;}
        public float LaundrySettlement{get;set;}
        public float TransportationSettlement{get;set;}
        public float MiscSettlement { get; set; }


        public double? TotalSettlement { get; set; }
        public double? TotMeal { get; set; }
        public double? TotTicket { get; set; }
        public double? TotHotel { get; set; }

        public double? TotLaundry { get; set; }
        public double? TotTransportation { get; set; }
        public double? TotOther { get; set; }

        public double? DifferenceSettlement { get; set; }
        public double? AllTotal { get; set; }

        public HttpPostedFileBase ReceiptFileLaundry { get; set; }
        public HttpPostedFileBase ReceiptFileOther { get; set; }
        public HttpPostedFileBase ReceiptFileTransportation { get; set; }

        public bool extend_flag { get; set; }

        [SettlementDateValidator]
        public DateTime? Start_Extend { get; set; }
        [SettlementDateValidator]
        public DateTime? End_Extend { get; set; }

    }
}