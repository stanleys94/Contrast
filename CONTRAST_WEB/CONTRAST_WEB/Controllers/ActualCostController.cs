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
using System.Security.Claims;
using PagedList;
namespace CONTRAST_WEB.Controllers
{
    public class ActualCostController : Controller
    {
        private CONTRASTEntities db = new CONTRASTEntities();

        // GET: ActualCost
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;
            tb_m_employee model = await GetData.EmployeeInfo(identity.Name);
            List<ActualCostShtHelper> ActualCostHelperObject = new List<ActualCostShtHelper>();
            List<List<SelectListItem>> vendorInfo = new List<List<SelectListItem>>();

            //get rejected list number
            List<vw_rejected_actualcost_verification> RejectedObject = new List<vw_rejected_actualcost_verification>();
            RejectedObject = await GetData.ActualCostRejected();

            //get new request number
            List<vw_actualcost_preparation> PreparationObject = new List<vw_actualcost_preparation>();
            PreparationObject = await GetData.ActualcostPreparation();

            for (int k = 0; k < PreparationObject.Count(); k++)
            {
                ActualCostHelperObject.Add(new ActualCostShtHelper());
                ActualCostHelperObject[k].TravelRequest = new vw_actualcost_preparation();
                ActualCostHelperObject[k].ActualCost = new tb_r_travel_actualcost();
                ActualCostHelperObject[k].TravelRequest = PreparationObject[k];
            }

            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            
            //filter
            if (!String.IsNullOrEmpty(searchString))
            {
                List<ActualCostShtHelper> temp = new List<ActualCostShtHelper>();
                for (int k = 0; k < ActualCostHelperObject.Count; k++)
                {
                    if (ActualCostHelperObject[k].TravelRequest.name.ToLower().Contains(searchString.ToLower())
                        || ActualCostHelperObject[k].TravelRequest.group_code.ToLower().Contains(searchString.ToLower())
                        || ActualCostHelperObject[k].TravelRequest.destination_name.ToLower().Contains(searchString.ToLower())
                        || ActualCostHelperObject[k].TravelRequest.jenis_transaksi.ToLower().Contains(searchString.ToLower())
                        )
                     temp.Add(ActualCostHelperObject[k]);
                }
                ActualCostHelperObject = temp;
            }            

            ViewBag.New = ActualCostHelperObject.Count();
            ViewBag.Rejected = RejectedObject.Count();
            ViewBag.RL = vendorInfo;
            ViewBag.RL3 = await GetData.TaxInfo();
                        
            int pageSize = 15;
            int pageNumber = (page ?? 1);

            //get vendor info
            for (int k = 0; k < ActualCostHelperObject.ToPagedList(pageNumber, pageSize).ToList().Count(); k++)
            {
                if (ActualCostHelperObject.ToPagedList(pageNumber, pageSize)[k].TravelRequest.jenis_transaksi.ToLower() == "ticket")
                    vendorInfo.Add((await GetData.VendorTicketInfo()));
                else
                if (ActualCostHelperObject.ToPagedList(pageNumber, pageSize)[k].TravelRequest.jenis_transaksi.ToLower() == "hotel")
                    vendorInfo.Add((await GetData.VendorHotelInfo()));
            }
            return View(ActualCostHelperObject.ToPagedList(pageNumber, pageSize));
        }

        [Authorize]
        [Authorize(Roles = "contrast.user")]
        public async Task<ActionResult> Rejected(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;
            tb_m_employee model = await GetData.EmployeeInfo(identity.Name);

            List<ActualCostShtHelper> ActualCostHelperObject = new List<ActualCostShtHelper>();
            List<tb_r_travel_actualcost> Rejected = new List<tb_r_travel_actualcost>();
            List<List<SelectListItem>> vendorInfo = new List<List<SelectListItem>>();

            ViewBag.RL3 = await GetData.TaxInfo();

            //get new request number
            List<vw_actualcost_preparation> PreparationObject = new List<vw_actualcost_preparation>();
            PreparationObject = await GetData.ActualcostPreparation();

            //get rejected list number
            List<vw_rejected_actualcost_verification> RejectedObject = new List<vw_rejected_actualcost_verification>();
            RejectedObject = await GetData.ActualCostRejected();

            for (int k = 0; k < RejectedObject.Count(); k++)
            {
                ActualCostHelperObject.Add(new ActualCostShtHelper());
                ActualCostHelperObject[k].ActualCost = new tb_r_travel_actualcost();
                ActualCostHelperObject[k].TravelRequestRejected = new vw_rejected_actualcost_verification();
                ActualCostHelperObject[k].TravelRequestRejected = RejectedObject[k];
            }
            
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;


            //filter
            if (!String.IsNullOrEmpty(searchString))
            {
                List<ActualCostShtHelper> temp = new List<ActualCostShtHelper>();
                for (int k = 0; k < ActualCostHelperObject.Count; k++)
                {
                    if (
                        ActualCostHelperObject[k].TravelRequestRejected.name.ToLower().Contains(searchString.ToLower())
                        || ActualCostHelperObject[k].TravelRequestRejected.group_code.ToLower().Contains(searchString.ToLower())
                        || ActualCostHelperObject[k].TravelRequestRejected.destination_name.ToLower().Contains(searchString.ToLower())
                        || ActualCostHelperObject[k].TravelRequestRejected.jenis_transaksi.ToLower().Contains(searchString.ToLower())
                        )
                        temp.Add(ActualCostHelperObject[k]);

                }
                /*if (temp.Count() > 0)*/
                ActualCostHelperObject = temp;

            }

            

            ViewBag.Rejected = ActualCostHelperObject.Count();
            ViewBag.RL = vendorInfo;
            ViewBag.New = PreparationObject.Count();


            int pageSize = 15;
            int pageNumber = (page ?? 1);

            //get vendor info
            for (int k = 0; k < ActualCostHelperObject.Count(); k++)
            {
                if (ActualCostHelperObject.ToPagedList(pageNumber, pageSize)[k].TravelRequestRejected.jenis_transaksi.ToLower() == "ticket")
                    vendorInfo.Add((await GetData.VendorTicketInfo()));
                else
                if (ActualCostHelperObject.ToPagedList(pageNumber, pageSize)[k].TravelRequestRejected.jenis_transaksi.ToLower() == "hotel")
                    vendorInfo.Add((await GetData.VendorHotelInfo()));
            }

            return View(ActualCostHelperObject.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(List<ActualCostShtHelper> model, string insert = "", string search = "")
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            List<List<SelectListItem>> vendorInfo = new List<List<SelectListItem>>();

            if (search != "")
            {
                int i = 0, PreparationObject = 0, RejectedObject = 0;

                List<ActualCostShtHelper> FilteredList = new List<ActualCostShtHelper>();
                if (model != null)
                {
                    for (int k = 0; k < model.Count(); k++)
                    {
                        if (model[k].TravelRequest != null)
                        {
                            if (
                                model[k].TravelRequest.name.ToLower().Contains(search.ToLower()) ||
                                model[k].TravelRequest.group_code.ToLower().Contains(search.ToLower()) ||
                                model[k].TravelRequest.jenis_transaksi.ToLower().Contains(search.ToLower()) ||
                                model[k].TravelRequest.destination_name.ToLower().Contains(search.ToLower())
                                )
                            {
                                FilteredList.Add(new ActualCostShtHelper());
                                FilteredList[i] = model[k];
                                if (FilteredList[i].TravelRequest.jenis_transaksi.ToLower() == "ticket") vendorInfo.Add((await GetData.VendorTicketInfo()));
                                else
                                if (FilteredList[i].TravelRequest.jenis_transaksi.ToLower() == "hotel") vendorInfo.Add((await GetData.VendorHotelInfo()));
                                i++; PreparationObject++;
                            }
                        }
                        else
                        if (model[k].TravelRequestRejected != null)
                        {
                            if (
                                model[k].TravelRequestRejected.name.ToLower().Contains(search.ToLower()) ||
                                model[k].TravelRequestRejected.group_code.ToLower().Contains(search.ToLower()) ||
                                model[k].TravelRequestRejected.jenis_transaksi.ToLower().Contains(search.ToLower()) ||
                                model[k].TravelRequestRejected.destination_name.ToLower().Contains(search.ToLower())
                                )
                            {
                                FilteredList.Add(new ActualCostShtHelper());
                                FilteredList[i] = model[k];
                                if (FilteredList[i].TravelRequestRejected.jenis_transaksi.ToLower() == "ticket") vendorInfo.Add((await GetData.VendorTicketInfo()));
                                else
                                if (FilteredList[i].TravelRequestRejected.jenis_transaksi.ToLower() == "hotel") vendorInfo.Add((await GetData.VendorHotelInfo()));

                                i++; RejectedObject++;
                            }
                        }

                    }
                }

                ViewBag.New = PreparationObject;
                ViewBag.Rejected = RejectedObject;
                ViewBag.Total = i;

                ViewBag.RL = vendorInfo;
                //ViewBag.RL2 = await GetData.VendorHotelInfo();
                ViewBag.RL3 = await GetData.TaxInfo();
                ModelState.Clear();

                if (FilteredList.Count() != 0) return View("Index", FilteredList);
                else
                {
                    FilteredList.Add(new ActualCostShtHelper());
                    return View("Index", FilteredList);
                }
            }
            else if (insert == "" && search == "")
            {
                ModelState.Clear();
                return RedirectToAction("Index");
            }
            else if (insert == "SUBMIT")
            {

                for (int ck = 0; ck < model.Count(); ck++)
                {
                    if (model[ck].ActualCost.amount != 0)
                    {
                        model[ck].ActualCost.vendor_code = await GetData.VendorCodeInfo(model[ck].ActualCost.vendor_code);
                        model[ck].ActualCost.create_date = DateTime.Now;
                        model[ck].ActualCost.user_created = identity.Name;
                        if (model[ck].TravelRequest != null)
                        {
                            model[ck].ActualCost.no_reg = Convert.ToInt32(model[ck].TravelRequest.no_reg);
                            model[ck].ActualCost.group_code = model[ck].TravelRequest.group_code;
                            model[ck].ActualCost.id_request = model[ck].TravelRequest.id_request;
                            model[ck].ActualCost.jenis_transaksi = model[ck].TravelRequest.jenis_transaksi;
                        }
                        else
                        if (model[ck].TravelRequestRejected != null)
                        {
                            model[ck].ActualCost.no_reg = Convert.ToInt32(model[ck].TravelRequestRejected.no_reg);
                            model[ck].ActualCost.group_code = model[ck].TravelRequestRejected.group_code;
                            model[ck].ActualCost.id_request = model[ck].TravelRequestRejected.id_request;
                            model[ck].ActualCost.jenis_transaksi = model[ck].TravelRequestRejected.jenis_transaksi;
                        }

                        model[ck].ActualCost.pac_insert_by = identity.Name;
                        model[ck].ActualCost.pac_insert_datetime = DateTime.Now;
                        model[ck].ActualCost.pac_status = "1";


                        string division_r = await GetData.GetDivMapping(model[ck].ActualCost.no_reg.ToString());
                        tb_m_budget budget = await GetData.GetCostWbs(model[ck].TravelRequest != null ? (bool)model[ck].TravelRequest.overseas_flag : (bool)model[ck].TravelRequestRejected.overseas_flag, division_r.Trim());

                        model[ck].ActualCost.wbs_no = budget.eoa_wbs_no;
                        model[ck].ActualCost.cost_center = budget.cost_center;

                        //total plus tax
                        double tax = await GetData.TaxValueInfo(model[ck].ActualCost.tax);
                        int amount_tax = Convert.ToInt32((tax * Convert.ToDouble(model[ck].ActualCost.amount)) / 100);
                        model[ck].ActualCost.amount_total = amount_tax + model[ck].ActualCost.amount;
                        model[ck].ActualCost.information_actualcost = "ACTUAL COST";

                        if (model[ck].TravelRequestRejected != null) await UpdateData.ActualCostPersonal(model[ck].TravelRequestRejected.id_actualcost);
                        await InsertData.ActualCost(model[ck].ActualCost);
                        ModelState.Clear();
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.Clear();
                return RedirectToRoute("index");
            }
        }

    }
}
