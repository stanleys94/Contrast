using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace CONTRAST_WEB.Models
{
    public static class UpdateData
    {
        public static async Task ActualCost(ActualCostVerifiedHelper model, string position)
        {
            tb_r_travel_actualcost UpdatedData = new tb_r_travel_actualcost();
            UpdatedData = await GetData.ActualCostOrigin(model.ActualCost_Verified.id_actualcost);
            if (position.Trim() == "AP")
            {
                UpdatedData.ap_verified_by = model.EmployeeInfo.code;
                UpdatedData.ap_verified_datetime = DateTime.Now;
                UpdatedData.ap_verified_status = model.flag;
                UpdatedData.final_status = "1";
                if (model.flag == "2") UpdatedData.final_status = "2";
            }            
            else
            if (position.Trim() == "DpH-GA")
            {
                UpdatedData.dph_verified_by = model.EmployeeInfo.code;
                UpdatedData.dph_verified_datetime = DateTime.Now;
                UpdatedData.dph_verified_status = model.flag;
                if (model.flag == "2") UpdatedData.final_status = "2";

            }
            else
            if (position.Trim() == "STAFF-GA")
            {
                UpdatedData.ga_insert_by = model.EmployeeInfo.code;
                UpdatedData.ga_insert_datetime = DateTime.Now;
                UpdatedData.ga_status = model.flag;
                if (model.flag == "2") UpdatedData.final_status = "2";
            }
            else
            if (position.Trim() == "DpH-PAC")
            {
                UpdatedData.sh_verified_by = model.EmployeeInfo.code;
                UpdatedData.sh_verified_datetime = DateTime.Now;
                UpdatedData.sh_verified_status = model.flag;
                if (model.flag == "2") UpdatedData.final_status = "2";
            }

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PutAsJsonAsync("api/ActualCost/" + model.ActualCost_Verified.id_actualcost, UpdatedData);
                HttpResponseMessage response = await client.PutAsync("api/ActualCost/" + model.ActualCost_Verified.id_actualcost, new StringContent(
                                new JavaScriptSerializer().Serialize(UpdatedData), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task Settlement(SettlementVerifiedHelper model, string position)
        {
            
            List<tb_r_travel_actualcost> UpdatedData = new List<tb_r_travel_actualcost>();
            UpdatedData = await GetData.ActualCostOrigins(model.Settlement_Verified.group_code);
            if (position.Trim() == "AP")
            {
                for(int k=0;k<UpdatedData.Count();k++)
                {
                    
                    UpdatedData[k].ap_verified_by = model.EmployeeInfo.code;
                    UpdatedData[k].ap_verified_datetime = DateTime.Now;
                    UpdatedData[k].ap_verified_status = model.flag;
                    //UpdatedData[k].final_status = "1";
                    //if (model.flag == "2") UpdatedData[k].final_status = "2";
                    if (model.flag == "1" && UpdatedData[k].final_status == null) UpdatedData[k].final_status = "1";
                }
            }
            else
            if (position.Trim() == "DpH-GA")
            {
                for (int k = 0; k < UpdatedData.Count(); k++)
                {
                    UpdatedData[k].dph_verified_by = model.EmployeeInfo.code;
                    UpdatedData[k].dph_verified_datetime = DateTime.Now;
                    UpdatedData[k].dph_verified_status = model.flag;
                    //if (model.flag == "2") UpdatedData[k].final_status = "2";
                }
            }

            else
            if (position.Trim() == "STAFF-GA")
            {
                for (int k = 0; k < UpdatedData.Count(); k++)
                {
                    UpdatedData[k].ga_insert_by = model.EmployeeInfo.code;
                    UpdatedData[k].ga_insert_datetime = DateTime.Now;
                    UpdatedData[k].ga_status = model.flag;
                   // if (model.flag == "2") UpdatedData[k].final_status = "2";
                }
            }

            foreach (var item in UpdatedData)
            {                
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(Constant.Baseurl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                    //HttpResponseMessage response = await client.PutAsJsonAsync("api/ActualCost/" + model.Settlement_Verified.id_actualcost, UpdatedData);
                    HttpResponseMessage response = await client.PutAsync("api/ActualCost/" + item.id_actualcost, new StringContent(
                                    new JavaScriptSerializer().Serialize(item), Encoding.UTF8, "application/json"));
                }
            }
        }

        public static async Task SettlementRejected(SettlementVerifiedHelper model, string position, tb_r_record_rejected_verification rejected)
        {
            List<tb_r_travel_actualcost> UpdatedData = new List<tb_r_travel_actualcost>();
            UpdatedData = await GetData.ActualCostOrigins(model.Settlement_Verified.group_code);
            if (position.Trim() == "AP")
            {
                for (int k = 0; k < UpdatedData.Count(); k++)
                {
                    UpdatedData[k].ap_verified_by = model.EmployeeInfo.code;
                    UpdatedData[k].ap_verified_datetime = DateTime.Now;
                    UpdatedData[k].ap_verified_status = model.flag;
                    //UpdatedData[k].final_status = "1";
                    if (model.flag == "2"&&UpdatedData[k].final_status==null) UpdatedData[k].final_status = "2";
                }
            }
            else
            if (position.Trim() == "DpH-GA")
            {
                for (int k = 0; k < UpdatedData.Count(); k++)
                {
                    UpdatedData[k].dph_verified_by = model.EmployeeInfo.code;
                    UpdatedData[k].dph_verified_datetime = DateTime.Now;
                    UpdatedData[k].dph_verified_status = model.flag;
                    if (model.flag == "2" && UpdatedData[k].final_status == null) UpdatedData[k].final_status = "2";
                }
            }

            else
            if (position.Trim() == "STAFF-GA")
            {
                for (int k = 0; k < UpdatedData.Count(); k++)
                {
                    UpdatedData[k].ga_insert_by = model.EmployeeInfo.code;
                    UpdatedData[k].ga_insert_datetime = DateTime.Now;
                    UpdatedData[k].ga_status = model.flag;
                    if (model.flag == "2" && UpdatedData[k].final_status == null) UpdatedData[k].final_status = "2";
                }
            }

            foreach (var item in UpdatedData)
            {
                rejected.id_actualcost = item.id_actualcost;

                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(Constant.Baseurl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                    //HttpResponseMessage response = await client.PutAsJsonAsync("api/ActualCost/" + model.Settlement_Verified.id_actualcost, UpdatedData);
                    HttpResponseMessage response = await client.PutAsync("api/ActualCost/" + item.id_actualcost, new StringContent(
                                    new JavaScriptSerializer().Serialize(item), Encoding.UTF8, "application/json"));
                }

                if(item.final_status=="2")await Utility.RecordRejected(rejected);
            }
        }

        public static async Task RejectedClearance(string group_code)
        {
            List<tb_r_travel_actualcost> UpdatedData = new List<tb_r_travel_actualcost>();
            UpdatedData = await GetData.ActualCostOrigins(group_code);
           
            for (int k = 0; k < UpdatedData.Count(); k++)
            {                
                if(UpdatedData[k].final_status=="2"&&UpdatedData[k].information_actualcost=="Settlement")
                    UpdatedData[k].final_status = "3";
            }
        
            foreach (var item in UpdatedData)
            {
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(Constant.Baseurl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                    //HttpResponseMessage response = await client.PutAsJsonAsync("api/ActualCost/" + model.Settlement_Verified.id_actualcost, UpdatedData);
                    HttpResponseMessage response = await client.PutAsync("api/ActualCost/" + item.id_actualcost, new StringContent(
                                    new JavaScriptSerializer().Serialize(item), Encoding.UTF8, "application/json"));
                }
            }
        }
        
        public static async Task FixedCost(FixedCostVerifierHelper model, string position)
        {
            tb_r_travel_actualcost UpdatedData = new tb_r_travel_actualcost();
            UpdatedData = await GetData.ActualCostOrigin(model.FixedCost_Verified.id_actualcost);
            if (position.Trim() == "AP")
            {
                UpdatedData.ap_verified_by = model.EmployeeInfo.code;
                UpdatedData.ap_verified_datetime = DateTime.Now;
                UpdatedData.ap_verified_status = model.flag;
                UpdatedData.final_status = "1";
                if (model.flag == "2") UpdatedData.final_status = "2";
            }

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PutAsJsonAsync("api/ActualCost/" + model.FixedCost_Verified.id_actualcost, UpdatedData);
                HttpResponseMessage response = await client.PutAsync("api/ActualCost/" + model.FixedCost_Verified.id_actualcost, new StringContent(
                                new JavaScriptSerializer().Serialize(UpdatedData), Encoding.UTF8, "application/json"));
                
            }
        }
        
        //time offset fixed
        public static async Task TravelRequest(string group_code, string status_request)
        {
            List<tb_r_travel_request> TRSource = new List<tb_r_travel_request>();
            TRSource = await GetData.TravelRequestGCList(group_code);
            for (int k = 0; k < TRSource.Count(); k++)
            {
                TRSource[k]=await Utility.TravelRequestTimeOffset(TRSource[k]);
                TRSource[k].status_request = "7";
                if (status_request == "1") TRSource[k].active_flag = true;
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(Constant.Baseurl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                    //HttpResponseMessage response = await client.PutAsJsonAsync("api/TravelRequest/" + TRSource[k].id_request, TRSource[k]);
                    HttpResponseMessage response = await client.PutAsync("api/TravelRequest/" + TRSource[k].id_request, new StringContent(
                                new JavaScriptSerializer().Serialize(TRSource[k]), Encoding.UTF8, "application/json"));

                }
            }

        }

        public static async Task TravelRequest(tb_r_travel_request model)
        {
            var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            model.create_date = model.create_date.Value.ToUniversalTime();
            model.create_date = model.create_date + offset;
            model.end_date = model.end_date.Value.ToUniversalTime();
            model.end_date = model.end_date + offset;
            model.start_date = model.start_date.Value.ToUniversalTime();
            model.start_date = model.start_date + offset;


            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PutAsJsonAsync("api/TravelRequest/" + TRSource[k].id_request, TRSource[k]);
                HttpResponseMessage response = await client.PutAsync("api/TravelRequest/" + model.id_request, new StringContent(
                            new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
}   
        }

        public static async Task Budget(string wbs_no, string cost_center, double money)
        {
            tb_m_budget UpdatedData = new tb_m_budget();
            UpdatedData = await GetData.Budget(wbs_no,cost_center);
            UpdatedData.available_amount = UpdatedData.available_amount - money;
            UpdatedData.budget_remining = UpdatedData.available_amount;

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PutAsJsonAsync("api/ActualCost/" + model.ActualCost_Verified.id_actualcost, UpdatedData);
                HttpResponseMessage response = await client.PutAsync("api/Budget/" + UpdatedData.id_budget, new StringContent(
                                new JavaScriptSerializer().Serialize(UpdatedData), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task TravelRequestPersonal(tb_r_travel_request request)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PutAsJsonAsync("api/TravelRequest/" + request.id_request, request);

                HttpResponseMessage response = await client.PutAsync("api/TravelRequest/" + request.id_request, new StringContent(
                               new JavaScriptSerializer().Serialize(request), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task PhotoIcon(tb_m_photo_employee request)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PutAsJsonAsync("api/TravelRequest/" + request.id_request, request);

                HttpResponseMessage response = await client.PutAsync("api/PhotoEmployee/" + request.photo_id, new StringContent(
                               new JavaScriptSerializer().Serialize(request), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task ActualCostPersonal(int id_actualcost)
        {
            //tb_r_travel_actualcost UpdatedData = model;
            tb_r_travel_actualcost updated = await GetData.ActualCostOrigin(id_actualcost);
            updated.information_actualcost = "REJECTED ACTUALCOST";

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PutAsJsonAsync("api/ActualCost/" + UpdatedData.id_actualcost, UpdatedData);
                HttpResponseMessage response = await client.PutAsync("api/ActualCost/" + updated.id_actualcost, new StringContent(
                               new JavaScriptSerializer().Serialize(updated), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task UpdateTravelRequestForBPD(int id_travelrequest)
        {
            //tb_r_travel_actualcost UpdatedData = model;
            tb_r_travel_request updated = await GetData.TravelRequest(id_travelrequest);
            updated.active_flag = true;
            updated.status_request = "98";

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PutAsJsonAsync("api/ActualCost/" + UpdatedData.id_actualcost, UpdatedData);
                HttpResponseMessage response = await client.PutAsync("api/TravelRequest/" + updated.id_request, new StringContent(
                               new JavaScriptSerializer().Serialize(updated), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task UpdateActualCostForBPD(int id_actualcost)
        {
            //tb_r_travel_actualcost UpdatedData = model;
            tb_r_travel_actualcost updated = await GetData.ActualCostOrigin(id_actualcost);
            updated.information_actualcost = "CANCEL TRAVEL BY AP";
            updated.final_status = "2";

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PutAsJsonAsync("api/ActualCost/" + UpdatedData.id_actualcost, UpdatedData);
                HttpResponseMessage response = await client.PutAsync("api/ActualCost/" + updated.id_actualcost, new StringContent(
                               new JavaScriptSerializer().Serialize(updated), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task InvoiceActualCost(tb_r_invoice_actualcost update)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PutAsJsonAsync("api/ActualCost/" + UpdatedData.id_actualcost, UpdatedData);
                HttpResponseMessage response = await client.PutAsync("api/TableInvoiceActualcost/" + update.id_invoice, new StringContent(
                               new JavaScriptSerializer().Serialize(update), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task TravelDitDel(int id)
        {
            //tb_r_travel_actualcost UpdatedData = model;
            tb_r_travel_dit updated = await GetData.TravelDitListId(id);
            updated.active_flag = false;
            updated.comments = "deleted";

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PutAsJsonAsync("api/ActualCost/" + UpdatedData.id_actualcost, UpdatedData);
                HttpResponseMessage response = await client.PutAsync("api/TravelDIT/" + updated.id_dit, new StringContent(
                               new JavaScriptSerializer().Serialize(updated), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task TravelRequestCommentRead(string group_code)
        {
            List<tb_r_travel_request_comment> updated = new List<tb_r_travel_request_comment>();
            updated = await GetData.Comment(group_code);
            for (int k = 0; k < updated.Count(); k++)
            {
                updated[k].read_flag = true;
                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(Constant.Baseurl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                    //HttpResponseMessage response = await client.PutAsJsonAsync("api/ActualCost/" + UpdatedData.id_actualcost, UpdatedData);
                    HttpResponseMessage response = await client.PutAsync("api/TravelRequestComment/" + updated[k].id_comment, new StringContent(
                                   new JavaScriptSerializer().Serialize(updated[k]), Encoding.UTF8, "application/json"));
                }
            }
        }
    }
}