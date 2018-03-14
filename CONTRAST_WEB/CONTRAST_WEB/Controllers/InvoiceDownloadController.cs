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
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp;

namespace CONTRAST_WEB.Controllers
{
    public class InvoiceDownloadController : Controller
    {
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        // GET: InvoiceDownload
        public async Task<ActionResult> Index(tb_m_employee model)
        {
            List<vw_invoice_actualcost_new> result = await GetData.InvoiceActualCostNew();
            List<InvoiceHelper> invoice = new List<InvoiceHelper>();
            foreach (var item in result)
            {
                tb_m_verifier_employee loged = await GetData.ActualCostPosition(Convert.ToInt32(model.code));
                InvoiceHelper temp = new InvoiceHelper();
                temp.invoice = item;
                temp.loged_employee = model;
                invoice.Add(temp);
            }
            for (int k = 0; k < result.Count(); k++)
            {
                tb_m_verifier_employee loged = await GetData.ActualCostPosition(Convert.ToInt32(model.code));
                InvoiceHelper temp = new InvoiceHelper();
                temp.invoice = result[k];

                if (temp.invoice.amount_1 == null) temp.invoice.amount_1 = 0;
                if (temp.invoice.amount_2 == null) temp.invoice.amount_2 = 0;
                if (temp.invoice.amount_3 == null) temp.invoice.amount_3 = 0;
                if (temp.invoice.amount_4 == null) temp.invoice.amount_4 = 0;
                if (temp.invoice.amount_5 == null) temp.invoice.amount_5 = 0;
                if (temp.invoice.amount_6 == null) temp.invoice.amount_6 = 0;

                temp.invoice.amount_total = temp.invoice.amount_1 + temp.invoice.amount_2 + temp.invoice.amount_3 + temp.invoice.amount_4 + temp.invoice.amount_5 + temp.invoice.amount_6;

                temp.loged_employee = model;
                int? amt = 0;

                if (temp.invoice.amount_1 == null)
                {
                    if (temp.invoice.amount_2 != null) amt = temp.invoice.amount_2;
                    else if (temp.invoice.amount_3 != null) amt = temp.invoice.amount_3;
                    else if (temp.invoice.amount_4 != null) amt = temp.invoice.amount_4;
                    else if (temp.invoice.amount_5 != null) amt = temp.invoice.amount_5;
                    else if (temp.invoice.amount_6 != null) amt = temp.invoice.amount_6;

                    temp.invoice.amount_1 = amt;
                    amt = 0;
                }


                string str = "";


                if (temp.invoice.destination_1 == null)
                {
                    if (temp.invoice.destination_2 != null) str = temp.invoice.destination_2;
                    else if (temp.invoice.destination_3 != null) str = temp.invoice.destination_3;
                    else if (temp.invoice.destination_4 != null) str = temp.invoice.destination_4;
                    else if (temp.invoice.destination_5 != null) str = temp.invoice.destination_5;
                    else if (temp.invoice.destination_6 != null) str = temp.invoice.destination_6;

                    temp.invoice.destination_1 = str;
                    str = "";
                }

                DateTime? date_temp = new DateTime();

                if (temp.invoice.startDate_1 == null)
                {
                    if (temp.invoice.startDate_2 != null) date_temp = temp.invoice.startDate_2;
                    else if (temp.invoice.startDate_3 != null) date_temp = temp.invoice.startDate_3;
                    else if (temp.invoice.startDate_4 != null) date_temp = temp.invoice.startDate_4;
                    else if (temp.invoice.startDate_5 != null) date_temp = temp.invoice.startDate_5;
                    else if (temp.invoice.startDate_6 != null) date_temp = temp.invoice.startDate_6;

                    temp.invoice.startDate_1 = date_temp;
                    date_temp = new DateTime();
                }

                if (temp.invoice.endDate_1 == null)
                {
                    if (temp.invoice.endDate_2 != null) date_temp = temp.invoice.endDate_2;
                    else if (temp.invoice.endDate_3 != null) date_temp = temp.invoice.endDate_3;
                    else if (temp.invoice.endDate_4 != null) date_temp = temp.invoice.endDate_4;
                    else if (temp.invoice.endDate_5 != null) date_temp = temp.invoice.endDate_5;
                    else if (temp.invoice.endDate_6 != null) date_temp = temp.invoice.endDate_6;

                    temp.invoice.endDate_1 = date_temp;
                    date_temp = new DateTime();
                }

            }
            if (result.Count == 0)
            {
                InvoiceHelper temp = new InvoiceHelper();
                temp.loged_employee = model;
                invoice.Add(temp);
                return View(invoice);
            }

            return View(invoice.OrderBy(m => m.invoice.create_date).ToList());
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        public async Task<ActionResult> Search(List<InvoiceHelper> model, string search, DateTime? start, DateTime? end)
        {
            string lower = search;
            List<vw_invoice_actualcost_new> result = await GetData.InvoiceActualCostNew();
            List<InvoiceHelper> invoice = new List<InvoiceHelper>();
            List<InvoiceHelper> Filter = new List<InvoiceHelper>();

            foreach (var item in result)
            {
                bool flag = false;
                if (
                    item.employee_input.ToLower().Contains(lower) ||
                    item.group_code.ToLower().Contains(lower) ||
                    item.destination_1.ToLower().Contains(lower) ||
                    item.vendor_code.ToLower().Contains(lower) ||
                    item.vendor_name.ToLower().Contains(lower) ||
                    item.jenis_transaksi.ToLower().Contains(lower) ||
                    item.name.ToLower().Contains(lower) ||
                    item.no_reg.ToString().Contains(lower)
                    )
                {
                    tb_m_verifier_employee loged = await GetData.ActualCostPosition(Convert.ToInt32(model[0].loged_employee.code));
                    InvoiceHelper temp = new InvoiceHelper();
                    temp.invoice = item;
                    temp.loged_employee = model[0].loged_employee;
                    invoice.Add(temp);
                }
            }
            foreach (var item in invoice)
            {
                if (start.HasValue && end.HasValue)
                {
                    if (item.invoice.create_date >= start && item.invoice.create_date <= end)
                    {
                        tb_m_verifier_employee loged = await GetData.ActualCostPosition(Convert.ToInt32(model[0].loged_employee.code));
                        Filter.Add(item);
                    }
                }
                else if (start.HasValue)
                {
                    if (item.invoice.create_date >= start)
                    {
                        tb_m_verifier_employee loged = await GetData.ActualCostPosition(Convert.ToInt32(model[0].loged_employee.code));
                        Filter.Add(item);
                    }
                }
                else if (end.HasValue)
                {
                    if (item.invoice.create_date <= end)
                    {
                        tb_m_verifier_employee loged = await GetData.ActualCostPosition(Convert.ToInt32(model[0].loged_employee.code));
                        Filter.Add(item);
                    }
                }
                else Filter.Add(item);
            }
            if (Filter.Count == 0)
            {
                InvoiceHelper temp = new InvoiceHelper();
                temp.loged_employee = model[0].loged_employee;
                Filter.Add(temp);
                ModelState.Clear();
                return View("Index", Filter);
            }
            ModelState.Clear();
            return View("Index", Filter.OrderBy(m => m.invoice.create_date).ToList());
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        public async Task<ActionResult> Download(List<InvoiceHelper> model, int download)
        {
            ModelState.Clear();
            InvoiceHelper temp = new InvoiceHelper();
            vw_invoice_actualcost_new newModel = await GetData.InvoiceActualCostNewID(Convert.ToInt32(model[download].invoice.id_data));
            temp.loged_employee = model[download].loged_employee;
            temp.invoice = new vw_invoice_actualcost_new();
            temp.invoice.amount_total = model[download].invoice.amount_total;
            temp.invoice.cost_center = model[download].invoice.cost_center;
            temp.invoice.create_date = model[download].invoice.create_date;
            temp.invoice.employee_input = model[download].invoice.employee_input;
            temp.invoice.group_code = model[download].invoice.group_code;
            temp.invoice.id_data = model[download].invoice.id_data;
            temp.invoice.invoice_number = model[download].invoice.invoice_number;
            temp.invoice.jenis_transaksi = model[download].invoice.jenis_transaksi;
            temp.invoice.name = model[download].invoice.name;
            temp.invoice.no_reg = model[download].invoice.no_reg;
            temp.invoice.qty = model[download].invoice.qty;
            temp.invoice.tax_invoice_number = model[download].invoice.tax_invoice_number;
            temp.invoice.vendor_code = model[download].invoice.vendor_code;
            temp.invoice.vendor_name = model[download].invoice.vendor_name;
            temp.invoice.wbs_no = model[download].invoice.wbs_no;

            List<string> hotel_flight = new List<string>();
            List<string> destination = new List<string>();
            List<string> vendor_id = new List<string>();
            List<int?> duration = new List<int?>();
            List<int?> amount = new List<int?>();
            List<DateTime?> start_date = new List<DateTime?>();
            List<DateTime?> end_date = new List<DateTime?>();

            if (newModel.startDate_1 != null)
            {
                destination.Add(newModel.destination_1);
                duration.Add(newModel.Duration_1);
                amount.Add(newModel.amount_1);
                start_date.Add(newModel.startDate_1);
                end_date.Add(newModel.endDate_1);
                hotel_flight.Add(newModel.hotel_flight_name_1);
            }

            if (newModel.startDate_2 != null)
            {
                destination.Add(newModel.destination_2);
                duration.Add(newModel.Duration_2);
                amount.Add(newModel.amount_2);
                start_date.Add(newModel.startDate_2);
                end_date.Add(newModel.endDate_2);
                hotel_flight.Add(newModel.hotel_flight_name_2);
            }

            if (newModel.startDate_3 != null)
            {
                destination.Add(newModel.destination_3);
                duration.Add(newModel.Duration_3);
                amount.Add(newModel.amount_3);
                start_date.Add(newModel.startDate_3);
                end_date.Add(newModel.endDate_3);
                hotel_flight.Add(newModel.hotel_flight_name_3);
            }

            if (newModel.startDate_4 != null)
            {
                destination.Add(newModel.destination_4);
                duration.Add(newModel.Duration_4);
                amount.Add(newModel.amount_4);
                start_date.Add(newModel.startDate_4);
                end_date.Add(newModel.endDate_4);
                hotel_flight.Add(newModel.hotel_flight_name_4);
            }

            if (newModel.startDate_5 != null)
            {
                destination.Add(newModel.destination_5);
                duration.Add(newModel.Duration_5);
                amount.Add(newModel.amount_5);
                start_date.Add(newModel.startDate_5);
                end_date.Add(newModel.endDate_5);
                hotel_flight.Add(newModel.hotel_flight_name_5);
            }

            if (newModel.startDate_6 != null)
            {
                destination.Add(newModel.destination_6);
                duration.Add(newModel.Duration_6);
                amount.Add(newModel.amount_6);
                start_date.Add(newModel.startDate_6);
                end_date.Add(newModel.endDate_6);
                hotel_flight.Add(newModel.hotel_flight_name_6);
            }

            ViewBag.Count = start_date.Count;

            try
            {

                temp.invoice.startDate_1 = start_date[0];
                temp.invoice.endDate_1 = end_date[0];
                temp.invoice.destination_1 = destination[0];
                temp.invoice.amount_1 = amount[0];
                temp.invoice.Duration_1 = duration[0];
                temp.invoice.hotel_flight_name_1 = hotel_flight[0];

                temp.invoice.startDate_2 = start_date[1];
                temp.invoice.endDate_2 = end_date[1];
                temp.invoice.destination_2 = destination[1];
                temp.invoice.amount_2 = amount[1];
                temp.invoice.Duration_2 = duration[1];
                temp.invoice.hotel_flight_name_2 = hotel_flight[1];

                temp.invoice.startDate_3 = start_date[2];
                temp.invoice.endDate_3 = end_date[2];
                temp.invoice.destination_3 = destination[2];
                temp.invoice.amount_3 = amount[2];
                temp.invoice.Duration_3 = duration[2];
                temp.invoice.hotel_flight_name_3 = hotel_flight[2];

                temp.invoice.startDate_4 = start_date[3];
                temp.invoice.endDate_4 = end_date[3];
                temp.invoice.destination_4 = destination[3];
                temp.invoice.amount_4 = amount[3];
                temp.invoice.Duration_4 = duration[3];
                temp.invoice.hotel_flight_name_4 = hotel_flight[3];

                temp.invoice.startDate_5 = start_date[4];
                temp.invoice.endDate_5 = end_date[4];
                temp.invoice.destination_5 = destination[4];
                temp.invoice.amount_5 = amount[4];
                temp.invoice.Duration_5 = duration[4];
                temp.invoice.hotel_flight_name_5 = hotel_flight[4];

                temp.invoice.startDate_6 = start_date[5];
                temp.invoice.endDate_6 = end_date[5];
                temp.invoice.destination_6 = destination[5];
                temp.invoice.amount_6 = amount[5];
                temp.invoice.Duration_6 = duration[5];
                temp.invoice.hotel_flight_name_6 = hotel_flight[5];
            }
            catch (Exception ex)
            {
                ModelState.Clear();
                return View("Download", temp);
            }
            ModelState.Clear();
            return View("Download", temp);
        }


        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        public async Task<ActionResult> Print(InvoiceHelper model)
        {
            tb_m_verifier_employee pos = new tb_m_verifier_employee();
            pos = await GetData.EmployeeVerifier(Convert.ToInt32(model.loged_employee.code));

            PdfDocument document = new PdfDocument();
            document.Info.Title = "Purchase Receipt";
            PdfPage page = document.AddPage();
            page.Orientation = PdfSharp.PageOrientation.Portrait;
            XSize size = PageSizeConverter.ToSize(PdfSharp.PageSize.A4);
            page.Height = size.Height;
            page.Width = size.Width;

            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont head = new XFont("Arial Narrow", 18, XFontStyle.Regular);
            XFont address = new XFont("Arial Narrow", 11, XFontStyle.Regular);
            XFont body = new XFont("Calibri", 9, XFontStyle.Regular);
            XFont total = new XFont("Lucida Grande", 11, XFontStyle.Bold);
            XFont title = new XFont("Lucida Grande", 11, XFontStyle.Bold);
            XFont subtitle = new XFont("Lucida Grande", 10, XFontStyle.Bold);
            XFont credit = new XFont("Lucida Sans", 9, XFontStyle.Regular);
            XFont valid = new XFont("Lucida Sans", 8, XFontStyle.Regular);

            XPen header_line = new XPen(XColors.Black, 2);
            XPen body_line = new XPen(XColors.DimGray, 0.5);
            XPen POLine = new XPen(XColors.SteelBlue, 3);

            string PT_TAM = "PT. TOYOTA ASTRA MOTOR";
            string receipt = "";
            if (pos.code.Contains("100775") || pos.code.Contains("101495"))
            {
                receipt = "GOODS RECEIPT";
            }
            else receipt = "PURCHASE ORDER";

            string add1 = "Jl. Laks. Yos Sudarso, Sunter II";
            string add2 = "Jakarta Utara - Indonesia";
            string add3 = "Phone :62-21 - 6515551(Hunting)";

            string btr = model.invoice.group_code;
            string name = model.invoice.name.ToUpper();
            string noreg = model.invoice.no_reg.ToString();
            string TAMPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "tam_logo.PNG");
            string ContrastPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "small_logo_horizontal.png");
            // string date = model.invoice.start_date.ToString();

            List<string> start_date = new List<string>();
            List<string> start_time = new List<string>();
            List<string> end_date = new List<string>();
            List<string> end_time = new List<string>();
            List<string> hotel_ticket = new List<string>();
            List<int?> duration = new List<int?>();
            List<string> destination = new List<string>();

            List<int?> amount = new List<int?>();


            start_date.Add(Convert.ToDateTime(model.invoice.startDate_1).ToString("dd MMM yyyy"));
            start_time.Add(Convert.ToDateTime(model.invoice.startDate_1).ToString("hh:mm tt"));

            end_date.Add(Convert.ToDateTime(model.invoice.endDate_1).ToString("dd MMM yyyy"));
            end_time.Add(Convert.ToDateTime(model.invoice.endDate_1).ToString("hh:mm tt"));

            destination.Add(model.invoice.destination_1);
            amount.Add(model.invoice.amount_1);

            duration.Add(model.invoice.Duration_1);
            if (model.invoice.hotel_flight_name_1 != null) hotel_ticket.Add(model.invoice.hotel_flight_name_1);
            else hotel_ticket.Add(" ");

            if (model.invoice.startDate_2 != null)
            {
                start_date.Add(Convert.ToDateTime(model.invoice.startDate_2).ToString("dd MMM yyyy"));
                start_time.Add(Convert.ToDateTime(model.invoice.startDate_2).ToString("hh:mm tt"));

                end_date.Add(Convert.ToDateTime(model.invoice.endDate_2).ToString("dd MMM yyyy"));
                end_time.Add(Convert.ToDateTime(model.invoice.endDate_2).ToString("hh:mm tt"));

                amount.Add(model.invoice.amount_2);
                destination.Add(model.invoice.destination_2);

                duration.Add(model.invoice.Duration_2);

                if (model.invoice.hotel_flight_name_2 != null) hotel_ticket.Add(model.invoice.hotel_flight_name_2);
                else hotel_ticket.Add(" ");
            }

            if (model.invoice.startDate_3 != null)
            {
                start_date.Add(Convert.ToDateTime(model.invoice.startDate_3).ToString("dd MMM yyyy"));
                start_time.Add(Convert.ToDateTime(model.invoice.startDate_3).ToString("hh:mm tt"));

                end_date.Add(Convert.ToDateTime(model.invoice.endDate_3).ToString("dd MMM yyyy"));
                end_time.Add(Convert.ToDateTime(model.invoice.endDate_3).ToString("hh:mm tt"));

                amount.Add(model.invoice.amount_3);
                destination.Add(model.invoice.destination_3);

                duration.Add(model.invoice.Duration_3);
                if (model.invoice.hotel_flight_name_3 != null) hotel_ticket.Add(model.invoice.hotel_flight_name_3);
                else hotel_ticket.Add(" ");
            }

            if (model.invoice.startDate_4 != null)
            {
                start_date.Add(Convert.ToDateTime(model.invoice.startDate_4).ToString("dd MMM yyyy"));
                start_time.Add(Convert.ToDateTime(model.invoice.startDate_4).ToString("hh:mm tt"));

                end_date.Add(Convert.ToDateTime(model.invoice.endDate_4).ToString("dd MMM yyyy"));
                end_time.Add(Convert.ToDateTime(model.invoice.endDate_4).ToString("hh:mm tt"));

                amount.Add(model.invoice.amount_4);
                destination.Add(model.invoice.destination_4);

                duration.Add(model.invoice.Duration_4);
                if (model.invoice.hotel_flight_name_4 != null) hotel_ticket.Add(model.invoice.hotel_flight_name_4);
                else hotel_ticket.Add(" ");
            }

            if (model.invoice.startDate_5 != null)
            {
                start_date.Add(Convert.ToDateTime(model.invoice.startDate_5).ToString("dd MMM yyyy"));
                start_time.Add(Convert.ToDateTime(model.invoice.startDate_5).ToString("hh:mm tt"));

                end_date.Add(Convert.ToDateTime(model.invoice.endDate_5).ToString("dd MMM yyyy"));
                end_time.Add(Convert.ToDateTime(model.invoice.endDate_5).ToString("hh:mm tt"));

                amount.Add(model.invoice.amount_5);
                destination.Add(model.invoice.destination_5);

                duration.Add(model.invoice.Duration_5);
                if (model.invoice.hotel_flight_name_5 != null) hotel_ticket.Add(model.invoice.hotel_flight_name_5);
                else hotel_ticket.Add(" ");
            }

            if (model.invoice.startDate_6 != null)
            {
                start_date.Add(Convert.ToDateTime(model.invoice.startDate_6).ToString("dd MMM yyyy"));
                start_time.Add(Convert.ToDateTime(model.invoice.startDate_6).ToString("hh:mm tt"));

                end_date.Add(Convert.ToDateTime(model.invoice.endDate_6).ToString("dd MMM yyyy"));
                end_time.Add(Convert.ToDateTime(model.invoice.endDate_6).ToString("hh:mm tt"));

                amount.Add(model.invoice.amount_6);
                destination.Add(model.invoice.destination_6);

                duration.Add(model.invoice.Duration_6);
                if (model.invoice.hotel_flight_name_6 != null) hotel_ticket.Add(model.invoice.hotel_flight_name_6);
                else hotel_ticket.Add(" ");
            }

            int x_pad = 5;
            int y_pad = 10;
            int legend = 50;


            gfx.DrawRectangle(XBrushes.GhostWhite, 50, 735, 495, 63);

            XImage TAM = XImage.FromFile(TAMPath);
            gfx.DrawImage(TAM, x_pad + 50, 50, 40, 40);

            XImage contrast = XImage.FromFile(ContrastPath);
            gfx.DrawImage(contrast, x_pad + 50, 735);

            gfx.DrawString(PT_TAM, head, XBrushes.Black, 95 + x_pad, 60, XStringFormats.TopLeft);
            gfx.DrawRectangle(XBrushes.GhostWhite, 377, 55, 168, 28);
            gfx.DrawLine(POLine, 377, 55, 377, 83);

            gfx.DrawString(receipt, head, XBrushes.Gray, 385, 60, XStringFormats.TopLeft);


            gfx.DrawLine(header_line, 50, 95, 545, 95);

            gfx.DrawString(add1, address, XBrushes.Black, 50 + x_pad, 105, XStringFormats.TopLeft);
            gfx.DrawString(add2, address, XBrushes.Black, 50 + x_pad, 120, XStringFormats.TopLeft);
            gfx.DrawString(add3, address, XBrushes.Black, 50 + x_pad, 135, XStringFormats.TopLeft);

            gfx.DrawString("BTA", address, XBrushes.Black, 385, 105, XStringFormats.TopLeft);
            gfx.DrawString("NOREG", address, XBrushes.Black, 385, 120, XStringFormats.TopLeft);
            gfx.DrawString("NAME", address, XBrushes.Black, 385, 135, XStringFormats.TopLeft);

            gfx.DrawString(":", address, XBrushes.Black, 385 + legend - 10, 105, XStringFormats.TopLeft);
            gfx.DrawString(":", address, XBrushes.Black, 385 + legend - 10, 120, XStringFormats.TopLeft);
            gfx.DrawString(":", address, XBrushes.Black, 385 + legend - 10, 135, XStringFormats.TopLeft);

            gfx.DrawString(btr, address, XBrushes.Black, 385 + legend, 105, XStringFormats.TopLeft);
            gfx.DrawString(noreg, address, XBrushes.Black, 385 + legend, 120, XStringFormats.TopLeft);
            gfx.DrawString(name, address, XBrushes.Black, 385 + legend, 135, XStringFormats.TopLeft);

            gfx.DrawLine(body_line, 75, 200, 520, 200);

            gfx.DrawString("DETAIL TRAVEL", title, XBrushes.Black, 75 + x_pad, 205, XStringFormats.TopLeft);

            gfx.DrawLine(body_line, 75, 225, 520, 225);

            gfx.DrawString("From", subtitle, XBrushes.Black, 85, 235, XStringFormats.TopLeft);
            gfx.DrawString("To", subtitle, XBrushes.Black, 180, 235, XStringFormats.TopLeft);

            gfx.DrawString("Depart", subtitle, XBrushes.Black, 295 + x_pad, 235, XStringFormats.TopLeft);
            gfx.DrawString("Return", subtitle, XBrushes.Black, 405 + x_pad, 235, XStringFormats.TopLeft);

            int gap_now = 0;

            string from = "Jakarta";
            for (int i = 0; i < start_date.Count(); i++)
            {

                gap_now = (i + 1) * 12;

                gfx.DrawString(from, body, XBrushes.Black, 85, 235 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(destination[i], body, XBrushes.Black, 180, 235 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(start_date[i] + " - " + start_time[i], body, XBrushes.Black, 295 + x_pad, 235 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(end_date[i] + " - " + end_time[i], body, XBrushes.Black, 405 + x_pad, 235 + gap_now, XStringFormats.TopLeft);

                from = destination[i];
            }

            gfx.DrawLine(body_line, 75, 280 + gap_now, 520, 280 + gap_now);

            gfx.DrawString("DESCRIPTION", title, XBrushes.Black, 85, 285 + gap_now, XStringFormats.TopLeft);
            gfx.DrawLine(body_line, 75, 305 + gap_now, 520, 305 + gap_now);

            gfx.DrawRectangle(XBrushes.GhostWhite, 82, 320 + gap_now, 210, 20);
            gfx.DrawLine(POLine, 85, 320 + gap_now, 82, 340 + gap_now);
            gfx.DrawString("VENDOR ID", title, XBrushes.DarkBlue, 95, 323 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString(model.invoice.vendor_code, body, XBrushes.Black, 95, 343 + gap_now, XStringFormats.TopLeft);

            gfx.DrawRectangle(XBrushes.GhostWhite, 302, 320 + gap_now, 210, 20);
            gfx.DrawLine(POLine, 305, 320 + gap_now, 302, 340 + gap_now);
            gfx.DrawString("VENDOR NAME", title, XBrushes.DarkBlue, 315, 323 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString(model.invoice.vendor_name.ToUpper(), body, XBrushes.Black, 315, 343 + gap_now, XStringFormats.TopLeft);

            string trans = "";
            if (model.invoice.jenis_transaksi.Contains("hotel")) trans = "Hotel";
            else if (model.invoice.jenis_transaksi.Contains("ticket")) trans = "Airlines";

            gfx.DrawRectangle(XBrushes.GhostWhite, 82, 365 + gap_now, 430, 20);
            gfx.DrawLine(POLine, 85, 365 + gap_now, 82, 385 + gap_now);
            gfx.DrawString(model.invoice.jenis_transaksi.ToUpper() + " DETAILS", title, XBrushes.DarkBlue, 95, 368 + gap_now, XStringFormats.TopLeft);

            gfx.DrawString(trans, subtitle, XBrushes.Black, 95, 388 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString("Date", subtitle, XBrushes.Black, 300, 388 + gap_now, XStringFormats.TopLeft);
            if (model.invoice.jenis_transaksi.Contains("hotel")) gfx.DrawString("Day(s)", subtitle, XBrushes.Black, 365, 388 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString("Price", subtitle, XBrushes.Black, 405, 388 + gap_now, XStringFormats.TopLeft);
            for (int k = 0; k < start_date.Count(); k++)
            {

                gfx.DrawString(model.invoice.jenis_transaksi + " " + hotel_ticket[k], body, XBrushes.Black, 95, 403 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(start_date[k], body, XBrushes.Black, 300, 403 + gap_now, XStringFormats.TopLeft);
                if (model.invoice.jenis_transaksi.Contains("hotel")) gfx.DrawString(duration[k].ToString(), body, XBrushes.Black, 370, 403 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString("Rp. " + Convert.ToInt32(amount[k]).ToString("N"), body, XBrushes.Black, 405, 403 + gap_now, XStringFormats.TopLeft);
                gap_now = gap_now + 15;
            }
            gfx.DrawRectangle(XBrushes.LightCyan, 82, 560 + 20, 428, 30);
            gfx.DrawLine(POLine, 85, 560 + 20, 82, 560 + 50);
            gfx.DrawString("TOTAL", title, XBrushes.DarkBlue, 85 + x_pad, 564 + 25, XStringFormats.TopLeft);
            gfx.DrawString("Rp. " + Convert.ToInt32(model.invoice.amount_total).ToString("N"), body, XBrushes.Black, 405, 564 + 25, XStringFormats.TopLeft);

            gfx.DrawString("This document was issued electronically and is therefore valid without signature", valid, XBrushes.Black, 50, 710, XStringFormats.TopLeft);
            gfx.DrawLine(XPens.Gray, 50, 725, 545, 725);
            gfx.DrawString("Printed By :", credit, XBrushes.Gray, 385, 723 + 15, XStringFormats.TopLeft);

            gfx.DrawString(model.loged_employee.name.Trim(' '), credit, XBrushes.Gray, 385, 723 + 25, XStringFormats.TopLeft);
            gfx.DrawString("(" + model.loged_employee.code.Trim(' ') + ")", credit, XBrushes.Gray, 385, 723 + 35, XStringFormats.TopLeft);

            gfx.DrawString("Printed Date :", credit, XBrushes.Gray, 385, 727 + 45, XStringFormats.TopLeft);
            gfx.DrawString(DateTime.Now.ToString("dd MMM yyyy hh:mm tt"), credit, XBrushes.Gray, 385, 727 + 55, XStringFormats.TopLeft);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;
            return File(stream, "application/pdf", receipt.Replace(" ", "_") + "_" + model.invoice.group_code.Trim(' ') + "_" + model.invoice.jenis_transaksi.Trim(' ').ToUpper() + "_" + DateTime.Now.ToString("yyMMdd-hh-mm-tt") + ".pdf");
        }



    }
}