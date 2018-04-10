using ClosedXML.Excel;
using CONTRAST_WEB.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CONTRAST_WEB.Controllers
{
    public class TrackProgressController : Controller
    {
        // GET: TrackProgess
        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(tb_m_employee model, string user = "")
        {
         
            List<TrackingHelper> track = new List<TrackingHelper>();
            List<vw_tracking_transaction_data_new> new_list = new List<vw_tracking_transaction_data_new>();
            int privillage = 0;
            tb_m_employee_source_data Admin = await GetData.GetDivisionSource(Convert.ToInt32(model.code));
            tb_m_verifier_employee verifier = await GetData.EmployeeVerifier(Convert.ToInt32(model.code));

            string division = Admin.Divisi;
            if (Admin.Divisi.Contains("and1")) Admin.Divisi = division.Replace("and1", "&");

            //if (verifier.position != null) privillage = 1;
            //else privillage = 2;
            if (user.Contains("all")) privillage = 1;
            else if (user.Contains("admin")) privillage = 2;
            else if (user.Contains("user")) privillage = 3;

            //if (privillage == 1) new_list = await GetData.TrackingListAll();
            //else if (privillage == 2) new_list = await GetData.TrackingListAll();
            ////else if (privillage == 2) new_list = await GetData.TrackingListDivisonAll(model.unit_code_code);

            //if (new_list.Count > 0)
            //{
            //    foreach (var item in new_list)
            //    {
            //        TrackingHelper temp = new TrackingHelper();
            //        temp.login_id = model.code;
            //        temp.login_name = model.name;
            //        temp.TrackedList = item;
            //        track.Add(temp);
            //    }
            //}
            //else
            //{
            //    TrackingHelper temp = new TrackingHelper();
            //    temp.login_id = model.code;
            //    temp.login_name = model.name;
            //    track.Add(temp);
            //    return View(track);
            //}
            //return View(track.OrderBy(m => m.TrackedList.create_date).ToList());

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
                    temp.privilage = user;
                    temp.TrackedList = item;
                    track.Add(temp);
                }
            }
            else
            {
                TrackingHelper temp = new TrackingHelper();
                temp.login_id = model.code;
                temp.login_name = model.name;
                temp.privilage = user;
                track.Add(temp);
                return View(track);
            }
            return View("Index", track.OrderBy(m => m.TrackedList.group_code).ThenBy(m => m.TrackedList.create_date).ToList());
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Search(List<TrackingHelper> Model, string search = "", DateTime? start = null, DateTime? end = null, string insert = "", string download = "", int? detail = null)
        {

            if (detail.HasValue)
            {
                int id_data = Convert.ToInt32(detail);
                Model[id_data].id_data = Model[id_data].TrackedList.id_data;
                TrackingHelper newModel = new TrackingHelper();
                newModel = Model[id_data];
                return RedirectToAction("Details", "TrackProgress", newModel);
            }
            List<TrackingHelper> track = new List<TrackingHelper>();
            int privillage = 0;
            tb_m_verifier_employee employee = await GetData.EmployeeVerifier(Convert.ToInt32(Model[0].login_id));
            tb_m_employee logged_employee = new tb_m_employee();

            //if (employee.position != null) privillage = 1;
            //else privillage = 2;
            logged_employee.code = Model[0].login_id;
            logged_employee = await GetData.EmployeeInfo(logged_employee);

            tb_m_employee_source_data Admin = await GetData.GetDivisionSource(Convert.ToInt32(logged_employee.code));

            string division = Admin.Divisi;
            if (Admin.Divisi.Contains("and1")) Admin.Divisi = division.Replace("and1", "&");

            if (Model[0].privilage.Contains("all")) privillage = 1;
            else if (Model[0].privilage.Contains("admin")) privillage = 2;
            else if (Model[0].privilage.Contains("user")) privillage = 3;

            if (insert == "Search")
            {
                //List<vw_tracking_transaction_data_new> new_list = new List<vw_tracking_transaction_data_new>();
                //if (privillage == 1) new_list = await GetData.TrackingList(search, start, end);
                //else new_list = await GetData.TrackingListDivison(logged_employee.unit_code_code, search, start, end);

                //if (new_list.Count > 0)
                //{
                //    foreach (var item in new_list)
                //    {
                //        TrackingHelper temp = new TrackingHelper();
                //        temp.login_id = Model[0].login_id;
                //        temp.login_name = Model[0].login_name;
                //        temp.TrackedList = item;
                //        track.Add(temp);
                //    }
                //}
                //else
                //{
                //    TrackingHelper temp = new TrackingHelper();
                //    temp.login_id = Model[0].login_id;
                //    temp.login_name = Model[0].login_name;
                //    track.Add(temp);
                //    return View("Index", track);
                //}
                //ModelState.Clear();
                //return View("Index", track.OrderBy(m => m.TrackedList.create_date).ToList());
                List<vw_tracking_transaction_data_new> new_list = new List<vw_tracking_transaction_data_new>();
                if (privillage == 1) new_list = await GetData.TrackingListAllSearch(search, start, end);
                else if (privillage == 2) new_list = await GetData.TrackingListDivisonAllSearch(Admin.Divisi, search, start, end);
                else if (privillage == 3) new_list = await GetData.TrackingListIndividualSearch(logged_employee.code, search, start, end);

                if (new_list.Count > 0)
                {
                    foreach (var item in new_list)
                    {
                        TrackingHelper temp = new TrackingHelper();
                        temp.login_id = Model[0].login_id;
                        temp.login_name = Model[0].login_name;
                        temp.privilage = Model[0].privilage;

                        temp.TrackedList = item;
                        track.Add(temp);
                    }
                }
                else
                {
                    TrackingHelper temp = new TrackingHelper();
                    temp.login_id = Model[0].login_id;
                    temp.login_name = Model[0].login_name;
                    temp.privilage = Model[0].privilage;
                    track.Add(temp);
                    return View("Index", track);
                }
                ModelState.Clear();
                return View("Index", track.OrderBy(m => m.TrackedList.group_code).ThenBy(m => m.TrackedList.create_date).ToList());
            }
            else if (download == "Download")
            {
                if (Model.Count > 0)
                {
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
                    InsertData.Cell(1, 17).Value = "Created Date";

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
                    }
                    MemoryStream excelStream = new MemoryStream();
                    CreateExcell.SaveAs(excelStream);
                    excelStream.Position = 0;
                    return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Transaction Tracking Data.xlsx");
                }
                else
                {
                    ViewBag.error = "There's no Data to Download";
                    ModelState.Clear();
                    return View("Index", Model);
                }
            }
            else
            {
                List<vw_tracking_transaction_data_new> new_list = await GetData.TrackingListAll();

                if (new_list.Count > 0)
                {
                    foreach (var item in new_list)
                    {
                        TrackingHelper temp = new TrackingHelper();
                        temp.login_id = Model[0].login_id;
                        temp.login_name = Model[0].login_name;
                        temp.TrackedList = item;
                        track.Add(temp);
                    }
                }
                else
                {
                    TrackingHelper temp = new TrackingHelper();
                    temp.login_id = Model[0].login_id;
                    temp.login_name = Model[0].login_name;
                    track.Add(temp);
                    return View("Index", track);
                }
                ModelState.Clear();
                return View("Index", track.OrderBy(m => m.TrackedList.group_code).ThenBy(m => m.TrackedList.create_date).ToList());
            }
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
                    if (item.path_file != "Error")
                    {

                        //new_cost.Path = "http://passport.toyota.astra.co.id:5006/";
                        //new_cost.Path = "http://10.85.40.68:91/";
                        new_cost.Path = Constant.Baseurl;
                        
                        string[] newPath = item.path_file.Split('\\');
                        for (int k = newPath.Count() - 2; k < newPath.Count(); k++)
                        {

                            if (k < (newPath.Count() - 1)) new_cost.Path = new_cost.Path + newPath[k].Replace(" ", "%20") + "/";
                            else new_cost.Path = new_cost.Path + newPath[k].Replace(" ", "%20");
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
                    Detailed.Executed[i].pic_path =Constant.Baseurl;
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
