using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CONTRAST_WEB.Models;
using System.Text;
using System.IO;
using System.Globalization;
using System.Security.Claims;
using PagedList;

namespace CONTRAST_WEB.Controllers
{
    public class GenerateFileController : Controller
    {
        // GET: GenerateFile
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page, DateTime? startdate, DateTime? enddate)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;
            tb_m_employee model = await GetData.EmployeeInfo(identity.Name);

            List<GenerateFileHelper> Generate = new List<GenerateFileHelper>();
            List<vw_actualcost_generate_file> data = new List<vw_actualcost_generate_file>();
            data = await GetData.GenerateFileData();

            foreach (var item in data)
            {
                GenerateFileHelper temp = new GenerateFileHelper();
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
                List<GenerateFileHelper> temp = new List<GenerateFileHelper>();
                for (int k = 0; k < Generate.Count; k++)
                {
                    if (Generate[k].Entity.EMPLOYEE_NAME.ToLower().Contains(searchString.ToLower()) ||
                        Generate[k].Entity.BTR_NO.ToLower().Contains(searchString.ToLower()) ||
                        Generate[k].Entity.DESTINATION.ToLower().Contains(searchString.ToLower()) ||
                        Generate[k].Entity.COST_CENTER.ToLower().Contains(searchString.ToLower()) ||
                        Generate[k].Entity.WBS_ELEMENT.ToLower().Contains(searchString.ToLower()) ||
                        Generate[k].Entity.TRAVEL_TYPE.ToLower().Contains(searchString.ToLower()) ||
                        Generate[k].Entity.BUDGET.ToLower().Contains(searchString.ToLower()) ||
                        Generate[k].Entity.VENDOR_NAME.ToLower().Contains(searchString.ToLower()))
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
                List<GenerateFileHelper> temp = new List<GenerateFileHelper>();
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
            return View("Index", Generate.OrderBy(b => b.Entity.PV_DATE).ToList().ToPagedList(pageNumber, pageSize));
            //return View("Index", Generate.OrderBy(b=>b.Entity.PV_DATE).ToList());
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Generate(List<GenerateFileHelper> model)
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
                tb_r_record_generate_file record = new tb_r_record_generate_file();
                record.generate_by = item.No_Reg.ToString();
                record.group_code = item.Entity.PV_NO;
                record.pv_date = DateTime.ParseExact(item.Entity.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                record.trans_type = item.Entity.TRANS_TYPE;
                record.vendor_code = item.Entity.VENDOR;
                record.item_no = item.Entity.ITEM_NO.ToString();
                record.generate_date = DateTime.Now;

                await InsertData.RecordGenerateFile(record);

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
            string download_name = "AP_to_vendor_" + DateTime.Now.ToString() + ".txt";
            return File(memoryStream.GetBuffer(), "text/plain", download_name);
            //return File(memoryStream.GetBuffer(), "text/plain", "AP.txt");
        }

    }
}