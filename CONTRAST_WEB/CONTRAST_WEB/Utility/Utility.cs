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
        public static int CountDuration(DateTime start_date,DateTime end_date)
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

        public static string UploadPhoto(HttpPostedFileBase file, string formatter)
        {
            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
            {
                string fileName = formatter+DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString()
                    + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".jpg";
                //fileName = formatter + fileName;
                string fileContentType = file.ContentType;
                byte[] fileBytes = new byte[file.ContentLength];

                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.TPhotoEmployeeFolder), fileName);

                //var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

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

        public static async Task SpecCase(tb_m_spec_employee model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/SpecialEmployee", model);

                HttpResponseMessage response = await client.PostAsync("api/SpecialEmployee", new StringContent(
                                    new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

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
            sw.WriteLine("-"+DateTime.Now+":"+eventlog);
            sw.Close();

        }
    }
}