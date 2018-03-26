using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.Models
{
    public class TrackingHelper
    {
        public vw_tracking_transaction_data_new TrackedList { get; set; }
        public string login_name { get; set; }
        public string login_id { get; set; }
        public string privilage { get; set; }
        public int id_data { get; set; }
    }
}