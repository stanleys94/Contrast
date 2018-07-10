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
using PagedList.Mvc;
using System.Security.Claims;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using PdfSharp;

namespace CONTRAST_WEB.Controllers
{
    public class TrackProgressController : Controller
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

            //#1 verifier employee
            //#2 admin istd
            //#3 individual

            //cek privillege
            for (int k = 0; k < claims.Count(); k++)
            {
                if (claims[k] == "contrast.travelcoordinator" || claims[k] == "contrast.secretary" || claims[k] == "contrast.adminga" || claims[k] == "contrast.ap" || claims[k] == "contrast.dphfad" || claims[k] == "contrast.dphga" || claims[k] == "contrast.dphpac" || claims[k] == "contrast.shfad" || claims[k] == "contrast.shpac" || claims[k] == "contrast.staffga" || claims[k] == "contrast.staffpac")
                {
                    privillage = 1;
                    privillage_desc = "all";
                    break;
                }
                else
                if (claims[k] == "contrast.administd")
                {
                    privillage = 2;
                    privillage_desc = " admin";
                    break;
                }
                else
                {
                    privillage_desc = " user";
                    privillage = 3;
                }
            }

            //pagination
            int pageSize = 15;
            int pageNumber = (page ?? 1);

            //data aggregation
            if (privillage == 1)
                new_list = await GetData.TrackingListAll();
            else if (privillage == 2)
            {
                tb_m_employee_source_data Admin = await GetData.GetDivisionSource(Convert.ToInt32(model.code));
                tb_m_verifier_employee verifier = await GetData.EmployeeVerifier(Convert.ToInt32(model.code));

                string division = Admin.Divisi;
                if (Admin.Divisi.Contains("and1")) Admin.Divisi = division.Replace("and1", "&");

                new_list = await GetData.TrackingListDivisonAll(Admin.Divisi.Trim());
            }
            else 
                if (privillage == 3) new_list = await GetData.TrackingListIndividual(model.code);

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
                    //by group code
                    if (track[k].TrackedList.group_code.ToLower().Contains(searchString.ToLower())
                        || track[k].TrackedList.name.ToLower().Contains(searchString.ToLower())
                        || track[k].TrackedList.destination_name.ToLower().Contains(searchString.ToLower())
                        || track[k].TrackedList.verified_flag.ToLower().Contains(searchString.ToLower())
                        || track[k].TrackedList.TYPES_OF_TRANSACTIONS.ToLower().Contains(searchString.ToLower())
                       )
                        temp.Add(track[k]);
                }
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
                        track[k].TrackedList.create_date >= startdate
                        && track[k].TrackedList.create_date <= enddate
                       )
                        temp.Add(track[k]);
                }
                /*if (temp.Count() > 0)*/
                track = temp;
            }

            List<int> id_data = new List<int>();
            for (int k = 0; k < track.Count; k++)
                id_data.Add(track[k].TrackedList.id_data);
            ViewBag.id_data = id_data;
            //return View(track.OrderBy(m => m.TrackedList.group_code).ToPagedList(pageNumber, pageSize));
            return View(track.OrderByDescending(m => m.TrackedList.id_data).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SearchDetail(List<TrackingHelper> Model, int? detail = null)
        {
            int id_data = Convert.ToInt32(detail);
            Model[id_data].id_data = Model[id_data].TrackedList.id_data;
            TrackingHelper newModel = new TrackingHelper();
            newModel = Model[id_data];
            return RedirectToAction("Details", "TrackProgress", newModel);
        }

        public async Task<ActionResult> Details(TrackingHelper mood)
        {
            TrackingHelper Model = new TrackingHelper();
            Model.TrackedList = await GetData.TrackingListID(mood.id_data);

            TrackingDetailHelper Detailed = new TrackingDetailHelper();
            Detailed.privilage = mood.privilage;
            Detailed.logged_id = new tb_m_employee();
            Detailed.logged_id = await GetData.EmployeeInfo(mood.login_id);

            List<tb_r_travel_request> TravelCode = await GetData.TravelRequestGCList(Model.TrackedList.group_code);
            List<tb_r_travel_request_participant> Participant_List = new List<tb_r_travel_request_participant>();

            if (TravelCode.Count() > 0)
            {
                if (TravelCode[0].participants_flag == true)
                {
                    PARTICIPANT current = new PARTICIPANT();
                    current.name = await GetData.EmployeeNameInfo(TravelCode[0].no_reg);
                    current.no_reg = (int)TravelCode[0].no_reg;
                    current.division = await GetData.GetDivisionSourceName((int)TravelCode[0].no_reg);
                    if (TravelCode[0].invited_by == TravelCode[0].no_reg)
                    {
                        current.status = "Parent";
                    }
                    else
                    {
                        current.status = "Children";
                    }

                    Detailed.Participant.Add(current);

                    if (current.status == "Parent")
                    {
                        List<tb_r_travel_request_participant> participant_list = await GetData.TravelRequestParticipant(Detailed.Participant[0].no_reg.ToString(), Detailed.Participant[0].BTA);
                        foreach (var item in participant_list)
                        {
                            PARTICIPANT part = new PARTICIPANT();
                            part.name = await GetData.EmployeeNameInfo(item.no_reg);
                            part.division = await GetData.GetDivisionSourceName((int)item.no_reg);
                            part.status = "Children";
                            part.no_reg = (int) item.no_reg;
                            List<tb_r_travel_request> BTA = await GetData.TravelRequestList();
                            List<tb_r_travel_request> children = BTA.Where(b => b.no_reg == (int?)part.no_reg && (b.participants_flag==true)  && b.invited_by == current.no_reg && b.start_date == TravelCode[0].start_date && b.id_destination_city == TravelCode[0].id_destination_city).ToList();
                        }
                    }
                }
            }
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
                if (TravelCode[i].path_general != null)
                {
                    if (TravelCode[i].path_general.Contains("http")) Detailed.Itinierary = TravelCode[i].path_general;
                }
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

                if (item.information_actualcost.Contains("ACTUAL COST") || item.information_actualcost.Contains("REJECTED ACTUALCOST") )
                {
                    if (item.ap_verified_status != null)
                    {
                        new_cost.Approved = "AP";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.ap_verified_datetime);
                        if (item.ap_verified_status.Contains("1")) new_cost.Approved_Status = "Approved";
                        else if (item.ap_verified_status.Contains("2")) new_cost.Approved_Status = "Rejected";                     
                        if (!item.ap_verified_status.Contains("1")) new_cost.Pending = "None";
                        else new_cost.Pending = "None";
                    }
                    else if (item.dph_verified_status != null)
                    {
                        new_cost.Approved = "DpH-GA";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.dph_verified_datetime);
                        if (item.dph_verified_status.Contains("1")) new_cost.Approved_Status = "Approved";
                        else if (item.dph_verified_status.Contains("2")) new_cost.Approved_Status = "Rejected";
                        if (!item.dph_verified_status.Contains("1")) new_cost.Pending = "None";
                        else new_cost.Pending = "AP";
                    }
                    else if (item.ga_status != null)
                    {
                        new_cost.Approved = "Staff-GA";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.ga_insert_datetime);
                        if (item.ga_status.Contains("1")) new_cost.Approved_Status = "Approved";
                        else if (item.ga_status.Contains("2")) new_cost.Approved_Status = "Rejected";
                        if (!item.ga_status.Contains("1")) new_cost.Pending = "None";
                        else new_cost.Pending = "DpH-GA";
                    }
                    else if (item.sh_verified_status != null)
                    {
                        new_cost.Approved = "DpH-PAC";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.sh_verified_datetime);
                        if (item.sh_verified_status.Contains("1")) new_cost.Approved_Status = "Approved";
                        else if (item.sh_verified_status.Contains("2")) new_cost.Approved_Status = "Rejected";
                        if (!item.sh_verified_status.Contains("1")) new_cost.Pending = "None";
                        else new_cost.Pending = "Staff-GA";
                    }
                    else
                    {
                        new_cost.Approved = "Staff-PAC";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.create_date);
                        new_cost.Approved_Status = "Submitted";
                        new_cost.Pending = "DpH-PAC";
                    }
                    Detailed.ActualCost.Add(new_cost);
                }
                else if (item.information_actualcost.Contains("Settlement"))
                {
                    if (item.final_status != null)
                    {
                        if (item.final_status.Contains("2"))
                        {
                            new_cost.Approved = "None";
                            new_cost.ApprovedDate = Convert.ToDateTime(item.create_date);
                            new_cost.Approved_Status = "Rejected";
                            new_cost.Pending = "None";
                        }
                        else if (item.final_status.Contains("3"))
                        {
                            new_cost.Approved = "None";
                            new_cost.ApprovedDate = Convert.ToDateTime(item.create_date);
                            new_cost.Approved_Status = "Inactive Settlement";
                            new_cost.Pending = "None";
                        }
                        else
                        {
                            if (item.ap_verified_status != null)
                            {
                                new_cost.Approved = "AP";
                                new_cost.ApprovedDate = Convert.ToDateTime(item.ap_verified_datetime);
                                if (item.ap_verified_status.Contains("1")) new_cost.Approved_Status = "Approved";
                                else if (item.ap_verified_status.Contains("2")) new_cost.Approved_Status = "Rejected";
                                if (!item.ap_verified_status.Contains("1")) new_cost.Pending = "None";
                                else new_cost.Pending = "None";
                            }
                            else if (item.dph_verified_status != null)
                            {
                                new_cost.Approved = "DpH-GA";
                                new_cost.ApprovedDate = Convert.ToDateTime(item.dph_verified_datetime);
                                if (item.dph_verified_status.Contains("1")) new_cost.Approved_Status = "Approved";
                                else if (item.dph_verified_status.Contains("2")) new_cost.Approved_Status = "Rejected";
                                if (!item.dph_verified_status.Contains("1")) new_cost.Pending = "None";
                                else new_cost.Pending = "AP";
                            }
                            else if (item.ga_status != null)
                            {
                                new_cost.Approved = "Staff-GA";
                                new_cost.ApprovedDate = Convert.ToDateTime(item.ga_insert_datetime);
                                if (item.ga_status.Contains("1")) new_cost.Approved_Status = "Approved";
                                else if (item.ga_status.Contains("2")) new_cost.Approved_Status = "Rejected";
                                if (!item.ga_status.Contains("1")) new_cost.Pending = "None";
                                else new_cost.Pending = "DpH-GA";
                            }
                        }
                    }
                    else
                    {
                        new_cost.Approved = "None";
                        new_cost.ApprovedDate = Convert.ToDateTime(item.create_date);
                        new_cost.Approved_Status = "Submitted";
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
                else if (item.information_actualcost.Contains("BPD")|| item.information_actualcost.Contains("CANCEL TRAVEL"))
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

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Download(string sortOrder, string currentFilter, string searchString, int? page, DateTime? startdate, DateTime? enddate)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;
            tb_m_employee employee = await GetData.EmployeeInfo(identity.Name);

            List<TrackingHelper> Model = new List<TrackingHelper>();
            List<vw_tracking_transaction_data_new> new_list = new List<vw_tracking_transaction_data_new>();
            int privillage = 0;
            string privillage_desc = "";


            //#1 verifier employee
            //#2 admin istd
            //#3 individual

            //cek privillege
            for (int k = 0; k < claims.Count(); k++)
            {
                if (claims[k] == "contrast.travelcoordinator" || claims[k] == "contrast.secretary" || claims[k] == "contrast.adminga" || claims[k] == "contrast.ap" || claims[k] == "contrast.dphfad" || claims[k] == "contrast.dphga" || claims[k] == "contrast.dphpac" || claims[k] == "contrast.shfad" || claims[k] == "contrast.shpac" || claims[k] == "contrast.staffga" || claims[k] == "contrast.staffpac")
                {
                    privillage = 1;
                    privillage_desc = "all";
                    break;
                }
                else
                if (claims[k] == "contrast.administd")
                {
                    privillage = 2;
                    privillage_desc = " admin";
                    break;
                }
                else
                {
                    privillage_desc = " user";
                    privillage = 3;
                }
            }

            //pagination
            int pageSize = 15;
            int pageNumber = (page ?? 1);

            //data aggregation
            if (privillage == 1)
                new_list = await GetData.TrackingListAll();
            else if (privillage == 2)
            {
                tb_m_employee_source_data Admin = await GetData.GetDivisionSource(Convert.ToInt32(employee.code));
                tb_m_verifier_employee verifier = await GetData.EmployeeVerifier(Convert.ToInt32(employee.code));

                string division = Admin.Divisi;
                if (Admin.Divisi.Contains("and1")) Admin.Divisi = division.Replace("and1", "&");

                new_list = await GetData.TrackingListDivisonAll(Admin.Divisi.Trim());
            }
            else
                if (privillage == 3) new_list = await GetData.TrackingListIndividual(employee.code);

            if (new_list.Count > 0)
            {
                foreach (var item in new_list)
                {
                    TrackingHelper temp = new TrackingHelper();
                    temp.login_id = employee.code;
                    temp.login_name = employee.name;
                    temp.privilage = privillage_desc;
                    temp.TrackedList = item;
                    Model.Add(temp);
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
            if (!String.IsNullOrEmpty(searchString))
            {
                List<TrackingHelper> temp = new List<TrackingHelper>();
                for (int k = 0; k < Model.Count; k++)
                {
                    //by group code
                    if (Model[k].TrackedList.group_code.ToLower().Contains(searchString.ToLower())
                        || Model[k].TrackedList.name.ToLower().Contains(searchString.ToLower())
                        || Model[k].TrackedList.destination_name.ToLower().Contains(searchString.ToLower())
                        || Model[k].TrackedList.verified_flag.ToLower().Contains(searchString.ToLower())
                        || Model[k].TrackedList.TYPES_OF_TRANSACTIONS.ToLower().Contains(searchString.ToLower())
                       )
                        temp.Add(Model[k]);
                }
                Model = temp;
            }

            //date filter
            if (startdate != null && enddate != null)
            {
                List<TrackingHelper> temp = new List<TrackingHelper>();
                for (int k = 0; k < Model.Count; k++)
                {
                    //by group code
                    if (
                        Model[k].TrackedList.create_date >= startdate
                        && Model[k].TrackedList.create_date <= enddate
                       )
                        temp.Add(Model[k]);
                }
                /*if (temp.Count() > 0)*/
                Model = temp;
            }

            //List<TrackingHelper> Model = new List<TrackingHelper>();
            //for (int k = 0; k < id_data.Count(); k++)
            //{
            //    Model.Add(new TrackingHelper());
            //    Model[k].TrackedList = await GetData.DownloadFileTrack(id_data[k]);
            //}

            XLWorkbook CreateExcell = new XLWorkbook();
            var InsertData = CreateExcell.Worksheets.Add("Tracking Data");

            InsertData.Cell(1, 1).Value = "Name";
            InsertData.Cell(1, 2).Value = "Employee Code";
            InsertData.Cell(1, 3).Value = "Group Code";
            InsertData.Cell(1, 4).Value = "Destination";
            InsertData.Cell(1, 5).Value = "Transaction";
            InsertData.Cell(1, 6).Value = "Amount";
            InsertData.Cell(1, 7).Value = "Transaction Type";
            InsertData.Cell(1, 8).Value = "WBS No";
            InsertData.Cell(1, 9).Value = "Cost Center";
            InsertData.Cell(1, 10).Value = "Tax";
            InsertData.Cell(1, 11).Value = "Vendor Code";
            InsertData.Cell(1, 12).Value = "Invoice_number";
            InsertData.Cell(1, 13).Value = "Amount Total";
            InsertData.Cell(1, 14).Value = "Start Date";
            InsertData.Cell(1, 15).Value = "End Date";
            InsertData.Cell(1, 16).Value = "Approval";
            InsertData.Cell(1, 17).Value = "Create Date";
            InsertData.Cell(1, 18).Value = "Reason of Assignment";
            InsertData.Cell(1, 19).Value = "Activity Name";
            InsertData.Cell(1, 20).Value = "Approval Date";

            for (int i = 0; i < Model.Count(); i++)
            {
                InsertData.Cell(i + 2, 1).Value = Model[i].TrackedList.name;
                InsertData.Cell(i + 2, 2).Value = Model[i].TrackedList.no_reg;
                InsertData.Cell(i + 2, 3).Value = Model[i].TrackedList.group_code;
                InsertData.Cell(i + 2, 4).Value = Model[i].TrackedList.destination_name;
                InsertData.Cell(i + 2, 5).Value = Model[i].TrackedList.jenis_transaksi;
                InsertData.Cell(i + 2, 6).Value = Model[i].TrackedList.amount;
                InsertData.Cell(i + 2, 7).Value = Model[i].TrackedList.TYPES_OF_TRANSACTIONS;
                InsertData.Cell(i + 2, 8).Value = Model[i].TrackedList.wbs_no;
                InsertData.Cell(i + 2, 9).Value = Model[i].TrackedList.cost_center;
                InsertData.Cell(i + 2, 10).Value = Model[i].TrackedList.tax;
                InsertData.Cell(i + 2, 11).Value = Model[i].TrackedList.vendor_code;
                InsertData.Cell(i + 2, 12).Value = Model[i].TrackedList.invoice_number;
                InsertData.Cell(i + 2, 13).Value = Model[i].TrackedList.amount_total;
                InsertData.Cell(i + 2, 14).Value = Model[i].TrackedList.start_date;
                InsertData.Cell(i + 2, 15).Value = Model[i].TrackedList.end_date;
                InsertData.Cell(i + 2, 16).Value = Model[i].TrackedList.verified_flag;
                InsertData.Cell(i + 2, 17).Value = Model[i].TrackedList.create_date;
                InsertData.Cell(i + 2, 18).Value = Model[i].TrackedList.reason_of_assigment;
                InsertData.Cell(i + 2, 19).Value = Model[i].TrackedList.activity_name;
                InsertData.Cell(i + 2, 20).Value = Model[i].TrackedList.approval_date;
            }
            MemoryStream excelStream = new MemoryStream();
            CreateExcell.SaveAs(excelStream);
            excelStream.Position = 0;
            return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Transaction Tracking Data.xlsx");
        }

        public async Task<ActionResult> Print(string BTA)
        {
            List<tb_r_travel_request> request = await GetData.TravelRequestGCList(BTA);
            List<tb_r_travel_actualcost> actual = await GetData.ActualCostBTA(BTA);

            int meal_allowance = 0, meal_reimburse = 0, ticket_allowance = 0, ticket_reimburse = 0, hotel_allowance = 0, hotel_reimburse = 0, winter_allowance = 0, winter_reimburse = 0;
            int misc_allowance = 0, misc_reimburse = 0, laundry_allowance = 0, laundry_reimburse = 0, transport_allowance = 0, transport_reimburse = 0;
            string HDDepart = null, HDDepartFlag = null, HDReturn = null, HDReturnFlag = null;
            DateTime? start_extend = null, end_extend = null;
            foreach (var item in actual)
            {
                if (item.information_actualcost == "BPD")
                {
                    if (item.jenis_transaksi.Contains("Meal"))
                    {
                        meal_allowance = meal_allowance + item.amount;
                    }
                    if (item.jenis_transaksi.Contains("Winter"))
                    {
                        winter_allowance = winter_allowance + item.amount;
                    }
                }
                else if (item.information_actualcost == "ACTUAL COST")
                {
                    if (item.jenis_transaksi.Contains("ticket"))
                    {
                        ticket_allowance = ticket_allowance + item.amount;
                    }
                    else if (item.jenis_transaksi.Contains("hotel"))
                    {
                        hotel_allowance = hotel_allowance + item.amount;
                    }
                }

                else if (item.information_actualcost == "Settlement")
                {
                    bool settle = false;
                    if (item.final_status != null)
                    {
                        if (item.final_status.Contains("3") && !item.final_status.Contains("2")) settle = false;
                        else settle = true;
                    }
                    else settle = true;
                    if (settle)
                    {
                        if (item.jenis_transaksi.Contains("Meal"))
                        {
                            meal_reimburse = meal_reimburse + item.amount;
                        }
                        else if (item.jenis_transaksi.Contains("Ticket"))
                        {
                            ticket_reimburse = ticket_reimburse + item.amount;
                        }
                        else if (item.jenis_transaksi.Contains("Winter"))
                        {
                            winter_reimburse = winter_reimburse + item.amount;
                        }
                        else if (item.jenis_transaksi.Contains("Hotel"))
                        {
                            ticket_reimburse = ticket_reimburse + item.amount;
                        }

                        else if (item.jenis_transaksi.Contains("Laundry"))
                        {
                            laundry_reimburse = laundry_reimburse + item.amount;
                        }
                        else if (item.jenis_transaksi.Contains("Transportation"))
                        {
                            transport_reimburse = transport_reimburse + item.amount;
                        }
                        else if (item.jenis_transaksi.Contains("Miscellaneous"))
                        {
                            misc_reimburse = misc_reimburse + item.amount;
                        }
                        if (item.additional1 == "True") HDDepartFlag = "Half Day Departure";
                        if (item.additional2 == "True") HDReturnFlag = "Half Day Return";
                        if (item.additional3 != null)
                        {
                            if (item.additional3 != "")
                            {
                                HDDepart = Convert.ToDateTime(item.additional3).ToString("hh:mm tt");
                            }
                        }
                        if (item.additional4 != null)
                        {
                            if (item.additional4 != "")
                            {
                                HDReturn = Convert.ToDateTime(item.additional4).ToString("hh:mm tt");
                            }
                        }
                        if (item.start_date_extend != null) start_extend = item.start_date_extend;
                        if (item.end_date_extend != null) end_extend = item.end_date_extend;
                    }
                }
            }
            SettlementPaidHelper model = new SettlementPaidHelper();
            model.Summary = new vw_summary_settlement();
            request = request.OrderBy(b => b.id_request).ToList();
            for (int k = 0; k < request.Count; k++)
            {
                if (k == 0)
                {
                    model.Summary.destination_1 = await GetData.DestinationNameInfo(request[k].id_destination_city);
                    model.Summary.startDate_1 = request[k].start_date;
                    model.Summary.endDate_1 = request[k].end_date;
                }
                else if (k == 1)
                {
                    model.Summary.destination_2 = await GetData.DestinationNameInfo(request[k].id_destination_city);
                    model.Summary.startDate_2 = request[k].start_date;
                    model.Summary.endDate_2 = request[k].end_date;
                }
                else if (k == 2)
                {
                    model.Summary.destination_3 = await GetData.DestinationNameInfo(request[k].id_destination_city);
                    model.Summary.startDate_3 = request[k].start_date;
                    model.Summary.endDate_3 = request[k].end_date;
                }
                else if (k == 3)
                {
                    model.Summary.destination_4 = await GetData.DestinationNameInfo(request[k].id_destination_city);
                    model.Summary.startDate_4 = request[k].start_date;
                    model.Summary.endDate_4 = request[k].end_date;
                }
                else if (k == 4)
                {
                    model.Summary.destination_5 = await GetData.DestinationNameInfo(request[k].id_destination_city);
                    model.Summary.startDate_5 = request[k].start_date;
                    model.Summary.endDate_5 = request[k].end_date;
                }
                else if (k == 5)
                {
                    model.Summary.destination_6 = await GetData.DestinationNameInfo(request[k].id_destination_city);
                    model.Summary.startDate_6 = request[k].start_date;
                    model.Summary.endDate_6 = request[k].end_date;
                }
            }
            model.HotelSettlement = hotel_reimburse;
            model.MealSettlement = meal_reimburse;
            model.TicketSettlement = ticket_reimburse;

            if (start_extend != null)
            {
                model.StartSettlement = start_extend;
            }
            if (end_extend != null)
            {
                model.EndSettlement = end_extend;
            }
            model.Summary.create_date = request[0].create_date;
            model.Summary.emp_name = await GetData.EmployeeNameInfo(request[0].no_reg);
            model.Summary.grand_total_settlement = hotel_reimburse + laundry_reimburse + meal_reimburse + misc_reimburse + ticket_reimburse + transport_reimburse;
            model.Summary.grand_total_settlement = model.Summary.grand_total_settlement + hotel_allowance + ticket_allowance + meal_allowance + winter_allowance;
            model.Summary.group_code = BTA;
            model.Summary.no_reg = request[0].no_reg;
            model.Summary.total_hotel = hotel_allowance;
            model.Summary.total_laundry = laundry_reimburse;
            model.Summary.total_meal = meal_allowance;
            model.Summary.total_miscellaneous = misc_reimburse;
            model.Summary.total_ticket = ticket_allowance;
            model.Summary.total_transportation = transport_reimburse;
            model.Summary.total_winter = winter_allowance;

            PdfDocument document = new PdfDocument();
            document.Info.Title = "Purchase Receipt";
            PdfPage page = document.AddPage();
            page.Orientation = PdfSharp.PageOrientation.Portrait;
            XSize size = PageSizeConverter.ToSize(PdfSharp.PageSize.A4);
            page.Height = size.Height;
            page.Width = size.Width;

            XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend);
            XFont watermark_font = new XFont("Calibri", 200, XFontStyle.Bold);
            XSize watermark_size = gfx.MeasureString("COPY", watermark_font);

            // Create a string format
            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Near;
            format.LineAlignment = XLineAlignment.Near;

            // Create a dimmed red brush
            XBrush brush = new XSolidBrush(XColor.FromArgb(150, 200, 200, 200));

            // Draw the string
            gfx.RotateTransform(-45);
            gfx.DrawString("COPY", watermark_font, brush, new XPoint(-350, 400), format);
            gfx.RotateTransform(45);




            XFont head = new XFont("Arial Narrow", 16, XFontStyle.Regular);
            XFont address = new XFont("Arial Narrow", 11, XFontStyle.Regular);
            XFont body = new XFont("Calibri", 10, XFontStyle.Regular);
            XFont bodyBold = new XFont("Calibri", 10, XFontStyle.Bold);
            XFont body_title = new XFont("Calibri", 10, XFontStyle.Regular);
            XFont total = new XFont("Lucida Grande", 11, XFontStyle.Bold);
            XFont title = new XFont("Lucida Grande", 12, XFontStyle.Regular);
            XFont titleBold = new XFont("Lucida Grande", 12, XFontStyle.Bold);
            XFont credit = new XFont("Lucida Sans", 9, XFontStyle.Regular);

            XPen header_line = new XPen(XColors.Black, 2);
            XPen body_line = new XPen(XColors.DimGray, 0.5);
            XPen POLine = new XPen(XColors.SteelBlue, 3);

            string PT_TAM = "PT. TOYOTA ASTRA MOTOR";
            string receipt = "SETTLEMENT RECEIPT";
            string add1 = "Jl. Laks. Yos Sudarso, Sunter II";
            string add2 = "Jakarta Utara - Indonesia";
            string add3 = "Phone :62-21 - 6515551(Hunting)";

            string btr = model.Summary.group_code;
            string name = model.Summary.emp_name.ToUpper();
            string noreg = model.Summary.no_reg.ToString();
            string TAMPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "tam_logo.PNG");
            string Paid = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "receipt-paid4.png");
            string ContrastPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "small_logo_horizontal.png");

            // string date = model.invoice.start_date.ToString();

            int total_Reimburse = 0;
            int total_Actual = 0;

            List<string> start_date = new List<string>();
            List<string> start_time = new List<string>();

            List<string> end_date = new List<string>();
            List<string> end_time = new List<string>();

            List<string> destination = new List<string>();
            List<string> from = new List<string>();

            int x_pad_max = 0;
            int x_pad = 0;
            int y_pad = 0;
            int y_pad_max = 0;
            int legend = 0;

            int count = 0;
            if (model.Summary.startDate_1.HasValue)
            {
                start_date.Add(Convert.ToDateTime(model.Summary.startDate_1).ToString("dd MMM yyyy"));
                end_date.Add(Convert.ToDateTime(model.Summary.endDate_1).ToString("dd MMM yyyy"));

                start_time.Add(Convert.ToDateTime(model.Summary.startDate_1).ToString("hh:mm tt"));
                end_time.Add(Convert.ToDateTime(model.Summary.endDate_1).ToString("hh:mm tt"));

                destination.Add(model.Summary.destination_1);
                from.Add("Jakarta");
                count++;
            }
            if (model.Summary.startDate_2.HasValue)
            {
                start_date.Add(Convert.ToDateTime(model.Summary.startDate_2).ToString("dd MMM yyyy"));
                end_date.Add(Convert.ToDateTime(model.Summary.endDate_2).ToString("dd MMM yyyy"));

                start_time.Add(Convert.ToDateTime(model.Summary.startDate_2).ToString("hh:mm tt"));
                end_time.Add(Convert.ToDateTime(model.Summary.endDate_2).ToString("hh:mm tt"));

                destination.Add(model.Summary.destination_2);
                from.Add(model.Summary.destination_1);
                count++;
            }
            if (model.Summary.startDate_3.HasValue)
            {
                start_date.Add(Convert.ToDateTime(model.Summary.startDate_3).ToString("dd MMM yyyy"));
                end_date.Add(Convert.ToDateTime(model.Summary.endDate_3).ToString("dd MMM yyyy"));

                start_time.Add(Convert.ToDateTime(model.Summary.startDate_3).ToString("hh:mm tt"));
                end_time.Add(Convert.ToDateTime(model.Summary.endDate_3).ToString("hh:mm tt"));

                destination.Add(model.Summary.destination_3);
                from.Add(model.Summary.destination_2);
                count++;
            }
            if (model.Summary.startDate_4.HasValue)
            {
                start_date.Add(Convert.ToDateTime(model.Summary.startDate_4).ToString("dd MMM yyyy"));
                end_date.Add(Convert.ToDateTime(model.Summary.endDate_4).ToString("dd MMM yyyy"));

                start_time.Add(Convert.ToDateTime(model.Summary.startDate_4).ToString("hh:mm tt"));
                end_time.Add(Convert.ToDateTime(model.Summary.endDate_4).ToString("hh:mm tt"));

                destination.Add(model.Summary.destination_4);
                from.Add(model.Summary.destination_3);
                count++;
            }
            if (model.Summary.startDate_5.HasValue)
            {
                start_date.Add(Convert.ToDateTime(model.Summary.startDate_5).ToString("dd MMM yyyy"));
                end_date.Add(Convert.ToDateTime(model.Summary.endDate_5).ToString("dd MMM yyyy"));

                start_time.Add(Convert.ToDateTime(model.Summary.startDate_5).ToString("hh:mm tt"));
                end_time.Add(Convert.ToDateTime(model.Summary.endDate_5).ToString("hh:mm tt"));

                destination.Add(model.Summary.destination_5);
                from.Add(model.Summary.destination_4);
                count++;
            }
            if (model.Summary.startDate_6.HasValue)
            {
                start_date.Add(Convert.ToDateTime(model.Summary.startDate_6).ToString("dd MMM yyyy"));
                end_date.Add(Convert.ToDateTime(model.Summary.endDate_6).ToString("dd MMM yyyy"));

                start_time.Add(Convert.ToDateTime(model.Summary.startDate_6).ToString("hh:mm tt"));
                end_time.Add(Convert.ToDateTime(model.Summary.endDate_6).ToString("hh:mm tt"));

                destination.Add(model.Summary.destination_6);
                from.Add(model.Summary.destination_5);
                count++;
            }
            if (model.StartSettlement.HasValue)
            {
                start_date.Add(Convert.ToDateTime(model.StartSettlement).ToString("dd MMM yyyy"));
                end_date.Add(Convert.ToDateTime(model.EndSettlement).ToString("dd MMM yyyy"));

                start_time.Add(Convert.ToDateTime(model.StartSettlement).ToString("hh:mm tt"));
                end_time.Add(Convert.ToDateTime(model.EndSettlement).ToString("hh:mm tt"));

                destination.Add("Extend");
                from.Add(destination[count - 1]);
            }

            //gfx.DrawRectangle(XBrushes.GhostWhite, 50, 735, 495, 63);

            XImage TAM = XImage.FromFile(TAMPath);
            gfx.DrawImage(TAM, 60, 50, 40, 40);

            //XImage contrast = XImage.FromFile(ContrastPath);
            //gfx.DrawImage(contrast, 60, 735);

            XImage paid = XImage.FromFile(Paid);
            gfx.DrawImage(paid, 60, 675, 150, 150);

            gfx.DrawString(PT_TAM, head, XBrushes.Black, 105, 60, XStringFormats.TopLeft);
            gfx.DrawRectangle(XBrushes.GhostWhite, 377, 55, 168, 28);
            gfx.DrawLine(POLine, 377, 55, 377, 83);

            gfx.DrawString(receipt, head, XBrushes.Gray, 385, 60, XStringFormats.TopLeft);

            gfx.DrawLine(header_line, 50, 95, 545, 95);

            gfx.DrawString(add1, address, XBrushes.Black, 60, 105, XStringFormats.TopLeft);
            gfx.DrawString(add2, address, XBrushes.Black, 60, 120, XStringFormats.TopLeft);
            gfx.DrawString(add3, address, XBrushes.Black, 60, 135, XStringFormats.TopLeft);

            gfx.DrawString("BTA", address, XBrushes.Black, 385, 105, XStringFormats.TopLeft);
            gfx.DrawString("NOREG", address, XBrushes.Black, 385, 120, XStringFormats.TopLeft);
            gfx.DrawString("NAME", address, XBrushes.Black, 385, 135, XStringFormats.TopLeft);

            gfx.DrawString(":", address, XBrushes.Black, 425, 105, XStringFormats.TopLeft);
            gfx.DrawString(":", address, XBrushes.Black, 425, 120, XStringFormats.TopLeft);
            gfx.DrawString(":", address, XBrushes.Black, 425, 135, XStringFormats.TopLeft);

            gfx.DrawString(btr, address, XBrushes.Black, 435, 105, XStringFormats.TopLeft);
            gfx.DrawString(noreg, address, XBrushes.Black, 435, 120, XStringFormats.TopLeft);
            gfx.DrawString(name, address, XBrushes.Black, 435, 135, XStringFormats.TopLeft);

            gfx.DrawLine(body_line, 75, 200, 520, 200);

            gfx.DrawString("DETAIL TRAVEL", title, XBrushes.Black, 85, 207, XStringFormats.TopLeft);

            gfx.DrawLine(body_line, 75, 230, 520, 230);

            int gap_now = 0;
            x_pad = 30;

            gfx.DrawString("From", body, XBrushes.Black, 85, 240, XStringFormats.TopLeft);
            gfx.DrawString("To", body, XBrushes.Black, 180, 240, XStringFormats.TopLeft);

            gfx.DrawString("Depart", body, XBrushes.Black, 280 + x_pad, 240, XStringFormats.TopLeft);
            gfx.DrawString("Return", body, XBrushes.Black, 385 + x_pad, 240, XStringFormats.TopLeft);

            int i = 0, last_date = 0;
            foreach (var item in start_date)
            {
                last_date = i;
                gap_now = (i + 1) * 15;
                gfx.DrawString(from[i], body, XBrushes.Black, 85, 240 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(destination[i], body, XBrushes.Black, 180, 240 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(start_date[i], body, XBrushes.Black, 280 + x_pad, 240 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(start_time[i], body, XBrushes.Black, 335 + x_pad, 240 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(end_date[i], body, XBrushes.Black, 385 + x_pad, 240 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(end_time[i], body, XBrushes.Black, 440 + x_pad, 240 + gap_now, XStringFormats.TopLeft);
                i++;
                
            }
            if (HDDepartFlag != null)
            {
                gap_now = (i + 1) * 15;
                gfx.DrawString(HDDepartFlag, body, XBrushes.Black, 85, 240 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(start_date[0], body, XBrushes.Black, 280 + x_pad, 240 + gap_now, XStringFormats.TopLeft);

                if (HDDepart != null)
                    gfx.DrawString(HDDepart, body, XBrushes.Black, 335 + x_pad, 240 + gap_now, XStringFormats.TopLeft);
                else gfx.DrawString("00:00 AM", body, XBrushes.Black, 335 + x_pad, 240 + gap_now, XStringFormats.TopLeft);
                i++;
            }
            if (HDReturnFlag != null)
            {
                gap_now = (i + 1) * 15;
                gfx.DrawString(HDReturnFlag, body, XBrushes.Black, 85, 240 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(end_date[last_date], body, XBrushes.Black, 385 + x_pad, 240 + gap_now, XStringFormats.TopLeft);
                if (HDReturn != null)
                    gfx.DrawString(HDReturn, body, XBrushes.Black, 440 + x_pad, 240 + gap_now, XStringFormats.TopLeft);
                else gfx.DrawString("00:00 AM", body, XBrushes.Black, 440 + x_pad, 240 + gap_now, XStringFormats.TopLeft);
                i++;
            }
            int xItem = 120;
            int xSuspense = 260;
            int xReimburse = 400;


            gfx.DrawLine(body_line, 75, 280 + gap_now, 520, 280 + gap_now);

            gfx.DrawString("ITEM", title, XBrushes.Black, xItem, 287 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString("SUSPENSE", title, XBrushes.Black, xSuspense, 287 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString("REIMBURSE", title, XBrushes.Black, xReimburse, 287 + gap_now, XStringFormats.TopLeft);
            gfx.DrawLine(body_line, 75, 310 + gap_now, 520, 310 + gap_now);

            // BUDGET DETAIL PER ITEM
            XTextFormatter tx = new XTextFormatter(gfx);
            XTextFormatter rx = new XTextFormatter(gfx);

            tx.Alignment = XParagraphAlignment.Right;
            rx.Alignment = XParagraphAlignment.Left;

            gfx.DrawRectangle(XBrushes.GhostWhite, 85 - 3, 320 + gap_now, 432, 20);
            gfx.DrawLine(POLine, 85, 320 + gap_now, 85 - 3, 340 + gap_now);

            gfx.DrawRectangle(XBrushes.GhostWhite, 85 - 3, 360 + gap_now, 432, 20);
            gfx.DrawLine(POLine, 85, 360 + gap_now, 85 - 3, 380 + gap_now);

            gfx.DrawRectangle(XBrushes.GhostWhite, 85 - 3, 400 + gap_now, 432, 20);
            gfx.DrawLine(POLine, 85, 400 + gap_now, 85 - 3, 420 + gap_now);

            gfx.DrawRectangle(XBrushes.LightCyan, 85 - 3, 450 + gap_now, 432, 20);
            gfx.DrawLine(POLine, 85, 450 + gap_now, 85 - 3, 470 + gap_now);

            gfx.DrawRectangle(XBrushes.LightCyan, 85 - 3, 516 + gap_now, 432, 34);
            gfx.DrawLine(POLine, 85, 516 + gap_now, 85 - 3, 550 + gap_now);


            XRect rect1 = new XRect(xItem + 5, 323 + gap_now, 200, 20);
            XRect rect1s = new XRect(xItem + 205, 323 + gap_now, 150, 20);

            rx.DrawString("Meal", body_title, XBrushes.DarkBlue, rect1);
            tx.DrawString("Rp. " + model.Summary.total_meal.ToString("N"), body, XBrushes.Black, rect1);
            tx.DrawString("Rp. " + model.MealSettlement.ToString("N"), body, XBrushes.Black, rect1s);


            XRect rect2 = new XRect(xItem + 5, 343 + gap_now, 200, 20);
            XRect rect2s = new XRect(xItem + 205, 343 + gap_now, 150, 20);

            rx.DrawString("Hotel", body_title, XBrushes.DarkBlue, rect2);
            tx.DrawString("Rp. " + model.Summary.total_hotel.ToString("N"), body, XBrushes.Black, rect2);
            tx.DrawString("Rp. " + model.HotelSettlement.ToString("N"), body, XBrushes.Black, rect2s);



            XRect rect3 = new XRect(xItem + 5, 363 + gap_now, 200, 20);
            XRect rect3s = new XRect(xItem + 205, 363 + gap_now, 150, 20);

            rx.DrawString("Ticket", body_title, XBrushes.DarkBlue, rect3);
            tx.DrawString("Rp. " + model.Summary.total_ticket.ToString("N"), body, XBrushes.Black, rect3);
            tx.DrawString("Rp. " + model.TicketSettlement.ToString("N"), body, XBrushes.Black, rect3s);


            XRect rect4 = new XRect(xItem + 5, 383 + gap_now, 200, 20);
            XRect rect4s = new XRect(xItem + 205, 383 + gap_now, 150, 20);

            rx.DrawString("Land Transport", body_title, XBrushes.DarkBlue, rect4);
            tx.DrawString("Rp. 0.00", body, XBrushes.Black, rect4);
            tx.DrawString("Rp. " + model.Summary.total_transportation.ToString("N"), body, XBrushes.Black, rect4s);

            XRect rect5 = new XRect(xItem + 5, 403 + gap_now, 200, 20);
            XRect rect5s = new XRect(xItem + 205, 403 + gap_now, 150, 20);

            rx.DrawString("Laundry", body_title, XBrushes.DarkBlue, rect5);
            tx.DrawString("Rp. 0.00", body, XBrushes.Black, rect5);
            tx.DrawString("Rp. " + model.Summary.total_laundry.ToString("N"), body, XBrushes.Black, rect5s);

            XRect rect6 = new XRect(xItem + 5, 423 + gap_now, 200, 20);
            XRect rect6s = new XRect(xItem + 205, 423 + gap_now, 150, 20);
            rx.DrawString("Misc", body_title, XBrushes.DarkBlue, rect6);
            tx.DrawString("Rp. 0.00", body, XBrushes.Black, rect6);
            tx.DrawString("Rp. " + model.Summary.total_miscellaneous.ToString("N"), body, XBrushes.Black, rect6s);



            // Perhitungan Total
            total_Actual = model.Summary.total_meal + model.Summary.total_ticket + model.Summary.total_hotel;
            int t_Reimburse = Convert.ToInt32(model.MealSettlement + model.HotelSettlement + model.TicketSettlement);
            total_Reimburse = model.Summary.total_laundry + model.Summary.total_transportation + model.Summary.total_miscellaneous + t_Reimburse;


            XRect rect7 = new XRect(xItem + 5, 454 + gap_now, 200, 20);
            XRect rect7s = new XRect(xItem + 205, 454 + gap_now, 150, 20);
            rx.DrawString("TOTAL", title, XBrushes.DarkBlue, rect7);
            tx.DrawString("Rp. " + total_Actual.ToString("N"), body, XBrushes.Black, rect7);
            tx.DrawString("Rp. " + total_Reimburse.ToString("N"), body, XBrushes.Black, rect7s);

            XRect rect9 = new XRect(xItem + 5, 526 + gap_now, 350, 20);
            rx.DrawString("GRAND TOTAL", titleBold, XBrushes.DarkBlue, rect9);
            tx.DrawString("Rp. " + Convert.ToInt32(model.Summary.grand_total_settlement).ToString("N"), bodyBold, XBrushes.Black, rect9);


            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;
            return File(stream, "application/pdf", receipt.Replace(" ", "_") + "_" + model.Summary.group_code.Trim(' ') + "_" + DateTime.Now.ToString("yyMMdd-hh-mm-tt") + ".pdf");

        }

    }
}
