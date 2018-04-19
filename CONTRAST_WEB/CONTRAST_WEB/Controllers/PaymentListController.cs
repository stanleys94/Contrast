using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using CONTRAST_WEB.Models;
using System.IO;
using System.Globalization;
using ClosedXML.Excel;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Claims;
using PagedList;


namespace CONTRAST_WEB.Controllers
{
    public class PaymentListController : Controller
    {
        // GET: PaymentList
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page, DateTime? startdate, DateTime? enddate)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;
            tb_m_employee model = await GetData.EmployeeInfo(identity.Name);

            List<PaymentListHelper> Generate = new List<PaymentListHelper>();
            List<vw_payment_list> data = new List<vw_payment_list>();
            data = await GetData.PaymentListData();

            foreach (var item in data)
            {
                PaymentListHelper temp = new PaymentListHelper();
                temp.Entity = item;
                temp.Name = model.name;
                temp.No_Reg = Convert.ToInt32(model.code);
                Generate.Add(temp);
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
                List<PaymentListHelper> temp = new List<PaymentListHelper>();
                for (int k = 0; k < Generate.Count; k++)
                {
                    if ((Generate[k].Entity.EMPLOYEE_NAME != null ? Generate[k].Entity.EMPLOYEE_NAME : "").ToLower().Contains(searchString.ToLower()) ||
                        (Generate[k].Entity.BTR_NO != null ? Generate[k].Entity.BTR_NO : "").ToLower().Contains(searchString.ToLower()) ||
                        (Generate[k].Entity.DESTINATION != null ? Generate[k].Entity.DESTINATION : "").ToLower().Contains(searchString.ToLower()) ||
                        (Generate[k].Entity.COST_CENTER != null ? Generate[k].Entity.COST_CENTER : "").ToLower().Contains(searchString.ToLower()) ||
                        (Generate[k].Entity.WBS_ELEMENT != null ? Generate[k].Entity.WBS_ELEMENT : "").ToLower().Contains(searchString.ToLower()) ||
                        (Generate[k].Entity.TRAVEL_TYPE != null ? Generate[k].Entity.TRAVEL_TYPE : "").ToLower().Contains(searchString.ToLower()) ||
                        (Generate[k].Entity.BUDGET != null ? Generate[k].Entity.BUDGET : "").ToLower().Contains(searchString.ToLower()) ||
                        (Generate[k].Entity.TRAVEL_TYPE != null ? Generate[k].Entity.TRAVEL_TYPE : "").ToLower().Contains(searchString.ToLower()))
                    {
                        temp.Add(Generate[k]);
                    }
                }
                if (temp.Count() > 0) Generate = temp;
                else Generate = temp;
            }


            //date filter
            if (startdate != null && enddate != null)
            {
                List<PaymentListHelper> temp = new List<PaymentListHelper>();
                for (int k = 0; k < Generate.Count; k++)
                {
                    //by group code
                    if (
                        DateTime.ParseExact(Generate[k].Entity.PV_DATE, "dd.MM.yyyy", CultureInfo.CurrentCulture) >= startdate
                        && DateTime.ParseExact(Generate[k].Entity.PV_DATE, "dd.MM.yyyy", CultureInfo.CurrentCulture) <= enddate
                       )
                        temp.Add(Generate[k]);
                }
                if (temp.Count() > 0) Generate = temp;
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);

            List<int> id_data = new List<int>();
            for (int k = 0; k < Generate.Count; k++)
                id_data.Add(Generate[k].Entity.id_data);
            ViewBag.id_data = id_data;
            return View("Index", Generate.OrderBy(b => b.Entity.PV_DATE).ToList().ToPagedList(pageNumber, pageSize));
            
        }

        //[HttpPost]
        //[Authorize]
        //[Authorize(Roles = "contrast.user")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Generate(List<int> id_data)
        {
            List<PaymentListHelper> model = new List<PaymentListHelper>();
            for (int k = 0; k < id_data.Count(); k++)
            {
                model.Add(new PaymentListHelper());
                model[k].Entity = await GetData.PaymentList(id_data[k]);
            }

            XLWorkbook CreateExcell = new XLWorkbook();
            var ExcelData = CreateExcell.Worksheets.Add("Payment List");

            ExcelData.Cell(1, 1).Value = "ID_DATA";
            ExcelData.Cell(1, 2).Value = "PV_NO";
            ExcelData.Cell(1, 3).Value = "PV_YEAR";
            ExcelData.Cell(1, 4).Value = "ITEM_NO";
            ExcelData.Cell(1, 5).Value = "PV_DATE";
            ExcelData.Cell(1, 6).Value = "PV_TYPE";
            ExcelData.Cell(1, 7).Value = "TRANS_TYPE";
            ExcelData.Cell(1, 8).Value = "VENDOR";
            ExcelData.Cell(1, 9).Value = "VENDOR_GRP";
            ExcelData.Cell(1, 10).Value = "INVOICE_NO";
            ExcelData.Cell(1, 11).Value = "TAX_NO";
            ExcelData.Cell(1, 12).Value = "PAYMENT_TERM";
            ExcelData.Cell(1, 13).Value = "PAYMENT_METHOD";
            ExcelData.Cell(1, 14).Value = "PLAN_PAYMENT_DT";
            ExcelData.Cell(1, 15).Value = "POSTING_DT";
            ExcelData.Cell(1, 16).Value = "TOTAL_AMOUNT";
            ExcelData.Cell(1, 17).Value = "DPP_AMOUNT";
            ExcelData.Cell(1, 18).Value = "CURRENCY";
            ExcelData.Cell(1, 19).Value = "TAX_CODE";
            ExcelData.Cell(1, 20).Value = "HEADER_TEXT";
            ExcelData.Cell(1, 21).Value = "BANK_TYPE";
            ExcelData.Cell(1, 22).Value = "GL_ACCOUTN";
            ExcelData.Cell(1, 23).Value = "AMOUNT";
            ExcelData.Cell(1, 24).Value = "COST_CENTER";
            ExcelData.Cell(1, 25).Value = "WBS_ELEMENT";
            ExcelData.Cell(1, 26).Value = "ITEM_TEXT";
            ExcelData.Cell(1, 27).Value = "STATUS";
            ExcelData.Cell(1, 28).Value = "SAP_DOC_NO";
            ExcelData.Cell(1, 29).Value = "SAP_DOC_YEAR";
            ExcelData.Cell(1, 30).Value = "BTR_NO";
            ExcelData.Cell(1, 31).Value = "EMPLOYEE_NAME";
            ExcelData.Cell(1, 32).Value = "DESTINATION";
            ExcelData.Cell(1, 33).Value = "ID_CITY";
            ExcelData.Cell(1, 34).Value = "BUDGET";
            ExcelData.Cell(1, 35).Value = "TOTAL_AMOUNT";
            ExcelData.Cell(1, 36).Value = "COST_CENTER";
            ExcelData.Cell(1, 37).Value = "WBS_ELEMENT";
            ExcelData.Cell(1, 38).Value = "TRAVEL_TYPE";
            ExcelData.Cell(1, 39).Value = "BTR_DATE";
            ExcelData.Cell(1, 40).Value = "PLAN_PAYMENT_DATE";
            ExcelData.Cell(1, 41).Value = "PAYMENT_METHOD";

            //int gap = 0;
            for (int i = 0; i < model.Count(); i++)
            {
                ExcelData.Cell(i + 2, 1).Value = model[i].Entity.id_data;
                ExcelData.Cell(i + 2, 2).Value = model[i].Entity.PV_NO;
                ExcelData.Cell(i + 2, 3).Value = model[i].Entity.PV_YEAR;
                ExcelData.Cell(i + 2, 4).Value = model[i].Entity.ITEM_NO;
                ExcelData.Cell(i + 2, 5).Value = model[i].Entity.PV_DATE;
                ExcelData.Cell(i + 2, 6).Value = model[i].Entity.PV_TYPE;
                ExcelData.Cell(i + 2, 7).Value = model[i].Entity.TRANS_TYPE;
                ExcelData.Cell(i + 2, 8).Value = model[i].Entity.VENDOR;
                ExcelData.Cell(i + 2, 9).Value = model[i].Entity.VENDOR_GRP;
                ExcelData.Cell(i + 2, 10).Value = model[i].Entity.INVOICE_NO;
                ExcelData.Cell(i + 2, 11).Value = model[i].Entity.TAX_NO;
                ExcelData.Cell(i + 2, 12).Value = model[i].Entity.PAYMENT_TERM;
                ExcelData.Cell(i + 2, 13).Value = model[i].Entity.PAYMENT_METHOD;
                ExcelData.Cell(i + 2, 14).Value = model[i].Entity.PLAN_PAYMENT_DT;
                ExcelData.Cell(i + 2, 15).Value = model[i].Entity.POSTING_DT;
                ExcelData.Cell(i + 2, 16).Value = model[i].Entity.TOTAL_AMOUNT;
                ExcelData.Cell(i + 2, 17).Value = model[i].Entity.DPP_AMOUNT;
                ExcelData.Cell(i + 2, 18).Value = model[i].Entity.CURRENCY;
                ExcelData.Cell(i + 2, 19).Value = model[i].Entity.TAX_CODE;
                ExcelData.Cell(i + 2, 20).Value = model[i].Entity.HEADER_TEXT;
                ExcelData.Cell(i + 2, 21).Value = model[i].Entity.BANK_TYPE;
                ExcelData.Cell(i + 2, 22).Value = model[i].Entity.gl_account;
                ExcelData.Cell(i + 2, 23).Value = model[i].Entity.AMOUNT;
                ExcelData.Cell(i + 2, 24).Value = model[i].Entity.COST_CENTER;
                ExcelData.Cell(i + 2, 25).Value = model[i].Entity.WBS_ELEMENT;
                ExcelData.Cell(i + 2, 26).Value = model[i].Entity.ITEM_TEXT;
                ExcelData.Cell(i + 2, 27).Value = model[i].Entity.STATUS;
                ExcelData.Cell(i + 2, 28).Value = model[i].Entity.SAP_DOC_NO;
                ExcelData.Cell(i + 2, 29).Value = model[i].Entity.SAP_DOC_YEAR;
                ExcelData.Cell(i + 2, 30).Value = model[i].Entity.BTR_NO;
                ExcelData.Cell(i + 2, 31).Value = model[i].Entity.EMPLOYEE_NAME;
                ExcelData.Cell(i + 2, 32).Value = model[i].Entity.DESTINATION;
                ExcelData.Cell(i + 2, 33).Value = model[i].Entity.ID_CITY;
                ExcelData.Cell(i + 2, 34).Value = model[i].Entity.BUDGET;
                ExcelData.Cell(i + 2, 35).Value = model[i].Entity.TOTAL_AMOUNT_;
                ExcelData.Cell(i + 2, 36).Value = model[i].Entity.COST_CENTER_;
                ExcelData.Cell(i + 2, 37).Value = model[i].Entity.WBS_ELEMENT_;
                ExcelData.Cell(i + 2, 38).Value = model[i].Entity.TRAVEL_TYPE;
                ExcelData.Cell(i + 2, 39).Value = model[i].Entity.BTR_DATE;
                ExcelData.Cell(i + 2, 40).Value = model[i].Entity.PLAN_PAYMENT_DATE;
                ExcelData.Cell(i + 2, 41).Value = model[i].Entity.PAYMENT_METHOD_;

                //gap = i;

            }

            foreach (var item in model)
            {
                if (item.Entity.PV_DATE != null)
                {
                    tb_r_record_payment_list record = new tb_r_record_payment_list();
                    record.group_code = item.Entity.PV_NO;
                    record.item_no = item.Entity.ITEM_NO.ToString();
                    record.trans_type = item.Entity.TRANS_TYPE;
                    record.vendor_code = item.Entity.VENDOR;
                    record.pv_date = DateTime.ParseExact(item.Entity.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    record.generate_by = model[0].No_Reg.ToString();
                    record.generate_date = DateTime.Now;

                    //await InsertData.RecordPaymentList(record);
                }
            }

            MemoryStream excelStream = new MemoryStream();
            CreateExcell.SaveAs(excelStream);
            excelStream.Position = 0;
            return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Payment List.xlsx");

        }
    }
}