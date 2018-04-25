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
using AutoMapper;
using System.Security.Claims;

namespace CONTRAST_WEB.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    public class TravelRequestController : Controller
    {
        public async Task<JsonResult> GetSearchValue2(string search, string code)
        {

            List<Class1> list = new List<Class1>();
            list = await GetData.SearchNameDiv(search);
            //list = await GetData.SearchName(search);
            List<Class1> filtered = new List<Class1>();
            foreach (var item in list)
            {
                if (!item.code.Contains(code)) filtered.Add(item);
            }

            return new JsonResult { Data = filtered, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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

        [Authorize]
        [Authorize(Roles = "contrast.user")]
        public async Task<ActionResult> Index(string applied = "")
        {
            var identity = (ClaimsIdentity)User.Identity;
            Utility.Logger(identity.Name);
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;
            tb_m_employee model = new tb_m_employee();

            if (applied == "") model = await GetData.EmployeeInfo(identity.Name);
            else model = await GetData.EmployeeInfo(applied);

            tb_m_employee created = await GetData.EmployeeInfo(identity.Name);
            ViewBag.loged_id = created.code.Trim(' ');
            ViewBag.loged_name = created.name.Trim(' ');
            
            //Get user name
            ViewBag.Username = model.name;

            //Get destination list info for dropdown list
            ViewBag.RL = await GetData.DestinationInfo();

            //Get purpose list info for travel purpose list
            ViewBag.RL2 = await GetData.PurposeInfo();

            //Prepare travel request information object to be used at view
            TravelRequestHelper model2 = new TravelRequestHelper();

            //Copy user login information
            model2.travel_request = new tb_r_travel_request();
            model2.employee_info = model;

            //Get user direct superior info
            var assignedby = await GetData.AssignedBy(model2.employee_info.unit_code_code);
            tb_m_travel_procedures procedures= new tb_m_travel_procedures();
            if (model2.employee_info.position.Trim() == "SECO"|| model2.employee_info.position.Trim() == "SEA"|| model2.employee_info.position.Trim() == "SMEC"|| model2.employee_info.position.Trim() == "AADV"|| model2.employee_info.position.Trim() == "GM"|| model2.employee_info.position.Trim() == "EGM")
            {
                procedures = await GetData.Procedures(model2.employee_info.position);
            }
            else
                procedures = await GetData.Procedures(model2.employee_info.@class);

            if (assignedby.pd == Convert.ToInt32(model2.employee_info.code)) model2.travel_request.assign_by = Convert.ToInt32(model2.employee_info.code);
            else
            //If the direct superior is DH, then get who is the DH from travel procedure table
            if (procedures.apprv_by_lvl1 == "DH") model2.travel_request.assign_by = assignedby.dh_code;
            else
            //If the direct superior is Div Director, then get who is the Div Director from travel procedure table
            if (procedures.apprv_by_lvl1 == "Div Director") model2.travel_request.assign_by = assignedby.director;
            //If the direct superior is vice president, then get who is the VP from travel procedure table
            else
            if (procedures.apprv_by_lvl1 == "VP") model2.travel_request.assign_by = assignedby.vp;
            else
            if (procedures.apprv_by_lvl1 == "EGM") model2.travel_request.assign_by = assignedby.egm;

            //if still empty - for special case
            if (model2.travel_request.assign_by == null)
            {
                if (assignedby.dh_code != null && model2.employee_info.position.Trim() != "DH" && model2.employee_info.position.Trim() != "EGM") model2.travel_request.assign_by = assignedby.dh_code;
                else
                    if (assignedby.egm != null && model2.employee_info.position.Trim() != "EGM") model2.travel_request.assign_by = assignedby.egm;
                else
                    if (assignedby.director != null) model2.travel_request.assign_by = assignedby.director;
                else
                    if (assignedby.local_fd != null) model2.travel_request.assign_by = assignedby.local_fd;
                else
                    if (assignedby.japan_fd != null) model2.travel_request.assign_by = assignedby.japan_fd;
                else
                    if (assignedby.vp != null) model2.travel_request.assign_by = assignedby.vp;
                else
                    if (assignedby.pd != null) model2.travel_request.assign_by = assignedby.pd;
            }

            //Get user direct superior name
            string boss = await GetData.EmployeeNameInfo(model2.travel_request.assign_by);
            if (boss == null) boss = "-No Data";
            ViewBag.Bossname = "Assigned by " + boss.Trim() + " (" + model2.travel_request.assign_by.ToString().Trim() + ")";
            ViewBag.Bossname2 = boss != null ? boss.Trim() : "No supperior registered";

            //Set request type default to false
            model2.travel_request.request_type = false;

            //Set activity id default to 3 (Regular)
            model2.travel_request.id_activity = 3;
            
            List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();
            bankName = await GetData.VendorEmployee(Convert.ToInt32(model.code));
            if (bankName.Count != 0)
            {
                model2.tbankname = bankName[0].Bank_Name;
                model2.tbankaccount = bankName[0].account_number;
            }
            else
            {
                ViewBag.ebankname = "No bank account registered,contact finance division";
                ViewBag.ebankaccount = "No bank name registered,contact finance division";
            }

            //special employee
            //List<tb_m_special_employee> special_model = await GetData.SpecialEmployee(identity.Name);
            List<tb_m_special_employee> special_model = await GetData.SpecialEmployee(model.code);
            if (special_model.Count != 0)
            {
                List<string> division_model = new List<string>(); 
                for (int k = 0; k < special_model.Count; k++)
                {
                    division_model.Add(special_model[k].Divisi);
                }
                model2.special_employee_flag = true;
                model2.travel_request.exep_empolyee = true;
                var selectListItems = division_model.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();
                ViewBag.division_name2 = selectListItems;
            }
            else
            { 
                tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(model.code));
                division.Divisi = division.Divisi.Replace("and1", "&");
                ViewBag.division_name = division.Divisi;
                model2.special_employee_flag = false;
            }

            ViewBag.Username = model2.employee_info.name;            
            return View(model2);
        }

        [Authorize]
        [Authorize(Roles = "contrast.user")]
        public async Task<ActionResult> Validate(TravelRequestHelper model, string validate, string add, string delete = "", string loged = "", string clear = "")
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;

            tb_m_employee created = await GetData.EmployeeInfo(identity.Name);
            ViewBag.loged_id = created.code.Trim(' ');
            ViewBag.loged_name = created.name.Trim(' ');

            if (validate != null && model.travel_request != null)
            {
                if (ModelState.IsValid)
                {
                    if (Request.Files["generaldoc"] != null)
                    {
                        HttpPostedFileBase file = Request.Files["generaldoc"];
                        model.generaldoc_file = file;
                    }
                    if (Request.Files["itinerarydoc"] != null)
                    {
                        HttpPostedFileBase file = Request.Files["itinerarydoc"];
                        model.itinerary_file = file;
                    }
                    if (Request.Files["invitationdoc"] != null)
                    {
                        HttpPostedFileBase file = Request.Files["invitationdoc"];
                        model.invitation_file = file;
                    }
                    if (Request.Files["proposaldoc"] != null)
                    {
                        HttpPostedFileBase file = Request.Files["proposaldoc"];
                        model.proposaldoc_file = file;
                    }

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<tb_r_travel_request, tb_r_travel_request>();
                    });
                    IMapper mapper = config.CreateMapper();

                    model.travel_request.no_reg = Convert.ToInt32(model.employee_info.code);

                    DateTime now = DateTime.Now;

                    ViewBag.Title = model.employee_info.code;

                    ViewBag.Username = model.employee_info.name;
                    model.travel_request.active_flag = false;
                    model.travel_request.status_request = "0";
                    model.travel_request.comments = "Comment";

                    model.travel_request.exep_empolyee = model.travel_request.exep_empolyee;
                    model.travel_request.additional2 = model.employee_info.unit_code_name;


                    model.travel_request.invited_by = model.travel_request.no_reg;
                    model.travel_request.create_date = now;

                    //list participant in string
                    if (model.participants != null)
                    {
                        List<string> ModelList = new List<string>();
                        for (int k = 0; k < model.participants.Count(); k++)
                        {
                            ModelList.Add(await GetData.EmployeeNameInfo(model.participants[k].no_reg));
                        }
                        ViewBag.RL3 = ModelList;
                    }
                    List<TravelRequestHelper> ListModel = new List<TravelRequestHelper>();

                    for (int c = 0; c < 3; c++)
                    {
                        {
                            //if (model.tend_date[c] == null) break;
                            if (c == 0 && model.tend_date0 == null) break;
                            else
                            if (c == 1 && model.tend_date1 == null) break;
                            else
                            if (c == 2 && model.tend_date2 == null) break;

                            ListModel.Add(new TravelRequestHelper());
                            ListModel[c].employee_info = new tb_m_employee();
                            ListModel[c].employee_info = model.employee_info;
                            ListModel[c].travel_request = new tb_r_travel_request();
                            ListModel[c].travel_request = mapper.Map<tb_r_travel_request>(model.travel_request);

                            //travel documents
                            if (model.generaldoc_file.FileName != "")
                            {
                                ListModel[c].generaldoc_file = model.generaldoc_file;
                                ListModel[c].travel_request.path_general = Utility.UploadFileUniversal(ListModel[c].generaldoc_file, Constant.TravelDocumentsFolder, "GENERAL_" + ListModel[c].travel_request.no_reg + "_" + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                            }
                            if (model.invitation_file.FileName != "")
                            {
                                ListModel[c].invitation_file = model.invitation_file;
                                ListModel[c].travel_request.path_invitation = Utility.UploadFileUniversal(ListModel[c].invitation_file, Constant.TravelDocumentsFolder, "INVITE_" + ListModel[c].travel_request.no_reg + "_" + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                            }
                            if (model.itinerary_file.FileName != "")
                            {
                                ListModel[c].itinerary_file = model.itinerary_file;
                                ListModel[c].travel_request.path_itinerary = Utility.UploadFileUniversal(ListModel[c].itinerary_file, Constant.TravelDocumentsFolder, "PLAN_" + ListModel[c].travel_request.no_reg + "_" + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                            }

                            if (model.proposaldoc_file.FileName != "")
                            {
                                ListModel[c].proposaldoc_file = model.proposaldoc_file;
                                ListModel[c].travel_request.additional1 = Utility.UploadFileUniversal(ListModel[c].proposaldoc_file, Constant.TravelDocumentsFolder, "PROPOSAL_" + ListModel[c].travel_request.no_reg + "_" + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                            }

                            if (c == 0)
                            {
                                ListModel[c].travel_request.start_date = model.tstart_date0;
                                ListModel[c].travel_request.end_date = model.tend_date0;
                            }
                            else
                            if (c == 1)
                            {
                                ListModel[c].travel_request.start_date = model.tstart_date1;
                                ListModel[c].travel_request.end_date = model.tend_date1;
                            }
                            else
                            if (c == 2)
                            {
                                ListModel[c].travel_request.start_date = model.tstart_date2;
                                ListModel[c].travel_request.end_date = model.tend_date2;
                            }

                            ListModel[c].travel_request.additional2 = model.travel_request.additional2;

                            ListModel[c].travel_request.id_destination_city = model.tid_destination_city[c];
                            ListModel[c].travel_request.air_ticket_flag = model.tair_ticket_flag[c];
                            ListModel[c].travel_request.destination_code = await GetData.RegionInfo(ListModel[c].travel_request.id_destination_city);

                            //ListModel[c].travel_request.overseas_flag = model.toverseas_flag[c];
                            ListModel[c].travel_request.user_created = Convert.ToInt32(created.code);
                            ListModel[c].travel_request.reason_of_assigment = model.treason;
                            ListModel[c].travel_request.travel_purpose = model.tpurpose;
                            ListModel[c].travel_request.id_activity = model.tactivity;
                            ListModel[c].travel_request.status_request = model.travel_request.status_request;

                            if (ListModel[c].travel_request.destination_code == 4) ListModel[c].travel_request.overseas_flag = false;
                            else
                                ListModel[c].travel_request.overseas_flag = true;

                            //cek partisipan
                            if (model.participants != null)
                            {
                                ListModel[c].participants = new List<tb_r_travel_request_participant>();
                                ListModel[c].participants = model.participants;
                            }
                        }
                        //if (model.tend_date.Count() == c) break;
                    }

                    //hitung
                    for (int i = 0; i < ListModel.Count(); i++)
                    {
                        bool same_date = false;
                        int week_day = 0;
                        TimeSpan range = ((DateTime)ListModel[i].travel_request.end_date).Date - ((DateTime)ListModel[i].travel_request.start_date).Date;
                        
                        if (ListModel.Count() > 1 && i > 0)
                        {
                            if (Convert.ToDateTime(ListModel[i].travel_request.start_date).Date == Convert.ToDateTime(ListModel[i - 1].travel_request.end_date).Date) same_date = true;
                        }
                        for (int k = 0; k <= range.Days; k++)
                        {
                            if (Convert.ToDateTime(ListModel[i].travel_request.start_date).AddDays(k).DayOfWeek == DayOfWeek.Saturday || Convert.ToDateTime(ListModel[i].travel_request.start_date).AddDays(k).DayOfWeek == DayOfWeek.Sunday) week_day++;
                        }

                        int duration = range.Days;
                        if (!same_date) duration = duration + 1;
                        
                        ListModel[i].travel_request.duration = duration;

                        var mealwinterallowance = await GetData.RateMealWinterInfo(ListModel[i]);
                        ListModel[i].travel_request.allowance_meal_idr = (mealwinterallowance.meal_allowance * duration) + (mealwinterallowance.meal_allowance * week_day);

                        // cek winter gak?
                        ///*
                        if (ListModel[i].travel_request.start_date.Value.Month == 12 || ListModel[i].travel_request.start_date.Value.Month == 1 || ListModel[i].travel_request.start_date.Value.Month == 2)
                        {
                            ListModel[i].travel_request.allowance_winter = mealwinterallowance.winter_allowance;
                        }
                        else
                            ListModel[i].travel_request.allowance_winter = 0;

                        if (ListModel[i].travel_request.air_ticket_flag == true)
                        {
                            var rateflight = await GetData.RateFlightInfo(ListModel[i]);
                            ListModel[i].travel_request.allowance_ticket = (rateflight.economy) * 2;
                        }
                        else
                        {
                            var rateland = await GetData.Procedures(ListModel[i].employee_info.@class);
                            ListModel[i].travel_request.allowance_ticket = (Convert.ToInt32(rateland.land_transport)) * 2;
                        }

                        var ratehotel = await GetData.RateHotelInfo(ListModel[i]);

                        if (!same_date)
                        {
                            if (ListModel[i].travel_request.overseas_flag == true) ListModel[i].travel_request.allowance_hotel = ratehotel.overseas * (duration - 1);
                            else
                                ListModel[i].travel_request.allowance_hotel = ratehotel.domestik * (duration - 1);
                        }
                        else
                        {
                            if (ListModel[i].travel_request.overseas_flag == true) ListModel[i].travel_request.allowance_hotel = ratehotel.overseas * duration;
                            else
                                ListModel[i].travel_request.allowance_hotel = ratehotel.domestik * duration;
                        }

                        ListModel[i].travel_request.apprv_flag_lvl1 = "0";
                        ListModel[i].travel_request.allowance_preparation = 0;
                        ListModel[i].travel_request.grand_total_allowance = (ListModel[i].travel_request.allowance_meal_idr != null ? ListModel[i].travel_request.allowance_meal_idr : 0) +
                                                                            (ListModel[i].travel_request.allowance_hotel != null ? ListModel[i].travel_request.allowance_hotel : 0) +
                                                                            (ListModel[i].travel_request.allowance_preparation != null ? ListModel[i].travel_request.allowance_preparation : 0) +
                                                                            (ListModel[i].travel_request.allowance_ticket != null ? ListModel[i].travel_request.allowance_ticket : 0) +
                                                                            (ListModel[i].travel_request.allowance_winter != null ? ListModel[i].travel_request.allowance_winter : 0)
                                                                            ;
                        ListModel[i].travel_request.create_date = now;

                        if (ListModel[i].participants != null)
                        {
                            for (int k = 0; k < ListModel[i].participants.Count(); k++)
                            {
                                ListModel[i].participants[k].created_date = now;
                                ListModel[i].participants[k].active_flag = true;
                                ListModel[i].participants[k].no_reg_parent = Convert.ToInt32(ListModel[i].employee_info.code.Trim());
                            }
                            ListModel[i].travel_request.participants_flag = true;
                        }
                        else
                            for (int k = 0; k < ListModel.Count(); k++)
                            {
                                ListModel[k].travel_request.participants_flag = false;
                            }
                        //for exep employee
                        ListModel[i].travel_request.exep_empolyee = false;
                    }

                    ///*
                    if (model.participants != null)
                        model.travel_request.participants_flag = true;
                    else
                        model.travel_request.participants_flag = false;

                    ViewBag.Destination = new string[ListModel.Count()];
                    for (int k = 0; k < ListModel.Count(); k++)
                    {
                        ViewBag.Destination[k] = await GetData.DestinationNameInfo(ListModel[k].travel_request.id_destination_city);
                    }
                    ViewBag.Bossname = await GetData.EmployeeNameInfo(model.travel_request.assign_by);
                    if (ViewBag.Bossname == null) ViewBag.Bossname = "-No Data";
                    //isi wbs no and cost center
                    tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(model.employee_info.code));
                    division.Divisi = division.Divisi.Replace("and1", "&");
                    ViewBag.division_name = division.Divisi;

                    string division_r = await GetData.GetDivMapping(ListModel[0].travel_request.no_reg.ToString());

                    tb_m_budget budget = await GetData.GetCostWbs((bool)ListModel[0].travel_request.overseas_flag, division_r.Trim());

                    ViewBag.budget = budget.available_amount;
                    ViewBag.wbs = budget.eoa_wbs_no;
                    ViewBag.costcenter = budget.cost_center;

                    List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();
                    bankName = await GetData.VendorEmployee(Convert.ToInt32(model.employee_info.code));
                    if (bankName.Count != 0)
                    {
                        ViewBag.bankName = bankName[0].Bank_Name;
                        ViewBag.bankAccount = bankName[0].account_number;
                    }
                    else
                    {
                        ViewBag.bankName = "- Not Available -";
                        ViewBag.bankAccount = "- Not Available -";
                    }

                    double? meal = 0, hotel = 0, ticket = 0, total = 0;
                    int? days = 0;

                    for (int k = 0; k < ListModel.Count; k++)
                    {
                        meal = meal + ListModel[k].travel_request.allowance_meal_idr;
                        hotel = hotel + ListModel[k].travel_request.allowance_hotel;
                        ticket = ticket + ListModel[k].travel_request.allowance_ticket;
                        total = total + ListModel[k].travel_request.grand_total_allowance;
                        days = days + ListModel[k].travel_request.duration;
                    }

                    ViewBag.Duration = days;
                    ViewBag.Meal = meal;
                    ViewBag.Hotel = hotel;
                    ViewBag.Ticket = ticket;
                    ViewBag.Total = total;

                    return View(ListModel);
                    // Do stuff
                }
                else
                {
                    ViewBag.RL = await GetData.DestinationInfo();
                    ViewBag.RL2 = await GetData.PurposeInfo();
                    //list participant in string

                    List<string> ModelList = new List<string>();
                    if (model.participants != null)
                    {
                        for (int k = 0; k < model.participants.Count(); k++)
                        {
                            ModelList.Add(await GetData.EmployeeNameInfo(model.participants[k].no_reg));
                        }
                    }
                    ViewBag.RL3 = ModelList;

                    string boss = await GetData.EmployeeNameInfo(model.travel_request.assign_by);
                    if (boss == null) boss = "-No data";
                    ViewBag.Bossname = "Assigned by " + boss.Trim() + " (" + model.travel_request.assign_by.ToString().Trim() + ")";

                    List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();
                    bankName = await GetData.VendorEmployee(Convert.ToInt32(model.employee_info.code));
                    if (bankName.Count != 0)
                    {
                        model.tbankname = bankName[0].Bank_Name;
                        model.tbankaccount = bankName[0].account_number;
                    }

                    tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(model.employee_info.code));
                    division.Divisi = division.Divisi.Replace("and1", "&");
                    ViewBag.division_name = division.Divisi;


                    ViewBag.Username = model.employee_info.name;
                    return View("Index", model);
                }

            }
            else
            if (add != null)
            {
                //List<tb_m_vendor_employee> valid = await GetData.VendorEmployee(Convert.ToInt32(model.tparticipant));
                //if (ModelState.IsValid || valid.Count > 0)
                //{
                List<string> ModelList = new List<string>();
                int noreg;
                string exist = "";

                if (model.tparticipant != null && int.TryParse(model.tparticipant, out noreg))
                {
                    if (model.participants == null)
                        model.participants = new List<tb_r_travel_request_participant>();
                    foreach (var item in model.participants)
                    {
                        if (item.no_reg == noreg) exist = await GetData.EmployeeNameInfo(noreg);
                    }
                    if (exist == "")
                    {
                        model.participants.Add(new tb_r_travel_request_participant());
                        model.participants[model.participants.Count() - 1].no_reg_parent = Convert.ToInt32(model.employee_info.code);
                        model.participants[model.participants.Count() - 1].active_flag = true;
                        model.participants[model.participants.Count() - 1].no_reg = Convert.ToInt32(noreg);

                        model.travel_request.participants_flag = true;
                    }

                }
                ViewBag.Exist = exist;
                if (model.participants != null)
                {
                    for (int k = 0; k < model.participants.Count(); k++)
                    {
                        ModelList.Add(await GetData.EmployeeNameInfo(model.participants[k].no_reg));
                    }
                }

                ModelState.Remove(ModelState.FirstOrDefault(m => m.Key.ToString().StartsWith("tparticipant")));
                model.tparticipant = "";

                List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();
                bankName = await GetData.VendorEmployee(Convert.ToInt32(model.employee_info.code));
                if (bankName.Count != 0)
                {
                    model.tbankname = bankName[0].Bank_Name;
                    model.tbankaccount = bankName[0].account_number;
                }

                tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(model.employee_info.code));
                division.Divisi = division.Divisi.Replace("and1", "&");
                ViewBag.division_name = division.Divisi;

                ViewBag.RL3 = ModelList;

                ViewBag.RL = await GetData.DestinationInfo();
                ViewBag.RL2 = await GetData.PurposeInfo();
                ViewBag.Bossname = "Assigned by " + await GetData.EmployeeNameInfo(model.travel_request.assign_by) + " (" + model.travel_request.assign_by.ToString() + ")";
                if (ViewBag.Bossname == null) ViewBag.Bossname = "-No data";
                return View("Index", model);               
            }
            else
            if (delete != "")
            {
                int del = Convert.ToInt32(delete);
                TravelRequestHelper temp = model;
                tb_r_travel_request_participant hold = new tb_r_travel_request_participant();
                int count = model.participants.Count;
                temp = model;
                temp.participants.RemoveAt(del);
                List<string> ModelList = new List<string>();

                temp.travel_request.participants_flag = true;
                for (int k = 0; k < temp.participants.Count(); k++)
                {
                    ModelList.Add(await GetData.EmployeeNameInfo(temp.participants[k].no_reg));
                }

                //ModelState.Remove(ModelState..FirstOrDefault(m => m.Key.ToString().StartsWith("participants")));
                while (ModelState.FirstOrDefault(ms => ms.Key.ToString().StartsWith("participants")).Value != null)
                {
                    ModelState.Remove(ModelState.FirstOrDefault(ms => ms.Key.ToString().StartsWith("participants")));
                }
                model.tparticipant = "";

                List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();
                bankName = await GetData.VendorEmployee(Convert.ToInt32(model.employee_info.code));
                if (bankName.Count != 0)
                {
                    temp.tbankname = bankName[0].Bank_Name;
                    temp.tbankaccount = bankName[0].account_number;
                }

                tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(model.employee_info.code));
                ViewBag.division_name = division.Divisi;
                division.Divisi = division.Divisi.Replace("and1", "&");

                ViewBag.RL3 = ModelList;

                ViewBag.RL = await GetData.DestinationInfo();
                ViewBag.RL2 = await GetData.PurposeInfo();
                ViewBag.Bossname = "Assigned by " + await GetData.EmployeeNameInfo(model.travel_request.assign_by) + " (" + model.travel_request.assign_by.ToString() + ")";
                return View("Index", temp);

            }
            else 
            if (clear == "")
            {
                TravelRequestHelper temp = model;
                temp = model;
                List<string> ModelList = new List<string>();

                List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();
                bankName = await GetData.VendorEmployee(Convert.ToInt32(model.employee_info.code));
                if (bankName.Count != 0)
                {
                    temp.tbankname = bankName[0].Bank_Name;
                    temp.tbankaccount = bankName[0].account_number;
                }

                tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(model.employee_info.code));
                ViewBag.division_name = division.Divisi;
                division.Divisi = division.Divisi.Replace("and1", "&");

                ViewBag.RL3 = ModelList;

                ViewBag.RL = await GetData.DestinationInfo();
                ViewBag.RL2 = await GetData.PurposeInfo();
                ViewBag.Bossname = "Assigned by " + await GetData.EmployeeNameInfo(model.travel_request.assign_by) + " (" + model.travel_request.assign_by.ToString() + ")";
                return View("Index", temp);

            }
            else
                return RedirectToAction("Index");
        }


        //[HttpPost]
        public async Task<ActionResult> IndexMSTR(string noreg, string dvc)
        {
            //string noreg = "101799";
            tb_m_employee model = new tb_m_employee();
            model = await GetData.EmployeeInfo(noreg);

            tb_m_employee created = await GetData.EmployeeInfo(model.code);
            ViewBag.loged_id = created.code.Trim(' ');
            ViewBag.loged_name = created.name.Trim(' ');

            //tb_m_employee model = await GetData.EmployeeInfo("101495");

            //Get user name
            ViewBag.Username = model.name;

            //Get destination list info for dropdown list
            ViewBag.RL = await GetData.DestinationInfo();

            //Get purpose list info for travel purpose list
            ViewBag.RL2 = await GetData.PurposeInfo();

            //Prepare travel request information object to be used at view
            TravelRequestHelper model2 = new TravelRequestHelper();

            //Copy user login information
            model2.travel_request = new tb_r_travel_request();
            model2.employee_info = model;

            //Get user direct superior info
            var assignedby = await GetData.AssignedBy(model2.employee_info.unit_code_code);
            tb_m_travel_procedures procedures = new tb_m_travel_procedures();
            if (model2.employee_info.position.Trim() == "SECO" || model2.employee_info.position.Trim() == "SEA" || model2.employee_info.position.Trim() == "SMEC" || model2.employee_info.position.Trim() == "AADV" || model2.employee_info.position.Trim() == "GM")
            {
                procedures = await GetData.Procedures(model2.employee_info.position);
            }
            else
                procedures = await GetData.Procedures(model2.employee_info.@class);

            if (assignedby.pd == Convert.ToInt32(model2.employee_info.code)) model2.travel_request.assign_by = Convert.ToInt32(model2.employee_info.code);
            else
            //If the direct superior is DH, then get who is the DH from travel procedure table
            if (procedures.apprv_by_lvl1 == "DH") model2.travel_request.assign_by = assignedby.dh_code;
            else
            //If the direct superior is Div Director, then get who is the Div Director from travel procedure table
            if (procedures.apprv_by_lvl1 == "Div Director") model2.travel_request.assign_by = assignedby.director;
            //If the direct superior is vice president, then get who is the VP from travel procedure table
            else
            if (procedures.apprv_by_lvl1 == "VP") model2.travel_request.assign_by = assignedby.vp;
            else
            if (procedures.apprv_by_lvl1 == "EGM") model2.travel_request.assign_by = assignedby.egm;

            //if still empty - for special case
            if (model2.travel_request.assign_by == null)
            {
                if (assignedby.dh_code != null) model2.travel_request.assign_by = assignedby.dh_code;
                else
                    if (assignedby.egm != null) model2.travel_request.assign_by = assignedby.egm;
                else
                    if (assignedby.director != null) model2.travel_request.assign_by = assignedby.director;
                else
                    if (assignedby.local_fd != null) model2.travel_request.assign_by = assignedby.local_fd;
                else
                    if (assignedby.japan_fd != null) model2.travel_request.assign_by = assignedby.japan_fd;
                else
                    if (assignedby.vp != null) model2.travel_request.assign_by = assignedby.vp;
                else
                    if (assignedby.pd != null) model2.travel_request.assign_by = assignedby.pd;
            }

            //Get user direct superior name
            string boss = await GetData.EmployeeNameInfo(model2.travel_request.assign_by);
            if (boss == null) boss = "-No Data";
            ViewBag.Bossname = "Assigned by " + boss.Trim() + " (" + model2.travel_request.assign_by.ToString().Trim() + ")";
            ViewBag.Bossname2 = boss != null ? boss.Trim() : "No supperior registered";

            //Set request type default to false
            model2.travel_request.request_type = false;

            //Set activity id default to 3 (Regular)
            model2.travel_request.id_activity = 3;

            List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();
            bankName = await GetData.VendorEmployee(Convert.ToInt32(model.code));
            if (bankName.Count != 0)
            {
                model2.tbankname = bankName[0].Bank_Name;
                model2.tbankaccount = bankName[0].account_number;
            }
            else
            {
                ViewBag.ebankname = "No bank account registered,contact finance division";
                ViewBag.ebankaccount = "No bank name registered,contact finance division";
            }

            //special employee
            //List<tb_m_special_employee> special_model = await GetData.SpecialEmployee(identity.Name);
            List<tb_m_special_employee> special_model = await GetData.SpecialEmployee(model.code);
            if (special_model.Count != 0)
            {
                List<string> division_model = new List<string>();
                for (int k = 0; k < special_model.Count; k++)
                {
                    division_model.Add(special_model[k].Divisi);
                }
                model2.special_employee_flag = true;
                model2.travel_request.exep_empolyee = true;
                var selectListItems = division_model.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();
                ViewBag.division_name2 = selectListItems;
            }
            else
            {
                tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(model.code));
                division.Divisi = division.Divisi.Replace("and1", "&");
                ViewBag.division_name = division.Divisi;
                model2.special_employee_flag = false;
            }

            ViewBag.Username = model2.employee_info.name;

            var headers = Request.Headers.GetValues("User-Agent");
            string userAgent = string.Join(" ", headers);

            if (userAgent.ToLower().Contains("ipad"))
                return View("IndexMSTR", model2);
            else
                return View("IndexMSTRMobile", model2);

        }

        [HttpPost]
        public async Task<ActionResult> ValidateMSTR(TravelRequestHelper model, string validate, string add, string delete = "", string loged = "")
        {
            tb_m_employee created = await GetData.EmployeeInfo(model.employee_info.code);
            ViewBag.loged_id = created.code.Trim(' ');
            ViewBag.loged_name = created.name.Trim(' ');

            if (validate != null && model.travel_request != null)
            {
                if (ModelState.IsValid)
                {
                    if (Request.Files["generaldoc"] != null)
                    {
                        HttpPostedFileBase file = Request.Files["generaldoc"];
                        model.generaldoc_file = file;
                    }
                    if (Request.Files["itinerarydoc"] != null)
                    {
                        HttpPostedFileBase file = Request.Files["itinerarydoc"];
                        model.itinerary_file = file;
                    }
                    if (Request.Files["invitationdoc"] != null)
                    {
                        HttpPostedFileBase file = Request.Files["invitationdoc"];
                        model.invitation_file = file;
                    }
                    if (Request.Files["proposaldoc"] != null)
                    {
                        HttpPostedFileBase file = Request.Files["proposaldoc"];
                        model.proposaldoc_file = file;
                    }

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<tb_r_travel_request, tb_r_travel_request>();
                    });
                    IMapper mapper = config.CreateMapper();

                    model.travel_request.no_reg = Convert.ToInt32(model.employee_info.code);

                    DateTime now = DateTime.Now;

                    ViewBag.Title = model.employee_info.code;

                    ViewBag.Username = model.employee_info.name;
                    model.travel_request.active_flag = false;
                    model.travel_request.status_request = "0";
                    model.travel_request.comments = "Comment";

                    model.travel_request.exep_empolyee = model.travel_request.exep_empolyee;
                    model.travel_request.additional2 = model.employee_info.unit_code_name;


                    model.travel_request.invited_by = model.travel_request.no_reg;
                    model.travel_request.create_date = now;

                    //list participant in string
                    if (model.participants != null)
                    {
                        List<string> ModelList = new List<string>();
                        for (int k = 0; k < model.participants.Count(); k++)
                        {
                            ModelList.Add(await GetData.EmployeeNameInfo(model.participants[k].no_reg));
                        }
                        ViewBag.RL3 = ModelList;
                    }
                    List<TravelRequestHelper> ListModel = new List<TravelRequestHelper>();

                    for (int c = 0; c < 3; c++)
                    {
                        {
                            //if (model.tend_date[c] == null) break;
                            if (c == 0 && model.tend_date0 == null) break;
                            else
                            if (c == 1 && model.tend_date1 == null) break;
                            else
                            if (c == 2 && model.tend_date2 == null) break;

                            ListModel.Add(new TravelRequestHelper());
                            ListModel[c].employee_info = new tb_m_employee();
                            ListModel[c].employee_info = model.employee_info;
                            ListModel[c].travel_request = new tb_r_travel_request();
                            ListModel[c].travel_request = mapper.Map<tb_r_travel_request>(model.travel_request);

                            //travel documents
                            if (model.generaldoc_file.FileName != "")
                            {
                                ListModel[c].generaldoc_file = model.generaldoc_file;
                                ListModel[c].travel_request.path_general = Utility.UploadFileUniversal(ListModel[c].generaldoc_file, Constant.TravelDocumentsFolder, "GENERAL_" + ListModel[c].travel_request.no_reg + "_" + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                            }
                            if (model.invitation_file.FileName != "")
                            {
                                ListModel[c].invitation_file = model.invitation_file;
                                ListModel[c].travel_request.path_invitation = Utility.UploadFileUniversal(ListModel[c].invitation_file, Constant.TravelDocumentsFolder, "INVITE_" + ListModel[c].travel_request.no_reg + "_" + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                            }
                            if (model.itinerary_file.FileName != "")
                            {
                                ListModel[c].itinerary_file = model.itinerary_file;
                                ListModel[c].travel_request.path_itinerary = Utility.UploadFileUniversal(ListModel[c].itinerary_file, Constant.TravelDocumentsFolder, "PLAN_" + ListModel[c].travel_request.no_reg + "_" + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                            }

                            if (model.proposaldoc_file.FileName != "")
                            {
                                ListModel[c].proposaldoc_file = model.proposaldoc_file;
                                ListModel[c].travel_request.additional1 = Utility.UploadFileUniversal(ListModel[c].proposaldoc_file, Constant.TravelDocumentsFolder, "PROPOSAL_" + ListModel[c].travel_request.no_reg + "_" + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                            }

                            if (c == 0)
                            {
                                ListModel[c].travel_request.start_date = model.tstart_date0;
                                ListModel[c].travel_request.end_date = model.tend_date0;
                            }
                            else
                            if (c == 1)
                            {
                                ListModel[c].travel_request.start_date = model.tstart_date1;
                                ListModel[c].travel_request.end_date = model.tend_date1;
                            }
                            else
                            if (c == 2)
                            {
                                ListModel[c].travel_request.start_date = model.tstart_date2;
                                ListModel[c].travel_request.end_date = model.tend_date2;
                            }

                            ListModel[c].travel_request.additional2 = model.travel_request.additional2;

                            ListModel[c].travel_request.id_destination_city = model.tid_destination_city[c];
                            ListModel[c].travel_request.air_ticket_flag = model.tair_ticket_flag[c];
                            ListModel[c].travel_request.destination_code = await GetData.RegionInfo(ListModel[c].travel_request.id_destination_city);

                            //ListModel[c].travel_request.overseas_flag = model.toverseas_flag[c];
                            ListModel[c].travel_request.user_created = Convert.ToInt32(created.code);
                            ListModel[c].travel_request.reason_of_assigment = model.treason;
                            ListModel[c].travel_request.travel_purpose = model.tpurpose;
                            ListModel[c].travel_request.id_activity = model.tactivity;
                            ListModel[c].travel_request.status_request = model.travel_request.status_request;

                            if (ListModel[c].travel_request.destination_code == 4) ListModel[c].travel_request.overseas_flag = false;
                            else
                                ListModel[c].travel_request.overseas_flag = true;

                            //cek partisipan
                            if (model.participants != null)
                            {
                                ListModel[c].participants = new List<tb_r_travel_request_participant>();
                                ListModel[c].participants = model.participants;
                            }
                            //c++;
                            //if (c > model.tend_date.Count()) break;
                        }
                        //if (model.tend_date.Count() == c) break;
                    }

                    //hitung
                    for (int i = 0; i < ListModel.Count(); i++)
                    {
                        bool same_date = false;
                        int week_day = 0;
                        TimeSpan range = ((DateTime)ListModel[i].travel_request.end_date).Date - ((DateTime)ListModel[i].travel_request.start_date).Date;
                       
                        if (ListModel.Count() > 1 && i > 0)
                        {
                            if (Convert.ToDateTime(ListModel[i].travel_request.start_date).Date == Convert.ToDateTime(ListModel[i - 1].travel_request.end_date).Date) same_date = true;
                        }
                        for (int k = 0; k <= range.Days; k++)
                        {
                            if (Convert.ToDateTime(ListModel[i].travel_request.start_date).AddDays(k).DayOfWeek == DayOfWeek.Saturday || Convert.ToDateTime(ListModel[i].travel_request.start_date).AddDays(k).DayOfWeek == DayOfWeek.Sunday) week_day++;
                        }

                        int duration = range.Days;
                        if (!same_date) duration = duration + 1;

                        ListModel[i].travel_request.duration = duration;

                        var mealwinterallowance = await GetData.RateMealWinterInfo(ListModel[i]);
                        ListModel[i].travel_request.allowance_meal_idr = (mealwinterallowance.meal_allowance * duration) + (mealwinterallowance.meal_allowance * week_day);

                        // cek winter gak?
                        ///*
                        if (ListModel[i].travel_request.start_date.Value.Month == 12 || ListModel[i].travel_request.start_date.Value.Month == 1 || ListModel[i].travel_request.start_date.Value.Month == 2)
                        {
                            ListModel[i].travel_request.allowance_winter = mealwinterallowance.winter_allowance;
                        }
                        else
                            ListModel[i].travel_request.allowance_winter = 0;

                        if (ListModel[i].travel_request.air_ticket_flag == true)
                        {
                            var rateflight = await GetData.RateFlightInfo(ListModel[i]);
                            ListModel[i].travel_request.allowance_ticket = (rateflight.economy) * 2;
                        }
                        else
                        {
                            var rateland = await GetData.Procedures(ListModel[i].employee_info.@class);
                            ListModel[i].travel_request.allowance_ticket = (Convert.ToInt32(rateland.land_transport)) * 2;
                        }

                        var ratehotel = await GetData.RateHotelInfo(ListModel[i]);

                        if (!same_date)
                        {
                            if (ListModel[i].travel_request.overseas_flag == true) ListModel[i].travel_request.allowance_hotel = ratehotel.overseas * (duration - 1);
                            else
                                ListModel[i].travel_request.allowance_hotel = ratehotel.domestik * (duration - 1);
                        }
                        else
                        {
                            if (ListModel[i].travel_request.overseas_flag == true) ListModel[i].travel_request.allowance_hotel = ratehotel.overseas * duration;
                            else
                                ListModel[i].travel_request.allowance_hotel = ratehotel.domestik * duration;
                        }

                        ListModel[i].travel_request.apprv_flag_lvl1 = "0";
                        ListModel[i].travel_request.allowance_preparation = 0;
                        ListModel[i].travel_request.grand_total_allowance = (ListModel[i].travel_request.allowance_meal_idr != null ? ListModel[i].travel_request.allowance_meal_idr : 0) +
                                                                            (ListModel[i].travel_request.allowance_hotel != null ? ListModel[i].travel_request.allowance_hotel : 0) +
                                                                            (ListModel[i].travel_request.allowance_preparation != null ? ListModel[i].travel_request.allowance_preparation : 0) +
                                                                            (ListModel[i].travel_request.allowance_ticket != null ? ListModel[i].travel_request.allowance_ticket : 0) +
                                                                            (ListModel[i].travel_request.allowance_winter != null ? ListModel[i].travel_request.allowance_winter : 0)
                                                                            ;
                        ListModel[i].travel_request.create_date = now;

                        if (ListModel[i].participants != null)
                        {
                            for (int k = 0; k < ListModel[i].participants.Count(); k++)
                            {
                                ListModel[i].participants[k].created_date = now;
                                ListModel[i].participants[k].active_flag = true;
                                ListModel[i].participants[k].no_reg_parent = Convert.ToInt32(ListModel[i].employee_info.code.Trim());
                            }
                            ListModel[i].travel_request.participants_flag = true;
                        }
                        else
                            for (int k = 0; k < ListModel.Count(); k++)
                            {
                                ListModel[k].travel_request.participants_flag = false;
                            }
                        //for exep employee
                        ListModel[i].travel_request.exep_empolyee = false;
                    }

                    ///*
                    if (model.participants != null)
                        model.travel_request.participants_flag = true;
                    else
                        model.travel_request.participants_flag = false;

                    //var stringArray = new string[3] { await GetData.DestinationNameInfo(ListModel[0].travel_request.id_destination_city), await GetData.DestinationNameInfo(ListModel[1].travel_request.id_destination_city), await GetData.DestinationNameInfo(ListModel[2].travel_request.id_destination_city) };
                    ViewBag.Destination = new string[ListModel.Count()];
                    for (int k = 0; k < ListModel.Count(); k++)
                    {
                        ViewBag.Destination[k] = await GetData.DestinationNameInfo(ListModel[k].travel_request.id_destination_city);
                    }
                    ViewBag.Bossname = await GetData.EmployeeNameInfo(model.travel_request.assign_by);
                    if (ViewBag.Bossname == null) ViewBag.Bossname = "-No Data";
                    //isi wbs no and cost center
                    tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(model.employee_info.code));
                    division.Divisi = division.Divisi.Replace("and1", "&");
                    ViewBag.division_name = division.Divisi;

                    string division_r = await GetData.GetDivMapping(ListModel[0].travel_request.no_reg.ToString());

                    tb_m_budget budget = await GetData.GetCostWbs((bool)ListModel[0].travel_request.overseas_flag, division_r.Trim());

                    ViewBag.budget = budget.available_amount;
                    ViewBag.wbs = budget.eoa_wbs_no;
                    ViewBag.costcenter = budget.cost_center;

                    List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();
                    bankName = await GetData.VendorEmployee(Convert.ToInt32(model.employee_info.code));
                    if (bankName.Count != 0)
                    {
                        ViewBag.bankName = bankName[0].Bank_Name;
                        ViewBag.bankAccount = bankName[0].account_number;
                    }
                    else
                    {
                        ViewBag.bankName = "- Not Available -";
                        ViewBag.bankAccount = "- Not Available -";
                    }

                    double? meal = 0, hotel = 0, ticket = 0, total = 0;
                    int? days = 0;

                    for (int k = 0; k < ListModel.Count; k++)
                    {
                        meal = meal + ListModel[k].travel_request.allowance_meal_idr;
                        hotel = hotel + ListModel[k].travel_request.allowance_hotel;
                        ticket = ticket + ListModel[k].travel_request.allowance_ticket;
                        total = total + ListModel[k].travel_request.grand_total_allowance;
                        days = days + ListModel[k].travel_request.duration;
                    }

                    ViewBag.Duration = days;
                    ViewBag.Meal = meal;
                    ViewBag.Hotel = hotel;
                    ViewBag.Ticket = ticket;
                    ViewBag.Total = total;

                    return View(ListModel);
                    // Do stuff
                }
                else
                {
                    ViewBag.RL = await GetData.DestinationInfo();
                    ViewBag.RL2 = await GetData.PurposeInfo();
                    //list participant in string

                    List<string> ModelList = new List<string>();
                    if (model.participants != null)
                    {
                        for (int k = 0; k < model.participants.Count(); k++)
                        {
                            ModelList.Add(await GetData.EmployeeNameInfo(model.participants[k].no_reg));
                        }
                    }
                    ViewBag.RL3 = ModelList;

                    string boss = await GetData.EmployeeNameInfo(model.travel_request.assign_by);
                    if (boss == null) boss = "-No data";
                    ViewBag.Bossname = "Assigned by " + boss.Trim() + " (" + model.travel_request.assign_by.ToString().Trim() + ")";

                    List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();
                    bankName = await GetData.VendorEmployee(Convert.ToInt32(model.employee_info.code));
                    if (bankName.Count != 0)
                    {
                        model.tbankname = bankName[0].Bank_Name;
                        model.tbankaccount = bankName[0].account_number;
                    }
                    //else
                    //{
                    //    model.tbankname    = "- Not Available -";
                    //    model.tbankaccount = "- Not Available -";
                    //}

                    tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(model.employee_info.code));
                    division.Divisi = division.Divisi.Replace("and1", "&");
                    ViewBag.division_name = division.Divisi;


                    ViewBag.Username = model.employee_info.name;
                    //return View("Index", model);
                    var headers = Request.Headers.GetValues("User-Agent");
                    string userAgent = string.Join(" ", headers);

                    if (userAgent.ToLower().Contains("ipad"))
                        return View("IndexMSTR", model);
                    else
                        return View("IndexMSTRMobile", model);
                }

            }
            else
            if (add != null)
            {
                //List<tb_m_vendor_employee> valid = await GetData.VendorEmployee(Convert.ToInt32(model.tparticipant));
                //if (ModelState.IsValid || valid.Count > 0)
                //{
                List<string> ModelList = new List<string>();
                int noreg;
                string exist = "";

                if (model.tparticipant != null && int.TryParse(model.tparticipant, out noreg))
                {
                    if (model.participants == null)
                        model.participants = new List<tb_r_travel_request_participant>();
                    foreach (var item in model.participants)
                    {
                        if (item.no_reg == noreg) exist = await GetData.EmployeeNameInfo(noreg);
                    }
                    if (exist == "")
                    {
                        model.participants.Add(new tb_r_travel_request_participant());
                        model.participants[model.participants.Count() - 1].no_reg_parent = Convert.ToInt32(model.employee_info.code);
                        model.participants[model.participants.Count() - 1].active_flag = true;
                        model.participants[model.participants.Count() - 1].no_reg = Convert.ToInt32(noreg);

                        model.travel_request.participants_flag = true;
                    }

                }
                ViewBag.Exist = exist;
                if (model.participants != null)
                {
                    for (int k = 0; k < model.participants.Count(); k++)
                    {
                        ModelList.Add(await GetData.EmployeeNameInfo(model.participants[k].no_reg));
                    }
                }

                ModelState.Remove(ModelState.FirstOrDefault(m => m.Key.ToString().StartsWith("tparticipant")));
                model.tparticipant = "";

                List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();
                bankName = await GetData.VendorEmployee(Convert.ToInt32(model.employee_info.code));
                if (bankName.Count != 0)
                {
                    model.tbankname = bankName[0].Bank_Name;
                    model.tbankaccount = bankName[0].account_number;
                }

                tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(model.employee_info.code));
                division.Divisi = division.Divisi.Replace("and1", "&");
                ViewBag.division_name = division.Divisi;

                ViewBag.RL3 = ModelList;

                ViewBag.RL = await GetData.DestinationInfo();
                ViewBag.RL2 = await GetData.PurposeInfo();
                ViewBag.Bossname = "Assigned by " + await GetData.EmployeeNameInfo(model.travel_request.assign_by) + " (" + model.travel_request.assign_by.ToString() + ")";
                if (ViewBag.Bossname == null) ViewBag.Bossname = "-No data";
                //return View("Index", model);
                //}
                //else
                //{
                //    ViewBag.RL = await GetData.DestinationInfo();
                //    ViewBag.RL2 = await GetData.PurposeInfo();
                //    //list participant in string

                //    List<string> ModelList = new List<string>();
                //    if (model.participants != null)
                //    {
                //        for (int k = 0; k < model.participants.Count(); k++)
                //        {
                //            ModelList.Add(await GetData.EmployeeNameInfo(model.participants[k].no_reg));
                //        }
                //    }
                //    ViewBag.RL3 = ModelList;

                //    string boss = await GetData.EmployeeNameInfo(model.travel_request.assign_by);
                //    if (boss == null) boss = "-No data";
                //    ViewBag.Bossname = "Assigned by " + boss.Trim() + " (" + model.travel_request.assign_by.ToString().Trim() + ")";

                //    List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();
                //    bankName = await GetData.VendorEmployee(Convert.ToInt32(model.employee_info.code));
                //    if (bankName.Count != 0)
                //    {
                //        model.tbankname = bankName[0].Bank_Name;
                //        model.tbankaccount = bankName[0].account_number;
                //    }
                //    //else
                //    //{
                //    //    model.tbankname    = "- Not Available -";
                //    //    model.tbankaccount = "- Not Available -";
                //    //}

                //    tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(model.employee_info.code));
                //    division.Divisi = division.Divisi.Replace("and1", "&");
                //    ViewBag.division_name = division.Divisi;


                //    ViewBag.Username = model.employee_info.name;
                //    return View("Index", model);
                //}
                //return View("Index", model);
                var headers = Request.Headers.GetValues("User-Agent");
                string userAgent = string.Join(" ", headers);

                if (userAgent.ToLower().Contains("ipad"))
                    return View("IndexMSTR", model);
                else
                    return View("IndexMSTRMobile", model);
            }
            else
            if (delete != "")
            {
                int del = Convert.ToInt32(delete);
                TravelRequestHelper temp = model;
                tb_r_travel_request_participant hold = new tb_r_travel_request_participant();
                int count = model.participants.Count;
                temp = model;
                temp.participants.RemoveAt(del);
                List<string> ModelList = new List<string>();

                temp.travel_request.participants_flag = true;
                for (int k = 0; k < temp.participants.Count(); k++)
                {
                    ModelList.Add(await GetData.EmployeeNameInfo(temp.participants[k].no_reg));
                }

                //ModelState.Remove(ModelState..FirstOrDefault(m => m.Key.ToString().StartsWith("participants")));
                while (ModelState.FirstOrDefault(ms => ms.Key.ToString().StartsWith("participants")).Value != null)
                {
                    ModelState.Remove(ModelState.FirstOrDefault(ms => ms.Key.ToString().StartsWith("participants")));
                }
                model.tparticipant = "";

                List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();
                bankName = await GetData.VendorEmployee(Convert.ToInt32(model.employee_info.code));
                if (bankName.Count != 0)
                {
                    temp.tbankname = bankName[0].Bank_Name;
                    temp.tbankaccount = bankName[0].account_number;
                }

                tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(model.employee_info.code));
                ViewBag.division_name = division.Divisi;
                division.Divisi = division.Divisi.Replace("and1", "&");

                ViewBag.RL3 = ModelList;

                ViewBag.RL = await GetData.DestinationInfo();
                ViewBag.RL2 = await GetData.PurposeInfo();
                ViewBag.Bossname = "Assigned by " + await GetData.EmployeeNameInfo(model.travel_request.assign_by) + " (" + model.travel_request.assign_by.ToString() + ")";
                //return View("Index", temp);
                //header
                var headers = Request.Headers.GetValues("User-Agent");
                string userAgent = string.Join(" ", headers);

                //return View("IndexMSTR", model);
                if (userAgent.ToLower().Contains("ipad"))
                    return View("IndexMSTR", temp);
                else
                    return View("IndexMSTRMobile", temp);

            }
            //else
            //if (clear == "")
            //{
            //    TravelRequestHelper temp = model;
            //    temp = model;
            //    List<string> ModelList = new List<string>();

            //    List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();
            //    bankName = await GetData.VendorEmployee(Convert.ToInt32(model.employee_info.code));
            //    if (bankName.Count != 0)
            //    {
            //        temp.tbankname = bankName[0].Bank_Name;
            //        temp.tbankaccount = bankName[0].account_number;
            //    }

            //    tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(model.employee_info.code));
            //    ViewBag.division_name = division.Divisi;
            //    division.Divisi = division.Divisi.Replace("and1", "&");

            //    ViewBag.RL3 = ModelList;

            //    ViewBag.RL = await GetData.DestinationInfo();
            //    ViewBag.RL2 = await GetData.PurposeInfo();
            //    ViewBag.Bossname = "Assigned by " + await GetData.EmployeeNameInfo(model.travel_request.assign_by) + " (" + model.travel_request.assign_by.ToString() + ")";
            //    return View("Index", temp);

            //}
            else
                return RedirectToAction("IndexMSTR");

        }

        public ActionResult Details(TravelRequestHelper model)
        {
            // from the storeId value, get the entity/object/resource
            List<TravelRequestHelper> ListModel = new List<TravelRequestHelper>();
            ListModel.Add(model);
            return View("Validate", ListModel); // view to render when we get invalid store id
        }

        public async System.Threading.Tasks.Task<ActionResult> SubmittedMSTR(TravelRequestHelper[] ListModel)
        {

            for (int k = 0; k < ListModel.Count(); k++)
            {
                await InsertData.TravelRequest(ListModel[k]);
                if (ListModel[k].travel_request.participants_flag == true)
                {
                    if (k <= ListModel.Count())
                    {
                        for (int i = 0; i < ListModel[k].participants.Count; i++)
                            await InsertData.TravelParticipant(ListModel[k].participants[i]);
                    }
                }
            }
            ViewBag.Username = ListModel[0].employee_info.name;

            return View("SubmittedMSTR");
            //return RedirectToAction("Index");
        }

    }
}
