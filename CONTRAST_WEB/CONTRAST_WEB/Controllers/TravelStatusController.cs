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
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp;
using System.Security.Claims;

namespace CONTRAST_WEB.Controllers
{
    public class TravelStatusController : Controller
    {
        [Authorize]
        [Authorize(Roles = "contrast.user")]        
        // GET: TravelStatus
        public async Task<ActionResult> Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;
            tb_m_employee model = await GetData.EmployeeInfo(identity.Name);

            List<vw_request_summary> ResponseList = new List<vw_request_summary>();
            ResponseList = await GetData.RequestSummaryListInfo(model.code);

            List<string> apprv_name = new List<string>();
            List<DateTime> return_date = new List<DateTime>();

            List<int> travel_duration = new List<int>();

            for (int k = 0; k < ResponseList.Count(); k++)
            {
                if (ResponseList[k].apprv_flag_lvl20 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl20));
                else

                if (ResponseList[k].apprv_flag_lvl19 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl19));
                else

                if (ResponseList[k].apprv_flag_lvl18 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl18));
                else

                if (ResponseList[k].apprv_flag_lvl17 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl17));
                else

                if (ResponseList[k].apprv_flag_lvl16 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl16));
                else

                if (ResponseList[k].apprv_flag_lvl15 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl15));
                else

                if (ResponseList[k].apprv_flag_lvl14 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl14));
                else

                if (ResponseList[k].apprv_flag_lvl13 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl13));
                else

                if (ResponseList[k].apprv_flag_lvl12 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl12));
                else

                if (ResponseList[k].apprv_flag_lvl11 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl11));
                else
                if (ResponseList[k].apprv_flag_lvl10 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl10));
                else
                if (ResponseList[k].apprv_flag_lvl9 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl9));
                else
                if (ResponseList[k].apprv_flag_lvl8 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl8));
                else
                if (ResponseList[k].apprv_flag_lvl7 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl7));
                else
                if (ResponseList[k].apprv_flag_lvl6 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl6));
                else
                if (ResponseList[k].apprv_flag_lvl5 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl5));
                else
                if (ResponseList[k].apprv_flag_lvl4 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl4));
                else
                if (ResponseList[k].apprv_flag_lvl3 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl3));
                else
                if (ResponseList[k].apprv_flag_lvl2 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl2));
                else
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl1));


                if (ResponseList[k].endDate_3 != null)
                    return_date.Add(ResponseList[k].endDate_3 ?? default(DateTime));
                else
                if (ResponseList[k].endDate_2 != null)
                    return_date.Add(ResponseList[k].endDate_2 ?? default(DateTime));
                else
                if (ResponseList[k].endDate_1 != null)
                    return_date.Add(ResponseList[k].endDate_1 ?? default(DateTime));


                travel_duration.Add((ResponseList[k].duration_1 ?? default(int)) + (ResponseList[k].duration_2 ?? default(int)) + (ResponseList[k].duration_3 ?? default(int)));
            }

            ViewBag.ReturnDate = return_date;
            ViewBag.Username = model.name;
            ViewBag.Bossname = apprv_name;
            ViewBag.Duration = travel_duration;

            return View(ResponseList);
        }

        
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        // Details: TravelStatustb_m_photo_employee
        public async Task<ActionResult> Details(vw_request_summary model)
        {
            TravelStatusHelper model2 = new TravelStatusHelper();
            model2.travel_request = model;
            model2.participants = await GetData.TravelRequestParticipant(model2.travel_request);

            List<string> ModelList = new List<string>();
            for (int i = 0; i < model2.participants.Count(); i++)
            {
                ModelList.Add(await GetData.EmployeeNameInfo(model2.participants[i].no_reg));
            }
            ViewBag.RL = ModelList;

            List<string> apprv_name = new List<string>();
            List<string> apprv_status = new List<string>();

            string temp = "";
            string ttemp = "";
            for (int k = 0; k < 19; k++)
            {
                temp = null;
                ttemp = null;
                if (k == 19)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl20);
                    ttemp = model2.travel_request.apprv_flag_lvl20;
                }
                else
                if (k == 18)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl19);
                    ttemp = model2.travel_request.apprv_flag_lvl19;
                }
                else
                if (k == 17)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl18);
                    ttemp = model2.travel_request.apprv_flag_lvl18;
                }
                else
                if (k == 16)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl17);
                    ttemp = model2.travel_request.apprv_flag_lvl17;
                }
                else
                if (k == 15)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl16);
                    ttemp = model2.travel_request.apprv_flag_lvl16;
                }
                else
                if (k == 14)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl15);
                    ttemp = model2.travel_request.apprv_flag_lvl15;
                }
                else
                if (k == 13)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl14);
                    ttemp = model2.travel_request.apprv_flag_lvl14;
                }
                else
                if (k == 12)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl13);
                    ttemp = model2.travel_request.apprv_flag_lvl13;
                }
                else
                if (k == 13)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl12);
                    ttemp = model2.travel_request.apprv_flag_lvl12;
                }
                else
                if (k == 12)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl13);
                    ttemp = model2.travel_request.apprv_flag_lvl13;
                }
                else
                if (k == 11)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl12);
                    ttemp = model2.travel_request.apprv_flag_lvl12;
                }
                else
                if (k == 10)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl11);
                    ttemp = model2.travel_request.apprv_flag_lvl11;
                }
                else
                if (k == 9)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl10);
                    ttemp = model2.travel_request.apprv_flag_lvl10;
                }
                else
                if (k == 8)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl9);
                    ttemp = model2.travel_request.apprv_flag_lvl9;
                }
                else
                if (k == 7)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl8);
                    ttemp = model2.travel_request.apprv_flag_lvl8;
                }
                else
                if (k == 6)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl7);
                    ttemp = model2.travel_request.apprv_flag_lvl7;
                }
                else
                if (k == 5)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl6);
                    ttemp = model2.travel_request.apprv_flag_lvl6;
                }
                else
                if (k == 4)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl5);
                    ttemp = model2.travel_request.apprv_flag_lvl5;
                }
                else
                if (k == 3)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl4);
                    ttemp = model2.travel_request.apprv_flag_lvl4;
                }
                else
                if (k == 2)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl3);
                    ttemp = model2.travel_request.apprv_flag_lvl3;
                }
                else
                if (k == 1)
                {
                    //if (model2.travel_request.apprv_flag_lvl2 != null) 
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl2);
                    ttemp = model2.travel_request.apprv_flag_lvl2;
                }
                else
                if (k == 0)
                {
                    //if (model2.travel_request.apprv_flag_lvl1 != null) 
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl1);
                    ttemp = model2.travel_request.apprv_flag_lvl1;
                }

                if (temp != null)
                {
                    apprv_name.Add(temp);
                    apprv_status.Add(ttemp);
                }
            }
            ViewBag.Bossname = apprv_name;
            ViewBag.StatusState = apprv_status;
            ViewBag.Approvalnum = apprv_status.Count;

            return View(model2);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Drop(TravelStatusHelper model, string download = "", string drop = "")
        {
            if (drop == "submit")
            {
                tb_r_travel_request model2 = new tb_r_travel_request();
                List<vw_request_summary> request = new List<vw_request_summary>();
                List<vw_request_summary> ResponseList = new List<vw_request_summary>();

                model2 = await GetData.TravelRequest(Convert.ToInt32(model.travel_request.id_request));
                model2.active_flag = true;
                model2.status_request = "99";

                double budget = Convert.ToDouble(model2.allowance_meal_idr + model2.allowance_preparation + model2.allowance_winter);

                await UpdateData.TravelRequestPersonal(model2);

                request = await GetData.RequestSummaryListInfo(model2.no_reg.ToString());

                foreach (var item in request)
                {
                    if (item.status_request != "99") ResponseList.Add(item);
                }

                string name = await GetData.EmployeeNameInfo(model2.no_reg);


                List<DateTime> return_date = new List<DateTime>();
                List<int> travel_duration = new List<int>();

                List<string> apprv_name = new List<string>();

                for (int k = 0; k < ResponseList.Count(); k++)
                {
                    if (ResponseList[k].apprv_flag_lvl5 != null)
                        apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl5));
                    else
                    if (ResponseList[k].apprv_flag_lvl4 != null)
                        apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl4));
                    else
                    if (ResponseList[k].apprv_flag_lvl3 != null)
                        apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl3));
                    else
                    if (ResponseList[k].apprv_flag_lvl2 != null)
                        apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl2));
                    else
                        apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl1));


                    if (ResponseList[k].endDate_3 != null)
                        return_date.Add(ResponseList[k].endDate_3 ?? default(DateTime));
                    else
                    if (ResponseList[k].endDate_2 != null)
                        return_date.Add(ResponseList[k].endDate_2 ?? default(DateTime));
                    else
                    if (ResponseList[k].endDate_1 != null)
                        return_date.Add(ResponseList[k].endDate_1 ?? default(DateTime));


                    travel_duration.Add((ResponseList[k].duration_1 ?? default(int)) + (ResponseList[k].duration_2 ?? default(int)) + (ResponseList[k].duration_3 ?? default(int)));
                }

                ViewBag.ReturnDate = return_date;
                //ViewBag.Username = model.employee_info.name;
                ViewBag.Bossname = apprv_name;
                ViewBag.Duration = travel_duration;

                ModelState.Clear();
                if (ResponseList.Count > 0) return View("Index", ResponseList.OrderBy(r => r.status_request).ToList());
                else
                {
                    return View("Index", ResponseList);
                }
            }
            else if (download == "download")
            {
                TravelStatusHelper model2 = new TravelStatusHelper();
                model2.travel_request = await GetData.RequestSummaryInfo(model.travel_request.group_code);
                model2.employee_info = new tb_m_employee();
                model2.employee_info.code = model2.travel_request.no_reg.ToString();
                model2.employee_info = await GetData.EmployeeInfo(model2.employee_info);
                model2.participants = await GetData.TravelRequestParticipant(model2.travel_request);

                List<string> Approval_Name = new List<string>();
                List<string> Approval_Status = new List<string>();
                string bossname = "";

                if (model2.travel_request.apprv_by_lvl1 != null)
                {
                    bossname = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl1);
                    Approval_Name.Add(bossname);
                    if (model2.travel_request.apprv_flag_lvl1 == null) Approval_Status.Add("Not Yet");
                    else if (model2.travel_request.apprv_flag_lvl1.Contains("0")) Approval_Status.Add("Pending");
                    else if (model2.travel_request.apprv_flag_lvl1.Contains("1")) Approval_Status.Add("Approved");
                    else if (model2.travel_request.apprv_flag_lvl1.Contains("2")) Approval_Status.Add("Rejected");
                }

                if (model2.travel_request.apprv_by_lvl2 != null)
                {
                    bossname = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl2);
                    Approval_Name.Add(bossname);
                    if (model2.travel_request.apprv_flag_lvl2 == null) Approval_Status.Add("Not Yet");
                    else if (model2.travel_request.apprv_flag_lvl2.Contains("0")) Approval_Status.Add("Pending");
                    else if (model2.travel_request.apprv_flag_lvl2.Contains("1")) Approval_Status.Add("Approved");
                    else if (model2.travel_request.apprv_flag_lvl2.Contains("2")) Approval_Status.Add("Rejected");
                }

                if (model2.travel_request.apprv_by_lvl3 != null)
                {
                    bossname = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl3);
                    Approval_Name.Add(bossname);
                    if (model2.travel_request.apprv_flag_lvl3 == null) Approval_Status.Add("Not Yet");
                    else if (model2.travel_request.apprv_flag_lvl3.Contains("0")) Approval_Status.Add("Pending");
                    else if (model2.travel_request.apprv_flag_lvl3.Contains("1")) Approval_Status.Add("Approved");
                    else if (model2.travel_request.apprv_flag_lvl3.Contains("2")) Approval_Status.Add("Rejected");
                }

                if (model2.travel_request.apprv_by_lvl4 != null)
                {
                    bossname = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl4);
                    Approval_Name.Add(bossname);
                    if (model2.travel_request.apprv_flag_lvl4 == null) Approval_Status.Add("Not Yet");
                    else if (model2.travel_request.apprv_flag_lvl4.Contains("0")) Approval_Status.Add("Pending");
                    else if (model2.travel_request.apprv_flag_lvl4.Contains("1")) Approval_Status.Add("Approved");
                    else if (model2.travel_request.apprv_flag_lvl4.Contains("2")) Approval_Status.Add("Rejected");
                }

                if (model2.travel_request.apprv_by_lvl5 != null)
                {
                    bossname = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl5);
                    Approval_Name.Add(bossname);
                    if (model2.travel_request.apprv_flag_lvl5 == null) Approval_Status.Add("Not Yet");
                    else if (model2.travel_request.apprv_flag_lvl5.Contains("0")) Approval_Status.Add("Pending");
                    else if (model2.travel_request.apprv_flag_lvl5.Contains("1")) Approval_Status.Add("Approved");
                    else if (model2.travel_request.apprv_flag_lvl5.Contains("2")) Approval_Status.Add("Rejected");
                }

                if (model2.travel_request.apprv_by_lvl6 != null)
                {
                    bossname = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl6);
                    Approval_Name.Add(bossname);
                    if (model2.travel_request.apprv_flag_lvl6 == null) Approval_Status.Add("Not Yet");
                    else if (model2.travel_request.apprv_flag_lvl6.Contains("0")) Approval_Status.Add("Pending");
                    else if (model2.travel_request.apprv_flag_lvl6.Contains("1")) Approval_Status.Add("Approved");
                    else if (model2.travel_request.apprv_flag_lvl6.Contains("2")) Approval_Status.Add("Rejected");
                }

                if (model2.travel_request.apprv_by_lvl7 != null)
                {
                    bossname = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl7);
                    Approval_Name.Add(bossname);
                    if (model2.travel_request.apprv_flag_lvl7 == null) Approval_Status.Add("Not Yet");
                    else if (model2.travel_request.apprv_flag_lvl7.Contains("0")) Approval_Status.Add("Pending");
                    else if (model2.travel_request.apprv_flag_lvl7.Contains("1")) Approval_Status.Add("Approved");
                    else if (model2.travel_request.apprv_flag_lvl7.Contains("2")) Approval_Status.Add("Rejected");
                }

                PdfDocument document = new PdfDocument();
                document.Info.Title = "Business Travel Assignment";
                PdfPage page = document.AddPage();
                page.Orientation = PdfSharp.PageOrientation.Portrait;
                XSize size = PageSizeConverter.ToSize(PdfSharp.PageSize.A4);
                page.Height = size.Height;
                page.Width = size.Width;

                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont head = new XFont("Arial Narrow", 18, XFontStyle.Regular);
                XFont address = new XFont("Arial Narrow", 11, XFontStyle.Regular);
                XFont body = new XFont("Calibri", 9, XFontStyle.Regular);
                XFont total = new XFont("Lucida Grande", 11, XFontStyle.Bold);
                XFont title = new XFont("Lucida Grande", 11, XFontStyle.Bold);
                XFont subtitle = new XFont("Lucida Grande", 10, XFontStyle.Regular);
                XFont credit = new XFont("Lucida Sans", 9, XFontStyle.Regular);
                XFont valid = new XFont("Lucida Sans", 8, XFontStyle.Regular);

                XPen header_line = new XPen(XColors.Black, 2);
                XPen body_line = new XPen(XColors.DimGray, 0.5);
                XPen POLine = new XPen(XColors.SteelBlue, 3);

                string PT_TAM = "PT. TOYOTA ASTRA MOTOR";
                string receipt = "BTA DETAILS";


                string add1 = "Jl. Laks. Yos Sudarso, Sunter II";
                string add2 = "Jakarta Utara - Indonesia";
                string add3 = "Phone :62-21 - 6515551(Hunting)";

                string btr = model2.travel_request.group_code;
                string name = model2.employee_info.name.ToUpper();
                string noreg = model2.travel_request.no_reg.ToString();
                string TAMPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "tam_logo.PNG");
                string ContrastPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "small_logo_horizontal.png");
                // string date = model.invoice.start_date.ToString();

                List<string> start_date = new List<string>();
                List<string> start_time = new List<string>();
                List<string> end_date = new List<string>();
                List<string> end_time = new List<string>();
                List<string> hotel_ticket = new List<string>();
                List<int?> duration = new List<int?>();
                List<string> destination = new List<string>();
                List<string> overseas = new List<string>();
                List<int?> amount = new List<int?>();
                List<string> reason = new List<string>();
                int lenght = 0;
                int length_count = 0;
                string temp = "";
                List<int> index = new List<int>();
                List<int> range = new List<int>();
                int index_now = 0;
                int count_now = 1;

                if (model2.travel_request.reason_of_assigment.Length > 45)
                {
                    length_count = model2.travel_request.reason_of_assigment.Length / 45;
                    temp = model2.travel_request.reason_of_assigment;

                    for (int t = temp.IndexOf(" "); t > -1; t = temp.IndexOf(" ", t + 1))
                    {
                        index.Add(t);
                    }

                    for (int k = 0; k < index.Count; k++)
                    {
                        if (index[k] > count_now * 45)
                        {
                            range.Add(index[k]);
                            count_now++;
                        }
                    }
                    for (int i = 0; i <= range.Count(); i++)
                    {
                        int lo = 0;
                        if (i < range.Count())
                        {
                            lo = range[i] - index_now;
                        }
                        else lo = temp.Length - index_now;

                        reason.Add(temp.Substring(index_now, lo));
                        if (i < range.Count) index_now = range[i] + 1;
                    }
                }
                start_date.Add(Convert.ToDateTime(model2.travel_request.startDate_1).ToString("dd MMM yyyy"));
                start_time.Add(Convert.ToDateTime(model2.travel_request.startDate_1).ToString("hh:mm tt"));

                end_date.Add(Convert.ToDateTime(model2.travel_request.endDate_1).ToString("dd MMM yyyy"));
                end_time.Add(Convert.ToDateTime(model2.travel_request.endDate_1).ToString("hh:mm tt"));

                destination.Add(model2.travel_request.destination_1);
                tb_m_destination over = await GetData.DestinationCityInfo(model2.travel_request.destination_1);
                if (over.id_region < 4) overseas.Add("Overseas");
                else overseas.Add("Domestic");

                if (model2.travel_request.startDate_2 != null)
                {
                    start_date.Add(Convert.ToDateTime(model2.travel_request.startDate_2).ToString("dd MMM yyyy"));
                    start_time.Add(Convert.ToDateTime(model2.travel_request.startDate_2).ToString("hh:mm tt"));

                    end_date.Add(Convert.ToDateTime(model2.travel_request.endDate_2).ToString("dd MMM yyyy"));
                    end_time.Add(Convert.ToDateTime(model2.travel_request.endDate_2).ToString("hh:mm tt"));

                    destination.Add(model2.travel_request.destination_2);
                    tb_m_destination over2 = await GetData.DestinationCityInfo(model2.travel_request.destination_2);
                    if (over2.id_region < 4) overseas.Add("Overseas");
                    else overseas.Add("Domestic");
                }

                if (model2.travel_request.startDate_3 != null)
                {
                    start_date.Add(Convert.ToDateTime(model2.travel_request.startDate_3).ToString("dd MMM yyyy"));
                    start_time.Add(Convert.ToDateTime(model2.travel_request.startDate_3).ToString("hh:mm tt"));

                    end_date.Add(Convert.ToDateTime(model2.travel_request.endDate_3).ToString("dd MMM yyyy"));
                    end_time.Add(Convert.ToDateTime(model2.travel_request.endDate_3).ToString("hh:mm tt"));

                    destination.Add(model2.travel_request.destination_3);
                    tb_m_destination over2 = await GetData.DestinationCityInfo(model2.travel_request.destination_3);
                    if (over2.id_region < 4) overseas.Add("Overseas");
                    else overseas.Add("Domestic");

                }

                if (model2.travel_request.startDate_4 != null)
                {
                    start_date.Add(Convert.ToDateTime(model2.travel_request.startDate_4).ToString("dd MMM yyyy"));
                    start_time.Add(Convert.ToDateTime(model2.travel_request.startDate_4).ToString("hh:mm tt"));

                    end_date.Add(Convert.ToDateTime(model2.travel_request.endDate_4).ToString("dd MMM yyyy"));
                    end_time.Add(Convert.ToDateTime(model2.travel_request.endDate_4).ToString("hh:mm tt"));

                    destination.Add(model2.travel_request.destination_4);
                    tb_m_destination over2 = await GetData.DestinationCityInfo(model2.travel_request.destination_4);
                    if (over2.id_region < 4) overseas.Add("Overseas");
                    else overseas.Add("Domestic");

                }

                if (model2.travel_request.startDate_5 != null)
                {
                    start_date.Add(Convert.ToDateTime(model2.travel_request.startDate_5).ToString("dd MMM yyyy"));
                    start_time.Add(Convert.ToDateTime(model2.travel_request.startDate_5).ToString("hh:mm tt"));

                    end_date.Add(Convert.ToDateTime(model2.travel_request.endDate_5).ToString("dd MMM yyyy"));
                    end_time.Add(Convert.ToDateTime(model2.travel_request.endDate_5).ToString("hh:mm tt"));

                    destination.Add(model2.travel_request.destination_5);
                    tb_m_destination over2 = await GetData.DestinationCityInfo(model2.travel_request.destination_5);
                    if (over2.id_region < 4) overseas.Add("Overseas");
                    else overseas.Add("Domestic");

                }

                if (model2.travel_request.startDate_6 != null)
                {
                    start_date.Add(Convert.ToDateTime(model2.travel_request.startDate_6).ToString("dd MMM yyyy"));
                    start_time.Add(Convert.ToDateTime(model2.travel_request.startDate_6).ToString("hh:mm tt"));

                    end_date.Add(Convert.ToDateTime(model2.travel_request.endDate_6).ToString("dd MMM yyyy"));
                    end_time.Add(Convert.ToDateTime(model2.travel_request.endDate_6).ToString("hh:mm tt"));

                    destination.Add(model2.travel_request.destination_6);
                    tb_m_destination over2 = await GetData.DestinationCityInfo(model2.travel_request.destination_6);
                    if (over2.id_region < 4) overseas.Add("Overseas");
                    else overseas.Add("Domestic");

                }

                int x_pad = 5;
                int y_pad = 10;
                int legend = 50;


                gfx.DrawRectangle(XBrushes.GhostWhite, 50, 735, 495, 63);

                XImage TAM = XImage.FromFile(TAMPath);
                gfx.DrawImage(TAM, x_pad + 50, 50, 40, 40);

                XImage contrast = XImage.FromFile(ContrastPath);
                gfx.DrawImage(contrast, x_pad + 50, 735);

                gfx.DrawString(PT_TAM, head, XBrushes.Black, 95 + x_pad, 60, XStringFormats.TopLeft);
                gfx.DrawRectangle(XBrushes.GreenYellow, 377, 55, 168, 28);
                gfx.DrawLine(POLine, 377, 55, 377, 83);

                gfx.DrawString(receipt, head, XBrushes.Black, 385, 60, XStringFormats.TopLeft);


                gfx.DrawLine(header_line, 50, 95, 545, 95);

                gfx.DrawString(add1, address, XBrushes.Black, 50 + x_pad, 105, XStringFormats.TopLeft);
                gfx.DrawString(add2, address, XBrushes.Black, 50 + x_pad, 120, XStringFormats.TopLeft);
                gfx.DrawString(add3, address, XBrushes.Black, 50 + x_pad, 135, XStringFormats.TopLeft);

                gfx.DrawString("BTA", address, XBrushes.Black, 385, 105, XStringFormats.TopLeft);
                gfx.DrawString("NOREG", address, XBrushes.Black, 385, 120, XStringFormats.TopLeft);
                gfx.DrawString("NAME", address, XBrushes.Black, 385, 135, XStringFormats.TopLeft);

                gfx.DrawString(":", address, XBrushes.Black, 385 + legend - 10, 105, XStringFormats.TopLeft);
                gfx.DrawString(":", address, XBrushes.Black, 385 + legend - 10, 120, XStringFormats.TopLeft);
                gfx.DrawString(":", address, XBrushes.Black, 385 + legend - 10, 135, XStringFormats.TopLeft);

                gfx.DrawString(btr, address, XBrushes.Black, 385 + legend, 105, XStringFormats.TopLeft);
                gfx.DrawString(noreg, address, XBrushes.Black, 385 + legend, 120, XStringFormats.TopLeft);
                gfx.DrawString(name, address, XBrushes.Black, 385 + legend, 135, XStringFormats.TopLeft);

                gfx.DrawLine(body_line, 75, 200, 520, 200);

                gfx.DrawString("DESTINATION DETAILS", title, XBrushes.Black, 85, 205, XStringFormats.TopLeft);

                gfx.DrawLine(body_line, 75, 225, 520, 225);

                gfx.DrawString("From", subtitle, XBrushes.Black, 85, 235, XStringFormats.TopLeft);
                gfx.DrawString("To", subtitle, XBrushes.Black, 170, 235, XStringFormats.TopLeft);

                gfx.DrawString("Region", subtitle, XBrushes.Black, 255, 235, XStringFormats.TopLeft);

                gfx.DrawString("Depart", subtitle, XBrushes.Black, 315 + x_pad, 235, XStringFormats.TopLeft);
                gfx.DrawString("Return", subtitle, XBrushes.Black, 415 + x_pad, 235, XStringFormats.TopLeft);

                int gap_now = 0;
                string planned = "";
                string from = "Jakarta";
                for (int i = 0; i < start_date.Count(); i++)
                {

                    gap_now = (i + 1) * 12;

                    gfx.DrawString(from, body, XBrushes.Black, 85, 235 + gap_now, XStringFormats.TopLeft);
                    gfx.DrawString(destination[i], body, XBrushes.Black, 170, 235 + gap_now, XStringFormats.TopLeft);

                    gfx.DrawString(overseas[i], body, XBrushes.Black, 255, 235 + gap_now, XStringFormats.TopLeft);

                    gfx.DrawString(start_date[i] + " - " + start_time[i], body, XBrushes.Black, 315 + x_pad, 235 + gap_now, XStringFormats.TopLeft);
                    gfx.DrawString(end_date[i] + " - " + end_time[i], body, XBrushes.Black, 415 + x_pad, 235 + gap_now, XStringFormats.TopLeft);

                    from = destination[i];
                }

                gfx.DrawLine(body_line, 75, 280 + gap_now, 520, 280 + gap_now);

                gfx.DrawString("BUSINESS TRAVEL ASSIGNMENT DETAILS", title, XBrushes.Black, 85, 285 + gap_now, XStringFormats.TopLeft);
                gfx.DrawLine(body_line, 75, 305 + gap_now, 520, 305 + gap_now);

                gfx.DrawString("Assignment Purpose", subtitle, XBrushes.DarkBlue, 95, 320 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString("Assignment Reason", subtitle, XBrushes.DarkBlue, 95, 335 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(":", subtitle, XBrushes.Black, 250, 320 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(":", subtitle, XBrushes.Black, 250, 335 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(model2.travel_request.travel_purpose, subtitle, XBrushes.Black, 260, 320 + gap_now, XStringFormats.TopLeft);

                if (reason.Count() > 0)
                {
                    for (int i = 0; i < reason.Count(); i++)
                    {
                        gfx.DrawString(reason[i], subtitle, XBrushes.Black, 260, 335 + gap_now, XStringFormats.TopLeft);
                        gap_now = gap_now + 15;
                    }
                }
                else
                {
                    gfx.DrawString(model2.travel_request.reason_of_assigment, subtitle, XBrushes.Black, 260, 335 + gap_now, XStringFormats.TopLeft);
                    gap_now = gap_now + 15;
                }
                gap_now = gap_now - 15;
                gfx.DrawString("Assignment Type", subtitle, XBrushes.DarkBlue, 95, 350 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString("Renewal Passport", subtitle, XBrushes.DarkBlue, 95, 365 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString("Participant", subtitle, XBrushes.DarkBlue, 95, 380 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(":", subtitle, XBrushes.Black, 250, 350 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(":", subtitle, XBrushes.Black, 250, 365 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(":", subtitle, XBrushes.Black, 250, 380 + gap_now, XStringFormats.TopLeft);


                if (model2.travel_request.request_type == false) planned = "Unplanned";
                else planned = "Planned";
                gfx.DrawString(planned, subtitle, XBrushes.Black, 260, 350 + gap_now, XStringFormats.TopLeft);

                if (model2.travel_request.passport_flag == false)
                    gfx.DrawString("No", subtitle, XBrushes.Black, 260, 365 + gap_now, XStringFormats.TopLeft);
                else
                    gfx.DrawString("Yes", subtitle, XBrushes.Black, 260, 365 + gap_now, XStringFormats.TopLeft);

                if (model2.participants.Count() > 0)
                {
                    foreach (var part in model2.participants)
                    {
                        string namePart = await GetData.EmployeeNameInfo(part.no_reg);
                        gfx.DrawString(namePart, subtitle, XBrushes.Black, 260, 380 + gap_now, XStringFormats.TopLeft);
                        gap_now = gap_now + 15;
                    }
                }
                else
                {
                    gfx.DrawString("None", subtitle, XBrushes.Black, 260, 380 + gap_now, XStringFormats.TopLeft);
                    gap_now = gap_now + 15;
                }

                gfx.DrawLine(body_line, 75, 400 + gap_now, 520, 400 + gap_now);
                gfx.DrawString("APPROVAL DETAILS", title, XBrushes.Black, 85, 405 + gap_now, XStringFormats.TopLeft);
                gfx.DrawLine(body_line, 75, 425 + gap_now, 520, 425 + gap_now);

                gfx.DrawString("Level", subtitle, XBrushes.Black, 115, 445 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString("Approval", subtitle, XBrushes.Black, 185, 445 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString("Status", subtitle, XBrushes.Black, 385, 445 + gap_now, XStringFormats.TopLeft);

                for (int k = 0; k < Approval_Name.Count; k++)
                {
                    gfx.DrawString((k + 1).ToString(), subtitle, XBrushes.Black, 115, 465 + gap_now, XStringFormats.TopLeft);
                    gfx.DrawString(Approval_Name[k], subtitle, XBrushes.Black, 185, 465 + gap_now, XStringFormats.TopLeft);

                    if (Approval_Status[k].Contains("Pending"))
                        gfx.DrawString(Approval_Status[k], subtitle, XBrushes.Orange, 385, 465 + gap_now, XStringFormats.TopLeft);
                    else if (Approval_Status[k].Contains("Approved"))
                        gfx.DrawString(Approval_Status[k], subtitle, XBrushes.Blue, 385, 465 + gap_now, XStringFormats.TopLeft);
                    else if (Approval_Status[k].Contains("Rejected"))
                        gfx.DrawString(Approval_Status[k], subtitle, XBrushes.Red, 385, 465 + gap_now, XStringFormats.TopLeft);
                    else gfx.DrawString(Approval_Status[k], subtitle, XBrushes.Black, 385, 465 + gap_now, XStringFormats.TopLeft);

                    gap_now = gap_now + 15;
                }

                MemoryStream stream = new MemoryStream();
                document.Save(stream, false);
                stream.Position = 0;
                return File(stream, "application/pdf", receipt.Replace(" ", "_") + "_" + model2.travel_request.group_code.Trim(' ') + "_" + DateTime.Now.ToString("yyMMdd-hh-mm-tt") + ".pdf");

            }
            return View("Details", model);
        }

        
        [Authorize]
        [Authorize(Roles = "contrast.user")] 
        // GET: TravelStatus
        public async Task<ActionResult> Comment(string group_code)
        {
            List<tb_r_travel_request_comment> comment = new List<tb_r_travel_request_comment>();
            comment = await GetData.Comment(group_code);
            ViewBag.group_code = group_code;
            return View(comment);
        }


        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddComment(string commentbox, string groupcode)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;
            tb_m_employee model = await GetData.EmployeeInfo(identity.Name);               

            if(!String.IsNullOrEmpty(commentbox))
                await InsertData.TravelStatuscomment(commentbox, groupcode, groupcode, Convert.ToInt32(identity.Name));            

            //return View("Comment", comment);
            return RedirectToAction("Comment", new { @group_code = groupcode });
        }


        //[HttpPost]         
        // GET: TravelStatus
        public async Task<ActionResult> IndexMSTR(string noreg, string mstr)
        {
            tb_m_employee model = new tb_m_employee();
            model = await GetData.EmployeeInfo(noreg);

            List<vw_request_summary> ResponseList = new List<vw_request_summary>();
            ResponseList = await GetData.RequestSummaryListInfo(model.code);

            List<string> apprv_name = new List<string>();
            List<DateTime> return_date = new List<DateTime>();

            List<int> travel_duration = new List<int>();

            for (int k = 0; k < ResponseList.Count(); k++)
            {
                if (ResponseList[k].apprv_flag_lvl5 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl5));
                else
                if (ResponseList[k].apprv_flag_lvl4 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl4));
                else
                if (ResponseList[k].apprv_flag_lvl3 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl3));
                else
                if (ResponseList[k].apprv_flag_lvl2 != null)
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl2));
                else
                    apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl1));


                if (ResponseList[k].endDate_3 != null)
                    return_date.Add(ResponseList[k].endDate_3 ?? default(DateTime));
                else
                if (ResponseList[k].endDate_2 != null)
                    return_date.Add(ResponseList[k].endDate_2 ?? default(DateTime));
                else
                if (ResponseList[k].endDate_1 != null)
                    return_date.Add(ResponseList[k].endDate_1 ?? default(DateTime));


                travel_duration.Add((ResponseList[k].duration_1 ?? default(int)) + (ResponseList[k].duration_2 ?? default(int)) + (ResponseList[k].duration_3 ?? default(int)));
            }

            ViewBag.ReturnDate = return_date;
            ViewBag.Username = model.name;
            ViewBag.Bossname = apprv_name;
            ViewBag.Duration = travel_duration;

            //return View("IndexMSTR",ResponseList);
            var headers = Request.Headers.GetValues("User-Agent");
            string userAgent = string.Join(" ", headers);

            if (userAgent.ToLower().Contains("ipad"))
                return View("IndexMSTR", ResponseList);
            else
                return View("IndexMSTRMobile", ResponseList);
        }

        // Details: TravelStatus
        public async Task<ActionResult> DetailsMSTR(vw_request_summary model)
        {
            TravelStatusHelper model2 = new TravelStatusHelper();
            model2.travel_request = model;
            model2.participants = await GetData.TravelRequestParticipant(model2.travel_request);

            List<string> ModelList = new List<string>();
            for (int i = 0; i < model2.participants.Count(); i++)
            {
                ModelList.Add(await GetData.EmployeeNameInfo(model2.participants[i].no_reg));
            }
            ViewBag.RL = ModelList;

            List<string> apprv_name = new List<string>();
            List<string> apprv_status = new List<string>();

            string temp = "";
            string ttemp = "";
            for (int k = 0; k < 19; k++)
            {
                temp = null;
                ttemp = null;
                if (k == 19)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl20);
                    ttemp = model2.travel_request.apprv_flag_lvl20;
                }
                else
                if (k == 18)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl19);
                    ttemp = model2.travel_request.apprv_flag_lvl19;
                }
                else
                if (k == 17)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl18);
                    ttemp = model2.travel_request.apprv_flag_lvl18;
                }
                else
                if (k == 16)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl17);
                    ttemp = model2.travel_request.apprv_flag_lvl17;
                }
                else
                if (k == 15)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl16);
                    ttemp = model2.travel_request.apprv_flag_lvl16;
                }
                else
                if (k == 14)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl15);
                    ttemp = model2.travel_request.apprv_flag_lvl15;
                }
                else
                if (k == 13)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl14);
                    ttemp = model2.travel_request.apprv_flag_lvl14;
                }
                else
                if (k == 12)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl13);
                    ttemp = model2.travel_request.apprv_flag_lvl13;
                }
                else
                if (k == 13)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl12);
                    ttemp = model2.travel_request.apprv_flag_lvl12;
                }
                else
                if (k == 12)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl13);
                    ttemp = model2.travel_request.apprv_flag_lvl13;
                }
                else
                if (k == 11)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl12);
                    ttemp = model2.travel_request.apprv_flag_lvl12;
                }
                else
                if (k == 10)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl11);
                    ttemp = model2.travel_request.apprv_flag_lvl11;
                }
                else
                if (k == 9)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl10);
                    ttemp = model2.travel_request.apprv_flag_lvl10;
                }
                else
                if (k == 8)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl9);
                    ttemp = model2.travel_request.apprv_flag_lvl9;
                }
                else
                if (k == 7)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl8);
                    ttemp = model2.travel_request.apprv_flag_lvl8;
                }
                else
                if (k == 6)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl7);
                    ttemp = model2.travel_request.apprv_flag_lvl7;
                }
                else
                if (k == 5)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl6);
                    ttemp = model2.travel_request.apprv_flag_lvl6;
                }
                else
                if (k == 4)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl5);
                    ttemp = model2.travel_request.apprv_flag_lvl5;
                }
                else
                if (k == 3)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl4);
                    ttemp = model2.travel_request.apprv_flag_lvl4;
                }
                else
                if (k == 2)
                {
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl3);
                    ttemp = model2.travel_request.apprv_flag_lvl3;
                }
                else
                if (k == 1)
                {
                    //if (model2.travel_request.apprv_flag_lvl2 != null) 
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl2);
                    ttemp = model2.travel_request.apprv_flag_lvl2;
                }
                else
                if (k == 0)
                {
                    //if (model2.travel_request.apprv_flag_lvl1 != null) 
                    temp = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl1);
                    ttemp = model2.travel_request.apprv_flag_lvl1;
                }

                if (temp != null)
                {
                    apprv_name.Add(temp);
                    apprv_status.Add(ttemp);
                }
            }
            ViewBag.Bossname = apprv_name;
            ViewBag.StatusState = apprv_status;
            ViewBag.Approvalnum = apprv_status.Count;

            //return View(model2);

            //return View(model2);

            var headers = Request.Headers.GetValues("User-Agent");
            string userAgent = string.Join(" ", headers);

            if (userAgent.ToLower().Contains("ipad"))
                return View("DetailsMSTR", model2);
            else
                return View("DetailsMSTRMobile", model2);
        }

        public async Task<ActionResult> DropMSTR(TravelStatusHelper model, string download = "", string drop = "")
        {
            if (drop == "submit")
            {
                tb_r_travel_request model2 = new tb_r_travel_request();
                List<vw_request_summary> request = new List<vw_request_summary>();
                List<vw_request_summary> ResponseList = new List<vw_request_summary>();

                model2 = await GetData.TravelRequest(Convert.ToInt32(model.travel_request.id_request));
                model2.active_flag = true;
                model2.status_request = "99";

                double budget = Convert.ToDouble(model2.allowance_meal_idr + model2.allowance_preparation + model2.allowance_winter);

                await UpdateData.TravelRequestPersonal(model2);

                request = await GetData.RequestSummaryListInfo(model2.no_reg.ToString());

                foreach (var item in request)
                {
                    if (item.status_request != "99") ResponseList.Add(item);
                }

                string name = await GetData.EmployeeNameInfo(model2.no_reg);


                List<DateTime> return_date = new List<DateTime>();
                List<int> travel_duration = new List<int>();

                List<string> apprv_name = new List<string>();

                for (int k = 0; k < ResponseList.Count(); k++)
                {
                    if (ResponseList[k].apprv_flag_lvl5 != null)
                        apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl5));
                    else
                    if (ResponseList[k].apprv_flag_lvl4 != null)
                        apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl4));
                    else
                    if (ResponseList[k].apprv_flag_lvl3 != null)
                        apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl3));
                    else
                    if (ResponseList[k].apprv_flag_lvl2 != null)
                        apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl2));
                    else
                        apprv_name.Add(await GetData.EmployeeNameInfo(ResponseList[k].apprv_by_lvl1));


                    if (ResponseList[k].endDate_3 != null)
                        return_date.Add(ResponseList[k].endDate_3 ?? default(DateTime));
                    else
                    if (ResponseList[k].endDate_2 != null)
                        return_date.Add(ResponseList[k].endDate_2 ?? default(DateTime));
                    else
                    if (ResponseList[k].endDate_1 != null)
                        return_date.Add(ResponseList[k].endDate_1 ?? default(DateTime));


                    travel_duration.Add((ResponseList[k].duration_1 ?? default(int)) + (ResponseList[k].duration_2 ?? default(int)) + (ResponseList[k].duration_3 ?? default(int)));
                }

                ViewBag.ReturnDate = return_date;
                //ViewBag.Username = model.employee_info.name;
                ViewBag.Bossname = apprv_name;
                ViewBag.Duration = travel_duration;

                ModelState.Clear();
                if (ResponseList.Count > 0) return View("IndexMSTR", ResponseList.OrderBy(r => r.status_request).ToList());
                else
                {
                    return View("IndexMSTR", ResponseList);
                }
            }
            else if (download == "download")
            {
                TravelStatusHelper model2 = new TravelStatusHelper();
                model2.travel_request = await GetData.RequestSummaryInfo(model.travel_request.group_code);
                model2.employee_info = new tb_m_employee();
                model2.employee_info.code = model2.travel_request.no_reg.ToString();
                model2.employee_info = await GetData.EmployeeInfo(model2.employee_info);
                model2.participants = await GetData.TravelRequestParticipant(model2.travel_request);

                List<string> Approval_Name = new List<string>();
                List<string> Approval_Status = new List<string>();
                string bossname = "";

                if (model2.travel_request.apprv_by_lvl1 != null)
                {
                    bossname = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl1);
                    Approval_Name.Add(bossname);
                    if (model2.travel_request.apprv_flag_lvl1 == null) Approval_Status.Add("Not Yet");
                    else if (model2.travel_request.apprv_flag_lvl1.Contains("0")) Approval_Status.Add("Pending");
                    else if (model2.travel_request.apprv_flag_lvl1.Contains("1")) Approval_Status.Add("Approved");
                    else if (model2.travel_request.apprv_flag_lvl1.Contains("2")) Approval_Status.Add("Rejected");
                }

                if (model2.travel_request.apprv_by_lvl2 != null)
                {
                    bossname = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl2);
                    Approval_Name.Add(bossname);
                    if (model2.travel_request.apprv_flag_lvl2 == null) Approval_Status.Add("Not Yet");
                    else if (model2.travel_request.apprv_flag_lvl2.Contains("0")) Approval_Status.Add("Pending");
                    else if (model2.travel_request.apprv_flag_lvl2.Contains("1")) Approval_Status.Add("Approved");
                    else if (model2.travel_request.apprv_flag_lvl2.Contains("2")) Approval_Status.Add("Rejected");
                }

                if (model2.travel_request.apprv_by_lvl3 != null)
                {
                    bossname = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl3);
                    Approval_Name.Add(bossname);
                    if (model2.travel_request.apprv_flag_lvl3 == null) Approval_Status.Add("Not Yet");
                    else if (model2.travel_request.apprv_flag_lvl3.Contains("0")) Approval_Status.Add("Pending");
                    else if (model2.travel_request.apprv_flag_lvl3.Contains("1")) Approval_Status.Add("Approved");
                    else if (model2.travel_request.apprv_flag_lvl3.Contains("2")) Approval_Status.Add("Rejected");
                }

                if (model2.travel_request.apprv_by_lvl4 != null)
                {
                    bossname = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl4);
                    Approval_Name.Add(bossname);
                    if (model2.travel_request.apprv_flag_lvl4 == null) Approval_Status.Add("Not Yet");
                    else if (model2.travel_request.apprv_flag_lvl4.Contains("0")) Approval_Status.Add("Pending");
                    else if (model2.travel_request.apprv_flag_lvl4.Contains("1")) Approval_Status.Add("Approved");
                    else if (model2.travel_request.apprv_flag_lvl4.Contains("2")) Approval_Status.Add("Rejected");
                }

                if (model2.travel_request.apprv_by_lvl5 != null)
                {
                    bossname = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl5);
                    Approval_Name.Add(bossname);
                    if (model2.travel_request.apprv_flag_lvl5 == null) Approval_Status.Add("Not Yet");
                    else if (model2.travel_request.apprv_flag_lvl5.Contains("0")) Approval_Status.Add("Pending");
                    else if (model2.travel_request.apprv_flag_lvl5.Contains("1")) Approval_Status.Add("Approved");
                    else if (model2.travel_request.apprv_flag_lvl5.Contains("2")) Approval_Status.Add("Rejected");
                }

                if (model2.travel_request.apprv_by_lvl6 != null)
                {
                    bossname = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl6);
                    Approval_Name.Add(bossname);
                    if (model2.travel_request.apprv_flag_lvl6 == null) Approval_Status.Add("Not Yet");
                    else if (model2.travel_request.apprv_flag_lvl6.Contains("0")) Approval_Status.Add("Pending");
                    else if (model2.travel_request.apprv_flag_lvl6.Contains("1")) Approval_Status.Add("Approved");
                    else if (model2.travel_request.apprv_flag_lvl6.Contains("2")) Approval_Status.Add("Rejected");
                }

                if (model2.travel_request.apprv_by_lvl7 != null)
                {
                    bossname = await GetData.EmployeeNameInfo(model2.travel_request.apprv_by_lvl7);
                    Approval_Name.Add(bossname);
                    if (model2.travel_request.apprv_flag_lvl7 == null) Approval_Status.Add("Not Yet");
                    else if (model2.travel_request.apprv_flag_lvl7.Contains("0")) Approval_Status.Add("Pending");
                    else if (model2.travel_request.apprv_flag_lvl7.Contains("1")) Approval_Status.Add("Approved");
                    else if (model2.travel_request.apprv_flag_lvl7.Contains("2")) Approval_Status.Add("Rejected");
                }

                PdfDocument document = new PdfDocument();
                document.Info.Title = "Business Travel Assignment";
                PdfPage page = document.AddPage();
                page.Orientation = PdfSharp.PageOrientation.Portrait;
                XSize size = PageSizeConverter.ToSize(PdfSharp.PageSize.A4);
                page.Height = size.Height;
                page.Width = size.Width;

                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont head = new XFont("Arial Narrow", 18, XFontStyle.Regular);
                XFont address = new XFont("Arial Narrow", 11, XFontStyle.Regular);
                XFont body = new XFont("Calibri", 9, XFontStyle.Regular);
                XFont total = new XFont("Lucida Grande", 11, XFontStyle.Bold);
                XFont title = new XFont("Lucida Grande", 11, XFontStyle.Bold);
                XFont subtitle = new XFont("Lucida Grande", 10, XFontStyle.Regular);
                XFont credit = new XFont("Lucida Sans", 9, XFontStyle.Regular);
                XFont valid = new XFont("Lucida Sans", 8, XFontStyle.Regular);

                XPen header_line = new XPen(XColors.Black, 2);
                XPen body_line = new XPen(XColors.DimGray, 0.5);
                XPen POLine = new XPen(XColors.SteelBlue, 3);

                string PT_TAM = "PT. TOYOTA ASTRA MOTOR";
                string receipt = "BTA DETAILS";


                string add1 = "Jl. Laks. Yos Sudarso, Sunter II";
                string add2 = "Jakarta Utara - Indonesia";
                string add3 = "Phone :62-21 - 6515551(Hunting)";

                string btr = model2.travel_request.group_code;
                string name = model2.employee_info.name.ToUpper();
                string noreg = model2.travel_request.no_reg.ToString();
                string TAMPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "tam_logo.PNG");
                string ContrastPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "small_logo_horizontal.png");
                // string date = model.invoice.start_date.ToString();

                List<string> start_date = new List<string>();
                List<string> start_time = new List<string>();
                List<string> end_date = new List<string>();
                List<string> end_time = new List<string>();
                List<string> hotel_ticket = new List<string>();
                List<int?> duration = new List<int?>();
                List<string> destination = new List<string>();
                List<string> overseas = new List<string>();
                List<int?> amount = new List<int?>();

                start_date.Add(Convert.ToDateTime(model2.travel_request.startDate_1).ToString("dd MMM yyyy"));
                start_time.Add(Convert.ToDateTime(model2.travel_request.startDate_1).ToString("hh:mm tt"));

                end_date.Add(Convert.ToDateTime(model2.travel_request.endDate_1).ToString("dd MMM yyyy"));
                end_time.Add(Convert.ToDateTime(model2.travel_request.endDate_1).ToString("hh:mm tt"));

                destination.Add(model2.travel_request.destination_1);
                tb_m_destination over = await GetData.DestinationCityInfo(model2.travel_request.destination_1);
                if (over.id_region < 4) overseas.Add("Overseas");
                else overseas.Add("Domestic");

                if (model2.travel_request.startDate_2 != null)
                {
                    start_date.Add(Convert.ToDateTime(model2.travel_request.startDate_2).ToString("dd MMM yyyy"));
                    start_time.Add(Convert.ToDateTime(model2.travel_request.startDate_2).ToString("hh:mm tt"));

                    end_date.Add(Convert.ToDateTime(model2.travel_request.endDate_2).ToString("dd MMM yyyy"));
                    end_time.Add(Convert.ToDateTime(model2.travel_request.endDate_2).ToString("hh:mm tt"));

                    destination.Add(model2.travel_request.destination_2);
                    tb_m_destination over2 = await GetData.DestinationCityInfo(model2.travel_request.destination_2);
                    if (over2.id_region < 4) overseas.Add("Overseas");
                    else overseas.Add("Domestic");
                }

                if (model2.travel_request.startDate_3 != null)
                {
                    start_date.Add(Convert.ToDateTime(model2.travel_request.startDate_3).ToString("dd MMM yyyy"));
                    start_time.Add(Convert.ToDateTime(model2.travel_request.startDate_3).ToString("hh:mm tt"));

                    end_date.Add(Convert.ToDateTime(model2.travel_request.endDate_3).ToString("dd MMM yyyy"));
                    end_time.Add(Convert.ToDateTime(model2.travel_request.endDate_3).ToString("hh:mm tt"));

                    destination.Add(model2.travel_request.destination_3);
                    tb_m_destination over2 = await GetData.DestinationCityInfo(model2.travel_request.destination_3);
                    if (over2.id_region < 4) overseas.Add("Overseas");
                    else overseas.Add("Domestic");

                }

                if (model2.travel_request.startDate_4 != null)
                {
                    start_date.Add(Convert.ToDateTime(model2.travel_request.startDate_4).ToString("dd MMM yyyy"));
                    start_time.Add(Convert.ToDateTime(model2.travel_request.startDate_4).ToString("hh:mm tt"));

                    end_date.Add(Convert.ToDateTime(model2.travel_request.endDate_4).ToString("dd MMM yyyy"));
                    end_time.Add(Convert.ToDateTime(model2.travel_request.endDate_4).ToString("hh:mm tt"));

                    destination.Add(model2.travel_request.destination_4);
                    tb_m_destination over2 = await GetData.DestinationCityInfo(model2.travel_request.destination_4);
                    if (over2.id_region < 4) overseas.Add("Overseas");
                    else overseas.Add("Domestic");

                }

                if (model2.travel_request.startDate_5 != null)
                {
                    start_date.Add(Convert.ToDateTime(model2.travel_request.startDate_5).ToString("dd MMM yyyy"));
                    start_time.Add(Convert.ToDateTime(model2.travel_request.startDate_5).ToString("hh:mm tt"));

                    end_date.Add(Convert.ToDateTime(model2.travel_request.endDate_5).ToString("dd MMM yyyy"));
                    end_time.Add(Convert.ToDateTime(model2.travel_request.endDate_5).ToString("hh:mm tt"));

                    destination.Add(model2.travel_request.destination_5);
                    tb_m_destination over2 = await GetData.DestinationCityInfo(model2.travel_request.destination_5);
                    if (over2.id_region < 4) overseas.Add("Overseas");
                    else overseas.Add("Domestic");

                }

                if (model2.travel_request.startDate_6 != null)
                {
                    start_date.Add(Convert.ToDateTime(model2.travel_request.startDate_6).ToString("dd MMM yyyy"));
                    start_time.Add(Convert.ToDateTime(model2.travel_request.startDate_6).ToString("hh:mm tt"));

                    end_date.Add(Convert.ToDateTime(model2.travel_request.endDate_6).ToString("dd MMM yyyy"));
                    end_time.Add(Convert.ToDateTime(model2.travel_request.endDate_6).ToString("hh:mm tt"));

                    destination.Add(model2.travel_request.destination_6);
                    tb_m_destination over2 = await GetData.DestinationCityInfo(model2.travel_request.destination_6);
                    if (over2.id_region < 4) overseas.Add("Overseas");
                    else overseas.Add("Domestic");

                }

                int x_pad = 5;
                int y_pad = 10;
                int legend = 50;


                gfx.DrawRectangle(XBrushes.GhostWhite, 50, 735, 495, 63);

                XImage TAM = XImage.FromFile(TAMPath);
                gfx.DrawImage(TAM, x_pad + 50, 50, 40, 40);

                XImage contrast = XImage.FromFile(ContrastPath);
                gfx.DrawImage(contrast, x_pad + 50, 735);

                gfx.DrawString(PT_TAM, head, XBrushes.Black, 95 + x_pad, 60, XStringFormats.TopLeft);
                gfx.DrawRectangle(XBrushes.GhostWhite, 377, 55, 168, 28);
                gfx.DrawLine(POLine, 377, 55, 377, 83);

                gfx.DrawString(receipt, head, XBrushes.Gray, 385, 60, XStringFormats.TopLeft);


                gfx.DrawLine(header_line, 50, 95, 545, 95);

                gfx.DrawString(add1, address, XBrushes.Black, 50 + x_pad, 105, XStringFormats.TopLeft);
                gfx.DrawString(add2, address, XBrushes.Black, 50 + x_pad, 120, XStringFormats.TopLeft);
                gfx.DrawString(add3, address, XBrushes.Black, 50 + x_pad, 135, XStringFormats.TopLeft);

                gfx.DrawString("BTA", address, XBrushes.Black, 385, 105, XStringFormats.TopLeft);
                gfx.DrawString("NOREG", address, XBrushes.Black, 385, 120, XStringFormats.TopLeft);
                gfx.DrawString("NAME", address, XBrushes.Black, 385, 135, XStringFormats.TopLeft);

                gfx.DrawString(":", address, XBrushes.Black, 385 + legend - 10, 105, XStringFormats.TopLeft);
                gfx.DrawString(":", address, XBrushes.Black, 385 + legend - 10, 120, XStringFormats.TopLeft);
                gfx.DrawString(":", address, XBrushes.Black, 385 + legend - 10, 135, XStringFormats.TopLeft);

                gfx.DrawString(btr, address, XBrushes.Black, 385 + legend, 105, XStringFormats.TopLeft);
                gfx.DrawString(noreg, address, XBrushes.Black, 385 + legend, 120, XStringFormats.TopLeft);
                gfx.DrawString(name, address, XBrushes.Black, 385 + legend, 135, XStringFormats.TopLeft);

                gfx.DrawLine(body_line, 75, 200, 520, 200);

                gfx.DrawString("DESTINATION DETAILS", title, XBrushes.Black, 85, 205, XStringFormats.TopLeft);

                gfx.DrawLine(body_line, 75, 225, 520, 225);

                gfx.DrawString("From", subtitle, XBrushes.Black, 85, 235, XStringFormats.TopLeft);
                gfx.DrawString("To", subtitle, XBrushes.Black, 170, 235, XStringFormats.TopLeft);

                gfx.DrawString("Region", subtitle, XBrushes.Black, 255, 235, XStringFormats.TopLeft);

                gfx.DrawString("Depart", subtitle, XBrushes.Black, 315 + x_pad, 235, XStringFormats.TopLeft);
                gfx.DrawString("Return", subtitle, XBrushes.Black, 415 + x_pad, 235, XStringFormats.TopLeft);

                int gap_now = 0;
                string planned = "";
                string from = "Jakarta";
                for (int i = 0; i < start_date.Count(); i++)
                {

                    gap_now = (i + 1) * 12;

                    gfx.DrawString(from, body, XBrushes.Black, 85, 235 + gap_now, XStringFormats.TopLeft);
                    gfx.DrawString(destination[i], body, XBrushes.Black, 170, 235 + gap_now, XStringFormats.TopLeft);

                    gfx.DrawString(overseas[i], body, XBrushes.Black, 255, 235 + gap_now, XStringFormats.TopLeft);

                    gfx.DrawString(start_date[i] + " - " + start_time[i], body, XBrushes.Black, 315 + x_pad, 235 + gap_now, XStringFormats.TopLeft);
                    gfx.DrawString(end_date[i] + " - " + end_time[i], body, XBrushes.Black, 415 + x_pad, 235 + gap_now, XStringFormats.TopLeft);

                    from = destination[i];
                }

                gfx.DrawLine(body_line, 75, 280 + gap_now, 520, 280 + gap_now);

                gfx.DrawString("BUSINESS TRAVEL ASSIGNMENT DETAILS", title, XBrushes.Black, 85, 285 + gap_now, XStringFormats.TopLeft);
                gfx.DrawLine(body_line, 75, 305 + gap_now, 520, 305 + gap_now);

                gfx.DrawString("Assignment Purpose", subtitle, XBrushes.DarkBlue, 95, 320 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString("Assignment Reason", subtitle, XBrushes.DarkBlue, 95, 335 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString("Assignment Type", subtitle, XBrushes.DarkBlue, 95, 350 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString("Renewal Passport", subtitle, XBrushes.DarkBlue, 95, 365 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString("Participant", subtitle, XBrushes.DarkBlue, 95, 380 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(":", subtitle, XBrushes.Black, 250, 320 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(":", subtitle, XBrushes.Black, 250, 335 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(":", subtitle, XBrushes.Black, 250, 350 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(":", subtitle, XBrushes.Black, 250, 365 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(":", subtitle, XBrushes.Black, 250, 380 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(model2.travel_request.travel_purpose, subtitle, XBrushes.Black, 260, 320 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(model2.travel_request.reason_of_assigment, subtitle, XBrushes.Black, 260, 335 + gap_now, XStringFormats.TopLeft);
                if (model2.travel_request.request_type == false) planned = "Unplanned";
                else planned = "Planned";
                gfx.DrawString(planned, subtitle, XBrushes.Black, 260, 350 + gap_now, XStringFormats.TopLeft);

                if (model2.travel_request.passport_flag == false)
                    gfx.DrawString("No", subtitle, XBrushes.Black, 260, 365 + gap_now, XStringFormats.TopLeft);
                else
                    gfx.DrawString("Yes", subtitle, XBrushes.Black, 260, 365 + gap_now, XStringFormats.TopLeft);

                if (model2.participants.Count() > 0)
                {
                    foreach (var part in model2.participants)
                    {
                        string namePart = await GetData.EmployeeNameInfo(part.no_reg);
                        gfx.DrawString(namePart, subtitle, XBrushes.Black, 260, 380 + gap_now, XStringFormats.TopLeft);
                        gap_now = gap_now + 15;
                    }
                }
                else
                {
                    gfx.DrawString("None", subtitle, XBrushes.Black, 260, 380 + gap_now, XStringFormats.TopLeft);
                    gap_now = gap_now + 15;
                }

                gfx.DrawLine(body_line, 75, 400 + gap_now, 520, 400 + gap_now);
                gfx.DrawString("APPROVAL DETAILS", title, XBrushes.Black, 85, 405 + gap_now, XStringFormats.TopLeft);
                gfx.DrawLine(body_line, 75, 425 + gap_now, 520, 425 + gap_now);

                gfx.DrawString("Level", subtitle, XBrushes.Black, 115, 445 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString("Approval", subtitle, XBrushes.Black, 185, 445 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString("Status", subtitle, XBrushes.Black, 385, 445 + gap_now, XStringFormats.TopLeft);

                for (int k = 0; k < Approval_Name.Count; k++)
                {
                    gfx.DrawString((k + 1).ToString(), subtitle, XBrushes.Black, 115, 465 + gap_now, XStringFormats.TopLeft);
                    gfx.DrawString(Approval_Name[k], subtitle, XBrushes.Black, 185, 465 + gap_now, XStringFormats.TopLeft);

                    if (Approval_Status[k].Contains("Pending"))
                        gfx.DrawString(Approval_Status[k], subtitle, XBrushes.Orange, 385, 465 + gap_now, XStringFormats.TopLeft);
                    else if (Approval_Status[k].Contains("Approved"))
                        gfx.DrawString(Approval_Status[k], subtitle, XBrushes.Blue, 385, 465 + gap_now, XStringFormats.TopLeft);
                    else if (Approval_Status[k].Contains("Rejected"))
                        gfx.DrawString(Approval_Status[k], subtitle, XBrushes.Red, 385, 465 + gap_now, XStringFormats.TopLeft);
                    else gfx.DrawString(Approval_Status[k], subtitle, XBrushes.Black, 385, 465 + gap_now, XStringFormats.TopLeft);

                    gap_now = gap_now + 15;
                }

                MemoryStream stream = new MemoryStream();
                document.Save(stream, false);
                stream.Position = 0;
                return File(stream, "application/pdf", receipt.Replace(" ", "_") + "_" + model2.travel_request.group_code.Trim(' ') + "_" + DateTime.Now.ToString("yyMMdd-hh-mm-tt") + ".pdf");

            }
            return View("/TravelRequest/SubmittedMSTR");
        }

    }
}