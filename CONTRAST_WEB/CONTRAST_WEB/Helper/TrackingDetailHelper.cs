using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.Models
{
    public class TrackingDetailHelper
    {
        public List<string> Destination { get; set; }
        public List<DateTime> StartDate { get; set; }
        public List<DateTime> EndDate { get; set; }

        public string GroupCode { get; set; }
        public string Name { get; set; }
        public string EmployeeCode { get; set; }
        public string Division { get; set; }
        public string Itinierary { get; set; }

        public List<COST> ActualCost { get; set; }
        public List<COST> SettlementCost { get; set; }
        public List<COST> BPD { get; set; }

        public List<string> HigherUp { get; set; }
        public List<string> HigherUpCode { get; set; }
        public List<string> HigherUpApprovalStatus { get; set; }
        public List<string> HigherUpApprovalDate { get; set; }

        public List<tb_r_travel_execution> Executed { get; set; }
        public List<string> Executed_Coor { get; set; }

        public List<tb_r_travel_settlement> Settle { get; set; }
        public tb_m_employee logged_id { get; set; }
        public string privilage { get; set; }

        public List<PARTICIPANT> Participant { get; set; }
    }

    public class COST
    {
        public string Transaction { get; set; }
        public string CostType { get; set; }
        public int Amount { get; set; }
        public string Approved { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string Vendor { get; set; }
        public string Pending { get; set; }
        public string Approved_Status { get; set; }
        public string Path { get; set; }
    }

    public class PARTICIPANT
    {
        public int no_reg { get; set; }
        public string name { get; set; }
        public string division { get; set; }
        public string status { get; set; }
        public string BTA { get; set; }
    }
}