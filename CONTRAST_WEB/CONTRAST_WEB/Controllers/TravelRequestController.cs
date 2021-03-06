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
using static CONTRAST_WEB.Models.Utility;

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

        public async Task<JsonResult> GetSearchValue(string search, string code, string liststring)
        {
            string[] temp = liststring.Split(',');
            List<Class1> ParticipantList = new List<Class1>();
            foreach (var item in temp)
            {
                Class1 temporary = new Class1();
                if (item != "")
                {
                    temporary.name = "";
                    temporary.code = item;
                    ParticipantList.Add(temporary);
                }
            }
            ParticipantList.OrderBy(b => b.code);

            List<Class1> list = new List<Class1>();
            list = await GetData.SearchName(search);
            List<Class1> filtered = new List<Class1>();
            List<int> Index = new List<int>();

            foreach (var item in list)
            {
                if (!item.code.Contains(code))
                {
                    filtered.Add(item);
                }
            }

            for (int k = 0; k < filtered.Count(); k++)
            {
                for (int i = 0; i < ParticipantList.Count(); i++)
                {
                    if (filtered[k].code.Contains(ParticipantList[i].code))
                    {
                        Index.Add(k);
                        ParticipantList.RemoveAt(i);
                        break;
                    }
                }
            }
            foreach (var item in Index)
            {
                filtered.RemoveAt(item);
            }

            return new JsonResult { Data = filtered, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ImportModelStateFromTempData]
        public async Task<ActionResult> Index(string applied = "")
        {
            List<string> noreg_participant = new List<string>();
            var identity = (ClaimsIdentity)User.Identity;
            Utility.Logger(identity.Name);
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;

            tb_m_employee created = await GetData.EmployeeInfo(identity.Name);
            ViewBag.loged_id = created.code.Trim(' ');
            ViewBag.loged_name = created.name.Trim(' ');

            tb_m_employee model = new tb_m_employee();
            //as normal user
            if (applied == "") model = await GetData.EmployeeInfo(identity.Name);
            else
            {
                tb_m_employee coordinated = await GetData.EmployeeInfo(applied);
                if (coordinated.name == null)
                {
                    model = await GetData.EmployeeInfo(identity.Name);
                }
                else
                {
                    List<Class1> CoorEmployee = await GetData.SearchNameDiv(coordinated.name.Trim());
                    if (CoorEmployee.Count > 0)
                    {
                        if (applied.Contains(CoorEmployee[0].code) || applied.Contains(identity.Name.Trim()))
                        {
                            model = await GetData.EmployeeInfo(applied);
                        }
                    }
                    //as travel coordinator

                    if (model.name == null)
                    {
                        model = await GetData.EmployeeInfo(identity.Name);
                    }
                }
            }
            //check special employee
            tb_m_special_employee_new spec_employee = await GetData.GetSpecialNoreg(model.code);

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

            //get employee assigned by
            model2.travel_request.assign_by = await Utility.AssignedBy(model2.employee_info);

            //get division name
            ViewBag.division_name = await GetData.GetDivisionSourceName(Convert.ToInt32(model.code));

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
                model2.tbankname = bankName.First().Bank_Name;
                model2.tbankaccount = bankName.First().account_number;
            }
            else
            {
                ViewBag.ebankname = "No bank account registered,contact finance division";
                ViewBag.ebankaccount = "No bank name registered,contact finance division";
            }

            //special employee check
            List<SelectListItem> SpecAssign = new List<SelectListItem>();
            if (spec_employee.no_reg != null)
            {
                model2.travel_request.exep_empolyee = true;
                if (spec_employee.apprv_lv_1_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_1);
                    temp.Value = spec_employee.apprv_lv_1_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_1_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_2);
                        temp1.Value = spec_employee.apprv_lv_1_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_1_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_3);
                        temp1.Value = spec_employee.apprv_lv_1_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_1_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_4);
                        temp1.Value = spec_employee.apprv_lv_1_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_1_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_5);
                        temp1.Value = spec_employee.apprv_lv_1_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_2_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_1);
                    temp.Value = spec_employee.apprv_lv_2_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_2_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_2);
                        temp1.Value = spec_employee.apprv_lv_2_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_2_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_3);
                        temp1.Value = spec_employee.apprv_lv_2_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_2_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_4);
                        temp1.Value = spec_employee.apprv_lv_2_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_2_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_5);
                        temp1.Value = spec_employee.apprv_lv_2_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_3_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_1);
                    temp.Value = spec_employee.apprv_lv_3_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_3_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_2);
                        temp1.Value = spec_employee.apprv_lv_3_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_3_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_3);
                        temp1.Value = spec_employee.apprv_lv_3_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_3_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_4);
                        temp1.Value = spec_employee.apprv_lv_3_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_3_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_5);
                        temp1.Value = spec_employee.apprv_lv_3_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_4_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_1);
                    temp.Value = spec_employee.apprv_lv_4_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_4_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_2);
                        temp1.Value = spec_employee.apprv_lv_4_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_4_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_3);
                        temp1.Value = spec_employee.apprv_lv_4_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_4_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_4);
                        temp1.Value = spec_employee.apprv_lv_4_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_4_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_5);
                        temp1.Value = spec_employee.apprv_lv_4_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_5_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_1);
                    temp.Value = spec_employee.apprv_lv_5_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_5_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_2);
                        temp1.Value = spec_employee.apprv_lv_5_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_5_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_3);
                        temp1.Value = spec_employee.apprv_lv_5_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_5_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_4);
                        temp1.Value = spec_employee.apprv_lv_5_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_5_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_5);
                        temp1.Value = spec_employee.apprv_lv_5_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_6_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_1);
                    temp.Value = spec_employee.apprv_lv_6_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_6_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_2);
                        temp1.Value = spec_employee.apprv_lv_6_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_6_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_3);
                        temp1.Value = spec_employee.apprv_lv_6_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_6_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_4);
                        temp1.Value = spec_employee.apprv_lv_6_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_6_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_5);
                        temp1.Value = spec_employee.apprv_lv_6_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_7_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_1);
                    temp.Value = spec_employee.apprv_lv_7_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_7_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_2);
                        temp1.Value = spec_employee.apprv_lv_7_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_7_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_3);
                        temp1.Value = spec_employee.apprv_lv_7_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_7_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_4);
                        temp1.Value = spec_employee.apprv_lv_7_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_7_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_5);
                        temp1.Value = spec_employee.apprv_lv_7_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
            }
            ViewBag.SpecAssignBy = SpecAssign;
            ViewBag.Username = model2.employee_info.name;
            return View(model2);
        }

        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [AcceptVerbs(HttpVerbs.Post), ExportModelStateToTempData]
        public async Task<ActionResult> Validate(TravelRequestHelper model, string validate, string add, string delete = "", string loged = "", string clear = "")
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;

            tb_m_employee created = await GetData.EmployeeInfo(identity.Name);
            ViewBag.loged_id = created.code.Trim(' ');
            ViewBag.loged_name = created.name.Trim(' ');

            string NoregParticipant = "";
            ViewBag.ListString = NoregParticipant;

            List<SelectListItem> SpecAssign = new List<SelectListItem>();
            tb_m_special_employee_new spec_employee = await GetData.GetSpecialNoreg(model.employee_info.code);
            if (spec_employee.no_reg != null)
            {
                model.travel_request.exep_empolyee = true;
                if (spec_employee.apprv_lv_1_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_1);
                    temp.Value = spec_employee.apprv_lv_1_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_1_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_2);
                        temp1.Value = spec_employee.apprv_lv_1_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_1_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_3);
                        temp1.Value = spec_employee.apprv_lv_1_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_1_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_4);
                        temp1.Value = spec_employee.apprv_lv_1_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_1_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_5);
                        temp1.Value = spec_employee.apprv_lv_1_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_2_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_1);
                    temp.Value = spec_employee.apprv_lv_2_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_2_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_2);
                        temp1.Value = spec_employee.apprv_lv_2_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_2_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_3);
                        temp1.Value = spec_employee.apprv_lv_2_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_2_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_4);
                        temp1.Value = spec_employee.apprv_lv_2_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_2_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_5);
                        temp1.Value = spec_employee.apprv_lv_2_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_3_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_1);
                    temp.Value = spec_employee.apprv_lv_3_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_3_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_2);
                        temp1.Value = spec_employee.apprv_lv_3_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_3_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_3);
                        temp1.Value = spec_employee.apprv_lv_3_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_3_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_4);
                        temp1.Value = spec_employee.apprv_lv_3_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_3_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_5);
                        temp1.Value = spec_employee.apprv_lv_3_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_4_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_1);
                    temp.Value = spec_employee.apprv_lv_4_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_4_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_2);
                        temp1.Value = spec_employee.apprv_lv_4_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_4_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_3);
                        temp1.Value = spec_employee.apprv_lv_4_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_4_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_4);
                        temp1.Value = spec_employee.apprv_lv_4_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_4_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_5);
                        temp1.Value = spec_employee.apprv_lv_4_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_5_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_1);
                    temp.Value = spec_employee.apprv_lv_5_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_5_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_2);
                        temp1.Value = spec_employee.apprv_lv_5_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_5_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_3);
                        temp1.Value = spec_employee.apprv_lv_5_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_5_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_4);
                        temp1.Value = spec_employee.apprv_lv_5_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_5_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_5);
                        temp1.Value = spec_employee.apprv_lv_5_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_6_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_1);
                    temp.Value = spec_employee.apprv_lv_6_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_6_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_2);
                        temp1.Value = spec_employee.apprv_lv_6_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_6_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_3);
                        temp1.Value = spec_employee.apprv_lv_6_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_6_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_4);
                        temp1.Value = spec_employee.apprv_lv_6_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_6_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_5);
                        temp1.Value = spec_employee.apprv_lv_6_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_7_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_1);
                    temp.Value = spec_employee.apprv_lv_7_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_7_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_2);
                        temp1.Value = spec_employee.apprv_lv_7_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_7_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_3);
                        temp1.Value = spec_employee.apprv_lv_7_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_7_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_4);
                        temp1.Value = spec_employee.apprv_lv_7_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_7_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_5);
                        temp1.Value = spec_employee.apprv_lv_7_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
            }
            ViewBag.SpecAssignBy = SpecAssign;


            if (validate != null && model.travel_request != null)
            {
                //if (Request.Files["generaldoc"] != null)
                //{
                //    HttpPostedFileBase file = Request.Files["generaldoc"];
                //    model.generaldoc_file = file;
                //    model.travel_request.path_general = "path";
                //}

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

                    model.travel_request.exep_empolyee = model.travel_request.exep_empolyee;

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
                    }

                    //hitung
                    for (int i = 0; i < ListModel.Count(); i++)
                    {
                        //do the calculation
                        ListModel[i]=await calculate.DateDurationAsync(ListModel[i],i);

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
                            ListModel[k].travel_request.participants_flag = false;
                        
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
                        ViewBag.Destination[k] = await GetData.DestinationNameInfo(ListModel[k].travel_request.id_destination_city);
                    
                    ViewBag.Bossname = await GetData.EmployeeNameInfo(model.travel_request.assign_by);
                    if (ViewBag.Bossname == null) ViewBag.Bossname = "-No Data";
                    //isi wbs no and cost center

                    //get division name
                    ViewBag.division_name = await GetData.GetDivisionSourceName(Convert.ToInt32(model.employee_info.code));

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

                    ViewBag.totalbudget = budget.available_amount - total;

                    return View(ListModel);
                    // Do stuff
                }
                else
                {                    
                    //if model is invalid
                    return Redirect("Index");
                }

            }
            else
            if (add != null)
            {
                List<string> ModelList = new List<string>();
                int noreg;
                string exist = "";
                string noBank = "";
                string self = "";
                ViewBag.noBank = noBank;
                ViewBag.Exist = noBank;
                ViewBag.Self = noBank;

                if (model.tparticipant != null && int.TryParse(model.tparticipant, out noreg))
                {
                    // cek if adding self for participant
                    if (model.tparticipant.Contains(model.employee_info.code.Trim()))
                    {
                        self = await GetData.EmployeeNameInfo(Convert.ToInt32(model.tparticipant));
                        ViewBag.Self = self;
                    }

                    // cek if employee has bank account
                    List<tb_m_vendor_employee> bankNamePart = GetData.VendorEmployeeValidate(Convert.ToInt32(model.tparticipant));
                    if (bankNamePart.Count < 1)
                    {
                        noBank = await GetData.EmployeeNameInfo(Convert.ToInt32(model.tparticipant));
                        ViewBag.noBank = noBank;
                    }
                    if (model.participants == null)
                        model.participants = new List<tb_r_travel_request_participant>();

                    // cek if employee already in list
                    foreach (var item in model.participants)
                    {
                        if (item.no_reg == noreg) exist = await GetData.EmployeeNameInfo(noreg);
                    }
                    if (exist == "" && noBank == "" && self == "")
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

                if (model.participants != null)
                {
                    foreach (var item in model.participants)
                    {
                        NoregParticipant = NoregParticipant + item.no_reg.ToString() + ',';
                    }
                }
                ViewBag.ListString = NoregParticipant;

                //special employee check


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

                division.Divisi = division.Divisi.Replace("and1", "&");
                ViewBag.division_name = division.Divisi;

                ViewBag.RL3 = ModelList;

                ViewBag.RL = await GetData.DestinationInfo();
                ViewBag.RL2 = await GetData.PurposeInfo();
                ViewBag.Bossname = "Assigned by " + await GetData.EmployeeNameInfo(model.travel_request.assign_by) + " (" + model.travel_request.assign_by.ToString() + ")";

                if (temp.participants != null)
                {
                    foreach (var item in temp.participants)
                    {
                        NoregParticipant = NoregParticipant + item.no_reg.ToString() + ',';
                    }
                }
                ViewBag.ListString = NoregParticipant;

                return View("Index", temp);

            }
            else 
            if (clear == "")
            {
                TravelRequestHelper temp = model;
                temp = model;

                List<string> ModelList = new List<string>();
                if (model.participants != null)
                {
                    for (int k = 0; k < model.participants.Count(); k++)
                    {
                        ModelList.Add(await GetData.EmployeeNameInfo(model.participants[k].no_reg));
                    }
                }
                ViewBag.RL3 = ModelList;

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

        
        public async Task<ActionResult> IndexMSTR(string noreg, string dvc)
        {
            //string noreg = "101799";
            tb_m_employee model = new tb_m_employee();
            model = await GetData.EmployeeInfo(noreg);

            tb_m_employee created = await GetData.EmployeeInfo(model.code);
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

            //get employee assigned by
            model2.travel_request.assign_by = await Utility.AssignedBy(model2.employee_info);

            //get division name
            ViewBag.division_name = await GetData.GetDivisionSourceName(Convert.ToInt32(model.code));

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
           
            tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(model.code));
            division.Divisi = division.Divisi.Replace("and1", "&");
            ViewBag.division_name = division.Divisi;
            model2.special_employee_flag = false;

            tb_m_special_employee_new spec_employee = await GetData.GetSpecialNoreg(model.code);
            List<SelectListItem> SpecAssign = new List<SelectListItem>();
            if (spec_employee.no_reg != null)
            {
                model2.travel_request.exep_empolyee = true;
                if (spec_employee.apprv_lv_1_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_1);
                    temp.Value = spec_employee.apprv_lv_1_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_1_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_2);
                        temp1.Value = spec_employee.apprv_lv_1_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_1_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_3);
                        temp1.Value = spec_employee.apprv_lv_1_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_1_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_4);
                        temp1.Value = spec_employee.apprv_lv_1_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_1_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_5);
                        temp1.Value = spec_employee.apprv_lv_1_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_2_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_1);
                    temp.Value = spec_employee.apprv_lv_2_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_2_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_2);
                        temp1.Value = spec_employee.apprv_lv_2_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_2_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_3);
                        temp1.Value = spec_employee.apprv_lv_2_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_2_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_4);
                        temp1.Value = spec_employee.apprv_lv_2_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_2_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_5);
                        temp1.Value = spec_employee.apprv_lv_2_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_3_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_1);
                    temp.Value = spec_employee.apprv_lv_3_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_3_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_2);
                        temp1.Value = spec_employee.apprv_lv_3_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_3_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_3);
                        temp1.Value = spec_employee.apprv_lv_3_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_3_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_4);
                        temp1.Value = spec_employee.apprv_lv_3_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_3_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_5);
                        temp1.Value = spec_employee.apprv_lv_3_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_4_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_1);
                    temp.Value = spec_employee.apprv_lv_4_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_4_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_2);
                        temp1.Value = spec_employee.apprv_lv_4_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_4_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_3);
                        temp1.Value = spec_employee.apprv_lv_4_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_4_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_4);
                        temp1.Value = spec_employee.apprv_lv_4_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_4_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_5);
                        temp1.Value = spec_employee.apprv_lv_4_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_5_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_1);
                    temp.Value = spec_employee.apprv_lv_5_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_5_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_2);
                        temp1.Value = spec_employee.apprv_lv_5_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_5_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_3);
                        temp1.Value = spec_employee.apprv_lv_5_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_5_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_4);
                        temp1.Value = spec_employee.apprv_lv_5_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_5_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_5);
                        temp1.Value = spec_employee.apprv_lv_5_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_6_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_1);
                    temp.Value = spec_employee.apprv_lv_6_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_6_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_2);
                        temp1.Value = spec_employee.apprv_lv_6_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_6_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_3);
                        temp1.Value = spec_employee.apprv_lv_6_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_6_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_4);
                        temp1.Value = spec_employee.apprv_lv_6_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_6_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_5);
                        temp1.Value = spec_employee.apprv_lv_6_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_7_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_1);
                    temp.Value = spec_employee.apprv_lv_7_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_7_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_2);
                        temp1.Value = spec_employee.apprv_lv_7_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_7_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_3);
                        temp1.Value = spec_employee.apprv_lv_7_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_7_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_4);
                        temp1.Value = spec_employee.apprv_lv_7_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_7_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_5);
                        temp1.Value = spec_employee.apprv_lv_7_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
            }
            ViewBag.SpecAssignBy = SpecAssign;

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

            List<SelectListItem> SpecAssign = new List<SelectListItem>();
            tb_m_special_employee_new spec_employee = await GetData.GetSpecialNoreg(model.employee_info.code);
            if (spec_employee.no_reg != null)
            {
                model.travel_request.exep_empolyee = true;
                if (spec_employee.apprv_lv_1_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_1);
                    temp.Value = spec_employee.apprv_lv_1_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_1_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_2);
                        temp1.Value = spec_employee.apprv_lv_1_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_1_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_3);
                        temp1.Value = spec_employee.apprv_lv_1_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_1_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_4);
                        temp1.Value = spec_employee.apprv_lv_1_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_1_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_1_5);
                        temp1.Value = spec_employee.apprv_lv_1_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_2_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_1);
                    temp.Value = spec_employee.apprv_lv_2_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_2_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_2);
                        temp1.Value = spec_employee.apprv_lv_2_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_2_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_3);
                        temp1.Value = spec_employee.apprv_lv_2_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_2_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_4);
                        temp1.Value = spec_employee.apprv_lv_2_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_2_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_2_5);
                        temp1.Value = spec_employee.apprv_lv_2_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_3_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_1);
                    temp.Value = spec_employee.apprv_lv_3_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_3_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_2);
                        temp1.Value = spec_employee.apprv_lv_3_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_3_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_3);
                        temp1.Value = spec_employee.apprv_lv_3_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_3_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_4);
                        temp1.Value = spec_employee.apprv_lv_3_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_3_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_3_5);
                        temp1.Value = spec_employee.apprv_lv_3_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_4_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_1);
                    temp.Value = spec_employee.apprv_lv_4_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_4_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_2);
                        temp1.Value = spec_employee.apprv_lv_4_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_4_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_3);
                        temp1.Value = spec_employee.apprv_lv_4_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_4_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_4);
                        temp1.Value = spec_employee.apprv_lv_4_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_4_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_4_5);
                        temp1.Value = spec_employee.apprv_lv_4_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_5_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_1);
                    temp.Value = spec_employee.apprv_lv_5_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_5_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_2);
                        temp1.Value = spec_employee.apprv_lv_5_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_5_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_3);
                        temp1.Value = spec_employee.apprv_lv_5_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_5_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_4);
                        temp1.Value = spec_employee.apprv_lv_5_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_5_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_5_5);
                        temp1.Value = spec_employee.apprv_lv_5_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_6_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_1);
                    temp.Value = spec_employee.apprv_lv_6_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_6_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_2);
                        temp1.Value = spec_employee.apprv_lv_6_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_6_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_3);
                        temp1.Value = spec_employee.apprv_lv_6_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_6_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_4);
                        temp1.Value = spec_employee.apprv_lv_6_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_6_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_6_5);
                        temp1.Value = spec_employee.apprv_lv_6_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
                else if (spec_employee.apprv_lv_7_1 != null)
                {
                    SelectListItem temp = new SelectListItem();
                    temp.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_1);
                    temp.Value = spec_employee.apprv_lv_7_1.ToString();
                    temp.Text = temp.Text + " (" + temp.Value + ")";
                    SpecAssign.Add(temp);

                    if (spec_employee.apprv_lv_7_2 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_2);
                        temp1.Value = spec_employee.apprv_lv_7_2.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_7_3 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_3);
                        temp1.Value = spec_employee.apprv_lv_7_3.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_7_4 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_4);
                        temp1.Value = spec_employee.apprv_lv_7_4.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }
                    if (spec_employee.apprv_lv_7_5 != null)
                    {
                        SelectListItem temp1 = new SelectListItem();
                        temp1.Text = await GetData.EmployeeNameInfo(spec_employee.apprv_lv_7_5);
                        temp1.Value = spec_employee.apprv_lv_7_5.ToString();
                        temp1.Text = temp1.Text + " (" + temp1.Value + ")";
                        SpecAssign.Add(temp1);
                    }

                }
            }
            ViewBag.SpecAssignBy = SpecAssign;

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

                    ViewBag.totalbudget = budget.available_amount - total;

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

            for (int i = 0; i < ListModel.Count(); i++)
            {
                tb_m_special_employee_new spec_employee = await GetData.GetSpecialNoreg(ListModel[i].travel_request.no_reg.ToString());
                if (spec_employee.no_reg != null)
                {
                    ListModel[i].travel_request.exep_empolyee = true;
                    ListModel[i].travel_request.apprv_by_lvl1 = ListModel[i].travel_request.assign_by;
                    List<SelectListItem> SpecAssign = new List<SelectListItem>();
                    if (spec_employee.apprv_lv_1_1 != null)
                    {
                        SelectListItem temp = new SelectListItem();
                        temp.Value = spec_employee.apprv_lv_1_1.ToString();
                        SpecAssign.Add(temp);

                        if (spec_employee.apprv_lv_1_2 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_1_2.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_1_3 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_1_3.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_1_4 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_1_4.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_1_5 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_1_5.ToString();
                            SpecAssign.Add(temp1);
                        }

                    }
                    if (spec_employee.apprv_lv_2_1 != null)
                    {
                        SelectListItem temp = new SelectListItem();
                        temp.Value = spec_employee.apprv_lv_2_1.ToString();
                        SpecAssign.Add(temp);

                        if (spec_employee.apprv_lv_2_2 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_2_2.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_2_3 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_2_3.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_2_4 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_2_4.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_2_5 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_2_5.ToString();
                            SpecAssign.Add(temp1);
                        }

                    }
                    if (spec_employee.apprv_lv_3_1 != null)
                    {
                        SelectListItem temp = new SelectListItem();
                        temp.Value = spec_employee.apprv_lv_3_1.ToString();
                        SpecAssign.Add(temp);

                        if (spec_employee.apprv_lv_3_2 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_3_2.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_3_3 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_3_3.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_3_4 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_3_4.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_3_5 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_3_5.ToString();
                            SpecAssign.Add(temp1);
                        }

                    }
                    if (spec_employee.apprv_lv_4_1 != null)
                    {
                        SelectListItem temp = new SelectListItem();
                        temp.Value = spec_employee.apprv_lv_4_1.ToString();
                        SpecAssign.Add(temp);

                        if (spec_employee.apprv_lv_4_2 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_4_2.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_4_3 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_4_3.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_4_4 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_4_4.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_4_5 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_4_5.ToString();
                            SpecAssign.Add(temp1);
                        }

                    }
                    if (spec_employee.apprv_lv_5_1 != null)
                    {
                        SelectListItem temp = new SelectListItem();
                        temp.Value = spec_employee.apprv_lv_5_1.ToString();
                        SpecAssign.Add(temp);

                        if (spec_employee.apprv_lv_5_2 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_5_2.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_5_3 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_5_3.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_5_4 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_5_4.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_5_5 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_5_5.ToString();
                            SpecAssign.Add(temp1);
                        }

                    }
                    if (spec_employee.apprv_lv_6_1 != null)
                    {
                        SelectListItem temp = new SelectListItem();
                        temp.Value = spec_employee.apprv_lv_6_1.ToString();
                        SpecAssign.Add(temp);

                        if (spec_employee.apprv_lv_6_2 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_6_2.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_6_3 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_6_3.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_6_4 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_6_4.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_6_5 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_6_5.ToString();
                            SpecAssign.Add(temp1);
                        }

                    }
                    if (spec_employee.apprv_lv_7_1 != null)
                    {
                        SelectListItem temp = new SelectListItem();
                        temp.Value = spec_employee.apprv_lv_7_1.ToString();
                        SpecAssign.Add(temp);

                        if (spec_employee.apprv_lv_7_2 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_7_2.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_7_3 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_7_3.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_7_4 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_7_4.ToString();
                            SpecAssign.Add(temp1);
                        }
                        if (spec_employee.apprv_lv_7_5 != null)
                        {
                            SelectListItem temp1 = new SelectListItem();
                            temp1.Value = spec_employee.apprv_lv_7_5.ToString();
                            SpecAssign.Add(temp1);
                        }

                    }

                    for (int k = 0; k < SpecAssign.Count; k++)
                    {
                        if (SpecAssign[k].Value.Contains(ListModel[i].travel_request.assign_by.ToString()))
                        {
                            SpecAssign.RemoveAt(k);
                            break;
                        }
                    }
                    for (int l = 0; l < SpecAssign.Count; l++)
                    {
                        if (l == 0) ListModel[i].travel_request.apprv_by_lvl2 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 1) ListModel[i].travel_request.apprv_by_lvl3 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 2) ListModel[i].travel_request.apprv_by_lvl4 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 3) ListModel[i].travel_request.apprv_by_lvl5 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 4) ListModel[i].travel_request.apprv_by_lvl6 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 5) ListModel[i].travel_request.apprv_by_lvl7 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 6) ListModel[i].travel_request.apprv_by_lvl8 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 7) ListModel[i].travel_request.apprv_by_lvl9 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 8) ListModel[i].travel_request.apprv_by_lvl10 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 9) ListModel[i].travel_request.apprv_by_lvl11 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 10) ListModel[i].travel_request.apprv_by_lvl12 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 11) ListModel[i].travel_request.apprv_by_lvl13 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 12) ListModel[i].travel_request.apprv_by_lvl14 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 13) ListModel[i].travel_request.apprv_by_lvl15 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 14) ListModel[i].travel_request.apprv_by_lvl16 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 15) ListModel[i].travel_request.apprv_by_lvl17 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 16) ListModel[i].travel_request.apprv_by_lvl18 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 17) ListModel[i].travel_request.apprv_by_lvl19 = Convert.ToInt32(SpecAssign[l].Value);
                        else if (l == 18) ListModel[i].travel_request.apprv_by_lvl20 = Convert.ToInt32(SpecAssign[l].Value);
                    }

                }


                await InsertData.TravelRequest(ListModel[i]);
                if (ListModel[i].travel_request.participants_flag == true)
                {
                    if (i <= ListModel.Count())
                    {
                        for (int j = 0; j < ListModel[i].participants.Count; j++)
                            await InsertData.TravelParticipant(ListModel[i].participants[j]);
                    }
                }
            }
            ViewBag.Username = ListModel[0].employee_info.name;

            return View("SubmittedMSTR");
            //return RedirectToAction("Index");
        }

    }
}
