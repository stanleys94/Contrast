using AutoMapper;
using CONTRAST_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CONTRAST_WEB.Controllers
{
    public class TravelConfirmationController : Controller
    {

        // GET: TravelConfirmation
        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Index(tb_m_employee model)
        {
            List<tb_r_travel_dit> DITList = new List<tb_r_travel_dit>();
            DITList = await GetData.TravelDitListInfo(model.code);

            return View(DITList);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Details(tb_r_travel_dit model)
        {

            //Get destination list info for dropdown list
            ViewBag.RL = await GetData.DestinationInfo();

            //Get purpose list info for travel purpose list
            ViewBag.RL2 = await GetData.PurposeInfo();

            //Prepare travel request information object to be used at view
            TravelRequestHelper model2 = new TravelRequestHelper();

            //Copy user login information
            model2.travel_request = new tb_r_travel_request();
            model2.employee_info = await GetData.EmployeeInfo(model.no_reg.ToString());

            //Get user direct superior info
            var assignedby = await GetData.AssignedBy(model2.employee_info.unit_code_code);
            var procedures = await GetData.Procedures(model2.employee_info.@class);

            //If the direct superior is DH, then get who is the DH from travel procedure table
            if (procedures.apprv_by_lvl1 == "DH") model2.travel_request.assign_by = assignedby.dh_code;
            else
            //If the direct superior is Div Director, then get who is the Div Director from travel procedure table
            if (procedures.apprv_by_lvl1 == "Div Director") model2.travel_request.assign_by = assignedby.director;
            //If the direct superior is vice president, then get who is the VP from travel procedure table
            else
            if (procedures.apprv_by_lvl1 == "VP") model2.travel_request.assign_by = assignedby.vp;

            //if still empty - for special case
            if (model2.travel_request.assign_by == null)
            {
                if (assignedby.dh_code != null) model2.travel_request.assign_by = assignedby.dh_code;
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
            ViewBag.Bossname = "Assigned by " + boss.Trim() + " (" + model2.travel_request.assign_by.ToString().Trim() + ")";

            //Set request type default to false
            model2.travel_request.request_type = false;

            //Set activity id default to 3 (Regular)
            model2.travel_request.id_activity = 3;


            List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();
            bankName = await GetData.VendorEmployee(Convert.ToInt32(model2.employee_info.code));
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

            model2.travel_request.no_reg = model.no_reg;
            model2.tid_destination_city = new int?[1];
            model2.tid_destination_city[0] = model.id_destination_city;
            model2.toverseas_flag = new bool?[1];
            model2.toverseas_flag[0] = model.overseas_flag;
            model2.tpurpose = model.travel_purpose;
            //model2.travel_request.request_type = model.overseas_flag;
            model2.employee_info.code = model.no_reg.ToString();
            model2.travel_request.id_dit = model.id_dit;
            model2.travel_request.request_type = true;



            ViewBag.Ditcity = await GetData.DestinationNameInfo(model2.tid_destination_city[0]);
            //ViewBag.Ditpurpose = await GetData.(model2.tpurpose);
            //Get user name
            ViewBag.Username = model2.employee_info.name;


            return View(model2);
        }

        public async Task<ActionResult> Validate(TravelRequestHelper model, string validate, string add, string delete = "")
        {
            if (validate != null)
            {
                if (ModelState.IsValid)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<tb_r_travel_request, tb_r_travel_request>();
                    });
                    IMapper mapper = config.CreateMapper();

                    model.travel_request.no_reg = Convert.ToInt32(model.employee_info.code);

                    DateTime now = DateTime.Now;

                    ViewBag.Title = model.employee_info.code;
                    //request no reg name
                    //model.employee_info = await GetData.EmployeeInfo(model.employee_info);

                    ViewBag.Username = model.employee_info.name;
                    model.travel_request.active_flag = false;
                    model.travel_request.status_request = "0";
                    model.travel_request.comments = "Comment";

                    model.travel_request.invited_by = model.travel_request.no_reg;
                    //model.travel_request.multiple_destination_flag = false;
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
                    //int c = 0;
                    //while (model.travel_request.multiple_destination_flag == true)
                    {
                        //if (model.tend_date[c] != null && model.tstart_date[c]!= null && model.toverseas_flag[c]!= null && model.tid_destination_city[c]!= null)
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

                            ListModel[c].travel_request.id_destination_city = model.tid_destination_city[c];
                            ListModel[c].travel_request.air_ticket_flag = model.tair_ticket_flag[c];
                            ListModel[c].travel_request.destination_code = await GetData.RegionInfo(ListModel[c].travel_request.id_destination_city);

                            //ListModel[c].travel_request.overseas_flag = model.toverseas_flag[c];
                            ListModel[c].travel_request.user_created = Convert.ToInt32(model.employee_info.code);
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
                        TimeSpan duration = ((DateTime)ListModel[i].travel_request.end_date - (DateTime)ListModel[i].travel_request.start_date);
                        ListModel[i].travel_request.duration = duration.Days;


                        // ListModel[i].travel_request.allowance_meal_idr = EmpInfo.meal_allowance * Convert.ToInt32(duration.Days);
                        var mealwinterallowance = await GetData.RateMealWinterInfo(ListModel[i]);
                        ListModel[i].travel_request.allowance_meal_idr = mealwinterallowance.meal_allowance * Convert.ToInt32(duration.Days);

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

                        if (ListModel[i].travel_request.overseas_flag == true) ListModel[i].travel_request.allowance_hotel = ratehotel.overseas * Convert.ToInt32(duration.Days);
                        else
                            ListModel[i].travel_request.allowance_hotel = ratehotel.domestik * Convert.ToInt32(duration.Days);

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
                    ViewBag.Ditcity = await GetData.DestinationNameInfo(model.tid_destination_city[0]);
                    return View("Details" + "", model);
                }

            }
            else
            if (add != null)
            {
                List<string> ModelList = new List<string>();

                if (model.tparticipant != null)
                {
                    if (model.participants == null)
                        model.participants = new List<tb_r_travel_request_participant>();
                    model.participants.Add(new tb_r_travel_request_participant());
                    model.participants[model.participants.Count() - 1].no_reg_parent = Convert.ToInt32(model.employee_info.code);
                    model.participants[model.participants.Count() - 1].active_flag = true;
                    model.participants[model.participants.Count() - 1].no_reg = Convert.ToInt32(model.tparticipant);



                    model.travel_request.participants_flag = true;
                    for (int k = 0; k < model.participants.Count(); k++)
                    {
                        ModelList.Add(await GetData.EmployeeNameInfo(model.participants[k].no_reg));
                    }

                    ModelState.Remove(ModelState.FirstOrDefault(m => m.Key.ToString().StartsWith("tparticipant")));
                    model.tparticipant = "";
                }

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
                ViewBag.Ditcity = await GetData.DestinationNameInfo(model.tid_destination_city[0]);
                return View("Details", model);

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
                ViewBag.Ditcity = await GetData.DestinationNameInfo(temp.tid_destination_city[0]);

                return View("Details", temp);

            }
            else
                return View();

        }


        public async System.Threading.Tasks.Task<ActionResult> IndexMSTR(string noreg)
        {
            tb_m_employee model = await GetData.EmployeeInfo(noreg);
            List<tb_r_travel_dit> DITList = new List<tb_r_travel_dit>();
            DITList = await GetData.TravelDitListInfo(model.code);

            //return View("IndexMstr",DITList);
            var headers = Request.Headers.GetValues("User-Agent");
            string userAgent = string.Join(" ", headers);

            if (userAgent.ToLower().Contains("ipad"))
                return View("IndexMSTR", DITList);
            else
                return View("IndexMSTRMobile", DITList);
        }

        public async System.Threading.Tasks.Task<ActionResult> DetailsMSTR(tb_r_travel_dit model)
        {

            //Get destination list info for dropdown list
            ViewBag.RL = await GetData.DestinationInfo();

            //Get purpose list info for travel purpose list
            ViewBag.RL2 = await GetData.PurposeInfo();

            //Prepare travel request information object to be used at view
            TravelRequestHelper model2 = new TravelRequestHelper();

            //Copy user login information
            model2.travel_request = new tb_r_travel_request();
            model2.employee_info = await GetData.EmployeeInfo(model.no_reg.ToString());

            //Get user direct superior info
            var assignedby = await GetData.AssignedBy(model2.employee_info.unit_code_code);
            var procedures = await GetData.Procedures(model2.employee_info.@class);

            //If the direct superior is DH, then get who is the DH from travel procedure table
            if (procedures.apprv_by_lvl1 == "DH") model2.travel_request.assign_by = assignedby.dh_code;
            else
            //If the direct superior is Div Director, then get who is the Div Director from travel procedure table
            if (procedures.apprv_by_lvl1 == "Div Director") model2.travel_request.assign_by = assignedby.director;
            //If the direct superior is vice president, then get who is the VP from travel procedure table
            else
            if (procedures.apprv_by_lvl1 == "VP") model2.travel_request.assign_by = assignedby.vp;

            //if still empty - for special case
            if (model2.travel_request.assign_by == null)
            {
                if (assignedby.dh_code != null) model2.travel_request.assign_by = assignedby.dh_code;
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
            ViewBag.Bossname = "Assigned by " + boss.Trim() + " (" + model2.travel_request.assign_by.ToString().Trim() + ")";

            //Set request type default to false
            model2.travel_request.request_type = false;

            //Set activity id default to 3 (Regular)
            model2.travel_request.id_activity = 3;


            List<tb_m_vendor_employee> bankName = new List<tb_m_vendor_employee>();
            bankName = await GetData.VendorEmployee(Convert.ToInt32(model2.employee_info.code));
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

            model2.travel_request.no_reg = model.no_reg;
            model2.tid_destination_city = new int?[1];
            model2.tid_destination_city[0] = model.id_destination_city;
            model2.toverseas_flag = new bool?[1];
            model2.toverseas_flag[0] = model.overseas_flag;
            model2.tpurpose = model.travel_purpose;
            //model2.travel_request.request_type = model.overseas_flag;
            model2.employee_info.code = model.no_reg.ToString();
            model2.travel_request.id_dit = model.id_dit;
            model2.travel_request.request_type = true;

            ViewBag.Ditcity = await GetData.DestinationNameInfo(model2.tid_destination_city[0]);
            //ViewBag.Ditpurpose = await GetData.(model2.tpurpose);
            //Get user name
            ViewBag.Username = model2.employee_info.name;

            //return View("DetailsMSTR",model2);

            var headers = Request.Headers.GetValues("User-Agent");
            string userAgent = string.Join(" ", headers);

            if (userAgent.ToLower().Contains("ipad"))
                return View("DetailsMSTR", model2);
            else
                return View("DetailsMSTRMobile", model2);
        }

        public async Task<ActionResult> ValidateMSTR(TravelRequestHelper model, string validate, string add, string delete = "")
        {
            if (validate != null)
            {
                if (ModelState.IsValid)
                {
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

                    model.travel_request.invited_by = model.travel_request.no_reg;
                    //model.travel_request.multiple_destination_flag = false;
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
                    //int c = 0;
                    //while (model.travel_request.multiple_destination_flag == true)
                    {
                        //if (model.tend_date[c] != null && model.tstart_date[c]!= null && model.toverseas_flag[c]!= null && model.tid_destination_city[c]!= null)
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

                            ListModel[c].travel_request.id_destination_city = model.tid_destination_city[c];
                            ListModel[c].travel_request.air_ticket_flag = model.tair_ticket_flag[c];
                            ListModel[c].travel_request.destination_code = await GetData.RegionInfo(ListModel[c].travel_request.id_destination_city);

                            //ListModel[c].travel_request.overseas_flag = model.toverseas_flag[c];
                            ListModel[c].travel_request.user_created = Convert.ToInt32(model.employee_info.code);
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
                        TimeSpan duration = ((DateTime)ListModel[i].travel_request.end_date - (DateTime)ListModel[i].travel_request.start_date);
                        ListModel[i].travel_request.duration = duration.Days;


                        // ListModel[i].travel_request.allowance_meal_idr = EmpInfo.meal_allowance * Convert.ToInt32(duration.Days);
                        var mealwinterallowance = await GetData.RateMealWinterInfo(ListModel[i]);
                        ListModel[i].travel_request.allowance_meal_idr = mealwinterallowance.meal_allowance * Convert.ToInt32(duration.Days);

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

                        if (ListModel[i].travel_request.overseas_flag == true) ListModel[i].travel_request.allowance_hotel = ratehotel.overseas * Convert.ToInt32(duration.Days);
                        else
                            ListModel[i].travel_request.allowance_hotel = ratehotel.domestik * Convert.ToInt32(duration.Days);

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

                    //isi wbs no and cost center
                    tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(model.employee_info.code));
                    division.Divisi = division.Divisi.Replace("and1", "&");
                    ViewBag.division_name = division.Divisi;

                    string division_r = await GetData.GetDivMapping(ListModel[0].travel_request.no_reg.ToString());

                    tb_m_budget budget = await GetData.GetCostWbs((bool)ListModel[0].travel_request.overseas_flag, division_r.Trim());


                    ViewBag.budget = budget.available_amount;
                    ViewBag.wbs = budget.eoa_wbs_no;
                    ViewBag.costcenter = budget.cost_center;
                    /*
                    ActualCostHelperObject[ck].ActualCost.wbs_no = budget.eoa_wbs_no;
                    ActualCostHelperObject[ck].ActualCost.cost_center = budget.cost_center;
                    */

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
                    
                    var headers = Request.Headers.GetValues("User-Agent");
                    string userAgent = string.Join(" ", headers);

                    if (userAgent.ToLower().Contains("ipad"))
                        return View("ValidateMSTR", ListModel);
                    else
                        return View("ValidateMSTRMobile", ListModel);
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
                    ViewBag.Ditcity = await GetData.DestinationNameInfo(model.tid_destination_city[0]);

                    //return View("DetailsMSTR" + "", model);

                    var headers = Request.Headers.GetValues("User-Agent");
                    string userAgent = string.Join(" ", headers);

                    if (userAgent.ToLower().Contains("ipad"))
                        return View("DetailsMSTR" + "", model);
                    else
                        return View("DetailsMSTRMobile" + "", model);
                }

            }
            else
            if (add != null)
            {
                List<string> ModelList = new List<string>();

                if (model.tparticipant != null)
                {
                    if (model.participants == null)
                        model.participants = new List<tb_r_travel_request_participant>();
                    model.participants.Add(new tb_r_travel_request_participant());
                    model.participants[model.participants.Count() - 1].no_reg_parent = Convert.ToInt32(model.employee_info.code);
                    model.participants[model.participants.Count() - 1].active_flag = true;
                    model.participants[model.participants.Count() - 1].no_reg = Convert.ToInt32(model.tparticipant);

                    model.travel_request.participants_flag = true;
                    for (int k = 0; k < model.participants.Count(); k++)
                    {
                        ModelList.Add(await GetData.EmployeeNameInfo(model.participants[k].no_reg));
                    }

                    ModelState.Remove(ModelState.FirstOrDefault(m => m.Key.ToString().StartsWith("tparticipant")));
                    model.tparticipant = "";
                }

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
                ViewBag.Ditcity = await GetData.DestinationNameInfo(model.tid_destination_city[0]);

                //return View("DetailsMSTR", model);

                var headers = Request.Headers.GetValues("User-Agent");
                string userAgent = string.Join(" ", headers);

                if (userAgent.ToLower().Contains("ipad"))
                    return View("DetailsMSTR", model);
                else
                    return View("DetailsMSTRMobile", model);

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
                ViewBag.Ditcity = await GetData.DestinationNameInfo(temp.tid_destination_city[0]);

                //return View("DetailsMSTR", temp);

                var headers = Request.Headers.GetValues("User-Agent");
                string userAgent = string.Join(" ", headers);

                if (userAgent.ToLower().Contains("ipad"))
                    return View("DetailsMSTR", temp);
                else
                    return View("DetailsMSTRMobile", temp);

            }
            else
                return View();

        }


    }
}