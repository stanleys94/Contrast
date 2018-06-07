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
using System.Security.Claims;
using PagedList;

namespace CONTRAST_WEB.Controllers
{
    public class VerifySettlementController : Controller
    {        
        [Authorize]
        [Authorize(Roles = "contrast.user")]       
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page,DateTime? startdate, DateTime? enddate)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;
            tb_m_employee model = await GetData.EmployeeInfo(identity.Name);

            tb_m_verifier_employee access_status = new tb_m_verifier_employee();
            access_status = await GetData.EmployeeVerifier(Convert.ToInt32(model.code));
            ViewBag.position = access_status.position;
            List<vw_settlement_verified> ResultObject = new List<vw_settlement_verified>();
            ResultObject = await GetData.SettlementVerifiedList(access_status.position);

            List<tb_r_travel_actualcost> Attachment = new List<tb_r_travel_actualcost>();

            List<SettlementVerifiedHelper> ResultObject2 = new List<SettlementVerifiedHelper>();

            for (int k = 0; k < ResultObject.Count(); k++)
            {
                ResultObject2.Add(new SettlementVerifiedHelper());
                ResultObject2[k].Settlement_Verified = ResultObject[k];
                ResultObject2[k].EmployeeInfo = model;
                ResultObject2[k].money = ResultObject[k].amount.ToString("c", Constant.culture);
                if (ResultObject2[k].Settlement_Verified.jenis_transaksi.Contains("Meal"))
                {
                    tb_r_travel_actualcost HalfDayCheck = await GetData.ActualCostOrigin(ResultObject2[k].Settlement_Verified.id_actualcost);
                    if (HalfDayCheck.additional1 != null)
                    {
                        if (HalfDayCheck.additional1.Contains("True"))
                        {
                            if (HalfDayCheck.additional3 != null)
                            {
                                if (HalfDayCheck.additional3 != "")
                                {
                                    DateTime check = Convert.ToDateTime(HalfDayCheck.additional3);
                                    ResultObject2[k].HDDepart = check.ToString("hh:mm:ss tt");
                                }
                            }
                            ResultObject2[k].HDDepartFlag = "Half Day";
                        }
                    }
                    if (HalfDayCheck.additional2 != null)
                    {
                        if (HalfDayCheck.additional2.Contains("True"))
                        {
                            if (HalfDayCheck.additional4 != null)
                            {
                                if (HalfDayCheck.additional4 != "")
                                {
                                    DateTime check = Convert.ToDateTime(HalfDayCheck.additional4);
                                    ResultObject2[k].HDReturn = check.ToString("hh:mm:ss tt");

                                }
                            }
                            ResultObject2[k].HDReturnFlag = "Half day";
                        }
                    }
                }
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
            if (!String.IsNullOrEmpty(searchString) )
            {
                List<SettlementVerifiedHelper> temp = new List<SettlementVerifiedHelper>();
                for (int k = 0; k < ResultObject2.Count; k++)
                {
                    //by group code
                    if (   ResultObject2[k].Settlement_Verified.group_code.ToLower().Contains(searchString.ToLower())
                        || ResultObject2[k].Settlement_Verified.name.ToLower().Contains(searchString.ToLower())
                        || ResultObject2[k].Settlement_Verified.destination_name.ToLower().Contains(searchString.ToLower())
                       )
                        temp.Add(ResultObject2[k]);
                } 
                if (temp.Count() > 0) ResultObject2 = temp;
            }

            //date filter
            if (startdate != null && enddate != null)
            {
                List<SettlementVerifiedHelper> temp = new List<SettlementVerifiedHelper>();
                for (int k = 0; k < ResultObject2.Count; k++)
                {
                    //by group code
                    if (
                        ResultObject2[k].Settlement_Verified.start_date >= startdate
                        && ResultObject2[k].Settlement_Verified.start_date <= enddate
                       )
                        temp.Add(ResultObject2[k]);
                }
                if (temp.Count() > 0) ResultObject2 = temp;
            }

            List<string> AttachmentPath = new List<string>();
            //attachment
            for (int k = 0; k < ResultObject2.Count; k++)
            {
                Attachment.Add(await GetData.ActualCostOrigin(ResultObject2[k].Settlement_Verified.id_actualcost));
                string temp = Attachment[k].path_file;
                if (temp != null)
                {
                    temp = temp.Split('\\').Last();
                    temp = Constant.Attch + "SettlementFolder/" + temp;
                    ResultObject2[k].path = temp;
                    //AttachmentPath.Add(temp);
                }
            }
            //attachment viewbag
            //ViewBag.Attachment = AttachmentPath;

            //sorting
            if (ResultObject2.Count > 0)
            {
                switch (sortOrder)
                {
                    case "name_desc":
                        ResultObject2 = ResultObject2.OrderByDescending(m => m.Settlement_Verified.name).ToList();
                        break;
                    case "group_desc":
                        ResultObject2 = ResultObject2.OrderByDescending(m => m.Settlement_Verified.group_code).ToList();
                        break;
                    case "type_desc":
                        ResultObject2 = ResultObject2.OrderByDescending(m => m.Settlement_Verified.jenis_transaksi).ToList();
                        break;
                    case "dest_desc":
                        ResultObject2 = ResultObject2.OrderByDescending(m => m.Settlement_Verified.destination_name).ToList();
                        break;
                    case "sdate_desc":
                        ResultObject2 = ResultObject2.OrderByDescending(m => m.Settlement_Verified.start_date).ToList();
                        break;
                    case "edate_desc":
                        ResultObject2 = ResultObject2.OrderByDescending(m => m.Settlement_Verified.end_date).ToList();
                        break;
                }
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);

            if (ResultObject2.Count == 0)
            {
                ResultObject2.Add(new SettlementVerifiedHelper());
                ResultObject2[0].EmployeeInfo = model;
                return View("Index", ResultObject2.ToPagedList(pageNumber, pageSize));
            }
            else
            return View(ResultObject2.ToPagedList(pageNumber, pageSize));
        }
        
        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(List<SettlementVerifiedHelper> model, string search, string insert, DateTime? start, DateTime? end,string sortOrder, string currentFilter, string searchString, int? page, DateTime? startdate, DateTime? enddate)
        {
            tb_m_verifier_employee access_status = new tb_m_verifier_employee();
            
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
                        tb_r_record_rejected_verification rejected = new tb_r_record_rejected_verification();
                        rejected.comment = model[k].comment;
                        rejected.id_actualcost = model[k].Settlement_Verified.id_actualcost;
                        rejected.process_reject = access_status.position;
                        rejected.user_created = Convert.ToInt32(model[k].EmployeeInfo.code);
                        rejected.created_date = DateTime.Now;

                        await Utility.RecordRejected(rejected);

                        model[k].flag = "2";
                        await UpdateData.SettlementRejected(model[k], access_status.position, rejected);

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

                        }
                        await UpdateData.Settlement(model[k], access_status.position);
                    }
                    else
                        model[k].flag = "0";
                }
                
                ModelState.Clear();
                //return View("Index", ResultObject2.OrderBy(m => m.Settlement_Verified.create_date).ToList());
                return RedirectToAction("Index", new { @searchString = searchString});
            }
            //else return View("Index", model.OrderBy(m => m.Settlement_Verified.create_date).ToList());
            return RedirectToAction("Index", new { @searchString = searchString });
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public ActionResult DownloadAttach()
        {
            //string[] name_file = 
            string file = HostingEnvironment.MapPath("~/SettlementFolder/Laundry_101795.png");
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(file, contentType, Path.GetFileName(file));
        }
    }
}
