using CONTRAST_WEB.Helper;
using CONTRAST_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CONTRAST_WEB.DTO
{
    public class TravelAssignmentDTO
    {
        public IdentityModel Identity { get; set; }

        public string employee_info_code { get; set; }
        public string employee_info_name { get; set; }
        public string employee_info_class { get; set; }
        public string employee_info_division { get; set; }
        public string employee_info_assigned_by { get; set; }
        public string employee_info_bankname { get; set; }
        public string employee_info_bankaccount { get; set; }
        
        public string travel_request_name { get; set; }
        public string travel_request_noreg { get; set; }
        public string travel_request_purpose { get; set; }
        //planned unplanned
        public bool travel_request_type { get; set; }
        public string travel_request_reason { get; set; }
        //budget activity type
        public string travel_request_id_activity { get; set; }
        public string travel_request_departure_city { get; set; }
        public string travel_request_destination_city { get; set; }
        public DateTime travel_request_depart_date { get; set; }
        public DateTime travel_request_depart_time { get; set; }
        public DateTime travel_request_arrive_date { get; set; }
        public DateTime travel_request_arrive_time { get; set; }

        public bool travel_request_airticket_flag { get; set; }
        public bool travel_request_multiple_flag { get; set; }
        public bool travel_request_passport_flag { get; set; }

        public string travel_participant_noreg { get; set;}


        public void AutoFillEmployeeInfo(tb_m_employee temp,string division,int? assignedby, tb_m_vendor_employee vendor)
        {
            this.employee_info_code    =temp.code;
            this.employee_info_name    =temp.name;
            this.employee_info_class   =temp.@class;
            this.employee_info_division =division;
            this.travel_request_type = false;
            this.employee_info_assigned_by = assignedby.ToString();
            this.employee_info_bankaccount = vendor.account_number;
            this.employee_info_bankname = vendor.Bank_Name;
        }


        public async Task<string> GetEmployeeInfoDivision(string noreg)
        {
            tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(noreg));
            division.Divisi = division.Divisi.Replace("and1", "&");
            return division.Divisi;
        }

        public async void SetIdentity(HttpContextBase a)
        {
            this.Identity = Systems.Identity(a);
            //this.AutoFillEmployeeInfo();
        }


    }
}