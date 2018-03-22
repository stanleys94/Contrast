using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CONTRAST_WEB.Models;
using System.Threading.Tasks;
using System.Globalization;
using System.Web.Hosting;
using System.IO;

namespace CONTRAST_WEB.Controllers
{
    public class VerifySettlementController : Controller
    {
        [HttpPost]
        //[Authorize]
        //[Authorize(Roles = "contrast.user")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(tb_m_employee model)
        {
            //tb_m_verifier_employee access_status = new tb_m_verifier_employee();
            //access_status = await GetData.EmployeeVerifier(Convert.ToInt32(model.code));
            //List<vw_settlement_verified> ResultObject = new List<vw_settlement_verified>();
            //ResultObject = await GetData.SettlementVerifiedList(access_status.position);

            //List<SettlementVerifiedHelper> ResultObject2 = new List<SettlementVerifiedHelper>();

            //for (int k = 0; k < ResultObject.Count(); k++)
            //{
            //    ResultObject2.Add(new SettlementVerifiedHelper());
            //    ResultObject2[k].Settlement_Verified = ResultObject[k];
            //    ResultObject2[k].EmployeeInfo = model;

            //    ResultObject2[k].money = ResultObject[k].amount.ToString("c", Constant.culture);
            //}
            //if (ResultObject2.Count > 0) return View(ResultObject2);
            //else
            //{
            //    SettlementVerifiedHelper temp = new SettlementVerifiedHelper();
            //    temp.EmployeeInfo = model;
            //    ResultObject2.Add(temp);
            //    return View(ResultObject2);
            //}
            //return View(ResultObject2);

            tb_m_verifier_employee access_status = new tb_m_verifier_employee();
            access_status = await GetData.EmployeeVerifier(Convert.ToInt32(model.code));
            ViewBag.position = access_status.position;
            List<vw_settlement_verified> ResultObject = new List<vw_settlement_verified>();
            ResultObject = await GetData.SettlementVerifiedList(access_status.position);

            List<SettlementVerifiedHelper> ResultObject2 = new List<SettlementVerifiedHelper>();

            for (int k = 0; k < ResultObject.Count(); k++)
            {
                ResultObject2.Add(new SettlementVerifiedHelper());
                ResultObject2[k].Settlement_Verified = ResultObject[k];
                ResultObject2[k].EmployeeInfo = model;
                ResultObject2[k].money = ResultObject[k].amount.ToString("c", Constant.culture);
            }
            if (ResultObject2.Count == 0)
            {
                ResultObject2.Add(new SettlementVerifiedHelper());
                ResultObject2[0].EmployeeInfo = model;
                return View("Index", ResultObject2);
            }
            //ModelState.Clear();
            return View(ResultObject2.OrderBy(m => m.Settlement_Verified.create_date).ToList());
        }

        [HttpPost]
        //[Authorize]
        //[Authorize(Roles = "contrast.user")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Index2(tb_m_employee model, string search = "")
        {
            //string noreg = TempData["Data"].ToString();
            //model = await GetData.EmployeeInfo(noreg);
            //tb_m_verifier_employee access_status = new tb_m_verifier_employee();
            //access_status = await GetData.EmployeeVerifier(Convert.ToInt32(model.code));
            //List<vw_settlement_verified> ResultObject = new List<vw_settlement_verified>();
            //string lower_search = search.ToLower();
            //ViewBag.search = search;

            //ResultObject = await GetData.SettlementVerifiedListFiltered(access_status.position, lower_search);
            //List<SettlementVerifiedHelper> ResultObject2 = new List<SettlementVerifiedHelper>();

            //for (int k = 0; k < ResultObject.Count(); k++)
            //{
            //    ResultObject2.Add(new SettlementVerifiedHelper());
            //    ResultObject2[k].Settlement_Verified = ResultObject[k];
            //    ResultObject2[k].EmployeeInfo = model;
            //    ResultObject2[k].money = ResultObject[k].amount.ToString("c", Constant.culture);
            //}
            //ViewBag.Noreg = model.code;
            //return View("Index", ResultObject2);

            string noreg = TempData["Data"].ToString();
            model = await GetData.EmployeeInfo(noreg);
            tb_m_verifier_employee access_status = new tb_m_verifier_employee();
            access_status = await GetData.EmployeeVerifier(Convert.ToInt32(model.code));
            ViewBag.position = access_status.position;
            List<vw_settlement_verified> ResultObject = new List<vw_settlement_verified>();
            string lower_search = search.ToLower();
            ViewBag.search = search;

            ResultObject = await GetData.SettlementVerifiedListFiltered(access_status.position, lower_search);
            List<SettlementVerifiedHelper> ResultObject2 = new List<SettlementVerifiedHelper>();

            for (int k = 0; k < ResultObject.Count(); k++)
            {
                ResultObject2.Add(new SettlementVerifiedHelper());
                ResultObject2[k].Settlement_Verified = ResultObject[k];
                ResultObject2[k].EmployeeInfo = model;
                ResultObject2[k].money = ResultObject[k].amount.ToString("c", Constant.culture);
            }
            ViewBag.Noreg = model.code;

            if (ResultObject2.Count == 0)
            {
                ResultObject2.Add(new SettlementVerifiedHelper());
                ResultObject2[0].EmployeeInfo = model;
                return View("Index", ResultObject2);
            }

            return View("Index", ResultObject2.OrderBy(m => m.Settlement_Verified.create_date).ToList());
        }

        [HttpPost]
        //[Authorize]
        //[Authorize(Roles = "contrast.user")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(List<SettlementVerifiedHelper> model, string search = "", string insert = "", DateTime? start = null, DateTime? end = null)
        {
            tb_m_verifier_employee access_status = new tb_m_verifier_employee();

            if (insert == "")
            {
                string noreg = model[0].EmployeeInfo.code;
                tb_m_employee employee = await GetData.EmployeeInfo(noreg);

                access_status = await GetData.EmployeeVerifier(Convert.ToInt32(employee.code));
                ViewBag.position = access_status.position;
                List<vw_settlement_verified> ResultObject = new List<vw_settlement_verified>();
                string lower_search = search.ToLower();
                ViewBag.search = search;

                ResultObject = await GetData.SettlementVerifiedListFiltered(access_status.position, lower_search);
                List<SettlementVerifiedHelper> ResultObject2 = new List<SettlementVerifiedHelper>();

                for (int k = 0; k < ResultObject.Count(); k++)
                {
                    if (start != null && end != null)
                    {
                        if (ResultObject[k].start_date >= start && ResultObject[k].start_date <= end)
                        {
                            SettlementVerifiedHelper temp = new SettlementVerifiedHelper();
                            temp.Settlement_Verified = ResultObject[k];
                            temp.EmployeeInfo = employee;
                            temp.money = ResultObject[k].amount.ToString("c", Constant.culture);
                            ResultObject2.Add(temp);

                        }
                    }
                    else if (start != null)
                    {
                        if (ResultObject[k].start_date >= start)
                        {
                            SettlementVerifiedHelper temp = new SettlementVerifiedHelper();
                            temp.Settlement_Verified = ResultObject[k];
                            temp.EmployeeInfo = employee;
                            temp.money = ResultObject[k].amount.ToString("c", Constant.culture);
                            ResultObject2.Add(temp);
                        }
                    }
                    else if (end != null)
                    {
                        if (ResultObject[k].start_date <= end)
                        {
                            SettlementVerifiedHelper temp = new SettlementVerifiedHelper();
                            temp.Settlement_Verified = ResultObject[k];
                            temp.EmployeeInfo = employee;
                            temp.money = ResultObject[k].amount.ToString("c", Constant.culture);
                            ResultObject2.Add(temp);
                        }
                    }
                    else
                    {
                        SettlementVerifiedHelper temp = new SettlementVerifiedHelper();
                        temp.Settlement_Verified = ResultObject[k];
                        temp.EmployeeInfo = employee;
                        temp.money = ResultObject[k].amount.ToString("c", Constant.culture);
                        ResultObject2.Add(temp);
                    }
                }

                if (ResultObject2.Count == 0)
                {
                    SettlementVerifiedHelper temp = new SettlementVerifiedHelper();
                    temp.EmployeeInfo = employee;
                    ResultObject2.Add(temp);
                }
                ModelState.Clear();
                ViewBag.Noreg = model[0].EmployeeInfo.code;
                return View("Index", ResultObject2.OrderBy(m => m.Settlement_Verified.create_date).ToList());
            }
            if (insert == "submit")
            {
                for (int k = 0; k < model.Count(); k++)
                {
                    model[k].money = model[k].money.Replace(".", "");
                    model[k].money = model[k].money.Replace("Rp", "");

                    access_status = await GetData.EmployeeVerifier(Convert.ToInt32(model[k].EmployeeInfo.code));
                    ViewBag.position = access_status.position;


                    if (model[k].check_reject == true && model[k].check_verify == false)
                    {
                        tb_r_record_rejected_verification rejected = new tb_r_record_rejected_verification();
                        rejected.comment = model[k].comment;
                        rejected.id_actualcost = model[k].Settlement_Verified.id_actualcost;
                        rejected.process_reject = access_status.position;
                        rejected.user_created = Convert.ToInt32(model[k].EmployeeInfo.code);
                        rejected.created_date = DateTime.Now;

                        //await Utility.RecordRejected(rejected);
                        
                        model[k].flag = "2";
                        await UpdateData.SettlementRejected(model[k], access_status.position,rejected);
                        
                    }
                    else
                    if (model[k].check_verify == true && model[k].check_reject == false)
                    {
                        //model[k].flag = "1";
                        //await UpdateData.Settlement(model[k], access_status.position);

                        model[k].flag = "1";
                        if (access_status.position.Trim() == "AP")
                        {
                            model[k].money = model[k].money.Replace(".", "");
                            model[k].money = model[k].money.Replace("Rp", "");

                            await UpdateData.Budget(model[k].Settlement_Verified.wbs_no, model[k].Settlement_Verified.cost_center, Convert.ToDouble(model[k].money));
                            //await UpdateData.Budget(model[k].Settlement_Verified.wbs_no, model[k].Settlement_Verified.cost_center, double.Parse(model[k].money, NumberStyles.Currency));
                        }
                        await UpdateData.Settlement(model[k], access_status.position);
                    }
                    else
                        model[k].flag = "0";
                }


                //reset         
                List<vw_settlement_verified> ResultObject = new List<vw_settlement_verified>();
                ResultObject = await GetData.SettlementVerifiedList(access_status.position);

                List<SettlementVerifiedHelper> ResultObject2 = new List<SettlementVerifiedHelper>();

                for (int k = 0; k < ResultObject.Count(); k++)
                {
                    ResultObject2.Add(new SettlementVerifiedHelper());
                    ResultObject2[k].Settlement_Verified = ResultObject[k];
                    ResultObject2[k].EmployeeInfo = model[0].EmployeeInfo;
                }
                

                if (ResultObject2.Count == 0)
                {
                    ResultObject2.Add(new SettlementVerifiedHelper());
                    ResultObject2[0].EmployeeInfo = model[0].EmployeeInfo;
                    return View("Index", ResultObject2);
                }
                ModelState.Clear();
                return View("Index", ResultObject2.OrderBy(m => m.Settlement_Verified.create_date).ToList());
            }
            else return View("Index", model.OrderBy(m => m.Settlement_Verified.create_date).ToList());
        }

        [HttpPost]
        //[Authorize]
        //[Authorize(Roles = "contrast.user")]
        //[ValidateAntiForgeryToken]
        public ActionResult DownloadAttach()
        {
            //string[] name_file = 
            string file = HostingEnvironment.MapPath("~/SettlementFolder/Laundry_101795.png");
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(file, contentType, Path.GetFileName(file));
        }
    }
}
