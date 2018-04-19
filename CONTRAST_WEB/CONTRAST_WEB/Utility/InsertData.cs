using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace CONTRAST_WEB.Models
{
    public static class InsertData
    {
        //Execute API call to post to TravelRequest table 
        public static async Task TravelRequest(TravelRequestHelper model)
        {
            var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            model.travel_request.start_date = model.travel_request.start_date.Value.ToUniversalTime();
            model.travel_request.start_date = model.travel_request.start_date + offset;
            model.travel_request.end_date = model.travel_request.end_date.Value.ToUniversalTime();
            model.travel_request.end_date = model.travel_request.end_date + offset;
            model.travel_request.create_date = model.travel_request.create_date.Value.ToUniversalTime();
            model.travel_request.create_date = model.travel_request.create_date + offset;

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/TravelRequest", model.travel_request);
                HttpResponseMessage response = await client.PostAsync("api/TravelRequest", new StringContent(
                                new JavaScriptSerializer().Serialize(model.travel_request), Encoding.UTF8, "application/json"));
            }
        }

        //Execute API call to post to TravelExecution table 
        public static async Task TravelExecution(tb_r_travel_execution model)
        {

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/TravelExecution", model);
                HttpResponseMessage response = await client.PostAsync("api/TravelExecution", new StringContent(
                               new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        //Execute API call to post to TravelSettlementReceipt table 
        public static async Task TravelSettlementReceipt(tb_r_travel_settlement_receipt model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.PostAsync("api/TravelSettlementReceipt", new StringContent(
                               new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        //Execute API call to post to ActualCost table         
        public static async Task ActualCost(tb_r_travel_actualcost model)
        {
            var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            if (model.start_date_extend != null)
            {
                model.start_date_extend = model.start_date_extend.Value.ToUniversalTime();
                model.start_date_extend = model.start_date_extend + offset;
            }
            if (model.end_date_extend != null)
            {
                model.end_date_extend = model.end_date_extend.Value.ToUniversalTime();
                model.end_date_extend = model.end_date_extend + offset;
            }
            if (model.amount < 2)
            {
                model.final_status = "8";
                model.information_actualcost = "TRANSACTION CLOSED";
            }
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.PostAsync("api/ActualCost", new StringContent(
                               new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        //Execute API call to post to Travel Participant table 
        public static async Task TravelParticipant(tb_r_travel_request_participant model)
        {
            var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            model.created_date = model.created_date.Value.ToUniversalTime();
            model.created_date = model.created_date + offset;

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();

                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.PostAsync("api/TravelRequestParticipant", new StringContent(
                               new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }


        public static async Task RecordGenerateFile(tb_r_record_generate_file model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/RecordGenerateFile", model);

                HttpResponseMessage response = await client.PostAsync("api/RecordGenFile", new StringContent(
                               new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task InvoiceWrite(tb_r_invoice_actualcost model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/StatusUploadMaster", model);

                HttpResponseMessage response = await client.PostAsync("api/TableInvoiceActualcost", new StringContent(
                               new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task StatusUpload(tb_r_statusUploadMaster model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/StatusUploadMaster", model);

                HttpResponseMessage response = await client.PostAsync("api/StatusUploadMaster", new StringContent(
                               new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        //Execute API call to post to Travel Status comment
        public static async Task TravelStatuscomment(string comment, string group_code, string name, int no_reg)
        {
            tb_r_travel_request_comment model = new tb_r_travel_request_comment();
            model.comment = comment;
            var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            model.create_date = DateTime.Now.ToUniversalTime();
            model.create_date = model.create_date + offset;
            model.group_code = group_code;
            model.no_reg_comment = no_reg;

            model.name = await GetData.EmployeeNameInfo(no_reg);
            model.read_flag = false;
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/TravelExecution", model);
                HttpResponseMessage response = await client.PostAsync("api/TravelRequestComment", new StringContent(
                               new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task RecordPaymentList(tb_r_record_payment_list model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.PostAsync("api/RecordPaymentList", new StringContent(
                new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }

        public static async Task RecordPaymentProposal(tb_r_record_payment_proposal model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.PostAsync("api/RecordPaymentProposal", new StringContent(
                new JavaScriptSerializer().Serialize(model), Encoding.UTF8, "application/json"));
            }
        }
    }       




}