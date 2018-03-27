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

namespace CONTRAST_WEB.Controllers
{
    public class PaymentProposalController : Controller
    {
        //GET: PaymentProposal
        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(tb_m_employee model)
        {
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
            return View("Index", Generate.OrderBy(b => b.Entity.vendor_code).ToList());
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Filter(List<PaymentProposalHelper> model, DateTime? startdate, DateTime? enddate, string search = "")
        {
            string lower = search.ToLower();
            List<PaymentProposalHelper> Generate = new List<PaymentProposalHelper>();
            List<vw_payment_proposal> data = new List<vw_payment_proposal>();
            if (search == "" && !startdate.HasValue && !enddate.HasValue) data = await GetData.PaymentProposalData();
            else
                data = await GetData.PaymentProposalDataFiltered(lower, startdate, enddate);

            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    PaymentProposalHelper temp = new PaymentProposalHelper();
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
                PaymentProposalHelper temp = new PaymentProposalHelper();
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
                List<vw_payment_proposal> dbObject1 = await GetData.PaymentProposalData();

                ExcelData.Cell(1, 1).Value = "ID_DATA";
                ExcelData.Cell(1, 2).Value = "VENDOR_CODE";
                ExcelData.Cell(1, 3).Value = "CURRENCY";
                ExcelData.Cell(1, 4).Value = "TOTAL_AMOUNT";
                ExcelData.Cell(1, 5).Value = "BENEFICIARY_NAME";
                ExcelData.Cell(1, 6).Value = "ACCOUNT_NUMBER";
                ExcelData.Cell(1, 7).Value = "EMPLOYEE_NAME";
                ExcelData.Cell(1, 8).Value = "REFFERENCE";

                int gap = 0;
                for (int i = 0; i < dbObject1.Count(); i++)
                {
                    ExcelData.Cell(i + 2, 1).Value = dbObject1[i].id_data;
                    ExcelData.Cell(i + 2, 2).Value = dbObject1[i].vendor_code;
                    ExcelData.Cell(i + 2, 3).Value = dbObject1[i].currency;
                    ExcelData.Cell(i + 2, 4).Value = dbObject1[i].total_amount;
                    ExcelData.Cell(i + 2, 5).Value = dbObject1[i].beneficiary_name;
                    ExcelData.Cell(i + 2, 6).Value = dbObject1[i].account_number;
                    ExcelData.Cell(i + 2, 7).Value = dbObject1[i].employee_name;
                    ExcelData.Cell(i + 2, 8).Value = dbObject1[i].refference;

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

                foreach (var ExcellData in dbObject1)
                {
                    tb_r_record_payment_proposal record = new tb_r_record_payment_proposal();
                    record.id_data = ExcellData.id_data;
                    record.vendor_code = ExcellData.vendor_code;
                    record.currency = ExcellData.currency;
                    record.total_amount = ExcellData.total_amount;
                    record.beneficiary_name = ExcellData.beneficiary_name;
                    record.account_number = ExcellData.account_number;
                    record.employee_name = ExcellData.employee_name;
                    record.refference = ExcellData.refference;
                    record.generate_by = model[0].No_Reg.ToString();
                    record.generate_date = DateTime.Now;

                    await InsertData.RecordPaymentProposal(record);
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