﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.Models
{
    public class SettlementVerifiedHelper
    {
        public vw_settlement_verified Settlement_Verified { get; set; }
        public tb_m_employee EmployeeInfo { get; set; }
        public bool check_verify { get; set; }
        public bool check_reject { get; set; }
        public string flag { get; set; }
        public string position { get; set; }
        public string money { get; set; }
        public string comment { get; set; }
    }
}