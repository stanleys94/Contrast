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


namespace CONTRAST_WEB.Controllers
{
    public class PaymentListController : Controller
    {
        // GET: PaymentList
        //[HttpPost]
        //[Authorize]
        //[Authorize(Roles = "contrast.user")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(tb_m_employee model)
        {
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
            return View("Index", Generate.OrderBy(b => b.Entity.PV_DATE).ToList());
        }

        //[HttpPost]
        //[Authorize]
        //[Authorize(Roles = "contrast.user")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Filter(List<PaymentListHelper> model, DateTime? startdate, DateTime? enddate, string search = "")
        {
            string lower = search.ToLower();
            List<PaymentListHelper> Generate = new List<PaymentListHelper>();
            List<vw_payment_list> data = new List<vw_payment_list>();
            if (search == "" && !startdate.HasValue && !enddate.HasValue) data = await GetData.PaymentListData();
            else
                data = await GetData.PaymentListDataFiltered(lower, startdate, enddate);

            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    PaymentListHelper temp = new PaymentListHelper();
                    temp.Entity = item;
                    temp.Name = model[0].Name;
                    temp.No_Reg = model[0].No_Reg;
                    if (startdate.HasValue) temp.StartDate = Convert.ToDateTime(startdate);
                    if (enddate.HasValue) temp.EndDate = Convert.ToDateTime(enddate);
                    Generate.Add(temp);
                }
            }
            else
            {
                PaymentListHelper temp = new PaymentListHelper();
                temp.Name = model[0].Name;
                temp.No_Reg = model[0].No_Reg;
                if (startdate.HasValue) temp.StartDate = Convert.ToDateTime(startdate);
                if (enddate.HasValue) temp.EndDate = Convert.ToDateTime(enddate);
                Generate.Add(temp);
            }
            ModelState.Clear();
            return View("Index", Generate);
        }

        [HttpPost]
        //[Authorize]
        //[Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]


        /*public async Task<ActionResult> Generate(List<PaymentListHelper> model)
        {
            //todo: add some data from your database into that string:
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            string MANDT, PV_NO, PV_YEAR, ITEM_NO, PV_DATE, PV_TYPE, TRANS_TYPE, VENDOR, VENDOR_GRP, INVOICE_NO, TAX_NO, PAYMENT_TERM, PAYMENT_METHOD, PLAN_PAYMENT_DT, POSTING_DT, TOTAL_AMOUNT, DPP_AMOUNT, CURRENCY, TAX_CODE, HEADER_TEXT, BANK_TYPE, gl_account, AMOUNT, COST_CENTER, WBS_ELEMENT, ITEM_TEXT, STATUS, SAP_DOC_NO, SAP_DOC_YEAR;
            tw.Write("MANDT\t" +
                "PV_NO\t" +
                "PV_YEAR\t" +
                "ITEM_NO\t" +
                "PV_DATE\t" +
                "PV_TYPE\t" +
                "TRANS_TYPE\t" +
                "VENDOR\t" +
                "VENDOR_GRP\t" +
                "INVOICE_NO\t" +
                "TAX_NO\t" +
                "PAYMENT_TERM\t" +
                "PAYMENT_METHOD\t" +
                "PLAN_PAYMENT_DT\t" +
                "POSTING_DT\t" +
                "TOTAL_AMOUNT\t" +
                "DPP_AMOUNT\t" +
                "CURRENCY\t" +
                "TAX_CODE\t" +
                "HEADER TEXT\t" +
                "BANK_TYPE\t" +
                "gl_account\t" +
                "AMOUNT\t" +
                "COST_CENTER\t" +
                "WBS_ELEMENT\t" +
                "ITEM_TEXT\t" +
                "STATUS\t" +
                "SAP_DOC_NO\t" +
                "SAP_DOC_YEAR");

            foreach (var item in model)
            {
                tb_r_record_payment_list record = new tb_r_record_payment_list();
                record.generate_by = item.No_Reg.ToString();
                record.group_code = item.Entity.PV_NO;
                record.pv_date = DateTime.ParseExact(item.Entity.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                record.trans_type = item.Entity.TRANS_TYPE;
                record.vendor_code = item.Entity.VENDOR;
                record.item_no = item.Entity.ITEM_NO.ToString();
                record.generate_date = DateTime.Now;

                //await InsertData.RecordGenerateFile(record);

                MANDT = (item.Entity.MANDT == null) ? "" : item.Entity.MANDT.ToString();
                PV_NO = (item.Entity.PV_NO == null) ? "" : item.Entity.PV_NO;
                PV_YEAR = (item.Entity.PV_YEAR == null) ? "" : item.Entity.PV_YEAR.ToString();
                ITEM_NO = item.Entity.ITEM_NO.ToString();
                PV_DATE = (item.Entity.PV_DATE == null) ? "" : item.Entity.PV_DATE;
                PV_TYPE = item.Entity.PV_TYPE.ToString();
                TRANS_TYPE = (item.Entity.TRANS_TYPE == null) ? "" : item.Entity.TRANS_TYPE.ToString();
                VENDOR = (item.Entity.VENDOR == null) ? "" : item.Entity.VENDOR.ToString();
                VENDOR_GRP = item.Entity.VENDOR_GRP.ToString();
                INVOICE_NO = (item.Entity.INVOICE_NO == null) ? "" : item.Entity.INVOICE_NO;
                TAX_NO = (item.Entity.TAX_NO == null) ? "" : item.Entity.TAX_NO;
                PAYMENT_TERM = (item.Entity.PAYMENT_TERM == null) ? "" : item.Entity.PAYMENT_TERM;
                PAYMENT_METHOD = (item.Entity.PAYMENT_METHOD == null) ? "" : item.Entity.PAYMENT_METHOD;
                PLAN_PAYMENT_DT = (item.Entity.PLAN_PAYMENT_DT == null) ? "" : item.Entity.PLAN_PAYMENT_DT;
                POSTING_DT = (item.Entity.POSTING_DT == null) ? "" : item.Entity.POSTING_DT.ToString();
                TOTAL_AMOUNT = (item.Entity.TOTAL_AMOUNT == null) ? "" : item.Entity.TOTAL_AMOUNT.ToString();
                DPP_AMOUNT = item.Entity.DPP_AMOUNT.ToString();
                CURRENCY = (item.Entity.CURRENCY == null) ? "" : item.Entity.CURRENCY;
                TAX_CODE = (item.Entity.TAX_CODE == null) ? "" : item.Entity.TAX_CODE;
                HEADER_TEXT = (item.Entity.HEADER_TEXT == null) ? "" : item.Entity.HEADER_TEXT;
                BANK_TYPE = (item.Entity.BANK_TYPE == null) ? "" : item.Entity.BANK_TYPE;
                gl_account = (item.Entity.gl_account == null) ? "" : item.Entity.gl_account.ToString();
                AMOUNT = (item.Entity.AMOUNT == null) ? "" : item.Entity.AMOUNT.ToString();
                COST_CENTER = (item.Entity.COST_CENTER == null) ? "" : item.Entity.COST_CENTER.ToString();

                WBS_ELEMENT = (item.Entity.WBS_ELEMENT == null) ? "" : item.Entity.WBS_ELEMENT;
                ITEM_TEXT = (item.Entity.ITEM_TEXT == null) ? "" : item.Entity.ITEM_TEXT;
                STATUS = (item.Entity.STATUS == null) ? "" : item.Entity.STATUS.ToString();
                SAP_DOC_NO = (item.Entity.SAP_DOC_NO == null) ? "" : item.Entity.SAP_DOC_NO.ToString();
                SAP_DOC_YEAR = (item.Entity.SAP_DOC_YEAR == null) ? "" : item.Entity.SAP_DOC_YEAR.ToString();

                string textwrite = "\r\n" + (MANDT + "\t" +
                    PV_NO + "\t" +
                    PV_YEAR + "\t" +
                    ITEM_NO + "\t" +
                    PV_DATE + "\t" +
                    PV_TYPE + "\t" +
                    TRANS_TYPE + "\t" +
                    VENDOR + "\t" +
                    VENDOR_GRP + "\t" +
                    INVOICE_NO + "\t" +
                    TAX_NO + "\t" +
                    PAYMENT_TERM + "\t" +
                    PAYMENT_METHOD + "\t" +
                    PLAN_PAYMENT_DT + "\t" +
                    POSTING_DT + "\t" +
                    TOTAL_AMOUNT + "\t" +
                    DPP_AMOUNT + "\t" +
                    CURRENCY + "\t" +
                    TAX_CODE + "\t" +
                    HEADER_TEXT + "\t" +
                    BANK_TYPE + "\t" +
                    gl_account + "\t" +
                    AMOUNT + "\t" +
                    COST_CENTER + "\t" +
                    WBS_ELEMENT + "\t" +
                    ITEM_TEXT + "\t" +
                    STATUS + "\t" +
                    SAP_DOC_NO + "\t" +
                    SAP_DOC_YEAR);


                tw.Write(textwrite);
            }
            tw.Flush();
            tw.Close();
            return View("Index");
        }*/

        //[HttpPost]
        //[Authorize]
        //[Authorize(Roles = "contrast.user")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Generate(List<PaymentListHelper> model, string download = "", string submit = "", string division_input = "")
        //public async Task<ActionResult> Download(FileType model, string download = "", string submit = "", string division_input = "")
        {
            //
            if (submit == "Submit")
            {
                XLWorkbook CreateExcell = new XLWorkbook();
                var ExcelData = CreateExcell.Worksheets.Add("Payment List");
                List<vw_payment_list> dbObject1 = await GetData.PaymentListData();

                ExcelData.Cell(1, 1).Value = "MANDT";
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

                int gap = 0;
                for (int i = 0; i < dbObject1.Count(); i++)
                {
                    ExcelData.Cell(i + 2, 1).Value = dbObject1[i].MANDT;
                    ExcelData.Cell(i + 2, 2).Value = dbObject1[i].PV_NO;
                    ExcelData.Cell(i + 2, 3).Value = dbObject1[i].PV_YEAR;
                    ExcelData.Cell(i + 2, 4).Value = dbObject1[i].ITEM_NO;
                    ExcelData.Cell(i + 2, 5).Value = dbObject1[i].PV_DATE;
                    ExcelData.Cell(i + 2, 6).Value = dbObject1[i].PV_TYPE;
                    ExcelData.Cell(i + 2, 7).Value = dbObject1[i].TRANS_TYPE;
                    ExcelData.Cell(i + 2, 8).Value = dbObject1[i].VENDOR;
                    ExcelData.Cell(i + 2, 9).Value = dbObject1[i].VENDOR_GRP;
                    ExcelData.Cell(i + 2, 10).Value = dbObject1[i].INVOICE_NO;
                    ExcelData.Cell(i + 2, 11).Value = dbObject1[i].TAX_NO;
                    ExcelData.Cell(i + 2, 12).Value = dbObject1[i].PAYMENT_TERM;
                    ExcelData.Cell(i + 2, 13).Value = dbObject1[i].PAYMENT_METHOD;
                    ExcelData.Cell(i + 2, 14).Value = dbObject1[i].PLAN_PAYMENT_DT;
                    ExcelData.Cell(i + 2, 15).Value = dbObject1[i].POSTING_DT;
                    ExcelData.Cell(i + 2, 16).Value = dbObject1[i].TOTAL_AMOUNT;
                    ExcelData.Cell(i + 2, 17).Value = dbObject1[i].DPP_AMOUNT;
                    ExcelData.Cell(i + 2, 18).Value = dbObject1[i].CURRENCY;
                    ExcelData.Cell(i + 2, 19).Value = dbObject1[i].TAX_CODE;
                    ExcelData.Cell(i + 2, 20).Value = dbObject1[i].HEADER_TEXT;
                    ExcelData.Cell(i + 2, 21).Value = dbObject1[i].BANK_TYPE;
                    ExcelData.Cell(i + 2, 22).Value = dbObject1[i].gl_account;
                    ExcelData.Cell(i + 2, 23).Value = dbObject1[i].AMOUNT;
                    ExcelData.Cell(i + 2, 24).Value = dbObject1[i].COST_CENTER;
                    ExcelData.Cell(i + 2, 25).Value = dbObject1[i].WBS_ELEMENT;
                    ExcelData.Cell(i + 2, 26).Value = dbObject1[i].ITEM_TEXT;
                    ExcelData.Cell(i + 2, 27).Value = dbObject1[i].STATUS;
                    ExcelData.Cell(i + 2, 28).Value = dbObject1[i].SAP_DOC_NO;
                    ExcelData.Cell(i + 2, 29).Value = dbObject1[i].SAP_DOC_YEAR;
                    ExcelData.Cell(i + 2, 30).Value = dbObject1[i].BTR_NO;
                    ExcelData.Cell(i + 2, 31).Value = dbObject1[i].EMPLOYEE_NAME;
                    ExcelData.Cell(i + 2, 32).Value = dbObject1[i].DESTINATION;
                    ExcelData.Cell(i + 2, 33).Value = dbObject1[i].ID_CITY;
                    ExcelData.Cell(i + 2, 34).Value = dbObject1[i].BUDGET;
                    ExcelData.Cell(i + 2, 35).Value = dbObject1[i].TOTAL_AMOUNT_;
                    ExcelData.Cell(i + 2, 36).Value = dbObject1[i].COST_CENTER_;
                    ExcelData.Cell(i + 2, 37).Value = dbObject1[i].WBS_ELEMENT_;
                    ExcelData.Cell(i + 2, 38).Value = dbObject1[i].TRAVEL_TYPE;
                    ExcelData.Cell(i + 2, 39).Value = dbObject1[i].BTR_DATE;
                    ExcelData.Cell(i + 2, 40).Value = dbObject1[i].PLAN_PAYMENT_DATE;
                    ExcelData.Cell(i + 2, 41).Value = dbObject1[i].PAYMENT_METHOD_;

                    //gap = i;

                }

                foreach (var ExcellData in dbObject1)
                {
                    tb_r_record_payment_list record = new tb_r_record_payment_list();
                    record.group_code = ExcellData.PV_NO;
                    record.item_no = ExcellData.ITEM_NO.ToString();
                    record.trans_type = ExcellData.TRANS_TYPE;
                    record.vendor_code = ExcellData.VENDOR;
                    record.pv_date = DateTime.ParseExact(ExcellData.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    record.generate_by = model[0].No_Reg.ToString();
                    record.generate_date = DateTime.Now;

                    await InsertData.RecordPaymentList(record);
                }

                /* ExcelData.Cell(gap + 5, 5).Value = "Approved";
                ExcelData.Cell(gap + 5, 7).Value = "Reviewed";
                ExcelData.Cell(gap + 5, 9).Value = "Prepare";
                ExcelData.Cell(gap + 10, 5).Value = "Keijiro Inada";
                ExcelData.Cell(gap + 10, 6).Value = "Darmawan Widjaja";
                ExcelData.Cell(gap + 10, 7).Value = "Ronny K";
                ExcelData.Cell(gap + 10, 8).Value = "Yesse VH";
                ExcelData.Cell(gap + 10, 9).Value = "Sabid Ismulani"; */

                MemoryStream excelStream = new MemoryStream();
                CreateExcell.SaveAs(excelStream);
                excelStream.Position = 0;
                return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Payment List.xlsx");
            }

            return View("Index");

        }
    }
}