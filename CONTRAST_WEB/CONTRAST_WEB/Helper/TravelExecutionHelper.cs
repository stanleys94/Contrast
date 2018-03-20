using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.Models
{
    public class TravelExecutionHelper
    {
        public vw_travel_execution_list TravelRequest { get; set; }
        public tb_r_travel_execution TravelExecution { get; set; }
        public string Templat { get; set; }
        public string Templong { get; set; }
        
        public string error_string { get; set; }
    }
}