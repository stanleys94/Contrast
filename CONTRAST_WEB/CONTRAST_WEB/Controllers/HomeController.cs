using CONTRAST_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Owin.Security;
using CONTRAST_WEB.CustomValidator;

namespace CONTRAST_WEB.Controllers
{

    public class HomeController : Controller
    {

        private CONTRASTEntities db = new CONTRASTEntities();

        [Authorize(Roles = "contrast.user")]
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            Utility.Logger(identity.Name);
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;

            //List<string> claims = new List<string>();
            //claims.Add("contrast.administd");
            //claims.Add("contrast.staffga");
            //claims.Add("contrast.user");
            //claims.Add("contrast.staffpac");
            //claims.Add("contrast.ap");
            //ViewBag.Privillege = claims;

            tb_m_employee model = await GetData.EmployeeInfo(identity.Name);


            //tb_m_employee model = await GetData.EmployeeInfo(identity.Name);
            //AP
            //tb_m_employee model = await GetData.EmployeeInfo("101419");
            //staff ga
            // tb_m_employee model = await GetData.EmployeeInfo("101495");
            // dph ga 100354  
            //tb_m_employee model = await GetData.EmployeeInfo("100354");
            //percobaan
            //tb_m_employee model = await GetData.EmployeeInfo("100626");
            //tb_m_employee model = await GetData.EmployeeInfo("101795");
            //tb_m_employee model = await GetData.EmployeeInfo("101795");

            ViewBag.photo = await GetData.PhotoEmployeeInfo(model.code);
            ViewBag.Username = model.name;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "contrast.user")]
        public async System.Threading.Tasks.Task<ActionResult> Back(tb_m_employee model)
        {
            ViewBag.Username = model.name;
            return View("Index", model);
        }


        [HttpPost]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Submitted(TravelRequestHelper[] ListModel)
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

                string division_r = await GetData.GetDivMapping(ListModel[i].travel_request.no_reg.ToString());

                tb_m_budget div_budget = await GetData.GetCostWbs((bool)ListModel[i].travel_request.overseas_flag, division_r);

                double total = Convert.ToDouble(ListModel[i].travel_request.grand_total_allowance);
                await UpdateData.Budget(div_budget.eoa_wbs_no, div_budget.cost_center, total);

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
            //return View("Index", ListModel[0].employee_info);
            return RedirectToAction("Index");
        }

    }
}
