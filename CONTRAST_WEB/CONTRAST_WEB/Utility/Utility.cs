using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CONTRAST_WEB.Models;
using System.IO;
using ClosedXML.Excel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Globalization;
using System.Text;
using System.Web.Script.Serialization;

namespace CONTRAST_WEB.Models
{
    public static class Utility
    {
        public static int CountDuration(DateTime start_date, DateTime end_date)
        {
            return 1;
        }


        public static string UploadSettlementReceipt(HttpPostedFileBase file, string formatter)
        {
            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
            {
                string fileName = file.FileName;
                fileName = formatter + fileName;
                string fileContentType = file.ContentType;
                byte[] fileBytes = new byte[file.ContentLength];

                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.TravelSettlementReceiptFolder), fileName);

                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));


                //await InsertData.TravelSettlementReceipt(model);
                file.SaveAs(path);
                return path;
            }
            return "Error";
        }

        public static string UploadFileUniversal(HttpPostedFileBase file, string filepath, string formatter)
        {
            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
            {
                string fileName = file.FileName.Split('\\').Last();
                fileName = formatter + fileName;
                string fileContentType = file.ContentType;
                byte[] fileBytes = new byte[file.ContentLength];

                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(filepath), fileName);

                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                file.SaveAs(path);
                path = Constant.DocumentFolder + fileName;
                return path;
            }
            return "Error";
        }

        public static string UploadPhoto(HttpPostedFileBase file, string formatter)
        {
            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
            {
                string fileName = formatter + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString()
                    + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".jpg";
                //fileName = formatter + fileName;
                string fileContentType = file.ContentType;
                byte[] fileBytes = new byte[file.ContentLength];

                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.TPhotoEmployeeFolder), fileName);

                file.SaveAs(path);
                return fileName;
            }
            return "Error";
        }

        //Upload Master 
        public static async Task RateHotel(tb_m_rate_hotel model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/RateHotel", model);
                HttpResponseMessage response = await client.PostAsync("api/RateHotel", new StringContent(
                                new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task RateFlight(tb_m_rate_flight model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/RateFlight", model);
                HttpResponseMessage response = await client.PostAsync("api/RateFlight", new StringContent(
                                 new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task Allowance(tb_m_overseas_allowance model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/OverseasAllowance", model);
                HttpResponseMessage response = await client.PostAsync("api/OverseasAllowance", new StringContent(
                                 new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task RateVisa(tb_m_rate_visa model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/RateVisa", model);
                HttpResponseMessage response = await client.PostAsync("api/RateVisa", new StringContent(
                                new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        //public static async Task SpecCase(tb_m_spec_employee model)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        //Passing service base url  
        //        client.BaseAddress = new Uri(Constant.Baseurl);

        //        client.DefaultRequestHeaders.Clear();
        //        //Define request data format  
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
        //        //HttpResponseMessage response = await client.PostAsJsonAsync("api/SpecialEmployee", model);

        //        HttpResponseMessage response = await client.PostAsync("api/SpecialEmployee", new StringContent(
        //                            new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
        //    }
        //}

        public static async Task Budget(tb_m_budget model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/Budget", model);

                HttpResponseMessage response = await client.PostAsync("api/Budget", new StringContent(
                                new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task Vendor(tb_m_vendor model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/Vendor", model);

                HttpResponseMessage response = await client.PostAsync("api/Vendor", new StringContent(
                                new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task RecordRejected(tb_r_record_rejected_verification model)
        {

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/RecordRejectedVerification", model);

                HttpResponseMessage response = await client.PostAsync("api/RecordRejectedVerification", new StringContent(
                                    new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task TravelProcedure(tb_m_travel_procedures model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/TravelProcedures", model);

                HttpResponseMessage response = await client.PostAsync("api/TravelProcedures", new StringContent(
                                    new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task OrganizationStructure(tb_m_employee_structure_organization model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/EmployeeStructureOrganization", model);

                HttpResponseMessage response = await client.PostAsync("api/EmployeeStructureOrganization", new StringContent(
                                        new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task BudgetSourceData(tb_m_budget_source_data model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/BudgetSourceData", model);

                HttpResponseMessage response = await client.PostAsync("api/BudgetSourceData", new StringContent(
                                            new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task RatePassport(tb_m_rate_passport model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/RatePassport", model);

                HttpResponseMessage response = await client.PostAsync("api/RatePassport", new StringContent(
                                        new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task VendorEmployee(tb_m_vendor_employee model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                // HttpResponseMessage response = await client.PostAsJsonAsync("api/VendorEmployee", model);

                HttpResponseMessage response = await client.PostAsync("api/VendorEmployee", new StringContent(
                                        new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task Logger(string eventlog)
        {
            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.LogFolder), "log.txt");
            var sw = new System.IO.StreamWriter(path, true);
            sw.WriteLine("-" + DateTime.Now + ":" + eventlog);
            sw.Close();

        }

        public static async Task<int?> AssignedBy(tb_m_employee model)
        {
            int? assign_by = null;
            //Get user direct superior info
            var assignedby = await GetData.AssignedBy(model.unit_code_code);
            tb_m_travel_procedures procedures = new tb_m_travel_procedures();
            if (model.position.Trim() == "SECO" || model.position.Trim() == "SEA" || model.position.Trim() == "SMEC" || model.position.Trim() == "AADV" || model.position.Trim() == "GM" || model.position.Trim() == "EGM")
            {
                procedures = await GetData.Procedures(model.position);
            }
            else
                procedures = await GetData.Procedures(model.@class);

            //mr nakata desu
            if (assignedby.pd == Convert.ToInt32(model.code)) assign_by = Convert.ToInt32(model.code);
            else
            //If the direct superior is DH, then get who is the DH from travel procedure table
            if (procedures.apprv_by_lvl1 == "DH") assign_by = assignedby.dh_code;
            else
            //If the direct superior is Div Director, then get who is the Div Director from travel procedure table
            if (procedures.apprv_by_lvl1 == "Div Director") assign_by = assignedby.director;
            //If the direct superior is vice president, then get who is the VP from travel procedure table
            else
            if (procedures.apprv_by_lvl1 == "VP") assign_by = assignedby.vp;
            else
            if (procedures.apprv_by_lvl1 == "EGM") assign_by = assignedby.egm;

            //if still empty - for special case
            if (assign_by == null)
            {
                if (assignedby.dh_code != null && model.position.Trim() != "DH" && model.position.Trim() != "EGM") assign_by = assignedby.dh_code;
                else
                    if (assignedby.egm != null && model.position.Trim() != "EGM") assign_by = assignedby.egm;
                else
                    if (assignedby.director != null) assign_by = assignedby.director;
                else
                    if (assignedby.local_fd != null) assign_by = assignedby.local_fd;
                else
                    if (assignedby.japan_fd != null) assign_by = assignedby.japan_fd;
                else
                    if (assignedby.vp != null) assign_by = assignedby.vp;
                else
                    if (assignedby.pd != null) assign_by = assignedby.pd;
            }

            return assign_by;
        }
    }
}