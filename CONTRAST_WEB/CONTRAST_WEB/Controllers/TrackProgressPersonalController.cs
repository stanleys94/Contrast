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
            
            //if (user.Contains("all")) privillage = 1;
            //else if (user.Contains("admin")) privillage = 2;
            //else if (user.Contains("user")) privillage = 3;            

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
            else
            {
                TrackingHelper temp = new TrackingHelper();
                temp.login_id = model.code;
                temp.login_name = model.name;
                temp.privilage = privillage_desc;
                track.Add(temp);
                //return View(track);
                return View(track.OrderBy(m => m.TrackedList.group_code).ToPagedList(pageNumber, pageSize));
            }
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
                       )
                        temp.Add(track[k]);
                }
                if (temp.Count() > 0) track = temp;
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
        public async Task<ActionResult> Search(List<TrackingHelper> Model, string search = "", DateTime? start = null, DateTime? end = null, string insert = "", string download = "")
        {
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
    }
}
