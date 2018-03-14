using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CONTRAST_WEB.Models;
using System.Threading.Tasks;
using System.Globalization;

namespace CONTRAST_WEB.Controllers
{
    public class VerifyBPDController : Controller
    {
        // GET: VerifyBPD
        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(tb_m_employee model)
        {
            tb_m_verifier_employee access_status = new tb_m_verifier_employee();
            access_status = await GetData.EmployeeVerifier(Convert.ToInt32(model.code));
            ViewBag.position = access_status.position;
            List<vw_BPD_verified> ResultObject = new List<vw_BPD_verified>();
            ResultObject = await GetData.FixedCostVerifiedList(access_status.position);


            List<FixedCostVerifierHelper> ResultObject2 = new List<FixedCostVerifierHelper>();

            for (int k = 0; k < ResultObject.Count(); k++)
            {
                ResultObject2.Add(new FixedCostVerifierHelper());
                ResultObject2[k].FixedCost_Verified = ResultObject[k];
                ResultObject2[k].EmployeeInfo = model;
                ResultObject2[k].money = ResultObject[k].amount.ToString("c", Constant.culture);
            }
            if (ResultObject2.Count == 0)
            {
                FixedCostVerifierHelper temp = new FixedCostVerifierHelper();
                temp.EmployeeInfo = model;
                ResultObject2.Add(temp);
                return View(ResultObject2);
            }
            ModelState.Clear();
            return View(ResultObject2.OrderBy(r => r.FixedCost_Verified.create_date).ToList());
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(List<FixedCostVerifierHelper> model, string search = "", string insert = "", DateTime? start = null, DateTime? end = null)
        {
            tb_m_verifier_employee access_status = new tb_m_verifier_employee();
            ViewBag.search = search;
            ViewBag.start = start;
            ViewBag.end = end;
            if (insert == "")
            {
                string noreg = model[0].EmployeeInfo.code;
                tb_m_employee employee = await GetData.EmployeeInfo(noreg);

                access_status = await GetData.EmployeeVerifier(Convert.ToInt32(noreg));
                ViewBag.position = access_status.position;
                List<vw_BPD_verified> ResultObject = new List<vw_BPD_verified>();

                string lower_search = search.ToLower();
                ViewBag.search = search;
                ResultObject = await GetData.FixedCostVerifiedListFiltered(access_status.position, lower_search);


                List<FixedCostVerifierHelper> ResultObject2 = new List<FixedCostVerifierHelper>();

                for (int k = 0; k < ResultObject.Count(); k++)
                {
                    if (start != null && end != null)
                    {
                        if (ResultObject[k].start_date >= start && ResultObject[k].start_date <= end)
                        {
                            FixedCostVerifierHelper temp = new FixedCostVerifierHelper();
                            temp.FixedCost_Verified = ResultObject[k];
                            temp.EmployeeInfo = model[0].EmployeeInfo;
                            temp.money = ResultObject[k].amount.ToString("c", Constant.culture);
                            ResultObject2.Add(temp);
                        }
                    }
                    else if (start != null)
                    {
                        if (ResultObject[k].start_date >= start)
                        {
                            FixedCostVerifierHelper temp = new FixedCostVerifierHelper();
                            temp.FixedCost_Verified = ResultObject[k];
                            temp.EmployeeInfo = model[0].EmployeeInfo;
                            temp.money = ResultObject[k].amount.ToString("c", Constant.culture);
                            ResultObject2.Add(temp);
                        }
                    }
                    else if (end != null)
                    {
                        if (ResultObject[k].start_date <= end)
                        {
                            FixedCostVerifierHelper temp = new FixedCostVerifierHelper();
                            temp.FixedCost_Verified = ResultObject[k];
                            temp.EmployeeInfo = model[0].EmployeeInfo;
                            temp.money = ResultObject[k].amount.ToString("c", Constant.culture);
                            ResultObject2.Add(temp);
                        }
                    }
                    else
                    {
                        FixedCostVerifierHelper temp = new FixedCostVerifierHelper();
                        temp.FixedCost_Verified = ResultObject[k];
                        temp.EmployeeInfo = model[0].EmployeeInfo;
                        temp.money = ResultObject[k].amount.ToString("c", Constant.culture);
                        ResultObject2.Add(temp);
                    }
                }
                if (ResultObject2.Count == 0)
                {
                    FixedCostVerifierHelper temp = new FixedCostVerifierHelper();
                    temp.EmployeeInfo = model[0].EmployeeInfo;
                    ResultObject2.Add(temp);
                    return View("Index", ResultObject2);
                }
                ModelState.Clear();
                ViewBag.Noreg = employee.code;
                return View("Index", ResultObject2.OrderBy(r => r.FixedCost_Verified.create_date).ToList());
            }
            else if (insert == "submit")
            {
                for (int k = 0; k < model.Count(); k++)
                {
                    model[k].money = model[k].money.Replace(".", "");
                    model[k].money = model[k].money.Replace("Rp", "");

                    access_status = await GetData.EmployeeVerifier(Convert.ToInt32(model[k].EmployeeInfo.code));
                    ViewBag.position = access_status.position;


                    if (model[k].check_reject == true && model[k].check_verify == false)
                    {
                        model[k].flag = "2";

                        tb_r_record_rejected_verification rejected = new tb_r_record_rejected_verification();
                    
                        
                        rejected.comment = "Cancel Travel By AP";
                        rejected.id_actualcost = model[k].FixedCost_Verified.id_actualcost;
                        rejected.process_reject = access_status.position;
                        rejected.user_created = Convert.ToInt32(model[k].EmployeeInfo.code);
                        rejected.created_date = DateTime.Now;

                        await UpdateData.UpdateActualCostForBPD(model[k].FixedCost_Verified.id_actualcost);
                        await UpdateData.UpdateTravelRequestForBPD(model[k].FixedCost_Verified.id_request);

                        await Utility.RecordRejected(rejected);
                        await UpdateData.FixedCost(model[k], access_status.position);
                    }
                    else
                    if (model[k].check_verify == true && model[k].check_reject == false)
                    {
                        //model[k].flag = "1";
                        //await UpdateData.FixedCost(model[k], access_status.position);
                        model[k].flag = "1";
                        if (access_status.position.Trim() == "AP")
                        {
                            model[k].money = model[k].money.Replace(".", "");
                            model[k].money = model[k].money.Replace("Rp", "");

                            await UpdateData.Budget(model[k].FixedCost_Verified.wbs_no, model[k].FixedCost_Verified.cost_center, Convert.ToDouble(model[k].money));

                            //await UpdateData.Budget(model[k].FixedCost_Verified.wbs_no, model[k].FixedCost_Verified.cost_center, double.Parse(model[k].money, NumberStyles.Currency));
                        }
                        await UpdateData.FixedCost(model[k], access_status.position);
                    }
                    else
                        model[k].flag = "0";
                }


                //reset         
                List<vw_BPD_verified> ResultObject = new List<vw_BPD_verified>();
                ResultObject = await GetData.FixedCostVerifiedList(access_status.position);

                List<FixedCostVerifierHelper> ResultObject2 = new List<FixedCostVerifierHelper>();

                for (int k = 0; k < ResultObject.Count(); k++)
                {
                    ResultObject2.Add(new FixedCostVerifierHelper());
                    ResultObject2[k].FixedCost_Verified = ResultObject[k];
                    ResultObject2[k].EmployeeInfo = model[0].EmployeeInfo;
                    ResultObject2[k].money = ResultObject[k].amount.ToString("c", Constant.culture);
                }
                ModelState.Clear();

                if (ResultObject2.Count == 0)
                {
                    FixedCostVerifierHelper temp = new FixedCostVerifierHelper();
                    temp.EmployeeInfo = model[0].EmployeeInfo;
                    ResultObject2.Add(temp);
                    return View("Index", ResultObject2);
                }
                ModelState.Clear();
                return View("Index", ResultObject2.OrderBy(r => r.FixedCost_Verified.create_date).ToList());
            }
            else return View("Index", model.OrderBy(r => r.FixedCost_Verified.create_date).ToList());
        }
    }
}