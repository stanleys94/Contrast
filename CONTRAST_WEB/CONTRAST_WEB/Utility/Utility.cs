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

        public static async Task Destination(tb_m_destination model)
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
                HttpResponseMessage response = await client.PostAsync("api/Destination", new StringContent(
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
            if (model.position.Trim() == "SECO" || model.position.Trim() == "SEA" || model.position.Trim() == "SMEC" || model.position.Trim() == "AADV" || model.position.Trim() == "GM" || model.position.Trim() == "EGM" || model.position.Trim() == "EC")
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

        public abstract class ModelStateTempDataTransfer : ActionFilterAttribute
        {
            protected static readonly string Key = typeof(ModelStateTempDataTransfer).FullName;
        }

        public class ExportModelStateToTempData : ModelStateTempDataTransfer
        {
            public override void OnActionExecuted(ActionExecutedContext filterContext)
            {
                //Only export when ModelState is not valid
                if (!filterContext.Controller.ViewData.ModelState.IsValid)
                {
                    //Export if we are redirecting
                    if ((filterContext.Result is RedirectResult) || (filterContext.Result is RedirectToRouteResult))
                    {
                        filterContext.Controller.TempData[Key] = filterContext.Controller.ViewData.ModelState;
                    }
                }

                base.OnActionExecuted(filterContext);
            }
        }

        public class ImportModelStateFromTempData : ModelStateTempDataTransfer
        {
            public override void OnActionExecuted(ActionExecutedContext filterContext)
            {
                ModelStateDictionary modelState = filterContext.Controller.TempData[Key] as ModelStateDictionary;

                if (modelState != null)
                {
                    //Only Import if we are viewing
                    if (filterContext.Result is ViewResult)
                    {
                        filterContext.Controller.ViewData.ModelState.Merge(modelState);
                    }
                    else
                    {
                        //Otherwise remove it.
                        filterContext.Controller.TempData.Remove(Key);
                    }
                }

                base.OnActionExecuted(filterContext);
            }
        }

        public static async Task<tb_r_travel_request> TravelRequestTimeOffset(tb_r_travel_request model)
        {
            var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            model.create_date = model.create_date.Value.ToUniversalTime();
            model.create_date = model.create_date + offset;
            model.end_date = model.end_date.Value.ToUniversalTime();
            model.end_date = model.end_date + offset;
            model.start_date = model.start_date.Value.ToUniversalTime();
            model.start_date = model.start_date + offset;

            if (model.apprv_date_lvl1 != null)
            {
                model.apprv_date_lvl1 = model.apprv_date_lvl1.Value.ToUniversalTime();
                model.apprv_date_lvl1 = model.apprv_date_lvl1 + offset;
            }

            if (model.apprv_date_lvl2 != null)
            { 
                model.apprv_date_lvl2 = model.apprv_date_lvl2.Value.ToUniversalTime();
                model.apprv_date_lvl2 = model.apprv_date_lvl2 + offset;
            }

            if (model.apprv_date_lvl3 != null)
            {
                model.apprv_date_lvl3 = model.apprv_date_lvl3.Value.ToUniversalTime();
                model.apprv_date_lvl3 = model.apprv_date_lvl3 + offset;
            }

            if (model.apprv_date_lvl4 != null)
            {
                model.apprv_date_lvl4 = model.apprv_date_lvl4.Value.ToUniversalTime();
                model.apprv_date_lvl4 = model.apprv_date_lvl4 + offset;
            }

            if (model.apprv_date_lvl5 != null)
            {
                model.apprv_date_lvl5 = model.apprv_date_lvl5.Value.ToUniversalTime();
                model.apprv_date_lvl5 = model.apprv_date_lvl5 + offset;
            }

            if (model.apprv_date_lvl6 != null)
            {
                model.apprv_date_lvl6 = model.apprv_date_lvl6.Value.ToUniversalTime();
                model.apprv_date_lvl6 = model.apprv_date_lvl6 + offset;
            }

            if (model.apprv_date_lvl7 != null)
            {
                model.apprv_date_lvl7 = model.apprv_date_lvl7.Value.ToUniversalTime();
                model.apprv_date_lvl7 = model.apprv_date_lvl7 + offset;
            }

            if (model.apprv_date_lvl8 != null)
            {
                model.apprv_date_lvl8 = model.apprv_date_lvl8.Value.ToUniversalTime();
                model.apprv_date_lvl8 = model.apprv_date_lvl8 + offset;
            }

            if (model.apprv_date_lvl9 != null)
            {
                model.apprv_date_lvl9 = model.apprv_date_lvl9.Value.ToUniversalTime();
                model.apprv_date_lvl9 = model.apprv_date_lvl9 + offset;
            }

            if (model.apprv_date_lvl10 != null)
            {
                model.apprv_date_lvl10 = model.apprv_date_lvl10.Value.ToUniversalTime();
                model.apprv_date_lvl10 = model.apprv_date_lvl10 + offset;
            }

            if (model.apprv_date_lvl11 != null)
            {
                model.apprv_date_lvl11 = model.apprv_date_lvl11.Value.ToUniversalTime();
                model.apprv_date_lvl11 = model.apprv_date_lvl11 + offset;
            }

            if (model.apprv_date_lvl12 != null)
            {
                model.apprv_date_lvl12 = model.apprv_date_lvl12.Value.ToUniversalTime();
                model.apprv_date_lvl12 = model.apprv_date_lvl12 + offset;
            }

            if (model.apprv_date_lvl13 != null)
            {
                model.apprv_date_lvl13 = model.apprv_date_lvl13.Value.ToUniversalTime();
                model.apprv_date_lvl13 = model.apprv_date_lvl13 + offset;
            }

            if (model.apprv_date_lvl14 != null)
            {
                model.apprv_date_lvl14 = model.apprv_date_lvl14.Value.ToUniversalTime();
                model.apprv_date_lvl14 = model.apprv_date_lvl14 + offset;
            }

            if (model.apprv_date_lvl15 != null)
            {
                model.apprv_date_lvl15 = model.apprv_date_lvl15.Value.ToUniversalTime();
                model.apprv_date_lvl15 = model.apprv_date_lvl15 + offset;
            }

            if (model.apprv_date_lvl16 != null)
            {
                model.apprv_date_lvl16 = model.apprv_date_lvl16.Value.ToUniversalTime();
                model.apprv_date_lvl16 = model.apprv_date_lvl16 + offset;
            }

            if (model.apprv_date_lvl17 != null)
            {
                model.apprv_date_lvl17 = model.apprv_date_lvl17.Value.ToUniversalTime();
                model.apprv_date_lvl17 = model.apprv_date_lvl17 + offset;
            }

            if (model.apprv_date_lvl18 != null)
            {
                model.apprv_date_lvl18 = model.apprv_date_lvl18.Value.ToUniversalTime();
                model.apprv_date_lvl18 = model.apprv_date_lvl18 + offset;
            }

            if (model.apprv_date_lvl19 != null)
            {
                model.apprv_date_lvl19 = model.apprv_date_lvl19.Value.ToUniversalTime();
                model.apprv_date_lvl19 = model.apprv_date_lvl19 + offset;
            }

            if (model.apprv_date_lvl20 != null)
            {
                model.apprv_date_lvl20 = model.apprv_date_lvl20.Value.ToUniversalTime();
                model.apprv_date_lvl20 = model.apprv_date_lvl20 + offset;
            }

            return model;
        }

        public static async Task<tb_r_travel_actualcost> ActualCostTimeOffset(tb_r_travel_actualcost model)
        {
            var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            model.create_date = model.create_date.Value.ToUniversalTime();
            model.create_date = model.create_date + offset;

            if (model.ap_verified_datetime != null)
            {
                model.ap_verified_datetime = model.ap_verified_datetime.Value.ToUniversalTime();
                model.ap_verified_datetime = model.ap_verified_datetime + offset;
            }

            if (model.dh_insert_datetime != null)
            {
                model.dh_insert_datetime = model.dh_insert_datetime.Value.ToUniversalTime();
                model.dh_insert_datetime = model.dh_insert_datetime + offset;
            }

            if (model.dph_verified_datetime != null)
            {
                model.dph_verified_datetime = model.dph_verified_datetime.Value.ToUniversalTime();
                model.dph_verified_datetime = model.dph_verified_datetime + offset;
            }

            if (model.fd_insert_datetime != null)
            {
                model.fd_insert_datetime = model.fd_insert_datetime.Value.ToUniversalTime();
                model.fd_insert_datetime = model.fd_insert_datetime + offset;
            }

            if (model.ga_insert_datetime != null)
            {
                model.ga_insert_datetime = model.ga_insert_datetime.Value.ToUniversalTime();
                model.ga_insert_datetime = model.ga_insert_datetime + offset;
            }

            if (model.pac_insert_datetime != null)
            {
                model.pac_insert_datetime = model.pac_insert_datetime.Value.ToUniversalTime();
                model.pac_insert_datetime = model.pac_insert_datetime + offset;
            }

            if (model.sh_verified_datetime != null)
            {
                model.sh_verified_datetime = model.sh_verified_datetime.Value.ToUniversalTime();
                model.sh_verified_datetime = model.sh_verified_datetime + offset;
            }

            if (model.apprv_date_lvl1 != null)
            {
                model.apprv_date_lvl1 = model.apprv_date_lvl1.Value.ToUniversalTime();
                model.apprv_date_lvl1 = model.apprv_date_lvl1 + offset;
            }

            if (model.apprv_date_lvl2 != null)
            {
                model.apprv_date_lvl2 = model.apprv_date_lvl2.Value.ToUniversalTime();
                model.apprv_date_lvl2 = model.apprv_date_lvl2 + offset;
            }

            if (model.apprv_date_lvl3 != null)
            {
                model.apprv_date_lvl3 = model.apprv_date_lvl3.Value.ToUniversalTime();
                model.apprv_date_lvl3 = model.apprv_date_lvl3 + offset;
            }

            if (model.apprv_date_lvl4 != null)
            {
                model.apprv_date_lvl4 = model.apprv_date_lvl4.Value.ToUniversalTime();
                model.apprv_date_lvl4 = model.apprv_date_lvl4 + offset;
            }

            if (model.apprv_date_lvl5 != null)
            {
                model.apprv_date_lvl5 = model.apprv_date_lvl5.Value.ToUniversalTime();
                model.apprv_date_lvl5 = model.apprv_date_lvl5 + offset;
            }

            if (model.apprv_date_lvl6 != null)
            {
                model.apprv_date_lvl6 = model.apprv_date_lvl6.Value.ToUniversalTime();
                model.apprv_date_lvl6 = model.apprv_date_lvl6 + offset;
            }

            if (model.apprv_date_lvl7 != null)
            {
                model.apprv_date_lvl7 = model.apprv_date_lvl7.Value.ToUniversalTime();
                model.apprv_date_lvl7 = model.apprv_date_lvl7 + offset;
            }

            if (model.apprv_date_lvl8 != null)
            {
                model.apprv_date_lvl8 = model.apprv_date_lvl8.Value.ToUniversalTime();
                model.apprv_date_lvl8 = model.apprv_date_lvl8 + offset;
            }

            if (model.apprv_date_lvl9 != null)
            {
                model.apprv_date_lvl9 = model.apprv_date_lvl9.Value.ToUniversalTime();
                model.apprv_date_lvl9 = model.apprv_date_lvl9 + offset;
            }

            if (model.apprv_date_lvl10 != null)
            {
                model.apprv_date_lvl10 = model.apprv_date_lvl10.Value.ToUniversalTime();
                model.apprv_date_lvl10 = model.apprv_date_lvl10 + offset;
            }

            if (model.apprv_date_lvl11 != null)
            {
                model.apprv_date_lvl11 = model.apprv_date_lvl11.Value.ToUniversalTime();
                model.apprv_date_lvl11 = model.apprv_date_lvl11 + offset;
            }

            if (model.apprv_date_lvl12 != null)
            {
                model.apprv_date_lvl12 = model.apprv_date_lvl12.Value.ToUniversalTime();
                model.apprv_date_lvl12 = model.apprv_date_lvl12 + offset;
            }

            if (model.apprv_date_lvl13 != null)
            {
                model.apprv_date_lvl13 = model.apprv_date_lvl13.Value.ToUniversalTime();
                model.apprv_date_lvl13 = model.apprv_date_lvl13 + offset;
            }

            if (model.apprv_date_lvl14 != null)
            {
                model.apprv_date_lvl14 = model.apprv_date_lvl14.Value.ToUniversalTime();
                model.apprv_date_lvl14 = model.apprv_date_lvl14 + offset;
            }

            if (model.apprv_date_lvl15 != null)
            {
                model.apprv_date_lvl15 = model.apprv_date_lvl15.Value.ToUniversalTime();
                model.apprv_date_lvl15 = model.apprv_date_lvl15 + offset;
            }

            if (model.apprv_date_lvl16 != null)
            {
                model.apprv_date_lvl16 = model.apprv_date_lvl16.Value.ToUniversalTime();
                model.apprv_date_lvl16 = model.apprv_date_lvl16 + offset;
            }

            if (model.apprv_date_lvl7 != null)
            {
                model.apprv_date_lvl17 = model.apprv_date_lvl17.Value.ToUniversalTime();
                model.apprv_date_lvl17 = model.apprv_date_lvl17 + offset;
            }

            if (model.apprv_date_lvl18 != null)
            {
                model.apprv_date_lvl18 = model.apprv_date_lvl18.Value.ToUniversalTime();
                model.apprv_date_lvl18 = model.apprv_date_lvl18 + offset;
            }

            if (model.apprv_date_lvl19 != null)
            {
                model.apprv_date_lvl19 = model.apprv_date_lvl19.Value.ToUniversalTime();
                model.apprv_date_lvl19 = model.apprv_date_lvl19 + offset;
            }

            if (model.apprv_date_lvl20 != null)
            {
                model.apprv_date_lvl20 = model.apprv_date_lvl20.Value.ToUniversalTime();
                model.apprv_date_lvl20 = model.apprv_date_lvl20 + offset;
            }

            return model;
        }



        public static async Task<DateTime> SingleTimeOffset(DateTime date)
        {
            var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            date = date.ToUniversalTime();
            date = date + offset;
            return date;
        }
    }
}
