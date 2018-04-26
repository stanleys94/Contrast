using ClosedXML.Excel;
using CONTRAST_WEB.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace CONTRAST_WEB.Controllers
{
    public class TrackProgressPersonalController : Controller
    {
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page, DateTime? startdate, DateTime? enddate)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;
            tb_m_employee model = await GetData.EmployeeInfo(identity.Name);

            List<TrackingHelper> track = new List<TrackingHelper>();
            List<vw_tracking_transaction_data_new> new_list = new List<vw_tracking_transaction_data_new>();
            int privillage = 0;
            string privillage_desc = "";
            tb_m_employee_source_data Admin = await GetData.GetDivisionSource(Convert.ToInt32(model.code));
            tb_m_verifier_employee verifier = await GetData.EmployeeVerifier(Convert.ToInt32(model.code));

            string division = Admin.Divisi;
            if (Admin.Divisi.Contains("and1")) Admin.Divisi = division.Replace("and1", "&");
            //#1 verifier employee
            //#2 admin istd
            //#3 individual

            //cek privillege
            privillage_desc = " user";
            privillage = 3;

            //pagination
            int pageSize = 15;
            int pageNumber = (page ?? 1);

            //data aggregation
            if (privillage == 1) new_list = await GetData.TrackingListAll();
            else if (privillage == 2) new_list = await GetData.TrackingListDivisonAll(Admin.Divisi.Trim());
            else if (privillage == 3) new_list = await GetData.TrackingListIndividual(model.code);
            if (new_list.Count > 0)
            {
                foreach (var item in new_list)
                {
                    TrackingHelper temp = new TrackingHelper();
                    temp.login_id = model.code;
                    temp.login_name = model.name;
                    temp.privilage = privillage_desc;
                    temp.TrackedList = item;
                    track.Add(temp);
                }
            }
            //else
            //{
            //    TrackingHelper temp = new TrackingHelper();
            //    temp.login_id = model.code;
            //    temp.login_name = model.name;
            //    temp.privilage = privillage_desc;
            //    track.Add(temp);
            //    //return View(track);
            //    //return View(track.OrderBy(m => m.TrackedList.group_code).ToPagedList(pageNumber, pageSize));
            //}
            //return View("Index", track.OrderBy(m => m.TrackedList.group_code).ThenBy(m => m.TrackedList.create_date).ToList());

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
                List<TrackingHelper> temp = new List<TrackingHelper>();
                for (int k = 0; k < track.Count; k++)
                {
                    //by anything
                    if (track[k].TrackedList.group_code.ToLower().Contains(searchString.ToLower())
                        || track[k].TrackedList.name.ToLower().Contains(searchString.ToLower())
                        || track[k].TrackedList.destination_name.ToLower().Contains(searchString.ToLower())
                        || track[k].TrackedList.verified_flag.ToLower().Contains(searchString.ToLower())
                        || track[k].TrackedList.TYPES_OF_TRANSACTIONS.ToLower().Contains(searchString.ToLower())
                       )
                        temp.Add(track[k]);
                }
                /*if (temp.Count() > 0)*/
                track = temp;

            }

            //date filter
            if (startdate != null && enddate != null)
            {
                List<TrackingHelper> temp = new List<TrackingHelper>();
                for (int k = 0; k < track.Count; k++)
                {
                    //by group code
                    if (
                        track[k].TrackedList.start_date >= startdate
                        && track[k].TrackedList.start_date <= enddate
                       )
                        temp.Add(track[k]);
                }
                if (temp.Count() > 0) track = temp;
            }
            return View(track.OrderByDescending(m => m.TrackedList.id_data).ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Search(List<TrackingHelper> Model, int? detail = null)
        {
            int id_data = Convert.ToInt32(detail);
            Model[id_data].id_data = Model[id_data].TrackedList.id_data;
            TrackingHelper newModel = new TrackingHelper();
            newModel = Model[id_data];
            return RedirectToAction("Details", "TrackProgressPersonal", newModel);
        }
        //wates
        public async Task<ActionResult> Details(TrackingHelper mood)
        {
            TrackingHelper Model = new TrackingHelper();
            Model.TrackedList = await GetData.TrackingListID(mood.id_data);

            TrackingDetailHelper Detailed = new TrackingDetailHelper();
            Detailed.privilage = mood.privilage;
            Detailed.logged_id = new tb_m_employee();
            Detailed.logged_id = await GetData.EmployeeInfo(mood.login_id);

            List<tb_r_travel_request> TravelCode = await GetData.TravelRequestGCList(Model.TrackedList.group_code);
            Detailed.Name = Model.TrackedList.name;
            Detailed.Division = Model.TrackedList.divisi;
            Detailed.GroupCode = Model.TrackedList.group_code;
            Detailed.EmployeeCode = Model.TrackedList.no_reg.ToString();

            Detailed.Destination = new List<string>();
            Detailed.StartDate = new List<DateTime>();
            Detailed.EndDate = new List<DateTime>();

            Detailed.HigherUpCode = new List<string>();
            Detailed.HigherUpApprovalDate = new List<string>();
            Detailed.HigherUp = new List<string>();
            Detailed.Settle = new List<tb_r_travel_settlement>();
            Detailed.ActualCost = new List<COST>();
            Detailed.BPD = new List<COST>();
            Detailed.Executed = new List<tb_r_travel_execution>();
            Detailed.HigherUpApprovalStatus = new List<string>();
            Detailed.SettlementCost = new List<COST>();

            for (int i = 0; i < TravelCode.Count(); i++)
            {
                string city = await GetData.DestinationNameInfo(TravelCode[i].id_destination_city);
                Detailed.Destination.Add(city);
                Detailed.StartDate.Add(Convert.ToDateTime(TravelCode[i].start_date));
                Detailed.EndDate.Add(Convert.ToDateTime(TravelCode[i].end_date));
            }

            if (TravelCode[0].apprv_by_lvl1.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl1.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl1));
                if (TravelCode[0].apprv_date_lvl1.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl1).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl1 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl1.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl1.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl1.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl2.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl2.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl2));
                if (TravelCode[0].apprv_date_lvl2.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl2).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl2 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl2.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl2.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl2.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";
                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl3.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl3.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl3));
                if (TravelCode[0].apprv_date_lvl3.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl3).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl3 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl3.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl3.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl3.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";
                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl4.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl4.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl4));
                if (TravelCode[0].apprv_date_lvl4.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl4).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl4 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl4.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl4.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl4.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl5.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl5.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl5));
                if (TravelCode[0].apprv_date_lvl5.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl5).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl5 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl5.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl5.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl5.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl6.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl6.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl6));
                if (TravelCode[0].apprv_date_lvl6.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl6).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl6 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl6.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl6.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl6.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl7.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl7.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl7));
                if (TravelCode[0].apprv_date_lvl7.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl7).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl7 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl7.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl7.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl7.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl8.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl8.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl8));
                if (TravelCode[0].apprv_date_lvl8.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl8).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl8 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl8.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl8.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl8.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl9.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl9.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl9));
                if (TravelCode[0].apprv_date_lvl9.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl9).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl9 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl9.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl9.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl9.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl10.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl10.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl10));
                if (TravelCode[0].apprv_date_lvl10.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl10).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl10 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl10.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl10.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl10.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl11.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl11.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl11));
                if (TravelCode[0].apprv_date_lvl11.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl11).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl11 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl11.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl11.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl11.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl12.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl12.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl12));
                if (TravelCode[0].apprv_date_lvl12.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl12).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl12 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl12.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl12.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl12.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl13.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl13.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl13));
                if (TravelCode[0].apprv_date_lvl13.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl13).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl13 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl13.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl13.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl13.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl14.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl14.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl14));
                if (TravelCode[0].apprv_date_lvl14.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl14).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl14 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl14.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl14.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl14.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl15.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl15.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl15));
                if (TravelCode[0].apprv_date_lvl15.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl15).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl15 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl15.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl15.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl15.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl16.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl16.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl16));
                if (TravelCode[0].apprv_date_lvl16.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl16).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl16 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl16.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl16.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl16.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl17.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl17.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl17));
                if (TravelCode[0].apprv_date_lvl17.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl17).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl17 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl17.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl17.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl17.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl18.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl18.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl18));
                if (TravelCode[0].apprv_date_lvl18.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl18).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl18 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl18.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl18.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl18.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl19.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl19.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl19));
                if (TravelCode[0].apprv_date_lvl19.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl19).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl19 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl19.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl19.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl19.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }
            if (TravelCode[0].apprv_by_lvl20.HasValue)
            {
                Detailed.HigherUpCode.Add(TravelCode[0].apprv_by_lvl20.ToString());
                Detailed.HigherUp.Add(await GetData.EmployeeNameInfo(TravelCode[0].apprv_by_lvl20));
                if (TravelCode[0].apprv_date_lvl20.HasValue) Detailed.HigherUpApprovalDate.Add(Convert.ToDateTime(TravelCode[0].apprv_date_lvl20).ToString());
                else Detailed.HigherUpApprovalDate.Add("N/A");
                string status = "";
                string date = "";
                if (TravelCode[0].apprv_flag_lvl20 != null)
                {
                    if (TravelCode[0].apprv_flag_lvl20.Contains("1")) status = "Approved";
                    else if (TravelCode[0].apprv_flag_lvl20.Contains("2")) status = "Rejected";
                    else if (TravelCode[0].apprv_flag_lvl20.Contains("0")) status = "Pending";
                }
                else status = "Not Yet";


                Detailed.HigherUpApprovalStatus.Add(status);
            }

            List<tb_r_travel_actualcost> ActualCost = await GetData.ActualCostBTA(TravelCode[0].group_code);
            List<COST> unlisted = new List<COST>();
            foreach (var item in ActualCost)
            {

                COST new_cost = new COST();
                new_cost.CostType = item.information_actualcost;
                new_cost.Transaction = item.jenis_transaksi;
                new_cost.Amount = item.amount;
                new_cost.Vendor = await GetData.VendorCodeInfo(item.vendor_code);

                if (item.information_actualcost.Contains("ACTUAL COST"))
                {
                    if (item.ap_verified_status != null)
                    {
                        new_cost.Approved = "AP";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.ap_verified_datetime);
                        if (item.ap_verified_status.Contains("1")) new_cost.Approved_Status = "Approved";
                        else if (item.ap_verified_status.Contains("2")) new_cost.Approved_Status = "Rejected";
                        else if (item.ap_verified_status.Contains("3")) new_cost.Approved_Status = "Rejected";
                        if (!item.ap_verified_status.Contains("1")) new_cost.Pending = "None";
                        else new_cost.Pending = "None";
                    }
                    else if (item.dph_verified_status != null)
                    {
                        new_cost.Approved = "DpH-GA";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.dph_verified_datetime);
                        if (item.dph_verified_status.Contains("1")) new_cost.Approved_Status = "Approved";
                        else if (item.dph_verified_status.Contains("2")) new_cost.Approved_Status = "Rejected";
                        else if (item.dph_verified_status.Contains("3")) new_cost.Approved_Status = "Rejected";
                        if (!item.dph_verified_status.Contains("1")) new_cost.Pending = "None";
                        else new_cost.Pending = "AP";
                    }
                    else if (item.ga_status != null)
                    {
                        new_cost.Approved = "Staff-GA";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.ga_insert_datetime);
                        if (item.ga_status.Contains("1")) new_cost.Approved_Status = "Approved";
                        else if (item.ga_status.Contains("2")) new_cost.Approved_Status = "Rejected";
                        else if (item.ga_status.Contains("3")) new_cost.Approved_Status = "Rejected";
                        if (!item.ga_status.Contains("1")) new_cost.Pending = "None";
                        else new_cost.Pending = "DpH-GA";
                    }
                    else if (item.sh_verified_status != null)
                    {
                        new_cost.Approved = "DpH-PAC";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.sh_verified_datetime);
                        if (item.sh_verified_status.Contains("1")) new_cost.Approved_Status = "Approved";
                        else if (item.sh_verified_status.Contains("2")) new_cost.Approved_Status = "Rejected";
                        else if (item.sh_verified_status.Contains("3")) new_cost.Approved_Status = "Rejected";
                        if (!item.sh_verified_status.Contains("1")) new_cost.Pending = "None";
                        else new_cost.Pending = "Staff-GA";
                    }
                    else
                    {
                        new_cost.Approved = "None";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.create_date);
                        new_cost.Approved_Status = "Not Created Yet";
                        new_cost.Pending = "Staff-PAC";
                    }
                    Detailed.ActualCost.Add(new_cost);
                }
                else if (item.information_actualcost.Contains("Settlement"))
                {
                    if (item.ap_verified_status != null)
                    {
                        new_cost.Approved = "AP";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.ap_verified_datetime);
                        if (item.ap_verified_status.Contains("1")) new_cost.Approved_Status = "Approved";
                        else if (item.ap_verified_status.Contains("2")) new_cost.Approved_Status = "Rejected";
                        else if (item.ap_verified_status.Contains("3")) new_cost.Approved_Status = "Rejected";
                        if (!item.ap_verified_status.Contains("1")) new_cost.Pending = "None";
                        else new_cost.Pending = "None";
                    }
                    else if (item.dph_verified_status != null)
                    {
                        new_cost.Approved = "DpH-GA";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.dph_verified_datetime);
                        if (item.dph_verified_status.Contains("1")) new_cost.Approved_Status = "Approved";
                        else if (item.dph_verified_status.Contains("2")) new_cost.Approved_Status = "Rejected";
                        else if (item.dph_verified_status.Contains("3")) new_cost.Approved_Status = "Rejected";
                        if (!item.dph_verified_status.Contains("1")) new_cost.Pending = "None";
                        else new_cost.Pending = "AP";
                    }
                    else if (item.ga_status != null)
                    {
                        new_cost.Approved = "Staff-GA";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.ga_insert_datetime);
                        if (item.ga_status.Contains("1")) new_cost.Approved_Status = "Approved";
                        else if (item.ga_status.Contains("2")) new_cost.Approved_Status = "Rejected";
                        else if (item.ga_status.Contains("3")) new_cost.Approved_Status = "Rejected";
                        if (!item.ga_status.Contains("1")) new_cost.Pending = "None";
                        else new_cost.Pending = "DpH-GA";
                    }

                    else
                    {
                        new_cost.Approved = "None";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.create_date);
                        new_cost.Approved_Status = "Not Created Yet";
                        new_cost.Pending = "Staff-GA";
                    }

                    if (item.path_file != null)
                    {
                        if (item.path_file != "Error")
                        {

                            //new_cost.Path = "http://passport.toyota.astra.co.id:5006/";
                            //new_cost.Path = "http://10.85.40.68:91/";
                            new_cost.Path = Constant.Attch;

                            string[] newPath = item.path_file.Split('\\');
                            for (int k = newPath.Count() - 2; k < newPath.Count(); k++)
                            {
                                if (k < (newPath.Count() - 1)) new_cost.Path = new_cost.Path + newPath[k].Replace(" ", "%20") + "/";
                                else new_cost.Path = new_cost.Path + newPath[k].Replace(" ", "%20");
                            }
                        }
                    }
                    Detailed.SettlementCost.Add(new_cost);
                }
                else if (item.information_actualcost.Contains("BPD"))
                {
                    if (item.ap_verified_status != null)
                    {
                        new_cost.Approved = "AP";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.ap_verified_datetime);
                        if (item.ap_verified_status.Contains("1")) new_cost.Approved_Status = "Approved";
                        else if (item.ap_verified_status.Contains("2")) new_cost.Approved_Status = "Rejected";
                        else if (item.ap_verified_status.Contains("3")) new_cost.Approved_Status = "Rejected";
                        if (!item.ap_verified_status.Contains("1")) new_cost.Pending = "None";
                        else new_cost.Pending = "None";
                    }

                    else
                    {
                        new_cost.Approved = "None";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.create_date);
                        new_cost.Approved_Status = "Not Approved yet";
                        new_cost.Pending = "AP";
                    }
                    Detailed.BPD.Add(new_cost);
                }

            }
            Detailed.Executed = await GetData.TravelExecution(Detailed.GroupCode);
            for (int i = 0; i < Detailed.Executed.Count; i++)
            {
                if (Detailed.Executed[i].pic_path != null)
                {
                    string[] path = Detailed.Executed[i].pic_path.Split('\\');
                    //ojo hardcode
                    //Detailed.Executed[i].pic_path = "http://passport.toyota.astra.co.id:5006/";
                    //Detailed.Executed[i].pic_path = "http://10.85.40.68:91/";
                    Detailed.Executed[i].pic_path = Constant.Attch;
                    for (int k = 2; k < path.Count(); k++)
                    {
                        if (k < path.Count() - 1) Detailed.Executed[i].pic_path = Detailed.Executed[i].pic_path + path[k].Replace(" ", "%20") + '/';
                        else Detailed.Executed[i].pic_path = Detailed.Executed[i].pic_path + path[k].Replace(" ", "%20");
                    }
                }

            }
            return View(Detailed);
        }
    }
}
