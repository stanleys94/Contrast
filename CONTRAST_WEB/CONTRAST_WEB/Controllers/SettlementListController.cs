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
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp;
using PdfSharp.Drawing.Layout;
using CONTRAST_WEB.CustomValidator;
using System.Security.Claims;


namespace CONTRAST_WEB.Controllers
{
    public class SettlementListController : Controller
    {

        public async Task<SettlementHelper> Sum(SettlementHelper model)
        {
            TimeSpan duration = ((DateTime)model.End_Extend - (DateTime)model.Start_Extend);
            var rank = await GetData.EmployeeInfo(model.TravelRequest.no_reg.ToString());

            //var diff = (model.End_Extend - model.Start_Extend);

            var meal_platform = await GetData.Procedures(rank.@class);
            if (model.halfday_flag1 & model.halfday_flag2 == true && (DateTime)model.End_Extend == (DateTime)model.Start_Extend)
            {
                model.MealSettlement = (float)meal_platform.meal_allowance;
            }
            else if (model.halfday_flag1 | model.halfday_flag2 == true && (DateTime)model.End_Extend == (DateTime)model.Start_Extend)
            {
                model.MealSettlement = (float)meal_platform.meal_allowance / 2;

            }
            else if (model.halfday_flag1 & model.halfday_flag2 == true)
            {
                model.MealSettlement = (float)meal_platform.meal_allowance * Convert.ToInt32(duration.Days) + (float)meal_platform.meal_allowance;
            }
            else if (model.halfday_flag1 | model.halfday_flag2 == true)
            {
                model.MealSettlement = (float)meal_platform.meal_allowance * Convert.ToInt32(duration.Days) + (float)meal_platform.meal_allowance / 2;
            }
            else
            {
                model.MealSettlement = (float)meal_platform.meal_allowance * Convert.ToInt32(duration.Days);
            }
            //ModelState.Remove(ModelState.FirstOrDefault(m => m.Key.ToString().StartsWith("MealSettlement")));
            return (model);
        }

        public async Task<JsonResult> GetSearchValue(string search, string code)
        {
            List<Class1> list = new List<Class1>();
            list = await GetData.SearchName(search);
            List<Class1> filtered = new List<Class1>();
            foreach (var item in list)
            {
                if (!item.code.Contains(code)) filtered.Add(item);
            }

            return new JsonResult { Data = filtered, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public async Task<JsonResult> GetSearchValue2(string search, string code)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            tb_m_employee_source_data div = await GetData.GetDivisionSource(Convert.ToInt32(identity.Name));
            if (div.Divisi.Contains("and1"))
            {
                div.Divisi = div.Divisi.Replace("and1", "&");
            }
            List<Class1> list = new List<Class1>();
            list = await GetData.SearchNameDiv(search, div.Divisi);
            //list = await GetData.SearchName(search);
            List<Class1> filtered = new List<Class1>();
            foreach (var item in list)
            {
                if (!item.code.Contains(code)) filtered.Add(item);
            }

            return new JsonResult { Data = filtered, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: SettlementList
        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string applied)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;
            tb_m_employee model = new tb_m_employee();

            if (applied != null) model = await GetData.EmployeeInfo(applied);
            else model = await GetData.EmployeeInfo(identity.Name);

            tb_m_employee logged = await GetData.EmployeeInfo(identity.Name);

            ViewBag.loged_id = logged.code.Trim();
            ViewBag.loged_name = logged.name.Trim(' ');

            ViewBag.applied_name = model.name.Trim();

            ViewBag.Employee = model;
            ViewBag.applied = model.code.Trim();

            List<vw_travel_for_settlement> ResponseList = new List<vw_travel_for_settlement>();
            List<vw_rejected_travel_for_settlement> RejectList = new List<vw_rejected_travel_for_settlement>();
            ResponseList = await GetData.TravelSettlementList(Convert.ToInt32(model.code));
            RejectList = await GetData.RejectTravelSettlementList(Convert.ToInt32(model.code));
            int k = ResponseList.Count;
            for (int i = 0; i < ResponseList.Count(); i++)
            {
                ResponseList[i].login_id = model.code;
                ResponseList[i].final_status = "0";
                ResponseList[i].comment = "";
                ResponseList[i].process_reject = "";
                k++;
            }

            foreach (var item in RejectList)
            {
                vw_travel_for_settlement temp = new vw_travel_for_settlement();
                temp.comment = item.comment != null ? item.comment : "No Comment";
                temp.destination_name = item.destination_name;
                temp.emp_name = item.emp_name;
                temp.end_date = item.end_date;
                temp.final_status = item.final_status;
                temp.grand_total_settlement = item.grand_total_settlement;
                temp.group_code = item.group_code;
                temp.id_destination_city = item.id_destination_city;
                temp.id_request = item.id_request;
                temp.login_id = model.code;
                temp.no_reg = item.no_reg;
                temp.process_reject = item.process_reject;
                temp.start_date = item.start_date;
                temp.total_hotel = item.total_hotel;
                temp.total_laundry = item.total_laundry;
                temp.total_meal = item.total_meal;
                temp.total_miscellaneous = item.total_miscellaneous;
                temp.total_preparation = item.total_preparation;
                temp.total_ticket = item.total_ticket;
                temp.total_transportation = item.total_transportation;
                temp.total_winter = item.total_winter;
                ResponseList.Add(temp);
            }
            return View(ResponseList);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        [NoCache]
        public async Task<ActionResult> Details(vw_travel_for_settlement model)
        {
            SettlementHelper Settlement = new SettlementHelper();
            Settlement.TravelRequest = model;
            Settlement.extend_flag = true;
            Response.Cache.SetNoStore();
            return View(Settlement);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        [NoCache]
        public async Task<ActionResult> Insert(SettlementHelper model, string sum, string insert)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;

            if (ModelState.IsValid)
            {
                if (insert != null)
                {
                    if (Request.Files["UploadedFile1"] != null)
                    {
                        HttpPostedFileBase file = Request.Files["UploadedFile1"];
                        model.ReceiptFileTransportation = file;
                    }
                    if (Request.Files["UploadedFile2"] != null)
                    {
                        HttpPostedFileBase file = Request.Files["UploadedFile2"];
                        model.ReceiptFileLaundry = file;
                    }
                    if (Request.Files["UploadedFile3"] != null)
                    {
                        HttpPostedFileBase file = Request.Files["UploadedFile3"];
                        model.ReceiptFileOther = file;
                    }

                    await UpdateData.RejectedClearance(model.TravelRequest.group_code);

                    tb_r_travel_actualcost ActualCostObject = new tb_r_travel_actualcost();
                    ActualCostObject.information_actualcost = "Settlement";
                    ActualCostObject.end_date_extend = model.End_Extend;
                    ActualCostObject.start_date_extend = model.Start_Extend;

                    ActualCostObject.user_created = model.TravelRequest.no_reg.ToString();
                    ActualCostObject.additional1 = model.halfday_flag1.ToString();
                    ActualCostObject.additional2 = model.halfday_flag2.ToString();


                    List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();

                    bankName = await GetData.VendorEmployee((model.TravelRequest.no_reg));

                    ActualCostObject.vendor_code = bankName[0].vendor_code_employee;

                    ActualCostObject.group_code = model.TravelRequest.group_code;
                    ActualCostObject.id_request = model.TravelRequest.id_request;
                    ActualCostObject.create_date = DateTime.Now;

                    string division_r = await GetData.GetDivMapping(model.TravelRequest.no_reg.ToString());
                    var region = await GetData.RegionInfo(model.TravelRequest.id_destination_city);
                    tb_m_budget budget = await GetData.GetCostWbs(region == 4 ? false : true, division_r.Trim());

                    ActualCostObject.wbs_no = budget.eoa_wbs_no;
                    ActualCostObject.cost_center = budget.cost_center;

                    ActualCostObject.no_reg = model.TravelRequest.no_reg ?? default(int);

                    List<vw_travel_for_settlement> ResponseList = new List<vw_travel_for_settlement>();

                    ResponseList = await GetData.TravelSettlementList(Convert.ToInt32(model.TravelRequest.login_id));
                    ///*
                    for (int i = 0; i < ResponseList.Count(); i++)
                    {
                        ResponseList[i].login_id = model.TravelRequest.login_id;
                    }

                    //temporary time container
                    DateTime? temp_start = ActualCostObject.start_date_extend;
                    DateTime? temp_end = ActualCostObject.end_date_extend;

                    if (model.MealSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.MealSettlement;
                        ActualCostObject.jenis_transaksi = "Meal";
                        await InsertData.ActualCost(ActualCostObject);
                        ActualCostObject.start_date_extend = temp_start;
                        ActualCostObject.end_date_extend = temp_end;
                    }

                    if (model.HotelSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.HotelSettlement;
                        ActualCostObject.jenis_transaksi = "Hotel";
                        await InsertData.ActualCost(ActualCostObject);
                        ActualCostObject.start_date_extend = temp_start;
                        ActualCostObject.end_date_extend = temp_end;
                    }

                    if (model.TicketSettlement > 0)
                    {

                        ActualCostObject.amount = (int)model.TicketSettlement;
                        ActualCostObject.jenis_transaksi = "Ticket";
                        await InsertData.ActualCost(ActualCostObject);
                        ActualCostObject.start_date_extend = temp_start;
                        ActualCostObject.end_date_extend = temp_end;
                    }

                    if (model.LaundrySettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.LaundrySettlement;
                        ActualCostObject.jenis_transaksi = "Laundry";
                        ActualCostObject.path_file = Utility.UploadSettlementReceipt(model.ReceiptFileLaundry, "Laundry_" + ActualCostObject.no_reg + "_" + "_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                        await InsertData.ActualCost(ActualCostObject);
                        ActualCostObject.start_date_extend = temp_start;
                        ActualCostObject.end_date_extend = temp_end;
                    }

                    if (model.TransportationSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.TransportationSettlement;
                        ActualCostObject.jenis_transaksi = "Transportation";
                        ActualCostObject.path_file = Utility.UploadSettlementReceipt(model.ReceiptFileTransportation, "Transport_" + ActualCostObject.no_reg + "_" + "_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                        await InsertData.ActualCost(ActualCostObject);
                        ActualCostObject.start_date_extend = temp_start;
                        ActualCostObject.end_date_extend = temp_end;
                    }

                    if (model.MiscSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.MiscSettlement;
                        ActualCostObject.jenis_transaksi = "Miscellaneous";
                        ActualCostObject.path_file = Utility.UploadSettlementReceipt(model.ReceiptFileOther, "Other_" + ActualCostObject.no_reg + "_" + "_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                        await InsertData.ActualCost(ActualCostObject);
                        ActualCostObject.start_date_extend = temp_start;
                        ActualCostObject.end_date_extend = temp_end;
                    }

                    //update data
                    //vw_summary_settlement SummarySettlementObject = new vw_summary_settlement();
                    //SummarySettlementObject = await GetData.SummarySettlementInfo(ActualCostObject.group_code);
                    SettlementPaidHelper SummarySettlementObject = new SettlementPaidHelper();
                    SummarySettlementObject.Summary = await GetData.SummarySettlementInfo(ActualCostObject.group_code);

                    //cek update data
                    if (model.MealSettlement == 0 && model.PreparationSettlement == 0 && model.HotelSettlement == 0 && model.TicketSettlement == 0 && model.LaundrySettlement == 0 && model.TransportationSettlement == 0 && model.MiscSettlement == 0)
                        await UpdateData.TravelRequest(ActualCostObject.group_code, "1");
                    else
                        await UpdateData.TravelRequest(ActualCostObject.group_code, "0");

                    //migrate ke helper baru

                    if (model.HotelSettlement > 0) SummarySettlementObject.HotelSettlement = model.HotelSettlement;
                    else SummarySettlementObject.HotelSettlement = 0;

                    if (model.MealSettlement > 0) SummarySettlementObject.MealSettlement = model.MealSettlement;
                    else SummarySettlementObject.MealSettlement = 0;

                    if (model.TicketSettlement > 0) SummarySettlementObject.TicketSettlement = model.TicketSettlement;
                    else SummarySettlementObject.TicketSettlement = 0;

                    if (model.Start_Extend.HasValue) SummarySettlementObject.StartSettlement = model.Start_Extend;
                    if (model.End_Extend.HasValue) SummarySettlementObject.EndSettlement = model.End_Extend;

                    SummarySettlementObject.Summary.total_meal = Convert.ToInt32(Convert.ToDouble(SummarySettlementObject.Summary.total_meal) - SummarySettlementObject.MealSettlement);
                    SummarySettlementObject.Summary.total_hotel = Convert.ToInt32(Convert.ToDouble(SummarySettlementObject.Summary.total_hotel) - SummarySettlementObject.HotelSettlement);
                    SummarySettlementObject.Summary.total_ticket = Convert.ToInt32(Convert.ToDouble(SummarySettlementObject.Summary.total_ticket) - SummarySettlementObject.TicketSettlement);

                    //if (!model.extend_flag)
                    return View("SummaryPaid", SummarySettlementObject);                    
                }
                else
                if (sum != null)
                {
                    SettlementHelper return_model = await Sum(model);
                    ModelState.Remove(ModelState.FirstOrDefault(m => m.Key.ToString().StartsWith("MealSettlement")));
                    return View("Insert", return_model);
                    //return View("Details", return_model);
                }
                else
                    return View("Details", model);
            }
            else
                return View("Details", model);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public ActionResult SummaryPaid(vw_summary_settlement model)
        {

            return View("Index");
        }

        // GET: SettlementList

        public async Task<ActionResult> IndexMSTR(string noreg)
        {
            tb_m_employee model = await GetData.EmployeeInfo(noreg);
            ViewBag.Employee = model;
            ViewBag.applied = model.code;
            List<vw_travel_for_settlement> ResponseList = new List<vw_travel_for_settlement>();
            List<vw_rejected_travel_for_settlement> RejectList = new List<vw_rejected_travel_for_settlement>();
            ResponseList = await GetData.TravelSettlementList(Convert.ToInt32(model.code));
            RejectList = await GetData.RejectTravelSettlementList(Convert.ToInt32(model.code));
            int k = ResponseList.Count;
            for (int i = 0; i < ResponseList.Count(); i++)
            {
                ResponseList[i].login_id = model.code;
                ResponseList[i].final_status = "0";
                ResponseList[i].comment = "";
                ResponseList[i].process_reject = "";
                k++;
            }

            foreach (var item in RejectList)
            {
                vw_travel_for_settlement temp = new vw_travel_for_settlement();
                temp.comment = item.comment != null ? item.comment : "No Comment";
                temp.destination_name = item.destination_name;
                temp.emp_name = item.emp_name;
                temp.end_date = item.end_date;
                temp.final_status = item.final_status;
                temp.grand_total_settlement = item.grand_total_settlement;
                temp.group_code = item.group_code;
                temp.id_destination_city = item.id_destination_city;
                temp.id_request = item.id_request;
                temp.login_id = model.code;
                temp.no_reg = item.no_reg;
                temp.process_reject = item.process_reject;
                temp.start_date = item.start_date;
                temp.total_hotel = item.total_hotel;
                temp.total_laundry = item.total_laundry;
                temp.total_meal = item.total_meal;
                temp.total_miscellaneous = item.total_miscellaneous;
                temp.total_preparation = item.total_preparation;
                temp.total_ticket = item.total_ticket;
                temp.total_transportation = item.total_transportation;
                temp.total_winter = item.total_winter;
                ResponseList.Add(temp);
            }

            var headers = Request.Headers.GetValues("User-Agent");
            string userAgent = string.Join(" ", headers);

            if (userAgent.ToLower().Contains("ipad"))
                return View("IndexMSTR", ResponseList);
            else
                return View("IndexMSTRMobile", ResponseList);

        }

        public async Task<ActionResult> DetailsMSTR(vw_travel_for_settlement model)
        {
            SettlementHelper Settlement = new SettlementHelper();
            Settlement.TravelRequest = model;
            Settlement.extend_flag = true;
            Response.Cache.SetNoStore();
            return View(Settlement);
        }

        public async Task<ActionResult> InsertMSTR(SettlementHelper model, string sum, string insert)
        {
            if (ModelState.IsValid)
            {
                if (insert != null)
                {
                    if (Request.Files["UploadedFile1"] != null)
                    {
                        HttpPostedFileBase file = Request.Files["UploadedFile1"];
                        model.ReceiptFileTransportation = file;
                    }
                    if (Request.Files["UploadedFile2"] != null)
                    {
                        HttpPostedFileBase file = Request.Files["UploadedFile2"];
                        model.ReceiptFileLaundry = file;
                    }
                    if (Request.Files["UploadedFile3"] != null)
                    {
                        HttpPostedFileBase file = Request.Files["UploadedFile3"];
                        model.ReceiptFileOther = file;
                    }

                    await UpdateData.RejectedClearance(model.TravelRequest.group_code);

                    tb_r_travel_actualcost ActualCostObject = new tb_r_travel_actualcost();
                    ActualCostObject.information_actualcost = "Settlement";
                    ActualCostObject.end_date_extend = model.End_Extend;
                    ActualCostObject.start_date_extend = model.Start_Extend;

                    ActualCostObject.user_created = model.TravelRequest.no_reg.ToString();
                    ActualCostObject.additional1 = model.halfday_flag1.ToString();
                    ActualCostObject.additional2 = model.halfday_flag2.ToString();


                    List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();

                    bankName = await GetData.VendorEmployee((model.TravelRequest.no_reg));

                    ActualCostObject.vendor_code = bankName[0].vendor_code_employee;

                    ActualCostObject.group_code = model.TravelRequest.group_code;
                    ActualCostObject.id_request = model.TravelRequest.id_request;
                    ActualCostObject.create_date = DateTime.Now;

                    string division_r = await GetData.GetDivMapping(model.TravelRequest.no_reg.ToString());
                    var region = await GetData.RegionInfo(model.TravelRequest.id_destination_city);
                    tb_m_budget budget = await GetData.GetCostWbs(region == 4 ? false : true, division_r.Trim());

                    ActualCostObject.wbs_no = budget.eoa_wbs_no;
                    ActualCostObject.cost_center = budget.cost_center;

                    ActualCostObject.no_reg = model.TravelRequest.no_reg ?? default(int);

                    List<vw_travel_for_settlement> ResponseList = new List<vw_travel_for_settlement>();

                    ResponseList = await GetData.TravelSettlementList(Convert.ToInt32(model.TravelRequest.login_id));
                    ///*
                    for (int i = 0; i < ResponseList.Count(); i++)
                    {
                        ResponseList[i].login_id = model.TravelRequest.login_id;
                    }

                    //temporary time container
                    DateTime? temp_start = ActualCostObject.start_date_extend;
                    DateTime? temp_end = ActualCostObject.end_date_extend;

                    if (model.MealSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.MealSettlement;
                        ActualCostObject.jenis_transaksi = "Meal";
                        await InsertData.ActualCost(ActualCostObject);
                        ActualCostObject.start_date_extend = temp_start;
                        ActualCostObject.end_date_extend = temp_end;
                    }

                    if (model.HotelSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.HotelSettlement;
                        ActualCostObject.jenis_transaksi = "Hotel";
                        await InsertData.ActualCost(ActualCostObject);
                        ActualCostObject.start_date_extend = temp_start;
                        ActualCostObject.end_date_extend = temp_end;
                    }

                    if (model.TicketSettlement > 0)
                    {

                        ActualCostObject.amount = (int)model.TicketSettlement;
                        ActualCostObject.jenis_transaksi = "Ticket";
                        await InsertData.ActualCost(ActualCostObject);
                        ActualCostObject.start_date_extend = temp_start;
                        ActualCostObject.end_date_extend = temp_end;
                    }

                    if (model.LaundrySettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.LaundrySettlement;
                        ActualCostObject.jenis_transaksi = "Laundry";
                        ActualCostObject.path_file = Utility.UploadSettlementReceipt(model.ReceiptFileLaundry, "Laundry_" + ActualCostObject.no_reg + "_" + "_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                        await InsertData.ActualCost(ActualCostObject);
                        ActualCostObject.start_date_extend = temp_start;
                        ActualCostObject.end_date_extend = temp_end;
                    }

                    if (model.TransportationSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.TransportationSettlement;
                        ActualCostObject.jenis_transaksi = "Transportation";
                        ActualCostObject.path_file = Utility.UploadSettlementReceipt(model.ReceiptFileTransportation, "Transport_" + ActualCostObject.no_reg + "_" + "_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                        await InsertData.ActualCost(ActualCostObject);
                        ActualCostObject.start_date_extend = temp_start;
                        ActualCostObject.end_date_extend = temp_end;
                    }

                    if (model.MiscSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.MiscSettlement;
                        ActualCostObject.jenis_transaksi = "Miscellaneous";
                        ActualCostObject.path_file = Utility.UploadSettlementReceipt(model.ReceiptFileOther, "Other_" + ActualCostObject.no_reg + "_" + "_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                        await InsertData.ActualCost(ActualCostObject);
                        ActualCostObject.start_date_extend = temp_start;
                        ActualCostObject.end_date_extend = temp_end;
                    }

                    //update data
                    //vw_summary_settlement SummarySettlementObject = new vw_summary_settlement();
                    //SummarySettlementObject = await GetData.SummarySettlementInfo(ActualCostObject.group_code);
                    SettlementPaidHelper SummarySettlementObject = new SettlementPaidHelper();
                    SummarySettlementObject.Summary = await GetData.SummarySettlementInfo(ActualCostObject.group_code);

                    //cek update data
                    if (model.MealSettlement == 0 && model.PreparationSettlement == 0 && model.HotelSettlement == 0 && model.TicketSettlement == 0 && model.LaundrySettlement == 0 && model.TransportationSettlement == 0 && model.MiscSettlement == 0)
                        await UpdateData.TravelRequest(ActualCostObject.group_code, "1");
                    else
                        await UpdateData.TravelRequest(ActualCostObject.group_code, "0");

                    //migrate ke helper baru

                    if (model.HotelSettlement > 0) SummarySettlementObject.HotelSettlement = model.HotelSettlement;
                    else SummarySettlementObject.HotelSettlement = 0;

                    if (model.MealSettlement > 0) SummarySettlementObject.MealSettlement = model.MealSettlement;
                    else SummarySettlementObject.MealSettlement = 0;

                    if (model.TicketSettlement > 0) SummarySettlementObject.TicketSettlement = model.TicketSettlement;
                    else SummarySettlementObject.TicketSettlement = 0;

                    if (model.Start_Extend.HasValue) SummarySettlementObject.StartSettlement = model.Start_Extend;
                    if (model.End_Extend.HasValue) SummarySettlementObject.EndSettlement = model.End_Extend;

                    SummarySettlementObject.Summary.total_meal = Convert.ToInt32(Convert.ToDouble(SummarySettlementObject.Summary.total_meal) - SummarySettlementObject.MealSettlement);
                    SummarySettlementObject.Summary.total_hotel = Convert.ToInt32(Convert.ToDouble(SummarySettlementObject.Summary.total_hotel) - SummarySettlementObject.HotelSettlement);
                    SummarySettlementObject.Summary.total_ticket = Convert.ToInt32(Convert.ToDouble(SummarySettlementObject.Summary.total_ticket) - SummarySettlementObject.TicketSettlement);

                    //if (!model.extend_flag)
                    return View("SummaryPaid", SummarySettlementObject);
                }
                else
                if (sum != null)
                {
                    SettlementHelper return_model = await Sum(model);
                    ModelState.Remove(ModelState.FirstOrDefault(m => m.Key.ToString().StartsWith("MealSettlement")));
                    return View("InsertMSTR", return_model);
                    //return View("Insert", return_model);
                    //return View("Details", return_model);
                }
                else
                    return View("DetailsMSTR", model);
            }
            else
                return View("DetailsMSTR", model);
        }


        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Print(SettlementPaidHelper model)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Purchase Receipt";
            PdfPage page = document.AddPage();
            page.Orientation = PdfSharp.PageOrientation.Portrait;
            XSize size = PageSizeConverter.ToSize(PdfSharp.PageSize.A4);
            page.Height = size.Height;
            page.Width = size.Width;

            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont head = new XFont("Arial Narrow", 16, XFontStyle.Regular);
            XFont address = new XFont("Arial Narrow", 11, XFontStyle.Regular);
            XFont body = new XFont("Calibri", 10, XFontStyle.Regular);
            XFont bodyBold = new XFont("Calibri", 10, XFontStyle.Bold);
            XFont body_title = new XFont("Calibri", 10, XFontStyle.Regular);
            XFont total = new XFont("Lucida Grande", 11, XFontStyle.Bold);
            XFont title = new XFont("Lucida Grande", 12, XFontStyle.Regular);
            XFont titleBold = new XFont("Lucida Grande", 12, XFontStyle.Bold);
            XFont credit = new XFont("Lucida Sans", 9, XFontStyle.Regular);

            XPen header_line = new XPen(XColors.Black, 2);
            XPen body_line = new XPen(XColors.DimGray, 0.5);
            XPen POLine = new XPen(XColors.SteelBlue, 3);

            string PT_TAM = "PT. TOYOTA ASTRA MOTOR";
            string receipt = "SETTLEMENT RECEIPT";
            string add1 = "Jl. Laks. Yos Sudarso, Sunter II";
            string add2 = "Jakarta Utara - Indonesia";
            string add3 = "Phone :62-21 - 6515551(Hunting)";

            string btr = model.Summary.group_code;
            string name = model.Summary.emp_name.ToUpper();
            string noreg = model.Summary.no_reg.ToString();
            string TAMPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "tam_logo.PNG");
            string Paid = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "receipt-paid4.png");
            string ContrastPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "small_logo_horizontal.png");

            // string date = model.invoice.start_date.ToString();

            int total_Reimburse = 0;
            int total_Actual = 0;

            List<string> start_date = new List<string>();
            List<string> start_time = new List<string>();

            List<string> end_date = new List<string>();
            List<string> end_time = new List<string>();

            List<string> destination = new List<string>();
            List<string> from = new List<string>();

            int x_pad_max = 0;
            int x_pad = 0;
            int y_pad = 0;
            int y_pad_max = 0;
            int legend = 0;

            int count = 0;
            if (model.Summary.startDate_1.HasValue)
            {
                start_date.Add(Convert.ToDateTime(model.Summary.startDate_1).ToString("dd MMM yyyy"));
                end_date.Add(Convert.ToDateTime(model.Summary.endDate_1).ToString("dd MMM yyyy"));

                start_time.Add(Convert.ToDateTime(model.Summary.startDate_1).ToString("hh:mm tt"));
                end_time.Add(Convert.ToDateTime(model.Summary.endDate_1).ToString("hh:mm tt"));

                destination.Add(model.Summary.destination_1);
                from.Add("Jakarta");
                count++;
            }
            if (model.Summary.startDate_2.HasValue)
            {
                start_date.Add(Convert.ToDateTime(model.Summary.startDate_2).ToString("dd MMM yyyy"));
                end_date.Add(Convert.ToDateTime(model.Summary.endDate_2).ToString("dd MMM yyyy"));

                start_time.Add(Convert.ToDateTime(model.Summary.startDate_2).ToString("hh:mm tt"));
                end_time.Add(Convert.ToDateTime(model.Summary.endDate_2).ToString("hh:mm tt"));

                destination.Add(model.Summary.destination_2);
                from.Add(model.Summary.destination_1);
                count++;
            }
            if (model.Summary.startDate_3.HasValue)
            {
                start_date.Add(Convert.ToDateTime(model.Summary.startDate_3).ToString("dd MMM yyyy"));
                end_date.Add(Convert.ToDateTime(model.Summary.endDate_3).ToString("dd MMM yyyy"));

                start_time.Add(Convert.ToDateTime(model.Summary.startDate_3).ToString("hh:mm tt"));
                end_time.Add(Convert.ToDateTime(model.Summary.endDate_3).ToString("hh:mm tt"));

                destination.Add(model.Summary.destination_3);
                from.Add(model.Summary.destination_2);
                count++;
            }
            if (model.Summary.startDate_4.HasValue)
            {
                start_date.Add(Convert.ToDateTime(model.Summary.startDate_4).ToString("dd MMM yyyy"));
                end_date.Add(Convert.ToDateTime(model.Summary.endDate_4).ToString("dd MMM yyyy"));

                start_time.Add(Convert.ToDateTime(model.Summary.startDate_4).ToString("hh:mm tt"));
                end_time.Add(Convert.ToDateTime(model.Summary.endDate_4).ToString("hh:mm tt"));

                destination.Add(model.Summary.destination_4);
                from.Add(model.Summary.destination_3);
                count++;
            }
            if (model.Summary.startDate_5.HasValue)
            {
                start_date.Add(Convert.ToDateTime(model.Summary.startDate_5).ToString("dd MMM yyyy"));
                end_date.Add(Convert.ToDateTime(model.Summary.endDate_5).ToString("dd MMM yyyy"));

                start_time.Add(Convert.ToDateTime(model.Summary.startDate_5).ToString("hh:mm tt"));
                end_time.Add(Convert.ToDateTime(model.Summary.endDate_5).ToString("hh:mm tt"));

                destination.Add(model.Summary.destination_5);
                from.Add(model.Summary.destination_4);
                count++;
            }
            if (model.Summary.startDate_6.HasValue)
            {
                start_date.Add(Convert.ToDateTime(model.Summary.startDate_6).ToString("dd MMM yyyy"));
                end_date.Add(Convert.ToDateTime(model.Summary.endDate_6).ToString("dd MMM yyyy"));

                start_time.Add(Convert.ToDateTime(model.Summary.startDate_6).ToString("hh:mm tt"));
                end_time.Add(Convert.ToDateTime(model.Summary.endDate_6).ToString("hh:mm tt"));

                destination.Add(model.Summary.destination_6);
                from.Add(model.Summary.destination_5);
                count++;
            }
            if (model.StartSettlement.HasValue)
            {
                start_date.Add(Convert.ToDateTime(model.StartSettlement).ToString("dd MMM yyyy"));
                end_date.Add(Convert.ToDateTime(model.EndSettlement).ToString("dd MMM yyyy"));

                start_time.Add(Convert.ToDateTime(model.StartSettlement).ToString("hh:mm tt"));
                end_time.Add(Convert.ToDateTime(model.EndSettlement).ToString("hh:mm tt"));

                destination.Add("Extend");
                from.Add(destination[count - 1]);
            }

            //gfx.DrawRectangle(XBrushes.GhostWhite, 50, 735, 495, 63);

            XImage TAM = XImage.FromFile(TAMPath);
            gfx.DrawImage(TAM, 60, 50, 40, 40);

            //XImage contrast = XImage.FromFile(ContrastPath);
            //gfx.DrawImage(contrast, 60, 735);

            XImage paid = XImage.FromFile(Paid);
            gfx.DrawImage(paid, 60, 675, 150, 150);

            gfx.DrawString(PT_TAM, head, XBrushes.Black, 105, 60, XStringFormats.TopLeft);
            gfx.DrawRectangle(XBrushes.GhostWhite, 377, 55, 168, 28);
            gfx.DrawLine(POLine, 377, 55, 377, 83);

            gfx.DrawString(receipt, head, XBrushes.Gray, 385, 60, XStringFormats.TopLeft);

            gfx.DrawLine(header_line, 50, 95, 545, 95);

            gfx.DrawString(add1, address, XBrushes.Black, 60, 105, XStringFormats.TopLeft);
            gfx.DrawString(add2, address, XBrushes.Black, 60, 120, XStringFormats.TopLeft);
            gfx.DrawString(add3, address, XBrushes.Black, 60, 135, XStringFormats.TopLeft);

            gfx.DrawString("BTA", address, XBrushes.Black, 385, 105, XStringFormats.TopLeft);
            gfx.DrawString("NOREG", address, XBrushes.Black, 385, 120, XStringFormats.TopLeft);
            gfx.DrawString("NAME", address, XBrushes.Black, 385, 135, XStringFormats.TopLeft);

            gfx.DrawString(":", address, XBrushes.Black, 425, 105, XStringFormats.TopLeft);
            gfx.DrawString(":", address, XBrushes.Black, 425, 120, XStringFormats.TopLeft);
            gfx.DrawString(":", address, XBrushes.Black, 425, 135, XStringFormats.TopLeft);

            gfx.DrawString(btr, address, XBrushes.Black, 435, 105, XStringFormats.TopLeft);
            gfx.DrawString(noreg, address, XBrushes.Black, 435, 120, XStringFormats.TopLeft);
            gfx.DrawString(name, address, XBrushes.Black, 435, 135, XStringFormats.TopLeft);

            gfx.DrawLine(body_line, 75, 200, 520, 200);

            gfx.DrawString("DETAIL TRAVEL", title, XBrushes.Black, 85, 207, XStringFormats.TopLeft);

            gfx.DrawLine(body_line, 75, 230, 520, 230);

            int gap_now = 0;
            x_pad = 30;

            gfx.DrawString("From", body, XBrushes.Black, 85, 240, XStringFormats.TopLeft);
            gfx.DrawString("To", body, XBrushes.Black, 180, 240, XStringFormats.TopLeft);

            gfx.DrawString("Depart", body, XBrushes.Black, 280 + x_pad, 240, XStringFormats.TopLeft);
            gfx.DrawString("Return", body, XBrushes.Black, 385 + x_pad, 240, XStringFormats.TopLeft);

            int i = 0;
            foreach (var item in start_date)
            {
                gap_now = (i + 1) * 15;
                gfx.DrawString(from[i], body, XBrushes.Black, 85, 240 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(destination[i], body, XBrushes.Black, 180, 240 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(start_date[i], body, XBrushes.Black, 280 + x_pad, 240 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(start_time[i], body, XBrushes.Black, 335 + x_pad, 240 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(end_date[i], body, XBrushes.Black, 385 + x_pad, 240 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(end_time[i], body, XBrushes.Black, 440 + x_pad, 240 + gap_now, XStringFormats.TopLeft);
                i++;
            }
            int xItem = 120;
            int xSuspense = 260;
            int xReimburse = 400;


            gfx.DrawLine(body_line, 75, 280 + gap_now, 520, 280 + gap_now);

            gfx.DrawString("ITEM", title, XBrushes.Black, xItem, 287 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString("SUSPENSE", title, XBrushes.Black, xSuspense, 287 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString("REIMBURSE", title, XBrushes.Black, xReimburse, 287 + gap_now, XStringFormats.TopLeft);
            gfx.DrawLine(body_line, 75, 310 + gap_now, 520, 310 + gap_now);

            // BUDGET DETAIL PER ITEM
            XTextFormatter tx = new XTextFormatter(gfx);
            XTextFormatter rx = new XTextFormatter(gfx);

            tx.Alignment = XParagraphAlignment.Right;
            rx.Alignment = XParagraphAlignment.Left;

            gfx.DrawRectangle(XBrushes.GhostWhite, 85 - 3, 320 + gap_now, 432, 20);
            gfx.DrawLine(POLine, 85, 320 + gap_now, 85 - 3, 340 + gap_now);

            gfx.DrawRectangle(XBrushes.GhostWhite, 85 - 3, 360 + gap_now, 432, 20);
            gfx.DrawLine(POLine, 85, 360 + gap_now, 85 - 3, 380 + gap_now);

            gfx.DrawRectangle(XBrushes.GhostWhite, 85 - 3, 400 + gap_now, 432, 20);
            gfx.DrawLine(POLine, 85, 400 + gap_now, 85 - 3, 420 + gap_now);

            gfx.DrawRectangle(XBrushes.LightCyan, 85 - 3, 450 + gap_now, 432, 20);
            gfx.DrawLine(POLine, 85, 450 + gap_now, 85 - 3, 470 + gap_now);

            gfx.DrawRectangle(XBrushes.LightCyan, 85 - 3, 516 + gap_now, 432, 34);
            gfx.DrawLine(POLine, 85, 516 + gap_now, 85 - 3, 550 + gap_now);


            XRect rect1 = new XRect(xItem + 5, 323 + gap_now, 200, 20);
            XRect rect1s = new XRect(xItem + 205, 323 + gap_now, 150, 20);

            rx.DrawString("Meal", body_title, XBrushes.DarkBlue, rect1);
            tx.DrawString("Rp. " + model.Summary.total_meal.ToString("N"), body, XBrushes.Black, rect1);
            tx.DrawString("Rp. " + model.MealSettlement.ToString("N"), body, XBrushes.Black, rect1s);


            XRect rect2 = new XRect(xItem + 5, 343 + gap_now, 200, 20);
            XRect rect2s = new XRect(xItem + 205, 343 + gap_now, 150, 20);

            rx.DrawString("Hotel", body_title, XBrushes.DarkBlue, rect2);
            tx.DrawString("Rp. " + model.Summary.total_hotel.ToString("N"), body, XBrushes.Black, rect2);
            tx.DrawString("Rp. " + model.HotelSettlement.ToString("N"), body, XBrushes.Black, rect2s);



            XRect rect3 = new XRect(xItem + 5, 363 + gap_now, 200, 20);
            XRect rect3s = new XRect(xItem + 205, 363 + gap_now, 150, 20);

            rx.DrawString("Ticket", body_title, XBrushes.DarkBlue, rect3);
            tx.DrawString("Rp. " + model.Summary.total_ticket.ToString("N"), body, XBrushes.Black, rect3);
            tx.DrawString("Rp. " + model.TicketSettlement.ToString("N"), body, XBrushes.Black, rect3s);


            XRect rect4 = new XRect(xItem + 5, 383 + gap_now, 200, 20);
            XRect rect4s = new XRect(xItem + 205, 383 + gap_now, 150, 20);

            rx.DrawString("Land Transport", body_title, XBrushes.DarkBlue, rect4);
            tx.DrawString("Rp. 0.00", body, XBrushes.Black, rect4);
            tx.DrawString("Rp. " + model.Summary.total_transportation.ToString("N"), body, XBrushes.Black, rect4s);

            XRect rect5 = new XRect(xItem + 5, 403 + gap_now, 200, 20);
            XRect rect5s = new XRect(xItem + 205, 403 + gap_now, 150, 20);

            rx.DrawString("Laundry", body_title, XBrushes.DarkBlue, rect5);
            tx.DrawString("Rp. 0.00", body, XBrushes.Black, rect5);
            tx.DrawString("Rp. " + model.Summary.total_laundry.ToString("N"), body, XBrushes.Black, rect5s);

            XRect rect6 = new XRect(xItem + 5, 423 + gap_now, 200, 20);
            XRect rect6s = new XRect(xItem + 205, 423 + gap_now, 150, 20);
            rx.DrawString("Misc", body_title, XBrushes.DarkBlue, rect6);
            tx.DrawString("Rp. 0.00", body, XBrushes.Black, rect6);
            tx.DrawString("Rp. " + model.Summary.total_miscellaneous.ToString("N"), body, XBrushes.Black, rect6s);



            // Perhitungan Total
            total_Actual = model.Summary.total_meal + model.Summary.total_ticket + model.Summary.total_hotel;
            int t_Reimburse = Convert.ToInt32(model.MealSettlement + model.HotelSettlement + model.TicketSettlement);
            total_Reimburse = model.Summary.total_laundry + model.Summary.total_transportation + model.Summary.total_miscellaneous + t_Reimburse;


            XRect rect7 = new XRect(xItem + 5, 454 + gap_now, 200, 20);
            XRect rect7s = new XRect(xItem + 205, 454 + gap_now, 150, 20);
            rx.DrawString("TOTAL", title, XBrushes.DarkBlue, rect7);
            tx.DrawString("Rp. " + total_Actual.ToString("N"), body, XBrushes.Black, rect7);
            tx.DrawString("Rp. " + total_Reimburse.ToString("N"), body, XBrushes.Black, rect7s);

            XRect rect9 = new XRect(xItem + 5, 526 + gap_now, 350, 20);
            rx.DrawString("GRAND TOTAL", titleBold, XBrushes.DarkBlue, rect9);
            tx.DrawString("Rp. " + Convert.ToInt32(model.Summary.grand_total_settlement).ToString("N"), bodyBold, XBrushes.Black, rect9);


            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;
            return File(stream, "application/pdf", receipt.Replace(" ", "_") + "_" + model.Summary.group_code.Trim(' ') + "_" + DateTime.Now.ToString("yyMMdd-hh-mm-tt") + ".pdf");
        }

    }
}
