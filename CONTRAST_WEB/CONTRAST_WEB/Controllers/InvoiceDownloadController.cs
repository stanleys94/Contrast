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
            List<tb_r_invoice_actualcost> issued = await GetData.TableInvoiceActualcostAll();

            List<InvoiceHelper> invoice = new List<InvoiceHelper>();
            List<InvoiceHelper> ga_issued = new List<InvoiceHelper>();

            tb_m_employee loged = new tb_m_employee();
            loged.code = model.code;
            loged = await GetData.EmployeeInfo(loged);

            tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(loged.code));

            if (!division.Departemen.Contains("GENERAL"))
            {
                foreach (var item in result)
                {
                    InvoiceHelper temp = new InvoiceHelper();
                    temp.invoice = item;
                    if (temp.invoice.startDate_1 == null)
                    {
                        if (temp.invoice.startDate_2 != null)
                        {
                            temp.invoice.startDate_1 = temp.invoice.startDate_2;
                            temp.invoice.endDate_1 = temp.invoice.endDate_2;
                            temp.invoice.destination_1 = temp.invoice.destination_2;
                            temp.invoice.Duration_1 = temp.invoice.Duration_2;
                        }
                        else if (temp.invoice.startDate_3 != null)
                        {
                            temp.invoice.startDate_1 = temp.invoice.startDate_3;
                            temp.invoice.endDate_1 = temp.invoice.endDate_3;
                            temp.invoice.destination_1 = temp.invoice.destination_3;
                            temp.invoice.Duration_1 = temp.invoice.Duration_3;
                        }
                        else if (temp.invoice.startDate_4 != null)
                        {
                            temp.invoice.startDate_1 = temp.invoice.startDate_4;
                            temp.invoice.endDate_1 = temp.invoice.endDate_4;
                            temp.invoice.destination_1 = temp.invoice.destination_4;
                            temp.invoice.Duration_1 = temp.invoice.Duration_4;
                        }
                        else if (temp.invoice.startDate_5 != null)
                        {
                            temp.invoice.startDate_1 = temp.invoice.startDate_5;
                            temp.invoice.endDate_1 = temp.invoice.endDate_5;
                            temp.invoice.destination_1 = temp.invoice.destination_5;
                            temp.invoice.Duration_1 = temp.invoice.Duration_5;
                        }
                        else if (temp.invoice.startDate_6 != null)
                        {
                            temp.invoice.startDate_1 = temp.invoice.startDate_6;
                            temp.invoice.endDate_1 = temp.invoice.endDate_6;
                            temp.invoice.destination_1 = temp.invoice.destination_6;
                            temp.invoice.Duration_1 = temp.invoice.Duration_6;
                        }

                    }
                    temp.loged_employee = loged;
                    invoice.Add(temp);
                }
            }
            foreach (var item in issued)
            {
                InvoiceHelper temp = new InvoiceHelper();
                if (loged != null) temp.loged_employee = loged;
                temp.invoice = new vw_invoice_actualcost_new();
                if (item.amount_1 != null) temp.invoice.amount_1 = item.amount_1;
                if (item.amount_2 != null) temp.invoice.amount_2 = item.amount_2;
                if (item.amount_3 != null) temp.invoice.amount_3 = item.amount_3;
                if (item.amount_4 != null) temp.invoice.amount_4 = item.amount_4;
                if (item.amount_5 != null) temp.invoice.amount_5 = item.amount_5;
                if (item.amount_6 != null) temp.invoice.amount_6 = item.amount_6;
                if (item.amount_total != null) temp.invoice.amount_total = item.amount_total;

                if (item.bank_account != null) temp.invoice.bank_account = item.bank_account;
                if (item.cost_center != null) temp.invoice.cost_center = item.cost_center;
                if (item.create_date != null) temp.invoice.create_date = item.create_date;

                if (item.destination_1 != null) temp.invoice.destination_1 = item.destination_1;
                if (item.destination_2 != null) temp.invoice.destination_2 = item.destination_2;
                if (item.destination_3 != null) temp.invoice.destination_3 = item.destination_3;
                if (item.destination_4 != null) temp.invoice.destination_4 = item.destination_4;
                if (item.destination_5 != null) temp.invoice.destination_5 = item.destination_5;
                if (item.destination_6 != null) temp.invoice.destination_6 = item.destination_6;

                if (item.Duration_1 != null) temp.invoice.Duration_1 = item.Duration_1;
                if (item.Duration_2 != null) temp.invoice.Duration_2 = item.Duration_2;
                if (item.Duration_3 != null) temp.invoice.Duration_3 = item.Duration_3;
                if (item.Duration_4 != null) temp.invoice.Duration_4 = item.Duration_4;
                if (item.Duration_5 != null) temp.invoice.Duration_5 = item.Duration_5;
                if (item.Duration_6 != null) temp.invoice.Duration_6 = item.Duration_6;

                if (item.employee_input != null) temp.invoice.employee_input = item.employee_input;

                if (item.endDate_1 != null) temp.invoice.endDate_1 = item.endDate_1;
                if (item.endDate_2 != null) temp.invoice.endDate_2 = item.endDate_2;
                if (item.endDate_3 != null) temp.invoice.endDate_3 = item.endDate_3;
                if (item.endDate_4 != null) temp.invoice.endDate_4 = item.endDate_4;
                if (item.endDate_5 != null) temp.invoice.endDate_5 = item.endDate_5;
                if (item.endDate_6 != null) temp.invoice.endDate_6 = item.endDate_6;

                if (item.group_code != null) temp.invoice.group_code = item.group_code;

                if (item.hotel_flight_name_1 != null) temp.invoice.hotel_flight_name_1 = item.hotel_flight_name_1;
                if (item.hotel_flight_name_2 != null) temp.invoice.hotel_flight_name_2 = item.hotel_flight_name_2;
                if (item.hotel_flight_name_3 != null) temp.invoice.hotel_flight_name_3 = item.hotel_flight_name_3;
                if (item.hotel_flight_name_4 != null) temp.invoice.hotel_flight_name_4 = item.hotel_flight_name_4;
                if (item.hotel_flight_name_5 != null) temp.invoice.hotel_flight_name_5 = item.hotel_flight_name_5;
                if (item.hotel_flight_name_6 != null) temp.invoice.hotel_flight_name_6 = item.hotel_flight_name_6;

                if (item.id_data != null) temp.invoice.id_data = item.id_data;
                if (item.invoice_number != null) temp.invoice.invoice_number = item.invoice_number;
                if (item.jenis_transaksi != null) temp.invoice.jenis_transaksi = item.jenis_transaksi;
                if (item.name != null) temp.invoice.name = item.name;
                if (item.no_reg != null) temp.invoice.no_reg = Convert.ToInt32(item.no_reg);

                if (item.startDate_1 != null) temp.invoice.startDate_1 = item.startDate_1;
                if (item.startDate_2 != null) temp.invoice.startDate_2 = item.startDate_2;
                if (item.startDate_3 != null) temp.invoice.startDate_3 = item.startDate_3;
                if (item.startDate_4 != null) temp.invoice.startDate_4 = item.startDate_4;
                if (item.startDate_5 != null) temp.invoice.startDate_5 = item.startDate_5;
                if (item.startDate_6 != null) temp.invoice.startDate_6 = item.startDate_6;

                if (item.tax_invoice_number != null) temp.invoice.tax_invoice_number = item.tax_invoice_number;
                if (item.vendor_code != null) temp.invoice.vendor_code = item.vendor_code.ToString();
                if (item.vendor_name != null) temp.invoice.vendor_name = item.vendor_name;
                if (item.wbs_no != null) temp.invoice.wbs_no = item.wbs_no;

                if (temp.invoice.destination_1 == null)
                {
                    if (temp.invoice.destination_2 != null)
                    {
                        temp.invoice.destination_1 = temp.invoice.destination_2;
                        temp.invoice.Duration_1 = temp.invoice.Duration_2;
                        temp.invoice.endDate_1 = temp.invoice.endDate_2;
                        temp.invoice.startDate_1 = temp.invoice.startDate_2;
                    }
                    else if (temp.invoice.destination_3 != null)
                    {
                        temp.invoice.destination_1 = temp.invoice.destination_3;
                        temp.invoice.Duration_1 = temp.invoice.Duration_3;
                        temp.invoice.endDate_1 = temp.invoice.endDate_3;
                        temp.invoice.startDate_1 = temp.invoice.startDate_3;
                    }
                    else if (temp.invoice.destination_4 != null)
                    {
                        temp.invoice.destination_1 = temp.invoice.destination_4;
                        temp.invoice.Duration_1 = temp.invoice.Duration_4;
                        temp.invoice.endDate_1 = temp.invoice.endDate_4;
                        temp.invoice.startDate_1 = temp.invoice.startDate_4;
                    }
                    else if (temp.invoice.destination_5 != null)
                    {
                        temp.invoice.destination_1 = temp.invoice.destination_5;
                        temp.invoice.Duration_1 = temp.invoice.Duration_5;
                        temp.invoice.endDate_1 = temp.invoice.endDate_5;
                        temp.invoice.startDate_1 = temp.invoice.startDate_5;
                    }
                    else if (temp.invoice.destination_6 != null)
                    {
                        temp.invoice.destination_1 = temp.invoice.destination_6;
                        temp.invoice.Duration_1 = temp.invoice.Duration_6;
                        temp.invoice.endDate_1 = temp.invoice.endDate_6;
                        temp.invoice.startDate_1 = temp.invoice.startDate_6;
                    }

                }

                if (division.Departemen.Contains("GENERAL") && item.GR_issued_by != null)
                {
                    ga_issued.Add(temp);
                }
                else invoice.Add(temp);
            }
            foreach (var item in ga_issued)
            {
                invoice.Add(item);
            }

            for (int k = 0; k < invoice.Count(); k++)
            {
                if (invoice[k].invoice.amount_1 == null) invoice[k].invoice.amount_1 = 0;
                if (invoice[k].invoice.amount_2 == null) invoice[k].invoice.amount_2 = 0;
                if (invoice[k].invoice.amount_3 == null) invoice[k].invoice.amount_3 = 0;
                if (invoice[k].invoice.amount_4 == null) invoice[k].invoice.amount_4 = 0;
                if (invoice[k].invoice.amount_5 == null) invoice[k].invoice.amount_5 = 0;
                if (invoice[k].invoice.amount_6 == null) invoice[k].invoice.amount_6 = 0;

                invoice[k].invoice.amount_total = invoice[k].invoice.amount_1 + invoice[k].invoice.amount_2 + invoice[k].invoice.amount_3 + invoice[k].invoice.amount_4 + invoice[k].invoice.amount_5 + invoice[k].invoice.amount_6;

                invoice[k].loged_employee = loged;
                int? amt = 0;

                if (invoice[k].invoice.amount_1 == null)
                {
                    if (invoice[k].invoice.amount_2 != null) amt = invoice[k].invoice.amount_2;
                    else if (invoice[k].invoice.amount_3 != null) amt = invoice[k].invoice.amount_3;
                    else if (invoice[k].invoice.amount_4 != null) amt = invoice[k].invoice.amount_4;
                    else if (invoice[k].invoice.amount_5 != null) amt = invoice[k].invoice.amount_5;
                    else if (invoice[k].invoice.amount_6 != null) amt = invoice[k].invoice.amount_6;

                    invoice[k].invoice.amount_1 = amt;
                    amt = 0;
                }


                string str = "";


                if (invoice[k].invoice.destination_1 == null)
                {
                    if (invoice[k].invoice.destination_2 != null) str = invoice[k].invoice.destination_2;
                    else if (invoice[k].invoice.destination_3 != null) str = invoice[k].invoice.destination_3;
                    else if (invoice[k].invoice.destination_4 != null) str = invoice[k].invoice.destination_4;
                    else if (invoice[k].invoice.destination_5 != null) str = invoice[k].invoice.destination_5;
                    else if (invoice[k].invoice.destination_6 != null) str = invoice[k].invoice.destination_6;

                    invoice[k].invoice.destination_1 = str;
                    str = "";
                }

                DateTime? date_invoice = new DateTime();

                if (invoice[k].invoice.startDate_1 == null)
                {
                    if (invoice[k].invoice.startDate_2 != null) date_invoice = invoice[k].invoice.startDate_2;
                    else if (invoice[k].invoice.startDate_3 != null) date_invoice = invoice[k].invoice.startDate_3;
                    else if (invoice[k].invoice.startDate_4 != null) date_invoice = invoice[k].invoice.startDate_4;
                    else if (invoice[k].invoice.startDate_5 != null) date_invoice = invoice[k].invoice.startDate_5;
                    else if (invoice[k].invoice.startDate_6 != null) date_invoice = invoice[k].invoice.startDate_6;

                    invoice[k].invoice.startDate_1 = date_invoice;
                    date_invoice = new DateTime();
                }

                if (invoice[k].invoice.endDate_1 == null)
                {
                    if (invoice[k].invoice.endDate_2 != null) date_invoice = invoice[k].invoice.endDate_2;
                    else if (invoice[k].invoice.endDate_3 != null) date_invoice = invoice[k].invoice.endDate_3;
                    else if (invoice[k].invoice.endDate_4 != null) date_invoice = invoice[k].invoice.endDate_4;
                    else if (invoice[k].invoice.endDate_5 != null) date_invoice = invoice[k].invoice.endDate_5;
                    else if (invoice[k].invoice.endDate_6 != null) date_invoice = invoice[k].invoice.endDate_6;

                    invoice[k].invoice.endDate_1 = date_invoice;
                    date_invoice = new DateTime();
                }

            }
            if (division.Departemen.Contains("GENERAL"))
            {
                ViewBag.outstanding = issued.Count-ga_issued.Count();
                ViewBag.issued = ga_issued.Count;
            }
            else
            {
                ViewBag.outstanding = result.Count;
                ViewBag.issued = issued.Count;
            }
            if (result.Count == 0)
            {
                InvoiceHelper temp = new InvoiceHelper();
                temp.loged_employee = loged;
                invoice.Add(temp);
                return View(invoice);
            }

            return View(invoice);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        public async Task<ActionResult> Search(List<InvoiceHelper> model, string search, DateTime? start, DateTime? end)
        {

            string lower = search.ToLower();
            List<vw_invoice_actualcost_new> result = await GetData.InvoiceActualCostNew();
            List<tb_r_invoice_actualcost> issued = await GetData.TableInvoiceActualcostAll();
            List<tb_r_invoice_actualcost> issued_filter = new List<tb_r_invoice_actualcost>();

            List<InvoiceHelper> invoice = new List<InvoiceHelper>();
            List<InvoiceHelper> Filter = new List<InvoiceHelper>();

            List<InvoiceHelper> ga_issued = new List<InvoiceHelper>();

            tb_m_employee loged = new tb_m_employee();
            loged = model[0].loged_employee;
            loged = await GetData.EmployeeInfo(loged);

            tb_m_employee_source_data division = await GetData.GetDivisionSource(Convert.ToInt32(loged.code));

            bool GA = false;
            if (division.Departemen.Contains("GENERAL")) GA = true;
            if (!GA)
            {
                foreach (var item in result)
                {
                    if (item.destination_1 == null)
                    {
                        if (item.destination_2 != null)
                        {
                            item.destination_1 = item.destination_2;
                            item.Duration_1 = item.Duration_2;
                            item.endDate_1 = item.endDate_2;
                            item.startDate_1 = item.startDate_2;
                        }
                        else if (item.destination_3 != null)
                        {
                            item.destination_1 = item.destination_3;
                            item.Duration_1 = item.Duration_3;
                            item.endDate_1 = item.endDate_3;
                            item.startDate_1 = item.startDate_3;
                        }
                        else if (item.destination_4 != null)
                        {
                            item.destination_1 = item.destination_4;
                            item.Duration_1 = item.Duration_4;
                            item.endDate_1 = item.endDate_4;
                            item.startDate_1 = item.startDate_4;
                        }
                        else if (item.destination_5 != null)
                        {
                            item.destination_1 = item.destination_5;
                            item.Duration_1 = item.Duration_5;
                            item.endDate_1 = item.endDate_5;
                            item.startDate_1 = item.startDate_5;
                        }
                        else if (item.destination_6 != null)
                        {
                            item.destination_1 = item.destination_6;
                            item.Duration_1 = item.Duration_6;
                            item.endDate_1 = item.endDate_6;
                            item.startDate_1 = item.startDate_6;
                        }

                    }

                    InvoiceHelper temp = new InvoiceHelper();
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

                        if (start.HasValue && end.HasValue)
                        {
                            if (item.create_date >= start && item.create_date <= end)
                            {
                                temp.invoice = item;
                            }
                        }
                        else if (start.HasValue)
                        {
                            if (item.create_date >= start)
                            {
                                temp.invoice = item;
                            }
                        }
                        else if (end.HasValue)
                        {
                            if (item.create_date <= end)
                            {
                                temp.invoice = item;
                            }
                        }
                        else temp.invoice = item;

                        temp.loged_employee = loged;
                        invoice.Add(temp);
                        Filter.Add(temp);
                    }
                    ViewBag.outstanding = invoice.Count();
                }
                foreach (var item in issued)
                {
                    if (item.destination_1 == null)
                    {
                        if (item.destination_2 != null)
                        {
                            item.destination_1 = item.destination_2;
                            item.Duration_1 = item.Duration_2;
                            item.endDate_1 = item.endDate_2;
                            item.startDate_1 = item.startDate_2;
                        }
                        else if (item.destination_3 != null)
                        {
                            item.destination_1 = item.destination_3;
                            item.Duration_1 = item.Duration_3;
                            item.endDate_1 = item.endDate_3;
                            item.startDate_1 = item.startDate_3;
                        }
                        else if (item.destination_4 != null)
                        {
                            item.destination_1 = item.destination_4;
                            item.Duration_1 = item.Duration_4;
                            item.endDate_1 = item.endDate_4;
                            item.startDate_1 = item.startDate_4;
                        }
                        else if (item.destination_5 != null)
                        {
                            item.destination_1 = item.destination_5;
                            item.Duration_1 = item.Duration_5;
                            item.endDate_1 = item.endDate_5;
                            item.startDate_1 = item.startDate_5;
                        }
                        else if (item.destination_6 != null)
                        {
                            item.destination_1 = item.destination_6;
                            item.Duration_1 = item.Duration_6;
                            item.endDate_1 = item.endDate_6;
                            item.startDate_1 = item.startDate_6;
                        }

                    }
                    if (
                        item.employee_input.ToLower().Contains(lower) ||
                        item.group_code.ToLower().Contains(lower) ||
                        item.destination_1.ToLower().Contains(lower) ||
                        item.vendor_code.ToString().ToLower().Contains(lower) ||
                        item.vendor_name.ToLower().Contains(lower) ||
                        item.jenis_transaksi.ToLower().Contains(lower) ||
                        item.name.ToLower().Contains(lower) ||
                        item.no_reg.ToString().Contains(lower)
                        )
                    {
                        if (start.HasValue && end.HasValue)
                        {
                            if (item.create_date >= start && item.create_date <= end) issued_filter.Add(item);
                        }
                        else if (start.HasValue)
                        {
                            if (item.create_date >= start) issued_filter.Add(item);
                        }
                        else if (end.HasValue)
                        {
                            if (item.create_date <= end) issued_filter.Add(item);
                        }
                        else issued_filter.Add(item);
                    }

                    foreach (var item2 in issued_filter)
                    {
                        InvoiceHelper temp = new InvoiceHelper();
                        if (loged != null) temp.loged_employee = loged;
                        temp.invoice = new vw_invoice_actualcost_new();
                        if (item2.amount_1 != null) temp.invoice.amount_1 = item2.amount_1;
                        if (item2.amount_2 != null) temp.invoice.amount_2 = item2.amount_2;
                        if (item2.amount_3 != null) temp.invoice.amount_3 = item2.amount_3;
                        if (item2.amount_4 != null) temp.invoice.amount_4 = item2.amount_4;
                        if (item2.amount_5 != null) temp.invoice.amount_5 = item2.amount_5;
                        if (item2.amount_6 != null) temp.invoice.amount_6 = item2.amount_6;
                        if (item2.amount_total != null) temp.invoice.amount_total = item2.amount_total;

                        if (item2.bank_account != null) temp.invoice.bank_account = item2.bank_account;
                        if (item2.cost_center != null) temp.invoice.cost_center = item2.cost_center;
                        if (item2.create_date != null) temp.invoice.create_date = item2.create_date;

                        if (item2.destination_1 != null) temp.invoice.destination_1 = item2.destination_1;
                        if (item2.destination_2 != null) temp.invoice.destination_2 = item2.destination_2;
                        if (item2.destination_3 != null) temp.invoice.destination_3 = item2.destination_3;
                        if (item2.destination_4 != null) temp.invoice.destination_4 = item2.destination_4;
                        if (item2.destination_5 != null) temp.invoice.destination_5 = item2.destination_5;
                        if (item2.destination_6 != null) temp.invoice.destination_6 = item2.destination_6;

                        if (item2.Duration_1 != null) temp.invoice.Duration_1 = item2.Duration_1;
                        if (item2.Duration_2 != null) temp.invoice.Duration_2 = item2.Duration_2;
                        if (item2.Duration_3 != null) temp.invoice.Duration_3 = item2.Duration_3;
                        if (item2.Duration_4 != null) temp.invoice.Duration_4 = item2.Duration_4;
                        if (item2.Duration_5 != null) temp.invoice.Duration_5 = item2.Duration_5;
                        if (item2.Duration_6 != null) temp.invoice.Duration_6 = item2.Duration_6;

                        if (item2.employee_input != null) temp.invoice.employee_input = item2.employee_input;

                        if (item2.endDate_1 != null) temp.invoice.endDate_1 = item2.endDate_1;
                        if (item2.endDate_2 != null) temp.invoice.endDate_2 = item2.endDate_2;
                        if (item2.endDate_3 != null) temp.invoice.endDate_3 = item2.endDate_3;
                        if (item2.endDate_4 != null) temp.invoice.endDate_4 = item2.endDate_4;
                        if (item2.endDate_5 != null) temp.invoice.endDate_5 = item2.endDate_5;
                        if (item2.endDate_6 != null) temp.invoice.endDate_6 = item2.endDate_6;

                        if (item2.group_code != null) temp.invoice.group_code = item2.group_code;

                        if (item2.hotel_flight_name_1 != null) temp.invoice.hotel_flight_name_1 = item2.hotel_flight_name_1;
                        if (item2.hotel_flight_name_2 != null) temp.invoice.hotel_flight_name_2 = item2.hotel_flight_name_2;
                        if (item2.hotel_flight_name_3 != null) temp.invoice.hotel_flight_name_3 = item2.hotel_flight_name_3;
                        if (item2.hotel_flight_name_4 != null) temp.invoice.hotel_flight_name_4 = item2.hotel_flight_name_4;
                        if (item2.hotel_flight_name_5 != null) temp.invoice.hotel_flight_name_5 = item2.hotel_flight_name_5;
                        if (item2.hotel_flight_name_6 != null) temp.invoice.hotel_flight_name_6 = item2.hotel_flight_name_6;

                        if (item2.id_data != null) temp.invoice.id_data = item2.id_data;
                        if (item2.invoice_number != null) temp.invoice.invoice_number = item2.invoice_number;
                        if (item2.jenis_transaksi != null) temp.invoice.jenis_transaksi = item2.jenis_transaksi;
                        if (item2.name != null) temp.invoice.name = item2.name;
                        if (item2.no_reg != null) temp.invoice.no_reg = Convert.ToInt32(item2.no_reg);

                        if (item2.startDate_1 != null) temp.invoice.startDate_1 = item2.startDate_1;
                        if (item2.startDate_2 != null) temp.invoice.startDate_2 = item2.startDate_2;
                        if (item2.startDate_3 != null) temp.invoice.startDate_3 = item2.startDate_3;
                        if (item2.startDate_4 != null) temp.invoice.startDate_4 = item2.startDate_4;
                        if (item2.startDate_5 != null) temp.invoice.startDate_5 = item2.startDate_5;
                        if (item2.startDate_6 != null) temp.invoice.startDate_6 = item2.startDate_6;

                        if (item2.tax_invoice_number != null) temp.invoice.tax_invoice_number = item2.tax_invoice_number;
                        if (item2.vendor_code != null) temp.invoice.vendor_code = item2.vendor_code.ToString();
                        if (item2.vendor_name != null) temp.invoice.vendor_name = item2.vendor_name;
                        if (item2.wbs_no != null) temp.invoice.wbs_no = item2.wbs_no;


                        if (division.Departemen.Contains("GENERAL") && item2.GR_issued_by != null)
                        {
                            ga_issued.Add(temp);
                        }
                        else Filter.Add(temp);
                    }
                    foreach (var item3 in ga_issued)
                    {
                        Filter.Add(item3);
                    }

                }

            }
            else
            {
                foreach (var item in issued)
                {
                    if (item.destination_1 == null)
                    {
                        if (item.destination_2 != null)
                        {
                            item.destination_1 = item.destination_2;
                            item.Duration_1 = item.Duration_2;
                            item.endDate_1 = item.endDate_2;
                            item.startDate_1 = item.startDate_2;
                        }
                        else if (item.destination_3 != null)
                        {
                            item.destination_1 = item.destination_3;
                            item.Duration_1 = item.Duration_3;
                            item.endDate_1 = item.endDate_3;
                            item.startDate_1 = item.startDate_3;
                        }
                        else if (item.destination_4 != null)
                        {
                            item.destination_1 = item.destination_4;
                            item.Duration_1 = item.Duration_4;
                            item.endDate_1 = item.endDate_4;
                            item.startDate_1 = item.startDate_4;
                        }
                        else if (item.destination_5 != null)
                        {
                            item.destination_1 = item.destination_5;
                            item.Duration_1 = item.Duration_5;
                            item.endDate_1 = item.endDate_5;
                            item.startDate_1 = item.startDate_5;
                        }
                        else if (item.destination_6 != null)
                        {
                            item.destination_1 = item.destination_6;
                            item.Duration_1 = item.Duration_6;
                            item.endDate_1 = item.endDate_6;
                            item.startDate_1 = item.startDate_6;
                        }

                    }
                    if (
                        item.employee_input.ToLower().Contains(lower) ||
                        item.group_code.ToLower().Contains(lower) ||
                        item.destination_1.ToLower().Contains(lower) ||
                        item.vendor_code.ToString().ToLower().Contains(lower) ||
                        item.vendor_name.ToLower().Contains(lower) ||
                        item.jenis_transaksi.ToLower().Contains(lower) ||
                        item.name.ToLower().Contains(lower) ||
                        item.no_reg.ToString().Contains(lower)
                        )
                    {
                        if (start.HasValue && end.HasValue)
                        {
                            if (item.create_date >= start && item.create_date <= end) issued_filter.Add(item);
                        }
                        else if (start.HasValue)
                        {
                            if (item.create_date >= start) issued_filter.Add(item);
                        }
                        else if (end.HasValue)
                        {
                            if (item.create_date <= end) issued_filter.Add(item);
                        }
                        else issued_filter.Add(item);
                    }

                }
                foreach (var item in issued_filter)
                {
                    InvoiceHelper temp = new InvoiceHelper();
                    if (loged != null) temp.loged_employee = loged;
                    temp.invoice = new vw_invoice_actualcost_new();
                    if (item.amount_1 != null) temp.invoice.amount_1 = item.amount_1;
                    if (item.amount_2 != null) temp.invoice.amount_2 = item.amount_2;
                    if (item.amount_3 != null) temp.invoice.amount_3 = item.amount_3;
                    if (item.amount_4 != null) temp.invoice.amount_4 = item.amount_4;
                    if (item.amount_5 != null) temp.invoice.amount_5 = item.amount_5;
                    if (item.amount_6 != null) temp.invoice.amount_6 = item.amount_6;
                    if (item.amount_total != null) temp.invoice.amount_total = item.amount_total;

                    if (item.bank_account != null) temp.invoice.bank_account = item.bank_account;
                    if (item.cost_center != null) temp.invoice.cost_center = item.cost_center;
                    if (item.create_date != null) temp.invoice.create_date = item.create_date;

                    if (item.destination_1 != null) temp.invoice.destination_1 = item.destination_1;
                    if (item.destination_2 != null) temp.invoice.destination_2 = item.destination_2;
                    if (item.destination_3 != null) temp.invoice.destination_3 = item.destination_3;
                    if (item.destination_4 != null) temp.invoice.destination_4 = item.destination_4;
                    if (item.destination_5 != null) temp.invoice.destination_5 = item.destination_5;
                    if (item.destination_6 != null) temp.invoice.destination_6 = item.destination_6;

                    if (item.Duration_1 != null) temp.invoice.Duration_1 = item.Duration_1;
                    if (item.Duration_2 != null) temp.invoice.Duration_2 = item.Duration_2;
                    if (item.Duration_3 != null) temp.invoice.Duration_3 = item.Duration_3;
                    if (item.Duration_4 != null) temp.invoice.Duration_4 = item.Duration_4;
                    if (item.Duration_5 != null) temp.invoice.Duration_5 = item.Duration_5;
                    if (item.Duration_6 != null) temp.invoice.Duration_6 = item.Duration_6;

                    if (item.employee_input != null) temp.invoice.employee_input = item.employee_input;

                    if (item.endDate_1 != null) temp.invoice.endDate_1 = item.endDate_1;
                    if (item.endDate_2 != null) temp.invoice.endDate_2 = item.endDate_2;
                    if (item.endDate_3 != null) temp.invoice.endDate_3 = item.endDate_3;
                    if (item.endDate_4 != null) temp.invoice.endDate_4 = item.endDate_4;
                    if (item.endDate_5 != null) temp.invoice.endDate_5 = item.endDate_5;
                    if (item.endDate_6 != null) temp.invoice.endDate_6 = item.endDate_6;

                    if (item.group_code != null) temp.invoice.group_code = item.group_code;

                    if (item.hotel_flight_name_1 != null) temp.invoice.hotel_flight_name_1 = item.hotel_flight_name_1;
                    if (item.hotel_flight_name_2 != null) temp.invoice.hotel_flight_name_2 = item.hotel_flight_name_2;
                    if (item.hotel_flight_name_3 != null) temp.invoice.hotel_flight_name_3 = item.hotel_flight_name_3;
                    if (item.hotel_flight_name_4 != null) temp.invoice.hotel_flight_name_4 = item.hotel_flight_name_4;
                    if (item.hotel_flight_name_5 != null) temp.invoice.hotel_flight_name_5 = item.hotel_flight_name_5;
                    if (item.hotel_flight_name_6 != null) temp.invoice.hotel_flight_name_6 = item.hotel_flight_name_6;

                    if (item.id_data != null) temp.invoice.id_data = item.id_data;
                    if (item.invoice_number != null) temp.invoice.invoice_number = item.invoice_number;
                    if (item.jenis_transaksi != null) temp.invoice.jenis_transaksi = item.jenis_transaksi;
                    if (item.name != null) temp.invoice.name = item.name;
                    if (item.no_reg != null) temp.invoice.no_reg = Convert.ToInt32(item.no_reg);

                    if (item.startDate_1 != null) temp.invoice.startDate_1 = item.startDate_1;
                    if (item.startDate_2 != null) temp.invoice.startDate_2 = item.startDate_2;
                    if (item.startDate_3 != null) temp.invoice.startDate_3 = item.startDate_3;
                    if (item.startDate_4 != null) temp.invoice.startDate_4 = item.startDate_4;
                    if (item.startDate_5 != null) temp.invoice.startDate_5 = item.startDate_5;
                    if (item.startDate_6 != null) temp.invoice.startDate_6 = item.startDate_6;

                    if (item.tax_invoice_number != null) temp.invoice.tax_invoice_number = item.tax_invoice_number;
                    if (item.vendor_code != null) temp.invoice.vendor_code = item.vendor_code.ToString();
                    if (item.vendor_name != null) temp.invoice.vendor_name = item.vendor_name;
                    if (item.wbs_no != null) temp.invoice.wbs_no = item.wbs_no;


                    if (division.Departemen.Contains("GENERAL") && item.GR_issued_by != null)
                    {
                        ga_issued.Add(temp);
                    }
                    else Filter.Add(temp);
                }
                foreach (var item in ga_issued)
                {
                    Filter.Add(item);
                }

            }
            if (GA)
            {
                ViewBag.outstanding = issued_filter.Count-ga_issued.Count;
                ViewBag.issued = ga_issued.Count;
            }
            else
            {
                ViewBag.outstanding = invoice.Count();
                ViewBag.issued = issued_filter.Count();
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
            return View("Index", Filter);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        public async Task<ActionResult> Download(List<InvoiceHelper> model, int download)
        {
            InvoiceHelper temp = new InvoiceHelper();
            List<tb_r_invoice_actualcost> tb_temp = new List<tb_r_invoice_actualcost>();
            tb_r_invoice_actualcost tb_temp2 = new tb_r_invoice_actualcost();
            vw_invoice_actualcost_new newModel = new vw_invoice_actualcost_new();
            List<vw_invoice_actualcost_new> BTA = new List<vw_invoice_actualcost_new>();

            tb_m_employee_source_data logged = await GetData.GetDivisionSource(Convert.ToInt32(model[download].loged_employee.code));
            if (logged.Departemen.Contains("GENERAL"))
            {
                tb_temp = await GetData.TableInvoiceActualcostSingle(model[download].invoice.group_code, model[download].invoice.jenis_transaksi);
                foreach (var item in tb_temp)
                {
                    vw_invoice_actualcost_new bta_add = new vw_invoice_actualcost_new();
                    if (item.vendor_code.ToString().Contains(model[download].invoice.vendor_code.Trim(' '))) tb_temp2 = item;
                    bta_add.destination_1 = item.destination_1;
                    bta_add.destination_2 = item.destination_2;
                    bta_add.destination_3 = item.destination_3;
                    bta_add.destination_4 = item.destination_4;
                    bta_add.destination_5 = item.destination_5;
                    bta_add.destination_6 = item.destination_6;
                }

                newModel.amount_1 = tb_temp2.amount_1;
                newModel.amount_2 = tb_temp2.amount_2;
                newModel.amount_3 = tb_temp2.amount_3;
                newModel.amount_4 = tb_temp2.amount_4;
                newModel.amount_5 = tb_temp2.amount_5;
                newModel.amount_6 = tb_temp2.amount_6;
                newModel.amount_total = tb_temp2.amount_total;
                newModel.bank_account = tb_temp2.bank_account;
                newModel.cost_center = tb_temp2.cost_center;
                newModel.create_date = tb_temp2.create_date;
                newModel.destination_1 = tb_temp2.destination_1;
                newModel.destination_2 = tb_temp2.destination_2;
                newModel.destination_3 = tb_temp2.destination_3;
                newModel.destination_4 = tb_temp2.destination_4;
                newModel.destination_5 = tb_temp2.destination_5;
                newModel.destination_6 = tb_temp2.destination_6;
                newModel.Duration_1 = tb_temp2.Duration_1;
                newModel.Duration_2 = tb_temp2.Duration_2;
                newModel.Duration_3 = tb_temp2.Duration_3;
                newModel.Duration_4 = tb_temp2.Duration_4;
                newModel.Duration_5 = tb_temp2.Duration_5;
                newModel.Duration_6 = tb_temp2.Duration_6;
                newModel.employee_input = tb_temp2.employee_input;
                newModel.endDate_1 = tb_temp2.endDate_1;
                newModel.endDate_2 = tb_temp2.endDate_2;
                newModel.endDate_3 = tb_temp2.endDate_3;
                newModel.endDate_4 = tb_temp2.endDate_4;
                newModel.endDate_5 = tb_temp2.endDate_5;
                newModel.endDate_6 = tb_temp2.endDate_6;
                newModel.group_code = tb_temp2.group_code;
                newModel.hotel_flight_name_1 = tb_temp2.hotel_flight_name_1;
                newModel.hotel_flight_name_2 = tb_temp2.hotel_flight_name_2;
                newModel.hotel_flight_name_3 = tb_temp2.hotel_flight_name_3;
                newModel.hotel_flight_name_4 = tb_temp2.hotel_flight_name_4;
                newModel.hotel_flight_name_5 = tb_temp2.hotel_flight_name_5;
                newModel.hotel_flight_name_6 = tb_temp2.hotel_flight_name_6;
                newModel.id_data = tb_temp2.id_data;
                newModel.invoice_number = tb_temp2.invoice_number;
                newModel.jenis_transaksi = tb_temp2.jenis_transaksi;
                newModel.name = tb_temp2.name;
                newModel.no_reg = Convert.ToInt32(tb_temp2.no_reg);
                newModel.qty = tb_temp2.qty;
                newModel.startDate_1 = tb_temp2.startDate_1;
                newModel.startDate_2 = tb_temp2.startDate_2;
                newModel.startDate_3 = tb_temp2.startDate_3;
                newModel.startDate_4 = tb_temp2.startDate_4;
                newModel.startDate_5 = tb_temp2.startDate_5;
                newModel.startDate_6 = tb_temp2.startDate_6;
                newModel.tax_invoice_number = tb_temp2.tax_invoice_number;
                newModel.vendor_code = tb_temp2.vendor_code.ToString();
                newModel.vendor_name = tb_temp2.vendor_name;
                newModel.wbs_no = tb_temp2.wbs_no;
            }
            else
            {
                newModel = await GetData.InvoiceActualCostNewID(Convert.ToInt32(model[download].invoice.id_data));
                BTA = await GetData.InvoiceActualCostNewBTA(newModel.group_code, newModel.jenis_transaksi);
            }

            List<Departure_City> from = new List<Departure_City>();
            Departure_City fromTemp = new Departure_City();
            temp.loged_employee = model[download].loged_employee;
            temp.invoice = new vw_invoice_actualcost_new();
            temp.invoice.amount_total = newModel.amount_total;
            temp.invoice.cost_center = newModel.cost_center;
            temp.invoice.create_date = newModel.create_date;
            temp.invoice.employee_input = newModel.employee_input;
            temp.invoice.group_code = newModel.group_code;
            temp.invoice.id_data = newModel.id_data;
            temp.invoice.invoice_number = newModel.invoice_number;
            temp.invoice.jenis_transaksi = newModel.jenis_transaksi;
            temp.invoice.name = newModel.name;
            temp.invoice.no_reg = newModel.no_reg;
            temp.invoice.qty = newModel.qty;
            temp.invoice.tax_invoice_number = newModel.tax_invoice_number;
            temp.invoice.vendor_code = newModel.vendor_code;
            temp.invoice.vendor_name = newModel.vendor_name;
            temp.invoice.wbs_no = newModel.wbs_no;

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
                fromTemp.City = "Jakarta";
                fromTemp.Destination = 1;
                from.Add(fromTemp);
            }

            if (newModel.startDate_2 != null)
            {
                ModelState.Clear();
                fromTemp = new Departure_City();
                destination.Add(newModel.destination_2);
                duration.Add(newModel.Duration_2);
                amount.Add(newModel.amount_2);
                start_date.Add(newModel.startDate_2);
                end_date.Add(newModel.endDate_2);
                hotel_flight.Add(newModel.hotel_flight_name_2);
                foreach (var item in BTA)
                {
                    if (item.destination_1 != null)
                    {
                        fromTemp.City = item.destination_1;
                        fromTemp.Destination = 2;
                        from.Add(fromTemp);
                    }
                }
            }

            if (newModel.startDate_3 != null)
            {
                ModelState.Clear();
                fromTemp = new Departure_City();

                destination.Add(newModel.destination_3);
                duration.Add(newModel.Duration_3);
                amount.Add(newModel.amount_3);
                start_date.Add(newModel.startDate_3);
                end_date.Add(newModel.endDate_3);
                hotel_flight.Add(newModel.hotel_flight_name_3);

                foreach (var item in BTA)
                {
                    if (item.destination_2 != null)
                    {
                        fromTemp.City = item.destination_2;
                        fromTemp.Destination = 3;
                        from.Add(fromTemp);
                    }
                }
            }

            if (newModel.startDate_4 != null)
            {
                ModelState.Clear();
                fromTemp = new Departure_City();

                destination.Add(newModel.destination_4);
                duration.Add(newModel.Duration_4);
                amount.Add(newModel.amount_4);
                start_date.Add(newModel.startDate_4);
                end_date.Add(newModel.endDate_4);
                hotel_flight.Add(newModel.hotel_flight_name_4);

                foreach (var item in BTA)
                {
                    if (item.destination_3 != null)
                    {
                        fromTemp.City = item.destination_3;
                        fromTemp.Destination = 4;
                        from.Add(fromTemp);
                    }
                }
            }

            if (newModel.startDate_5 != null)
            {
                ModelState.Clear();
                fromTemp = new Departure_City();

                destination.Add(newModel.destination_5);
                duration.Add(newModel.Duration_5);
                amount.Add(newModel.amount_5);
                start_date.Add(newModel.startDate_5);
                end_date.Add(newModel.endDate_5);
                hotel_flight.Add(newModel.hotel_flight_name_5);

                foreach (var item in BTA)
                {
                    if (item.destination_4 != null)
                    {
                        fromTemp.City = item.destination_4;
                        fromTemp.Destination = 5;
                        from.Add(fromTemp);
                    }
                }
            }

            if (newModel.startDate_6 != null)
            {
                ModelState.Clear();
                fromTemp = new Departure_City();

                destination.Add(newModel.destination_6);
                duration.Add(newModel.Duration_6);
                amount.Add(newModel.amount_6);
                start_date.Add(newModel.startDate_6);
                end_date.Add(newModel.endDate_6);
                hotel_flight.Add(newModel.hotel_flight_name_6);

                foreach (var item in BTA)
                {
                    if (item.destination_5 != null)
                    {
                        fromTemp.City = item.destination_5;
                        fromTemp.Destination = 6;
                    }
                }
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
                temp.from = from;

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
            tb_r_invoice_actualcost saved = new tb_r_invoice_actualcost();
            tb_r_invoice_actualcost updated = new tb_r_invoice_actualcost();
            List<tb_r_invoice_actualcost> List_Model = new List<tb_r_invoice_actualcost>();
            tb_m_employee_source_data pos = new tb_m_employee_source_data();
            vw_invoice_actualcost_new newModel = new vw_invoice_actualcost_new();

            pos = await GetData.GetDivisionSource(Convert.ToInt32(model.loged_employee.code));
            bool copy = false, copyGA = false, update = false;

            List<vw_invoice_actualcost_new> ListModel = new List<vw_invoice_actualcost_new>();
            if (pos.Departemen.Contains("GENERAL"))
            {
                List_Model = await GetData.TableInvoiceActualcostSingle(model.invoice.group_code, model.invoice.jenis_transaksi);
                foreach (var item in List_Model)
                {
                    vw_invoice_actualcost_new insert = new vw_invoice_actualcost_new();
                    insert.amount_1 = item.amount_1;
                    insert.amount_2 = item.amount_2;
                    insert.amount_3 = item.amount_3;
                    insert.amount_4 = item.amount_4;
                    insert.amount_5 = item.amount_5;
                    insert.amount_6 = item.amount_6;
                    insert.amount_total = item.amount_total;
                    insert.bank_account = item.bank_account;
                    insert.cost_center = item.cost_center;
                    insert.create_date = item.create_date;
                    insert.destination_1 = item.destination_1;
                    insert.destination_2 = item.destination_2;
                    insert.destination_3 = item.destination_3;
                    insert.destination_4 = item.destination_4;
                    insert.destination_5 = item.destination_5;
                    insert.destination_6 = item.destination_6;
                    insert.Duration_1 = item.Duration_1;
                    insert.Duration_2 = item.Duration_2;
                    insert.Duration_3 = item.Duration_3;
                    insert.Duration_4 = item.Duration_4;
                    insert.Duration_5 = item.Duration_5;
                    insert.Duration_6 = item.Duration_6;
                    insert.employee_input = item.employee_input;
                    insert.endDate_1 = item.endDate_1;
                    insert.endDate_2 = item.endDate_2;
                    insert.endDate_3 = item.endDate_3;
                    insert.endDate_4 = item.endDate_4;
                    insert.endDate_5 = item.endDate_5;
                    insert.endDate_6 = item.endDate_6;
                    insert.group_code = item.group_code;
                    insert.hotel_flight_name_1 = item.hotel_flight_name_1;
                    insert.hotel_flight_name_2 = item.hotel_flight_name_2;
                    insert.hotel_flight_name_3 = item.hotel_flight_name_3;
                    insert.hotel_flight_name_4 = item.hotel_flight_name_4;
                    insert.hotel_flight_name_5 = item.hotel_flight_name_5;
                    insert.hotel_flight_name_6 = item.hotel_flight_name_6;
                    insert.id_data = item.id_data;
                    insert.invoice_number = item.invoice_number;
                    insert.jenis_transaksi = item.jenis_transaksi;
                    insert.name = item.name;
                    insert.no_reg = Convert.ToInt32(item.no_reg);
                    insert.qty = item.qty;
                    insert.startDate_1 = item.startDate_1;
                    insert.startDate_2 = item.startDate_2;
                    insert.startDate_3 = item.startDate_3;
                    insert.startDate_4 = item.startDate_4;
                    insert.startDate_5 = item.startDate_5;
                    insert.startDate_6 = item.startDate_6;
                    insert.tax_invoice_number = item.tax_invoice_number;
                    insert.vendor_code = item.vendor_code.ToString();
                    insert.vendor_name = item.vendor_name;
                    insert.wbs_no = item.wbs_no;
                    ListModel.Add(insert);
                }
            }
            else
            {
                ListModel = await GetData.InvoiceActualCostNewBTA(model.invoice.group_code, model.invoice.jenis_transaksi);
            }
            foreach (var item in ListModel)
            {
                if (item.vendor_code == model.invoice.vendor_code) newModel = item;
            }
            List<tb_r_invoice_actualcost> list = await GetData.TableInvoiceActualcostSingle(model.invoice.group_code, model.invoice.jenis_transaksi);
           
            if (list.Count() > 0)
            {
                int index = 0;
                int check = 0;
                bool exist = false;
                for (int k = 0; k < list.Count(); k++)
                {
                    if (list[k].vendor_code == Convert.ToInt32(model.invoice.vendor_code))
                    {
                        index = k;
                        exist = true;
                    }
                }
                if (exist)
                {

                    if (list[index].amount_total == model.invoice.amount_total)
                    {
                        if (list[index].amount_1 != null && model.invoice.amount_1 != null)
                        {
                            if (list[index].amount_1 == model.invoice.amount_1) check++;
                        }
                        else if (list[index].amount_1 == null && model.invoice.amount_1 == null) check++;

                        if (list[index].amount_2 != null && model.invoice.amount_2 != null)
                        {
                            if (list[index].amount_2 == model.invoice.amount_2) check++;
                        }
                        else if (list[index].amount_2 == null && model.invoice.amount_2 == null) check++;

                        if (list[index].amount_3 != null && model.invoice.amount_3 != null)
                        {
                            if (list[index].amount_3 == model.invoice.amount_3) check++;
                        }
                        else if (list[index].amount_3 == null && model.invoice.amount_3 == null) check++;

                        if (list[index].amount_4 != null && model.invoice.amount_4 != null)
                        {
                            if (list[index].amount_4 == model.invoice.amount_4) check++;
                        }
                        else if (list[index].amount_4 == null && model.invoice.amount_4 == null) check++;

                        if (list[index].amount_5 != null && model.invoice.amount_5 != null)
                        {
                            if (list[index].amount_5 == model.invoice.amount_5) check++;
                        }
                        else if (list[index].amount_5 == null && model.invoice.amount_5 == null) check++;

                        if (list[index].amount_6 != null && model.invoice.amount_6 != null)
                        {
                            if (list[index].amount_6 == model.invoice.amount_6) check++;
                        }
                        else if (list[index].amount_6 == null && model.invoice.amount_6 == null) check++;

                        if (check == 6)
                        {
                            if (pos.Departemen.Contains("GENERAL"))
                            {
                                if (list[index].GR_issued_flag == null) copy = false;
                            }
                            copy = true;
                            saved = list[index];
                            update = false;
                        }
                        else
                        {
                            update = true;
                            updated = list[index];
                        }
                    }
                    else
                    {
                        copy = false;
                        updated = saved;
                        update = true;
                    }

                }

            }

            if (update || saved.id_data == null)
            {
                if (newModel.amount_1 != null) saved.amount_1 = newModel.amount_1;
                if (newModel.amount_2 != null) saved.amount_2 = newModel.amount_2;
                if (newModel.amount_3 != null) saved.amount_3 = newModel.amount_3;
                if (newModel.amount_4 != null) saved.amount_4 = newModel.amount_4;
                if (newModel.amount_5 != null) saved.amount_5 = newModel.amount_5;
                if (newModel.amount_6 != null) saved.amount_6 = newModel.amount_6;
                if (newModel.amount_total != null) saved.amount_total = newModel.amount_total;
                if (newModel.bank_account != null) saved.bank_account = newModel.bank_account;
                if (newModel.cost_center != null) saved.cost_center = newModel.cost_center;
                if (newModel.create_date != null) saved.create_date = newModel.create_date;
                if (newModel.destination_1 != null) saved.destination_1 = newModel.destination_1;
                if (newModel.destination_2 != null) saved.destination_2 = newModel.destination_2;
                if (newModel.destination_3 != null) saved.destination_3 = newModel.destination_3;
                if (newModel.destination_4 != null) saved.destination_4 = newModel.destination_4;
                if (newModel.destination_5 != null) saved.destination_5 = newModel.destination_5;
                if (newModel.destination_6 != null) saved.destination_6 = newModel.destination_6;
                if (newModel.Duration_1 != null) saved.Duration_1 = newModel.Duration_1;
                if (newModel.Duration_2 != null) saved.Duration_2 = newModel.Duration_2;
                if (newModel.Duration_3 != null) saved.Duration_3 = newModel.Duration_3;
                if (newModel.Duration_4 != null) saved.Duration_4 = newModel.Duration_4;
                if (newModel.Duration_5 != null) saved.Duration_5 = newModel.Duration_5;
                if (newModel.Duration_6 != null) saved.Duration_6 = newModel.Duration_6;
                if (newModel.employee_input != null) saved.employee_input = newModel.employee_input;
                if (newModel.endDate_1 != null) saved.endDate_1 = newModel.endDate_1;
                if (newModel.endDate_2 != null) saved.endDate_2 = newModel.endDate_2;
                if (newModel.endDate_3 != null) saved.endDate_3 = newModel.endDate_3;
                if (newModel.endDate_4 != null) saved.endDate_4 = newModel.endDate_4;
                if (newModel.endDate_5 != null) saved.endDate_5 = newModel.endDate_5;
                if (newModel.endDate_6 != null) saved.endDate_6 = newModel.endDate_6;
                if (newModel.group_code != null) saved.group_code = newModel.group_code;
                if (newModel.hotel_flight_name_1 != null) saved.hotel_flight_name_1 = newModel.hotel_flight_name_1;
                if (newModel.hotel_flight_name_2 != null) saved.hotel_flight_name_2 = newModel.hotel_flight_name_2;
                if (newModel.hotel_flight_name_3 != null) saved.hotel_flight_name_3 = newModel.hotel_flight_name_3;
                if (newModel.hotel_flight_name_4 != null) saved.hotel_flight_name_4 = newModel.hotel_flight_name_4;
                if (newModel.hotel_flight_name_5 != null) saved.hotel_flight_name_5 = newModel.hotel_flight_name_5;
                if (newModel.hotel_flight_name_6 != null) saved.hotel_flight_name_6 = newModel.hotel_flight_name_6;
                if (newModel.id_data != null) saved.id_data = Convert.ToInt32(newModel.id_data);
                if (newModel.invoice_number != null) saved.invoice_number = newModel.invoice_number;
                if (newModel.jenis_transaksi != null) saved.jenis_transaksi = newModel.jenis_transaksi;
                if (newModel.name != null) saved.name = newModel.name;
                if (newModel.no_reg != null) saved.no_reg = newModel.no_reg;
                if (newModel.qty != null) saved.qty = newModel.qty;
                if (newModel.startDate_1 != null) saved.startDate_1 = newModel.startDate_1;
                if (newModel.startDate_2 != null) saved.startDate_2 = newModel.startDate_2;
                if (newModel.startDate_3 != null) saved.startDate_3 = newModel.startDate_3;
                if (newModel.startDate_4 != null) saved.startDate_4 = newModel.startDate_4;
                if (newModel.startDate_5 != null) saved.startDate_5 = newModel.startDate_5;
                if (newModel.startDate_6 != null) saved.startDate_6 = newModel.startDate_6;
                if (newModel.tax_invoice_number != null) saved.tax_invoice_number = newModel.tax_invoice_number;
                if (newModel.vendor_code != null) saved.vendor_code = Convert.ToInt32(newModel.vendor_code);
                if (newModel.vendor_name != null) saved.vendor_name = newModel.vendor_name;
                if (newModel.wbs_no != null) saved.wbs_no = newModel.wbs_no;

                saved.PO_issued_date = print_date;
                saved.PO_issued_by = pos.noreg.ToString();
                saved.PO_issued_flag = 1;
            }

            else
            {
                if (pos.Departemen.Contains("GENERAL"))
                {
                    if (saved.GR_issued_flag == 1)
                    {
                        copy = true;
                        copyGA = true;
                    }
                    else
                    {
                        saved.GR_issued_date = print_date;
                        saved.GR_issued_by = pos.noreg.ToString();
                        saved.GR_issued_flag = 1;
                    }
                }
                else copy = true;
            }
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

            if (copy)
            {

                //gfx.TranslateTransform(page.Width / 2, page.Height / 2);

                //gfx.TranslateTransform(-page.Width / 2, -page.Height / 2);

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

            }

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
            if (pos.Departemen.Contains("GENERAL"))
            {
                receipt = "GOODS RECEIPT";
            }
            else receipt = "PURCHASE ORDER";

            string add1 = "Jl. Laks. Yos Sudarso, Sunter II";
            string add2 = "Jakarta Utara - Indonesia";
            string add3 = "Phone :62-21 - 6515551(Hunting)";

            string btr = saved.group_code;
            string name = saved.name.ToUpper();
            string noreg = saved.no_reg.ToString();
            string TAMPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "tam_logo.PNG");
            string ContrastPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(Constant.ImgPath), "small_logo_horizontal.png");
            // string date = saved.start_date.ToString();

            List<string> start_date = new List<string>();
            List<string> start_time = new List<string>();
            List<string> end_date = new List<string>();
            List<string> end_time = new List<string>();
            List<string> hotel_ticket = new List<string>();
            List<int?> duration = new List<int?>();
            List<string> destination = new List<string>();
            List<Departure_City> From = model.from;
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


            for (int i = 0; i < start_date.Count(); i++)
            {

                gap_now = (i + 1) * 12;

                gfx.DrawString(From[i].City, body, XBrushes.Black, 85, 235 + gap_now, XStringFormats.TopLeft);
                gfx.DrawString(destination[i], body, XBrushes.Black, 180, 235 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(start_date[i] + " - " + start_time[i], body, XBrushes.Black, 295 + x_pad, 235 + gap_now, XStringFormats.TopLeft);

                gfx.DrawString(end_date[i] + " - " + end_time[i], body, XBrushes.Black, 405 + x_pad, 235 + gap_now, XStringFormats.TopLeft);


            }

            gfx.DrawLine(body_line, 75, 280 + gap_now, 520, 280 + gap_now);

            gfx.DrawString("DESCRIPTION", title, XBrushes.Black, 85, 285 + gap_now, XStringFormats.TopLeft);
            gfx.DrawLine(body_line, 75, 305 + gap_now, 520, 305 + gap_now);

            gfx.DrawRectangle(XBrushes.GhostWhite, 82, 320 + gap_now, 210, 20);
            gfx.DrawLine(POLine, 85, 320 + gap_now, 82, 340 + gap_now);
            gfx.DrawString("VENDOR ID", title, XBrushes.DarkBlue, 95, 323 + gap_now, XStringFormats.TopLeft);
            gfx.DrawString(model.invoice.vendor_code.ToString(), body, XBrushes.Black, 95, 343 + gap_now, XStringFormats.TopLeft);

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

            if (copy) gfx.DrawString("Reissued By :", credit, XBrushes.Gray, 385, 723 + 15, XStringFormats.TopLeft);
            else
                gfx.DrawString("Issued By :", credit, XBrushes.Gray, 385, 723 + 15, XStringFormats.TopLeft);
            gfx.DrawString(model.loged_employee.name.Trim(' '), credit, XBrushes.Gray, 385, 723 + 25, XStringFormats.TopLeft);
            gfx.DrawString("(" + model.loged_employee.code.Trim(' ') + ")", credit, XBrushes.Gray, 385, 723 + 35, XStringFormats.TopLeft);

            gfx.DrawString("Printed Date :", credit, XBrushes.Gray, 385, 727 + 45, XStringFormats.TopLeft);
            gfx.DrawString(DateTime.Now.ToString("dd MMM yyyy hh:mm tt"), credit, XBrushes.Gray, 385, 727 + 55, XStringFormats.TopLeft);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;

            if (!copy && !pos.Departemen.Contains("GENERAL"))
            {
                 await InsertData.InvoiceWrite(saved);
            }

            if (!copyGA && pos.Departemen.Contains("GENERAL"))
            {
                 await UpdateData.InvoiceActualCost(saved);
            }
            if (!copy) return File(stream, "application/pdf", receipt.Replace(" ", "_") + "_" + model.invoice.group_code.Trim(' ') + "_" + model.invoice.jenis_transaksi.Trim(' ').ToUpper() + "_" + DateTime.Now.ToString("yyMMdd-hh-mm-tt") + ".pdf");
            else return File(stream, "application/pdf", receipt.Replace(" ", "_") + "_" + model.invoice.group_code.Trim(' ') + "_" + model.invoice.jenis_transaksi.Trim(' ').ToUpper() + "_" + DateTime.Now.ToString("yyMMdd-hh-mm-tt") + "_COPY_" + ".pdf");
        }



    }
}