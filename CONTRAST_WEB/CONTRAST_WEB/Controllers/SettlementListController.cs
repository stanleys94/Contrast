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

            model.MealSettlement = (float)meal_platform.meal_allowance * Convert.ToInt32(duration.Days);

            //ModelState.Remove(ModelState.FirstOrDefault(m => m.Key.ToString().StartsWith("MealSettlement")));
            return (model);
        }

        // GET: SettlementList
        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(tb_m_employee model)
        {
            ViewBag.Employee = model;

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
        public async Task<ActionResult> Insert(SettlementHelper model, string sum, string insert)
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
                    ActualCostObject.vendor_code = model.TravelRequest.no_reg.ToString();
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

                    if (model.MealSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.MealSettlement;
                        ActualCostObject.jenis_transaksi = "Meal";
                        await InsertData.ActualCost(ActualCostObject);
                    }

                    if (model.HotelSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.HotelSettlement;
                        ActualCostObject.jenis_transaksi = "Hotel";
                        await InsertData.ActualCost(ActualCostObject);
                    }

                    if (model.TicketSettlement > 0)
                    {

                        ActualCostObject.amount = (int)model.TicketSettlement;
                        ActualCostObject.jenis_transaksi = "Ticket";
                        await InsertData.ActualCost(ActualCostObject);
                    }

                    if (model.LaundrySettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.LaundrySettlement;
                        ActualCostObject.jenis_transaksi = "Laundry";
                        ActualCostObject.path_file = Utility.UploadSettlementReceipt(model.ReceiptFileLaundry, "Laundry_" + ActualCostObject.no_reg + "_" + "_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                        await InsertData.ActualCost(ActualCostObject);
                    }

                    if (model.TransportationSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.TransportationSettlement;
                        ActualCostObject.jenis_transaksi = "Transportation";
                        ActualCostObject.path_file = Utility.UploadSettlementReceipt(model.ReceiptFileTransportation, "Transport_" + ActualCostObject.no_reg + "_" + "_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                        await InsertData.ActualCost(ActualCostObject);
                    }

                    if (model.MiscSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.MiscSettlement;
                        ActualCostObject.jenis_transaksi = "Miscellaneous";
                        ActualCostObject.path_file = Utility.UploadSettlementReceipt(model.ReceiptFileOther, "Other_" + ActualCostObject.no_reg + "_" + "_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                        await InsertData.ActualCost(ActualCostObject);
                    }

                    //update data
                    vw_summary_settlement SummarySettlementObject = new vw_summary_settlement();
                    SummarySettlementObject = await GetData.SummarySettlementInfo(ActualCostObject.group_code);

                    if (model.MealSettlement == 0 && model.PreparationSettlement == 0 && model.HotelSettlement == 0 && model.TicketSettlement == 0 && model.LaundrySettlement == 0 && model.TransportationSettlement == 0 && model.MiscSettlement == 0)
                        await UpdateData.TravelRequest(ActualCostObject.group_code, "1");
                    else
                        await UpdateData.TravelRequest(ActualCostObject.group_code, "0");

                    //if (!model.extend_flag)
                    return View("SummaryPaid", SummarySettlementObject);
                    //else
                    //    return View("SummaryAdditional", SummarySettlementObject);
                    //*/
                    //return View("SummaryPaid", model);

                }
                else
                if (sum != null)
                {
                    SettlementHelper return_model = await Sum(model);
                    ModelState.Remove(ModelState.FirstOrDefault(m => m.Key.ToString().StartsWith("MealSettlement")));
                    return View("Details", return_model);
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
                temp.comment = item.comment;
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


            //return View(ResponseList);

            var headers = Request.Headers.GetValues("User-Agent");
            string userAgent = string.Join(" ", headers);

            if (userAgent.ToLower().Contains("ipad"))
                return View("IndexMSTR", ResponseList);
            else
                return View("IndexMSTRMobile", ResponseList);

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


                    tb_r_travel_actualcost ActualCostObject = new tb_r_travel_actualcost();
                    ActualCostObject.information_actualcost = "Settlement";
                    ActualCostObject.end_date_extend = model.End_Extend;
                    ActualCostObject.start_date_extend = model.Start_Extend;
                    ActualCostObject.user_created = model.TravelRequest.no_reg.ToString();
                    //ActualCostObject.vendor_code = model.TravelRequest.no_reg.ToString();
                    var vendor_code = await GetData.VendorEmployee(model.TravelRequest.no_reg);
                    ActualCostObject.vendor_code = vendor_code.ToString();
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

                    if (model.MealSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.MealSettlement;
                        ActualCostObject.jenis_transaksi = "Meal";
                        await InsertData.ActualCost(ActualCostObject);
                    }

                    if (model.HotelSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.HotelSettlement;
                        ActualCostObject.jenis_transaksi = "Hotel";
                        await InsertData.ActualCost(ActualCostObject);
                    }

                    if (model.TicketSettlement > 0)
                    {

                        ActualCostObject.amount = (int)model.TicketSettlement;
                        ActualCostObject.jenis_transaksi = "Ticket";
                        await InsertData.ActualCost(ActualCostObject);
                    }

                    if (model.LaundrySettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.LaundrySettlement;
                        ActualCostObject.jenis_transaksi = "Laundry";
                        ActualCostObject.path_file = Utility.UploadSettlementReceipt(model.ReceiptFileLaundry, "Laundry_" + ActualCostObject.no_reg + "_" + "_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                        await InsertData.ActualCost(ActualCostObject);
                    }

                    if (model.TransportationSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.TransportationSettlement;
                        ActualCostObject.jenis_transaksi = "Transportation";
                        ActualCostObject.path_file = Utility.UploadSettlementReceipt(model.ReceiptFileTransportation, "Transport_" + ActualCostObject.no_reg + "_" + "_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                        await InsertData.ActualCost(ActualCostObject);
                    }

                    if (model.MiscSettlement > 0)
                    {
                        ActualCostObject.amount = (int)model.MiscSettlement;
                        ActualCostObject.jenis_transaksi = "Miscellaneous";
                        ActualCostObject.path_file = Utility.UploadSettlementReceipt(model.ReceiptFileOther, "Other_" + ActualCostObject.no_reg + "_" + "_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                        await InsertData.ActualCost(ActualCostObject);
                    }

                    //update data
                    vw_summary_settlement SummarySettlementObject = new vw_summary_settlement();
                    SummarySettlementObject = await GetData.SummarySettlementInfo(ActualCostObject.group_code);

                    if (model.MealSettlement == 0 && model.PreparationSettlement == 0 && model.HotelSettlement == 0 && model.TicketSettlement == 0 && model.LaundrySettlement == 0 && model.TransportationSettlement == 0 && model.MiscSettlement == 0)
                        await UpdateData.TravelRequest(ActualCostObject.group_code, "1");
                    else
                        await UpdateData.TravelRequest(ActualCostObject.group_code, "0");

                    //if (!model.extend_flag)
                    return View("SummaryPaidMSTR", SummarySettlementObject);
                    //else
                    //    return View("SummaryAdditional", SummarySettlementObject);
                    //*/
                    //return View("SummaryPaid", model);

                }
                else
                if (sum != null)
                {
                    SettlementHelper return_model = await Sum(model);
                    ModelState.Remove(ModelState.FirstOrDefault(m => m.Key.ToString().StartsWith("MealSettlement")));
                    return View("DetailsMSTR", return_model);
                }
                else
                    return View("DetailsMSTR", model);
            }
            else
                return View("DetailsMSTR", model);
        }

        public async Task<ActionResult> DetailsMSTR(vw_travel_for_settlement model)
        {
            SettlementHelper Settlement = new SettlementHelper();
            Settlement.TravelRequest = model;
            Settlement.extend_flag = true;
            Response.Cache.SetNoStore();
            return View("DetailsMSTR", Settlement);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Print(vw_summary_settlement model)
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
            XFont body_title = new XFont("Calibri", 11, XFontStyle.Regular);
            XFont total = new XFont("Lucida Grande", 11, XFontStyle.Bold);
            XFont title = new XFont("Lucida Grande", 12, XFontStyle.Regular);
            XFont credit = new XFont("Lucida Sans", 9, XFontStyle.Regular);

            XPen header_line = new XPen(XColors.Black, 2);
            XPen body_line = new XPen(XColors.DimGray, 0.5);
            XPen POLine = new XPen(XColors.SteelBlue, 3);

            string PT_TAM = "PT. TOYOTA ASTRA MOTOR";
            string receipt = "SETTLEMENT RECEIPT";
            string add1 = "Jl. Laks. Yos Sudarso, Sunter II";
            string add2 = "Jakarta Utara - Indonesia";
            string add3 = "Phone :62-21 - 6515551(Hunting)";

            string btr = model.group_code;
            string name = model.emp_name.ToUpper();
            string noreg = model.no_reg.ToString();
            string TAMPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "tam_logo.PNG");
            string Paid = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "receipt-paid4.png");
            string ContrastPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "small_logo_horizontal.png");

            // string date = model.invoice.start_date.ToString();



            string[] start_date = new string[6];
            string[] start_time = new string[6];

            string[] end_date = new string[6];
            string[] end_time = new string[6];

            string[] destination = new string[6];
            string[] from = new string[6];

            int x_pad_max = 0;
            int x_pad = 0;
            int y_pad = 0;
            int y_pad_max = 0;
            int legend = 0;

            if (model.startDate_6.HasValue)
            {
                start_date[5] = Convert.ToDateTime(model.startDate_6).ToString("dd MMM yyyy");
                end_date[5] = Convert.ToDateTime(model.endDate_6).ToString("dd MMM yyyy");

                start_time[5] = Convert.ToDateTime(model.startDate_6).ToString("hh:mm tt");
                end_time[5] = Convert.ToDateTime(model.endDate_6).ToString("hh:mm tt");

                destination[5] = model.destination_6;
                from[5] = model.destination_5;
            }

            if (model.startDate_5.HasValue)
            {
                start_date[4] = Convert.ToDateTime(model.startDate_5).ToString("dd MMM yyyy");
                end_date[4] = Convert.ToDateTime(model.endDate_5).ToString("dd MMM yyyy");

                start_time[4] = Convert.ToDateTime(model.startDate_5).ToString("hh:mm tt");
                end_time[4] = Convert.ToDateTime(model.endDate_5).ToString("hh:mm tt");

                destination[4] = model.destination_5;
                from[4] = model.destination_4;
            }

            if (model.startDate_4.HasValue)
            {
                start_date[3] = Convert.ToDateTime(model.startDate_4).ToString("dd MMM yyyy");
                end_date[3] = Convert.ToDateTime(model.endDate_4).ToString("dd MMM yyyy");

                start_time[3] = Convert.ToDateTime(model.startDate_4).ToString("hh:mm tt");
                end_time[3] = Convert.ToDateTime(model.endDate_4).ToString("hh:mm tt");

                destination[3] = model.destination_4;
                from[3] = model.destination_3;
            }

            if (model.startDate_3.HasValue)
            {
                start_date[2] = Convert.ToDateTime(model.startDate_3).ToString("dd MMM yyyy");
                end_date[2] = Convert.ToDateTime(model.endDate_3).ToString("dd MMM yyyy");

                start_time[2] = Convert.ToDateTime(model.startDate_3).ToString("hh:mm tt");
                end_time[2] = Convert.ToDateTime(model.endDate_3).ToString("hh:mm tt");

                destination[2] = model.destination_3;
                from[2] = model.destination_2;
            }

            if (model.startDate_2.HasValue)
            {
                start_date[1] = Convert.ToDateTime(model.startDate_2).ToString("dd MMM yyyy");
                end_date[1] = Convert.ToDateTime(model.endDate_2).ToString("dd MMM yyyy");

                start_time[1] = Convert.ToDateTime(model.startDate_2).ToString("hh:mm tt");
                end_time[1] = Convert.ToDateTime(model.endDate_2).ToString("hh:mm tt");

                destination[1] = model.destination_2;
                from[1] = model.destination_1;
            }

            if (model.startDate_1.HasValue)
            {
                start_date[0] = Convert.ToDateTime(model.startDate_1).ToString("dd MMM yyyy");
                end_date[0] = Convert.ToDateTime(model.endDate_1).ToString("dd MMM yyyy");

                start_time[0] = Convert.ToDateTime(model.startDate_1).ToString("hh:mm tt");
                end_time[0] = Convert.ToDateTime(model.endDate_1).ToString("hh:mm tt");

                destination[0] = model.destination_1;
                from[0] = "Jakarta";
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

            gfx.DrawString("Depart", body, XBrushes.Black, 285 + x_pad, 240, XStringFormats.TopLeft);
            gfx.DrawString("Return", body, XBrushes.Black, 395 + x_pad, 240, XStringFormats.TopLeft);

            int i = 0;
            foreach (var item in start_date)
            {
                gap_now = (i + 1) * 15;
                gfx.DrawString(from[i], body, XBrushes.Black, 85, 240 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(destination[i], body, XBrushes.Black, 180, 240 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(start_date[i], body, XBrushes.Black, 285 + x_pad, 240 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(start_time[i], body, XBrushes.Black, 340 + x_pad, 240 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(end_date[i], body, XBrushes.Black, 395 + x_pad, 240 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(end_time[i], body, XBrushes.Black, 450 + x_pad, 240 + gap_now, XStringFormats.TopLeft);

            }

            gfx.DrawLine(body_line, 75, 280 + gap_now, 520, 280 + gap_now);
            gfx.DrawString("DETAIL BUDGET", title, XBrushes.Black, 85, 287 + gap_now, XStringFormats.TopLeft);
            gfx.DrawLine(body_line, 75, 310 + gap_now, 520, 310 + gap_now);

            // BUDGET DETAIL PER ITEM

            gfx.DrawRectangle(XBrushes.GhostWhite, 82, 320 + gap_now, 210, 20);
            gfx.DrawLine(POLine, 85, 320 + gap_now, 82, 340 + gap_now);
            gfx.DrawString("MEAL", title, XBrushes.DarkBlue, 95, 323 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString("Rp. " + model.total_meal.ToString("N"), body, XBrushes.Black, 95, 343 + gap_now, XStringFormats.TopLeft);

            gfx.DrawRectangle(XBrushes.GhostWhite, 82, 365 + gap_now, 210, 20);
            gfx.DrawLine(POLine, 85, 365 + gap_now, 82, 385 + gap_now);
            gfx.DrawString("HOTEL", title, XBrushes.DarkBlue, 95, 368 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString("Rp. " + model.total_hotel.ToString("N"), body, XBrushes.Black, 95, 388 + gap_now, XStringFormats.TopLeft);

            gfx.DrawRectangle(XBrushes.GhostWhite, 82, 410 + gap_now, 210, 20);
            gfx.DrawLine(POLine, 85, 410 + gap_now, 82, 430 + gap_now);
            gfx.DrawString("TICKET", title, XBrushes.DarkBlue, 95, 413 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString("Rp. " + model.total_ticket.ToString("N"), body, XBrushes.Black, 95, 433 + gap_now, XStringFormats.TopLeft);

            gfx.DrawRectangle(XBrushes.GhostWhite, 302, 320 + gap_now, 210, 20);
            gfx.DrawLine(POLine, 305, 320 + gap_now, 302, 340 + gap_now);
            gfx.DrawString("LAND TRANSPORTATION", title, XBrushes.DarkBlue, 315, 323 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString("Rp. " + model.total_transportation.ToString("N"), body, XBrushes.Black, 315, 343 + gap_now, XStringFormats.TopLeft);

            gfx.DrawRectangle(XBrushes.GhostWhite, 302, 365 + gap_now, 210, 20);
            gfx.DrawLine(POLine, 305, 365 + gap_now, 302, 385 + gap_now);
            gfx.DrawString("LAUNDRY", title, XBrushes.DarkBlue, 315, 368 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString("Rp. " + model.total_laundry.ToString("N"), body, XBrushes.Black, 315, 388 + gap_now, XStringFormats.TopLeft);

            gfx.DrawRectangle(XBrushes.GhostWhite, 302, 410 + gap_now, 210, 20);
            gfx.DrawLine(POLine, 305, 410 + gap_now, 302, 430 + gap_now);
            gfx.DrawString("OTHER", title, XBrushes.DarkBlue, 315, 413 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString("Rp. " + model.total_miscellaneous.ToString("N"), body, XBrushes.Black, 315, 433 + gap_now, XStringFormats.TopLeft);

            gap_now = gap_now + 20;
            gfx.DrawRectangle(XBrushes.LightCyan, 82, 450 + gap_now, 428, 50);
            gfx.DrawLine(POLine, 85, 450 + gap_now, 82, 500 + gap_now);
            gfx.DrawString("TOTAL BUDGET", title, XBrushes.DarkBlue, 85 + 10, 457 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString("Rp. " + Convert.ToInt32(model.grand_total_settlement).ToString("N"), body, XBrushes.Black, 85 + 10, 480 + gap_now, XStringFormats.TopLeft);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;
            return File(stream, "application/pdf", receipt.Replace(" ", "_") + "_" + model.group_code.Trim(' ') + "_" + DateTime.Now.ToString("yyMMdd-hh-mm-tt") + ".pdf");
        }

    }
}
