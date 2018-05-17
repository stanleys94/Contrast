using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CONTRAST_WEB.Models;
using System.Threading.Tasks;
using System.Globalization;
using System.Security.Claims;
using PagedList;

namespace CONTRAST_WEB.Controllers
{
    public class VerifyController : Controller
    {
        //// GET: Verify
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page, DateTime? startdate, DateTime? enddate)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;
            tb_m_employee model = await GetData.EmployeeInfo(identity.Name);

            tb_m_verifier_employee access_status = new tb_m_verifier_employee();
            access_status = await GetData.EmployeeVerifier(Convert.ToInt32(model.code));
            ViewBag.position = access_status.position;
            List<vw_actualCost_verified> ResultObject = new List<vw_actualCost_verified>();
            ResultObject = await GetData.ActualCostVerifiedList(access_status.position);

            List<ActualCostVerifiedHelper> ResultObject2 = new List<ActualCostVerifiedHelper>();

            for (int k = 0; k < ResultObject.Count(); k++)
            {
                ResultObject2.Add(new ActualCostVerifiedHelper());
                ResultObject2[k].ActualCost_Verified = ResultObject[k];
                ResultObject2[k].EmployeeInfo = model;
                //ResultObject2[k].position = access_status.position;
                ResultObject2[k].money = ResultObject[k].amount.ToString("c", Constant.culture);
            }

            //if search / page empty
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;
            if (startdate != null)
                ViewBag.startdate = startdate;
            else
                ViewBag.startdate = null;

            if (enddate != null)
                ViewBag.enddate = enddate;
            else
                ViewBag.enddate = null;


            //filter
            if (!String.IsNullOrEmpty(searchString))
            {
                List<ActualCostVerifiedHelper> temp = new List<ActualCostVerifiedHelper>();
                for (int k = 0; k < ResultObject2.Count; k++)
                {
                    //by group code
                    if (ResultObject2[k].ActualCost_Verified.group_code.ToLower().Contains(searchString.ToLower())
                        || ResultObject2[k].ActualCost_Verified.name.ToLower().Contains(searchString.ToLower())
                        || ResultObject2[k].ActualCost_Verified.destination_name.ToLower().Contains(searchString.ToLower())                        
                       )
                        temp.Add(ResultObject2[k]);
                }
                /*if (temp.Count() > 0) */ResultObject2 = temp;
            }

            //date filter
            if (startdate != null && enddate != null)
            {
                List<ActualCostVerifiedHelper> temp = new List<ActualCostVerifiedHelper>();
                for (int k = 0; k < ResultObject2.Count; k++)
                {
                    //by group code
                    if (
                        ResultObject2[k].ActualCost_Verified.start_date >= startdate
                        && ResultObject2[k].ActualCost_Verified.start_date <= enddate
                       )
                        temp.Add(ResultObject2[k]);
                }
               /* if (temp.Count() > 0) */ResultObject2 = temp;
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);

            if (ResultObject2.Count == 0)
            {
                ResultObject2.Add(new ActualCostVerifiedHelper());
                ResultObject2[0].EmployeeInfo = model;
                return View("Index", ResultObject2.ToPagedList(pageNumber, pageSize));
            }
            else
                return View(ResultObject2.OrderBy(m => m.ActualCost_Verified.create_date).ToPagedList(pageNumber, pageSize));

        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(List<ActualCostVerifiedHelper> model, string search, string insert, DateTime? start, DateTime? end, string sortOrder, string currentFilter, string searchString, int? page, DateTime? startdate, DateTime? enddate)
        {
            ViewBag.search = search;
            ViewBag.start = start;
            ViewBag.end = end;

            if (insert.ToLower() == "submit")
            {
                tb_m_verifier_employee access_status = new tb_m_verifier_employee();
                for (int k = 0; k < model.Count(); k++)
                {
                    model[k].money = model[k].money.Replace(".", "");
                    model[k].money = model[k].money.Replace("Rp", "");

                    access_status = await GetData.EmployeeVerifier(Convert.ToInt32(model[k].EmployeeInfo.code));
                    ViewBag.position = access_status.position;

                    if (model[k].check_reject == true && model[k].check_verify == false)
                    {
                        //model[k].flag = "2";
                        tb_r_record_rejected_verification rejected = new tb_r_record_rejected_verification();
                        rejected.comment = model[k].comment;
                        rejected.id_actualcost = model[k].ActualCost_Verified.id_actualcost;
                        rejected.process_reject = access_status.position;
                        rejected.user_created = Convert.ToInt32(model[k].EmployeeInfo.code);
                        rejected.created_date = DateTime.Now;
                        model[k].flag = "2";

                        await UpdateData.ActualCost(model[k], access_status.position);
                        await Utility.RecordRejected(rejected);

                    }
                    else
                    if (model[k].check_verify == true && model[k].check_reject == false)
                    {
                        //model[k].flag = "1";
                        //await UpdateData.ActualCost(model[k], access_status.position);

                        model[k].flag = "1";
                        if (access_status.position.Trim() == "AP")
                        {
                            //bool check = await GetData.GetDivisionDoubleCheck();
                            model[k].money = model[k].money.Replace(".", "");
                            model[k].money = model[k].money.Replace("Rp", "");
                            tb_r_travel_request down_payment = await GetData.TravelRequest(model[k].ActualCost_Verified.id_request);

                            if (model[k].ActualCost_Verified.jenis_transaksi == "hotel") await UpdateData.BudgetAdd(model[k].ActualCost_Verified.wbs_no, model[k].ActualCost_Verified.cost_center, Convert.ToDouble(down_payment.allowance_hotel));
                            else if (model[k].ActualCost_Verified.jenis_transaksi == "ticket") await UpdateData.BudgetAdd(model[k].ActualCost_Verified.wbs_no, model[k].ActualCost_Verified.cost_center, Convert.ToDouble(down_payment.allowance_ticket));

                            await UpdateData.Budget(model[k].ActualCost_Verified.wbs_no, model[k].ActualCost_Verified.cost_center, Convert.ToDouble(model[k].money));
                        }
                        await UpdateData.ActualCost(model[k], access_status.position);
                    }
                    else
                        model[k].flag = "0";
                }
                
                ModelState.Clear();
                
                //return View("Index", ResultObject2.OrderBy(r => r.ActualCost_Verified.create_date).ToList());
                return RedirectToAction("Index", new { @searchString = searchString });
            }
            else
                return RedirectToAction("Index", new { @searchString = searchString });
            //else return View("index", model.OrderBy(r => r.ActualCost_Verified.create_date).ToList());
        }
    }
}