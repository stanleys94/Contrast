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
    public class VerifyBPDController : Controller
    {
        // GET: VerifyBPD
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
                List<FixedCostVerifierHelper> temp = new List<FixedCostVerifierHelper>();
                for (int k = 0; k < ResultObject2.Count; k++)
                {
                    //by group code
                    if (ResultObject2[k].FixedCost_Verified.group_code.ToLower().Contains(searchString.ToLower())
                        || ResultObject2[k].FixedCost_Verified.name.ToLower().Contains(searchString.ToLower())
                        || ResultObject2[k].FixedCost_Verified.destination_name.ToLower().Contains(searchString.ToLower())
                       )
                        temp.Add(ResultObject2[k]);
                }
                if (temp.Count() > 0) ResultObject2 = temp;
            }

            //date filter
            if (startdate != null && enddate != null)
            {
                List<FixedCostVerifierHelper> temp = new List<FixedCostVerifierHelper>();
                for (int k = 0; k < ResultObject2.Count; k++)
                {
                    //by group code
                    if (
                        ResultObject2[k].FixedCost_Verified.start_date >= startdate
                        && ResultObject2[k].FixedCost_Verified.start_date <= enddate
                       )
                        temp.Add(ResultObject2[k]);
                }

                /*if (temp.Count() > 0)*/
                ResultObject2 = temp;

            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (ResultObject2.Count == 0)
            {
                FixedCostVerifierHelper temp = new FixedCostVerifierHelper();
                temp.EmployeeInfo = model;
                ResultObject2.Add(temp);
                return View("Index", ResultObject2.ToPagedList(pageNumber, pageSize));
            }
            else
                return View(ResultObject2.OrderBy(m => m.FixedCost_Verified.create_date).ToPagedList(pageNumber, pageSize));

        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(List<FixedCostVerifierHelper> model, string search, string insert, DateTime? start, DateTime? end, string sortOrder, string currentFilter, string searchString, int? page, DateTime? startdate, DateTime? enddate)
        {
            tb_m_verifier_employee access_status = new tb_m_verifier_employee();
            ViewBag.search = search;
            ViewBag.start = start;
            ViewBag.end = end;

            if (insert.ToLower() == "submit")
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

                        string division_r = await GetData.GetDivMapping(model[k].FixedCost_Verified.no_reg.ToString());
                        tb_r_travel_actualcost cost = await GetData.ActualCostOrigin(model[k].FixedCost_Verified.id_actualcost);

                        await UpdateData.BudgetAdd(cost.wbs_no, cost.cost_center, cost.amount);

                        rejected.comment = "Cancel Travel By AP";
                        rejected.id_actualcost = model[k].FixedCost_Verified.id_actualcost;
                        rejected.process_reject = access_status.position;
                        rejected.user_created = Convert.ToInt32(model[k].EmployeeInfo.code);
                        rejected.created_date = DateTime.Now;

                        await UpdateData.UpdateActualCostForBPD(model[k].FixedCost_Verified.id_actualcost);
                        await UpdateData.UpdateTravelRequestForBPD(model[k].FixedCost_Verified.id_request);

                        await Utility.RecordRejected(rejected);
                        await UpdateData.FixedCost(model[k], access_status.position);
                       
                        List<tb_r_travel_request> travelList = await GetData.TravelRequestGCList(model[k].FixedCost_Verified.group_code.Trim());
                        List<tb_r_travel_actualcost> actual_cost = await GetData.ActualCostBTA(model[k].FixedCost_Verified.group_code.Trim());
                        List<tb_r_travel_actualcost> new_list = actual_cost.Where(b=>b.jenis_transaksi.Contains("hotel") || b.jenis_transaksi.Contains("ticket")).ToList();

                        if (new_list.Count() == 0)
                        {
                            foreach (var item in travelList)
                            {
                                await UpdateData.BudgetAdd(cost.wbs_no, cost.cost_center, Convert.ToDouble(item.grand_total_allowance - item.allowance_meal_idr));
                            }
                        }
                        else if (new_list.Count()>0)
                        {
                            foreach (var item in travelList)
                            {
                                List<tb_r_travel_actualcost> TransactionType = new_list.Where(b => b.id_request == item.id_request).ToList();
                                int Hotel = TransactionType.Where(b => b.jenis_transaksi.Contains("hotel") && b.final_status == null).Count();
                                int Ticket = TransactionType.Where(b => b.jenis_transaksi.Contains("ticket") && b.final_status == null).Count();
                                int Hotel_Reject = TransactionType.Where(b => b.jenis_transaksi.Contains("hotel") && b.final_status != null).Count();
                                int Ticket_Reject = TransactionType.Where(b => b.jenis_transaksi.Contains("ticket") && b.final_status != null).Count();

                                if (Hotel == 0 && Ticket == 0 && Hotel_Reject == 0 && Ticket_Reject == 0)  await UpdateData.BudgetAdd(cost.wbs_no, cost.cost_center, Convert.ToDouble(item.allowance_ticket + item.allowance_hotel));
                                else if (Hotel == 0 && Hotel_Reject == 0 && Ticket_Reject == 0)  await UpdateData.BudgetAdd(cost.wbs_no, cost.cost_center, Convert.ToDouble(item.allowance_hotel));
                                else if (Ticket == 0 && Hotel_Reject == 0 && Ticket_Reject == 0)  await UpdateData.BudgetAdd(cost.wbs_no, cost.cost_center, Convert.ToDouble(item.allowance_ticket));
                            }
                        }
                       
                    }
                    else
                    if (model[k].check_verify == true && model[k].check_reject == false)
                    {
                        //model[k].flag = "1";
                        await UpdateData.FixedCost(model[k], access_status.position);
                        model[k].flag = "1";
                        if (access_status.position.Trim() == "AP")
                        {
                            model[k].money = model[k].money.Replace(".", "");
                            model[k].money = model[k].money.Replace("Rp", "");

                            //await UpdateData.Budget(model[k].FixedCost_Verified.wbs_no, model[k].FixedCost_Verified.cost_center, Convert.ToDouble(model[k].money));
                            //await UpdateData.Budget(model[k].FixedCost_Verified.wbs_no, model[k].FixedCost_Verified.cost_center, double.Parse(model[k].money, NumberStyles.Currency));
                        }
                        await UpdateData.FixedCost(model[k], access_status.position);
                    }
                    else
                        model[k].flag = "0";
                }

                ModelState.Clear();
                //return View("Index", ResultObject2.OrderBy(r => r.FixedCost_Verified.create_date).ToList());
                return RedirectToAction("Index", new { @searchString = searchString });
            }
            else
                //return View("Index", model.OrderBy(r => r.FixedCost_Verified.create_date).ToList());
                return RedirectToAction("Index", new { @searchString = searchString });
        }

    }
   
}