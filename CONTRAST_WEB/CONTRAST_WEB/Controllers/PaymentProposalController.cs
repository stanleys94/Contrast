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
using System.Security.Claims;
using PagedList;

namespace CONTRAST_WEB.Controllers
{
    public class PaymentProposalController : Controller
    {
        //GET: PaymentProposal
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page, DateTime? startdate, DateTime? enddate)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;
            tb_m_employee model = await GetData.EmployeeInfo(identity.Name);

            List<PaymentProposalHelper> Generate = new List<PaymentProposalHelper>();
            List<vw_payment_proposal> data = new List<vw_payment_proposal>();
            data = await GetData.PaymentProposalData();

            foreach (var item in data)
            {
                PaymentProposalHelper temp = new PaymentProposalHelper();
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
                List<PaymentProposalHelper> temp = new List<PaymentProposalHelper>();
                for (int k = 0; k < Generate.Count; k++)
                {
                    if ((Generate[k].Entity.vendor_code != null ? Generate[k].Entity.vendor_code : "").ToLower().Contains(searchString.ToLower()) ||
                        (Generate[k].Entity.currency != null ? Generate[k].Entity.currency : "").ToLower().Contains(searchString.ToLower()) ||
                        (Generate[k].Entity.beneficiary_name != null ? Generate[k].Entity.beneficiary_name : "").ToLower().Contains(searchString.ToLower()) ||
                        (Generate[k].Entity.account_number != null ? Generate[k].Entity.account_number : "").ToLower().Contains(searchString.ToLower()) ||
                        (Generate[k].Entity.employee_name != null ? Generate[k].Entity.employee_name : "").ToLower().Contains(searchString.ToLower()) ||
                        (Generate[k].Entity.refference != null ? Generate[k].Entity.refference : "").ToLower().Contains(searchString.ToLower()))
                    {
                        temp.Add(Generate[k]);
                    }
                }
                if (temp.Count() > 0) Generate = temp;
                else Generate = temp;
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View("Index", Generate.OrderBy(b => b.Entity.vendor_code).ToList().ToPagedList(pageNumber, pageSize));
            //return View("Index", Generate.OrderBy(b=>b.Entity.PV_DATE).ToList());     

        }
        
        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Generate(List<PaymentProposalHelper> model, string download = "", string submit = "", string division_input = "")
        {
            //
            if (submit == "Submit")
            {
                XLWorkbook CreateExcell = new XLWorkbook();
                var ExcelData = CreateExcell.Worksheets.Add("Payment Proposal");
                //List<vw_payment_proposal> dbObject1 = await GetData.PaymentProposalData();

                ExcelData.Cell(1, 1).Value = "ID_DATA";
                ExcelData.Cell(1, 2).Value = "VENDOR_CODE";
                ExcelData.Cell(1, 3).Value = "CURRENCY";
                ExcelData.Cell(1, 4).Value = "TOTAL_AMOUNT";
                ExcelData.Cell(1, 5).Value = "BENEFICIARY_NAME";
                ExcelData.Cell(1, 6).Value = "ACCOUNT_NUMBER";
                ExcelData.Cell(1, 7).Value = "EMPLOYEE_NAME";
                ExcelData.Cell(1, 8).Value = "REFFERENCE";

                int gap = 0;
                for (int i = 0; i < model.Count(); i++)
                {
                    ExcelData.Cell(i + 2, 1).Value = model[i].Entity.id_data;
                    ExcelData.Cell(i + 2, 2).Value = model[i].Entity.vendor_code;
                    ExcelData.Cell(i + 2, 3).Value = model[i].Entity.currency;
                    ExcelData.Cell(i + 2, 4).Value = model[i].Entity.total_amount;
                    ExcelData.Cell(i + 2, 5).Value = model[i].Entity.beneficiary_name;
                    ExcelData.Cell(i + 2, 6).Value = model[i].Entity.account_number;
                    ExcelData.Cell(i + 2, 7).Value = model[i].Entity.employee_name;
                    ExcelData.Cell(i + 2, 8).Value = model[i].Entity.refference;

                    gap = i;
                }

                ExcelData.Cell(gap + 5, 5).Value = "Approved";
                ExcelData.Cell(gap + 5, 7).Value = "Reviewed";
                ExcelData.Cell(gap + 5, 9).Value = "Prepare";
                ExcelData.Cell(gap + 10, 5).Value = "Keijiro Inada";
                ExcelData.Cell(gap + 10, 6).Value = "Darmawan Widjaja";
                ExcelData.Cell(gap + 10, 7).Value = "Ronny K";
                ExcelData.Cell(gap + 10, 8).Value = "Yesse VH";
                ExcelData.Cell(gap + 10, 9).Value = "Sabid Ismulani";

                foreach (var item in model)
                {
                    tb_r_record_payment_proposal record = new tb_r_record_payment_proposal();
                    record.id_data = item.Entity.id_data;
                    record.vendor_code = item.Entity.vendor_code;
                    record.currency = item.Entity.currency;
                    record.total_amount = item.Entity.total_amount;
                    record.beneficiary_name = item.Entity.beneficiary_name;
                    record.account_number = item.Entity.account_number;
                    record.employee_name = item.Entity.employee_name;
                    record.refference = item.Entity.refference;
                    record.generate_by = model[0].No_Reg.ToString();
                    record.generate_date = DateTime.Now;

                    //await InsertData.RecordPaymentProposal(record);
                }

                MemoryStream excelStream = new MemoryStream();
                CreateExcell.SaveAs(excelStream);
                excelStream.Position = 0;
                return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Payment Proposal.xlsx");
            }
            return View("Index");
        }
    }
}