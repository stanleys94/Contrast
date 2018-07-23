using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CONTRAST_WEB.Models;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.IO;

namespace CONTRAST_WEB.Controllers
{
    public class UploadMasterController : Controller
    {
        // GET: UploadMaster
        public async Task<ActionResult> Index(tb_m_employee model)
        {
            FileType result = new FileType();
            tb_m_verifier_employee access_status = new tb_m_verifier_employee();
            access_status = await GetData.EmployeeVerifier(Convert.ToInt32(model.code));

            result.name = model.name;
            result.no_reg = model.code;
            result.position = access_status.position;
            result.division = access_status.division_name;

            if (access_status != null)
            {
                return View("Index", result);
            }
            else return View("You don't have permision to access this page");
        }

        public async Task<ActionResult> Upload(FileType model, string download = "", string submit = "", string division_input = "")
        {
            if (submit == "Submit" && download == "")
            {
                if (Request != null)
                {
                    HttpPostedFileBase file = Request.Files["FileUpload"];
                    if (file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        ViewBag.error = "Wrong file type inserted, please enter .xlsx file only";
                        return View("Index", model);
                    }
                    if (((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName)))
                    {
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.ContentLength];
                        var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        string error = "";
                        bool flag_error = false, flag = false;
                        List<string> error_list = new List<string>();
                        using (XLWorkbook workBook = new XLWorkbook(file.InputStream))
                        {
                            IXLWorksheet workSheet = workBook.Worksheet(1);

                            if (model.excell_type == "Master Rate Hotel")
                            {
                                int k = 1, i = 2;
                                while (true)
                                {
                                    var check_table = workSheet.Cell(i, k);
                                    if (check_table.GetValue<String>() == "")
                                    {
                                        if (i == 2)
                                        {
                                            ViewBag.error_list = "Not Valid Excell or Excell not starting from Collumn 2";
                                            return View("Index", model);
                                        }
                                        break;
                                    }

                                    tb_m_rate_hotel ExcelObject1 = new tb_m_rate_hotel();

                                    while (true)
                                    {
                                        try
                                        {
                                            var cellString = workSheet.Cell(i, k);
                                            if (cellString.GetValue<string>() == "" && k < 5) throw new System.ArgumentException("There's no Value in this Cell");

                                            else if (k == 1) ExcelObject1.kelas = cellString.GetValue<string>();
                                            else if (k == 2) ExcelObject1.bintang = cellString.GetValue<string>();
                                            else if (k == 3) ExcelObject1.domestik = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 4) ExcelObject1.overseas = Convert.ToDouble(cellString.GetValue<string>());

                                            if (k >= 5)
                                            {
                                                if (cellString.GetValue<string>() != "") throw new System.ArgumentException("Invalid Format Excell");
                                                else break;
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            flag_error = true;
                                            error = "on row : " + i + ", collumn : " +k + "  " + ex.Message + "\n";
                                            error_list.Add(error);
                                            if (k >= 5) break;
                                        }
                                        k++;
                                    }

                                    i++;
                                    k = 1;
                                }
                                k = 1;
                                i = 2;

                                if (!flag_error)
                                {
                                    while (true)
                                    {
                                        var check_table = workSheet.Cell(i, k);
                                        if (check_table.GetValue<String>() == "") break;
                                        tb_m_rate_hotel ExcelObject1 = new tb_m_rate_hotel();

                                        while (true)
                                        {
                                            var cellString = workSheet.Cell(i, k);

                                            if (k == 1) ExcelObject1.kelas = cellString.GetValue<string>();
                                            else if (k == 2) ExcelObject1.bintang = cellString.GetValue<string>();
                                            else if (k == 3) ExcelObject1.domestik = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 4) ExcelObject1.overseas = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 5) break;
                                            k++;
                                        }
                                        ExcelObject1.user_created = model.no_reg;
                                        ExcelObject1.active_flag = true;
                                        ExcelObject1.start_date = DateTime.Now;
                                        await Utility.RateHotel(ExcelObject1);
                                        i++;
                                        k = 1;
                                    }
                                    flag = true;
                                }
                                else if (flag_error)
                                {
                                    ViewBag.errorlist = error_list;
                                    return View("Index", model);
                                }
                            }
                            else if (model.excell_type == "Master Rate Flight")

                            {
                                int k = 1, i = 2;
                                while (true)
                                {
                                    var check_table = workSheet.Cell(i, k);
                                    if (check_table.GetValue<String>() == "")
                                    {
                                        if (i == 2)
                                        {
                                            ViewBag.error_list = "Not Valid Excell or Excell not starting from Collumn 2";
                                            return View("Index", model);
                                        }
                                        break;
                                    }
                                    tb_m_rate_flight ExcelObject2 = new tb_m_rate_flight();

                                    while (true)
                                    {
                                        try
                                        {
                                            int region = 0;
                                            var cellString = workSheet.Cell(i, k);
                                            if (cellString.GetValue<string>() == "" && k < 5) throw new System.ArgumentException("There's no Value in this Cell");
                                           
                                            else if (k == 1) ExcelObject2.destination = cellString.GetValue<string>();
                                            else if (k == 2) region = Convert.ToInt32(cellString.GetValue<string>());
                                            else if (k == 3) ExcelObject2.economy = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 4) ExcelObject2.business = Convert.ToDouble(cellString.GetValue<string>());
                                            if (k > 4)
                                            {
                                                if (cellString.GetValue<string>() != "") throw new System.ArgumentException("Invalid Format Excell");
                                                else break;
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            flag_error = true;
                                            error = "on row : " + i + ", collumn : " + k + "  " + ex.Message + "\n";
                                            error_list.Add(error);
                                            if (k >= 4) break;
                                        }
                                        k++;
                                    }
                                    i++;
                                    k = 1;
                                }
                                k = 1;
                                i = 2;

                                if (!flag_error)
                                {
                                    List<tb_m_destination> region_list = await GetData.DestinationActive();
                                    while (true)
                                    {
                                        var check_table = workSheet.Cell(i, k);
                                        if (check_table.GetValue<String>() == "") break;
                                        tb_m_rate_flight ExcelObject2 = new tb_m_rate_flight();
                                        tb_m_destination add_destination = new tb_m_destination();
                                        
                                        while (true)
                                        {
                                            
                                            var cellString = workSheet.Cell(i, k);
                                            if (k == 1) ExcelObject2.destination = cellString.GetValue<string>();
                                            else if (k == 2)
                                            {
                                                try
                                                {
                                                    add_destination = region_list.Where(b => b.destination_name.Contains(ExcelObject2.destination)).First();
                                                }
                                                catch (Exception extra)
                                                {
                                                    add_destination.destination_name = ExcelObject2.destination;
                                                    add_destination.id_region = Convert.ToInt16(cellString.GetValue<string>());
                                                    if (add_destination.id_region == 0) add_destination.id_region = 4;
                                                }       
                                            } 
                                            else if (k == 3) ExcelObject2.economy = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 4) ExcelObject2.business = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 5) break;
                                            k++;
                                        }
                                        ExcelObject2.user_created = model.no_reg;
                                        ExcelObject2.active_flag = true;
                                        ExcelObject2.start_date = DateTime.Now;
                                        if (add_destination.active_flag != true)
                                        {
                                            add_destination.active_flag = true;
                                            add_destination.start_date = DateTime.Now.AddHours(7);
                                            await Utility.Destination(add_destination);
                                        }
                                        await Utility.RateFlight(ExcelObject2);
                                        
                                        i++;
                                        k = 1;
                                    }
                                    flag = true;
                                }
                                else if (flag_error)
                                {
                                    ViewBag.errorlist = error_list;
                                    return View("Index", model);
                                }
                            }
                            else if (model.excell_type == "Master Rate Visa")
                            {
                                int k = 1, i = 2;
                                while (true)
                                {
                                    var check_table = workSheet.Cell(i, k);
                                    if (check_table.GetValue<String>() == "")
                                    {
                                        if (i == 2)
                                        {
                                            ViewBag.error_list = "Not Valid Excell or Excell not starting from Collumn 2";
                                            return View("Index", model);
                                        }
                                        break;
                                    }
                                    tb_m_rate_visa ExcelObject4 = new tb_m_rate_visa();


                                    while (true)
                                    {
                                        try
                                        {
                                            var cellString = workSheet.Cell(i, k);
                                            if (cellString.GetValue<string>() == "" && k < 3) throw new System.ArgumentException("There's no Value in this Cell");

                                            else if (k == 1) ExcelObject4.destination = cellString.GetValue<string>();
                                            else if (k == 2) ExcelObject4.visa = Convert.ToDouble(cellString.GetValue<string>());
                                            if (k >= 3)
                                            {
                                                if (cellString.GetValue<string>() != "") throw new System.ArgumentException("Invalid Format Excell");
                                                else break;
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            flag_error = true;
                                            error = "on row : " + i + ", collumn : " + k + "  " + ex.Message + "\n";
                                            error_list.Add(error);
                                            if (k >= 3) break;
                                        }
                                        k++;
                                    }

                                    i++;
                                    k = 1;
                                }
                                k = 1;
                                i = 2;
                                if (!flag_error)
                                {
                                    while (true)
                                    {
                                        var check_table = workSheet.Cell(i, k);
                                        if (check_table.GetValue<String>() == "") break;
                                        tb_m_rate_visa ExcelObject4 = new tb_m_rate_visa();


                                        while (true)
                                        {

                                            var cellString = workSheet.Cell(i, k);

                                            if (k == 1) ExcelObject4.destination = cellString.GetValue<string>();
                                            else if (k == 2) ExcelObject4.visa = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 3) break;

                                            k++;
                                        }
                                        ExcelObject4.user_created = model.no_reg;
                                        ExcelObject4.active_flag = true;
                                        ExcelObject4.start_date = DateTime.Now;
                                        await Utility.RateVisa(ExcelObject4);
                                        i++;
                                        k = 1;
                                    }
                                    flag = true;
                                }
                                else if (flag_error)
                                {
                                    ViewBag.errorlist = error_list;
                                    return View("Index", model);
                                }
                            }
                            else if (model.excell_type == "Master Rate Passport")// not implemented yet
                            {
                                int k = 1, i = 2;
                                while (true)
                                {
                                    var check_table = workSheet.Cell(i, k);
                                    if (check_table.GetValue<String>() == "")
                                    {
                                        if (i == 2)
                                        {
                                            ViewBag.error_list = "Not Valid Excell or Excell not starting from Collumn 2";
                                            return View("Index", model);
                                        }
                                        break;
                                    }
                                    tb_m_rate_passport ExcelObject4 = new tb_m_rate_passport();


                                    while (true)
                                    {
                                        try
                                        {
                                            var cellString = workSheet.Cell(i, k);
                                            if (cellString.GetValue<string>() == "" && k < 3) throw new System.ArgumentException("There's no Value in this Cell");

                                            else if (k == 1) ExcelObject4.passport_type = cellString.GetValue<string>();
                                            else if (k == 2) ExcelObject4.rate_passport = Convert.ToInt32(cellString.GetValue<string>());
                                            if (k >= 3)
                                            {
                                                if (cellString.GetValue<string>() != "") throw new System.ArgumentException("Invalid Format Excell");
                                                else break;
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            flag_error = true;
                                            error = "on row : " + i + ", collumn : " +k + "  " + ex.Message + "\n";
                                            error_list.Add(error);
                                            if (k >= 3) break;
                                        }
                                        k++;
                                    }

                                    i++;
                                    k = 1;
                                }
                                k = 1;
                                i = 2;
                                if (!flag_error)
                                {
                                    while (true)
                                    {
                                        var check_table = workSheet.Cell(i, k);
                                        if (check_table.GetValue<String>() == "") break;
                                        tb_m_rate_passport ExcelObject4 = new tb_m_rate_passport();


                                        while (true)
                                        {

                                            var cellString = workSheet.Cell(i, k);

                                            if (k == 1) ExcelObject4.passport_type = cellString.GetValue<string>();
                                            else if (k == 2) ExcelObject4.rate_passport = Convert.ToInt32(cellString.GetValue<string>());

                                            if (k == 3) break;

                                            k++;
                                        }
                                        ExcelObject4.user_created = model.no_reg;
                                        ExcelObject4.active_flag = true;
                                        ExcelObject4.start_date = DateTime.Now;
                                        await Utility.RatePassport(ExcelObject4);
                                        i++;
                                        k = 1;
                                    }
                                    flag = true;
                                }
                                else if (flag_error)
                                {
                                    ViewBag.errorlist = error_list;
                                    return View("Index", model);
                                }
                            }
                            else if (model.excell_type == "Master Vendor Employee")
                            {
                                int k = 1, i = 2;
                                while (true)
                                {
                                    var check_table = workSheet.Cell(i, k);
                                    if (check_table.GetValue<String>() == "")
                                    {
                                        if (i == 2)
                                        {
                                            ViewBag.error_list = "Not Valid Excell or Excell not starting from Collumn 2";
                                            return View("Index", model);
                                        }
                                        break;
                                    }
                                    tb_m_vendor_employee ExcelObject6 = new tb_m_vendor_employee();

                                    while (true)
                                    {
                                        try
                                        {
                                            var cellString = workSheet.Cell(i, k);
                                            if (cellString.GetValue<string>() == "" && k < 9) throw new System.ArgumentException("There's no Value in this Cell");

                                            else if (k == 1) ExcelObject6.no_reg = Convert.ToInt32(cellString.GetValue<string>());
                                            else if (k == 2) ExcelObject6.vendor_code_employee = cellString.GetValue<string>();
                                            else if (k == 3) ExcelObject6.bank_number = cellString.GetValue<string>();
                                            else if (k == 4) ExcelObject6.bank_type = cellString.GetValue<string>();
                                            else if (k == 5) ExcelObject6.Bank_Name = cellString.GetValue<string>();
                                            else if (k == 6) ExcelObject6.account_number = cellString.GetValue<string>();
                                            else if (k == 7) ExcelObject6.employee_name = cellString.GetValue<string>();
                                            else if (k == 8) ExcelObject6.beneficiary_name = (cellString.GetValue<string>());
                                            if (k >= 9)
                                            {
                                                string self = cellString.GetValue<string>();
                                                if (cellString.GetValue<string>() != "") throw new System.ArgumentException("Invalid Format Excell");
                                                else break;
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            flag_error = true;
                                            error = "on row : " + i + ", collumn : " +k + "  " + ex.Message + "\n";
                                            error_list.Add(error);
                                            if (k >= 9) break;
                                        }
                                        k++;

                                    }
                                    i++;
                                    k = 1;
                                }
                                if (!flag_error)
                                {
                                    i = 2;
                                    k = 1;
                                    while (true)
                                    {
                                        var check_table = workSheet.Cell(i, k);
                                        if (check_table.GetValue<String>() == "") break;
                                        tb_m_vendor_employee ExcelObject6 = new tb_m_vendor_employee();

                                        while (true)
                                        {

                                            var cellString = workSheet.Cell(i, k);
                                            if (k == 1) ExcelObject6.no_reg = Convert.ToInt32(cellString.GetValue<string>());
                                            else if (k == 2) ExcelObject6.vendor_code_employee = cellString.GetValue<string>();
                                            else if (k == 3) ExcelObject6.bank_number = cellString.GetValue<string>();
                                            else if (k == 4) ExcelObject6.bank_type = cellString.GetValue<string>();
                                            else if (k == 5) ExcelObject6.Bank_Name = cellString.GetValue<string>();
                                            else if (k == 6) ExcelObject6.account_number = cellString.GetValue<string>();
                                            else if (k == 7) ExcelObject6.employee_name = cellString.GetValue<string>();
                                            else if (k == 8) ExcelObject6.beneficiary_name = (cellString.GetValue<string>());
                                            if (k == 9) break;
                                            k++;
                                        }
                                        ExcelObject6.start_date = DateTime.Now;
                                        ExcelObject6.user_created = model.no_reg;
                                        ExcelObject6.start_date = DateTime.Now;
                                        await Utility.VendorEmployee(ExcelObject6);
                                        i++;
                                        k = 1;

                                    }
                                    flag = true;
                                }
                                else if (flag_error)
                                {
                                    ViewBag.errorlist = error_list;
                                    return View("Index", model);
                                }
                            }
                            else if (model.excell_type == "Master Vendor Travel")
                            {
                                int k = 1, i = 2;
                                while (true)
                                {
                                    var check_table = workSheet.Cell(i, k);
                                    if (check_table.GetValue<String>() == "")
                                    {
                                        if (i == 2)
                                        {
                                            ViewBag.error_list = "Not Valid Excell or Excell not starting from Collumn 2";
                                            return View("Index", model);
                                        }
                                        break;
                                    }
                                    tb_m_vendor ExcelObject7 = new tb_m_vendor();

                                    while (true)
                                    {
                                        try
                                        {
                                            var cellString = workSheet.Cell(i, k);
                                            if (cellString.GetValue<string>() == "" && k < 10) throw new System.ArgumentException("There's no Value in this Cell");

                                            else if (k == 1) ExcelObject7.vendor_code = cellString.GetValue<string>();
                                            else if (k == 2) ExcelObject7.vendor_name = cellString.GetValue<string>();
                                            else if (k == 3) ExcelObject7.type = cellString.GetValue<string>();
                                            else if (k == 4) ExcelObject7.bank_account = cellString.GetValue<string>();
                                            else if (k == 5) ExcelObject7.account_holder = cellString.GetValue<string>();
                                            else if (k == 6) ExcelObject7.name_of_bank = cellString.GetValue<string>();
                                            else if (k == 7) ExcelObject7.swift_code = cellString.GetValue<string>();
                                            else if (k == 8) ExcelObject7.skn = Convert.ToInt32(cellString.GetValue<string>());
                                            else if (k == 9) ExcelObject7.email = cellString.GetValue<string>();
                                            if (k >= 10)
                                            {
                                                if (cellString.GetValue<string>() != "") throw new System.ArgumentException("Invalid Format Excell");
                                                else break;
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            flag_error = true;
                                            error = "on row : " + i + ", collumn : " +k + "  " + ex.Message + "\n";
                                            error_list.Add(error);
                                            if (k >= 10) break;
                                        }
                                        k++;
                                    }
                                    i++;
                                    k = 1;
                                }
                                if (!flag_error)
                                {
                                    i = 2;
                                    k = 1;
                                    while (true)
                                    {
                                        var check_table = workSheet.Cell(i, k);
                                        if (check_table.GetValue<String>() == "") break;
                                        tb_m_vendor ExcelObject7 = new tb_m_vendor();

                                        while (true)
                                        {

                                            var cellString = workSheet.Cell(i, k);

                                            if (k == 1) ExcelObject7.vendor_code = cellString.GetValue<string>();
                                            else if (k == 2) ExcelObject7.vendor_name = cellString.GetValue<string>();
                                            else if (k == 3) ExcelObject7.type = cellString.GetValue<string>();
                                            else if (k == 4) ExcelObject7.bank_account = cellString.GetValue<string>();
                                            else if (k == 5) ExcelObject7.account_holder = cellString.GetValue<string>();
                                            else if (k == 6) ExcelObject7.name_of_bank = cellString.GetValue<string>();
                                            else if (k == 7) ExcelObject7.swift_code = cellString.GetValue<string>();
                                            else if (k == 8) ExcelObject7.skn = Convert.ToInt32(cellString.GetValue<string>());
                                            else if (k == 9) ExcelObject7.email = cellString.GetValue<string>();

                                            else if (k == 10) break;

                                            k++;
                                        }
                                        ExcelObject7.user_created = model.no_reg;
                                        ExcelObject7.active_flag = true;
                                        ExcelObject7.start_date = DateTime.Now;

                                        await Utility.Vendor(ExcelObject7);
                                        i++;
                                        k = 1;

                                    }
                                    flag = true;
                                }
                                else if (flag_error)
                                {
                                    ViewBag.errorlist = error_list;
                                    return View("Index", model);
                                }
                            }
                            else if (model.excell_type == "Master Budget")
                            {
                                int k = 1, i = 2;
                                while (true)
                                {
                                    var check_table = workSheet.Cell(i, k);
                                    if (check_table.GetValue<String>() == "")
                                    {
                                        if (i == 2)
                                        {
                                            ViewBag.error_list = "Not Valid Excell or Excell not starting from Collumn 2";
                                            return View("Index", model);
                                        }
                                        break;
                                    }
                                    tb_m_budget ExcelObject9 = new tb_m_budget();


                                    while (true)
                                    {
                                        try
                                        {
                                            var cellString = workSheet.Cell(i, k);
                                            if (cellString.GetValue<string>() == "" && k < 19) throw new System.ArgumentException("There's no Value in this Cell");
                                            else if (k == 1) ExcelObject9.fiscal_year = Convert.ToInt16(cellString.GetValue<string>());
                                            else if (k == 2) ExcelObject9.eoa_no = cellString.GetValue<string>();
                                            else if (k == 3) ExcelObject9.eoa_item_new = cellString.GetValue<string>();
                                            else if (k == 4) ExcelObject9.cost_center = cellString.GetValue<string>();
                                            else if (k == 5) ExcelObject9.wbs_no_new = cellString.GetValue<string>();
                                            else if (k == 6) ExcelObject9.eoa_wbs_no = cellString.GetValue<string>();
                                            else if (k == 7) ExcelObject9.eoa_wbs_description = cellString.GetValue<string>();
                                            else if (k == 8) ExcelObject9.travel_type = (cellString.GetValue<string>());
                                            else if (k == 9) ExcelObject9.available_amount = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 10) ExcelObject9.amount_new = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 11) ExcelObject9.available_status = (cellString.GetValue<string>());
                                            else if (k == 12) ExcelObject9.initial_budget = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 13) ExcelObject9.budget_remining = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 14) ExcelObject9.reduction_option = cellString.GetValue<string>();
                                            else if (k == 15) ExcelObject9.bod_date = cellString.GetValue<string>();
                                            else if (k == 16) ExcelObject9.unit_code = cellString.GetValue<string>();
                                            else if (k == 17) ExcelObject9.division_name = cellString.GetValue<string>();
                                            else if (k == 18) ExcelObject9.flag_budget = cellString.GetValue<string>();
                                            if (k >= 19)
                                            {
                                                if (cellString.GetValue<string>() != "") throw new System.ArgumentException("Invalid Format Excell");
                                                else break;
                                            }


                                        }
                                        catch (Exception ex)
                                        {
                                            flag_error = true;
                                            error = "on row : " + i + ", collumn : " +k + "  " + ex.Message + "\n";
                                            error_list.Add(error);
                                            if (k >= 19) break;
                                        }
                                        k++;
                                    }

                                    i++;
                                    k = 1;
                                }
                                if (!flag_error)
                                {
                                    k = 1;
                                    i = 2;
                                    while (true)
                                    {
                                        var check_table = workSheet.Cell(i, k);
                                        if (check_table.GetValue<String>() == "") break;
                                        tb_m_budget ExcelObject9 = new tb_m_budget();

                                        while (true)
                                        {

                                            var cellString = workSheet.Cell(i, k);
                                            if (k == 1) ExcelObject9.fiscal_year = Convert.ToInt16(cellString.GetValue<string>());
                                            else if (k == 2) ExcelObject9.eoa_no = cellString.GetValue<string>();
                                            else if (k == 3) ExcelObject9.eoa_item_new = cellString.GetValue<string>();
                                            else if (k == 4) ExcelObject9.cost_center = cellString.GetValue<string>();
                                            else if (k == 5) ExcelObject9.wbs_no_new = cellString.GetValue<string>();
                                            else if (k == 6) ExcelObject9.eoa_wbs_no = cellString.GetValue<string>();
                                            else if (k == 7) ExcelObject9.eoa_wbs_description = cellString.GetValue<string>();
                                            else if (k == 8) ExcelObject9.travel_type = (cellString.GetValue<string>());
                                            else if (k == 9) ExcelObject9.available_amount = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 10) ExcelObject9.amount_new = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 11) ExcelObject9.available_status = (cellString.GetValue<string>());
                                            else if (k == 12) ExcelObject9.initial_budget = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 13) ExcelObject9.budget_remining = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 14) ExcelObject9.reduction_option = cellString.GetValue<string>();
                                            else if (k == 15) ExcelObject9.bod_date = cellString.GetValue<string>();
                                            else if (k == 16) ExcelObject9.unit_code = cellString.GetValue<string>();
                                            else if (k == 17) ExcelObject9.division_name = cellString.GetValue<string>();
                                            else if (k == 18) ExcelObject9.flag_budget = cellString.GetValue<string>();
                                            else if (k == 19) break;


                                            k++;
                                        }
                                        // ExcelObject9.create_date = DateTime.Now;
                                        ExcelObject9.active_flag = "1";
                                        ExcelObject9.user_created = model.no_reg;
                                        ExcelObject9.create_date = DateTime.Now;

                                        await Utility.Budget(ExcelObject9);
                                        i++;
                                        k = 1;
                                    }
                                    flag = true;
                                }
                                else if (flag_error)
                                {
                                    ViewBag.errorlist = error_list;
                                    return View("Index", model);
                                }
                            }
                            else if (model.excell_type == "Travel Procedure")
                            {
                                int k = 1, i = 2;
                                while (true)
                                {
                                    var check_table = workSheet.Cell(i, k);
                                    if (check_table.GetValue<String>() == "")
                                    {
                                        if (i == 2)
                                        {
                                            ViewBag.error_list = "Not Valid Excell or Excell not starting from Collumn 2";
                                            return View("Index", model);
                                        }
                                        break;
                                    }
                                    tb_m_travel_procedures ExcelObject10 = new tb_m_travel_procedures();


                                    while (true)
                                    {
                                        try
                                        {
                                            var cellString = workSheet.Cell(i, k);
                                            if (cellString.GetValue<string>() == "" && k < 40) throw new System.ArgumentException("There's no Value in this Cell");
                                            else if (k == 1) ExcelObject10.@class = cellString.GetValue<string>();
                                            else if (k == 2) ExcelObject10.destination_type = cellString.GetValue<string>();
                                            else if (k == 3) ExcelObject10.id_region = Convert.ToInt16(cellString.GetValue<string>());
                                            else if (k == 4) ExcelObject10.apprv_by_lvl1 = cellString.GetValue<string>();
                                            else if (k == 5) ExcelObject10.apprv_by_lvl2 = cellString.GetValue<string>();
                                            else if (k == 6) ExcelObject10.apprv_by_lvl3 = cellString.GetValue<string>();
                                            else if (k == 7) ExcelObject10.apprv_by_lvl4 = cellString.GetValue<string>();
                                            else if (k == 8) ExcelObject10.apprv_by_lvl5 = (cellString.GetValue<string>());
                                            else if (k == 9) ExcelObject10.extend_day = (cellString.GetValue<string>());
                                            else if (k == 10) ExcelObject10.reduction_day = (cellString.GetValue<string>());
                                            else if (k == 11) ExcelObject10.destination_revision = (cellString.GetValue<string>());
                                            else if (k == 12) ExcelObject10.member_revision = (cellString.GetValue<string>());
                                            else if (k == 13) ExcelObject10.diversion_approval_dh = (cellString.GetValue<string>());
                                            else if (k == 14) ExcelObject10.lt_submission = cellString.GetValue<string>();
                                            else if (k == 15) ExcelObject10.lt_submission_visa = cellString.GetValue<string>();
                                            else if (k == 16) ExcelObject10.flight_standard_allowance = cellString.GetValue<string>();
                                            else if (k == 17) ExcelObject10.upgrade_class_req = cellString.GetValue<string>();
                                            else if (k == 18) ExcelObject10.flight_reservation_lt = cellString.GetValue<string>();
                                            else if (k == 19) ExcelObject10.issued_ticket_lt = (cellString.GetValue<string>());
                                            else if (k == 20) ExcelObject10.issued_ticket_req_doc = (cellString.GetValue<string>());
                                            else if (k == 21) ExcelObject10.airlines_priority = (cellString.GetValue<string>());
                                            else if (k == 22) ExcelObject10.airlines_option = (cellString.GetValue<string>());
                                            else if (k == 23) ExcelObject10.hotel_standard_allowance = (cellString.GetValue<string>());
                                            else if (k == 24) ExcelObject10.upgrade_star_hotel = cellString.GetValue<string>();
                                            else if (k == 25) ExcelObject10.room_type = cellString.GetValue<string>();
                                            else if (k == 26) ExcelObject10.hotel_reservation_lt = cellString.GetValue<string>();
                                            else if (k == 27) ExcelObject10.issued_hotel_req_doc = cellString.GetValue<string>();
                                            else if (k == 28) ExcelObject10.meal_allowance = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 29) ExcelObject10.deduction = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 30) ExcelObject10.pre_allowance_1_to_14_day = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 31) ExcelObject10.pre_allowance_15_to_90_day = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 32) ExcelObject10.pre_allowance_90_more = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 33) ExcelObject10.extra_dep_arr = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 34) ExcelObject10.extra_weekend = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 35) ExcelObject10.winter_allowance = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 36) ExcelObject10.cash_lt_submission = cellString.GetValue<string>();
                                            else if (k == 37) ExcelObject10.communication_allowance = cellString.GetValue<string>();
                                            else if (k == 38) ExcelObject10.laundry_allowance = cellString.GetValue<string>();
                                            else if (k == 39) ExcelObject10.land_transport = cellString.GetValue<string>();
                                            if (k >= 40)
                                            {
                                                if (cellString.GetValue<string>() != "") throw new System.ArgumentException("Invalid Format Excell");
                                                else break;
                                            }


                                        }
                                        catch (Exception ex)
                                        {
                                            flag_error = true;
                                            error = "on row : " + i + ", collumn : " +k + "  " + ex.Message + "\n";
                                            error_list.Add(error);
                                            if (k >= 40) break;
                                        }
                                        k++;
                                    }

                                    i++;
                                    k = 1;
                                }
                                if (!flag_error)
                                {
                                    k = 1;
                                    i = 2;
                                    while (true)
                                    {
                                        var check_table = workSheet.Cell(i, k);
                                        if (check_table.GetValue<String>() == "") break;
                                        tb_m_travel_procedures ExcelObject10 = new tb_m_travel_procedures();

                                        while (true)
                                        {
                                            var cellString = workSheet.Cell(i, k);
                                            if (k == 1) ExcelObject10.@class = cellString.GetValue<string>();
                                            else if (k == 2) ExcelObject10.destination_type = cellString.GetValue<string>();
                                            else if (k == 3) ExcelObject10.id_region = Convert.ToInt16(cellString.GetValue<string>());
                                            else if (k == 4) ExcelObject10.apprv_by_lvl1 = cellString.GetValue<string>();
                                            else if (k == 5) ExcelObject10.apprv_by_lvl2 = cellString.GetValue<string>();
                                            else if (k == 6) ExcelObject10.apprv_by_lvl3 = cellString.GetValue<string>();
                                            else if (k == 7) ExcelObject10.apprv_by_lvl4 = cellString.GetValue<string>();
                                            else if (k == 8) ExcelObject10.apprv_by_lvl5 = (cellString.GetValue<string>());
                                            else if (k == 9) ExcelObject10.extend_day = (cellString.GetValue<string>());
                                            else if (k == 10) ExcelObject10.reduction_day = (cellString.GetValue<string>());
                                            else if (k == 11) ExcelObject10.destination_revision = (cellString.GetValue<string>());
                                            else if (k == 12) ExcelObject10.member_revision = (cellString.GetValue<string>());
                                            else if (k == 13) ExcelObject10.diversion_approval_dh = (cellString.GetValue<string>());
                                            else if (k == 14) ExcelObject10.lt_submission = cellString.GetValue<string>();
                                            else if (k == 15) ExcelObject10.lt_submission_visa = cellString.GetValue<string>();
                                            else if (k == 16) ExcelObject10.flight_standard_allowance = cellString.GetValue<string>();
                                            else if (k == 17) ExcelObject10.upgrade_class_req = cellString.GetValue<string>();
                                            else if (k == 18) ExcelObject10.flight_reservation_lt = cellString.GetValue<string>();
                                            else if (k == 19) ExcelObject10.issued_ticket_lt = (cellString.GetValue<string>());
                                            else if (k == 20) ExcelObject10.issued_ticket_req_doc = (cellString.GetValue<string>());
                                            else if (k == 21) ExcelObject10.airlines_priority = (cellString.GetValue<string>());
                                            else if (k == 22) ExcelObject10.airlines_option = (cellString.GetValue<string>());
                                            else if (k == 23) ExcelObject10.hotel_standard_allowance = (cellString.GetValue<string>());
                                            else if (k == 24) ExcelObject10.upgrade_star_hotel = cellString.GetValue<string>();
                                            else if (k == 25) ExcelObject10.room_type = cellString.GetValue<string>();
                                            else if (k == 26) ExcelObject10.hotel_reservation_lt = cellString.GetValue<string>();
                                            else if (k == 27) ExcelObject10.issued_hotel_req_doc = cellString.GetValue<string>();
                                            else if (k == 28) ExcelObject10.meal_allowance = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 29) ExcelObject10.deduction = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 30) ExcelObject10.pre_allowance_1_to_14_day = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 31) ExcelObject10.pre_allowance_15_to_90_day = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 32) ExcelObject10.pre_allowance_90_more = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 33) ExcelObject10.extra_dep_arr = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 34) ExcelObject10.extra_weekend = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 35) ExcelObject10.winter_allowance = Convert.ToDouble(cellString.GetValue<string>());
                                            else if (k == 36) ExcelObject10.cash_lt_submission = cellString.GetValue<string>();
                                            else if (k == 37) ExcelObject10.communication_allowance = cellString.GetValue<string>();
                                            else if (k == 38) ExcelObject10.laundry_allowance = cellString.GetValue<string>();
                                            else if (k == 39) ExcelObject10.land_transport = cellString.GetValue<string>();
                                            if (k >= 40)
                                            {
                                                break;
                                            }


                                            k++;
                                        }
                                        // ExcelObject10.create_date = DateTime.Now;
                                        ExcelObject10.active_flag = true;
                                        ExcelObject10.user_created = model.no_reg;
                                        ExcelObject10.start_date = DateTime.Now;

                                        await Utility.TravelProcedure(ExcelObject10);
                                        i++;
                                        k = 1;
                                    }
                                    flag = true;
                                }
                                else if (flag_error)
                                {
                                    ViewBag.errorlist = error_list;
                                    return View("Index", model);
                                }
                            }
                            else if (model.excell_type == "Organization Structure")
                            {


                                int k = 1, i = 2;
                                while (true)
                                {
                                    var check_table = workSheet.Cell(i, k);
                                    if (check_table.GetValue<String>() == "")
                                    {
                                        if (i == 2)
                                        {
                                            ViewBag.error_list = "Not Valid Excell or Excell not starting from Collumn 2";
                                            return View("Index", model);
                                        }
                                        break;
                                    }
                                    tb_m_employee_structure_organization ExcelObject11 = new tb_m_employee_structure_organization();


                                    while (true)
                                    {
                                        try
                                        {
                                            var cellString = workSheet.Cell(i, k);
                                            if (cellString.GetValue<string>() == "" && k == 1) throw new System.ArgumentException("There's no Value in this Cell");
                                            else if (k == 1) ExcelObject11.division = (cellString.GetValue<string>());
                                            else if (k == 2 && cellString.GetValue<string>() != "") ExcelObject11.division_head = cellString.GetValue<string>();
                                            else if (k == 3 && cellString.GetValue<string>() != "") ExcelObject11.sub_directorate = cellString.GetValue<string>();
                                            else if (k == 4 && cellString.GetValue<string>() != "") ExcelObject11.egm = cellString.GetValue<string>();
                                            else if (k == 5 && cellString.GetValue<string>() != "") ExcelObject11.directorate = cellString.GetValue<string>();
                                            else if (k == 6 && cellString.GetValue<string>() != "") ExcelObject11.directorate_head = cellString.GetValue<string>();
                                            else if (k == 7 && cellString.GetValue<string>() != "") ExcelObject11.fd_local = cellString.GetValue<string>();
                                            else if (k == 8 && cellString.GetValue<string>() != "") ExcelObject11.fd_japan = cellString.GetValue<string>();
                                            else if (k == 9 && cellString.GetValue<string>() != "") ExcelObject11.vp = cellString.GetValue<string>();
                                            else if (k == 10 && cellString.GetValue<string>() != "") ExcelObject11.pd = (cellString.GetValue<string>());
                                            if (k >= 11)
                                            {
                                                if (cellString.GetValue<string>() != "") throw new System.ArgumentException("Invalid Format Excell");
                                                else break;
                                            }


                                        }
                                        catch (Exception ex)
                                        {
                                            flag_error = true;
                                            error = "on row : " + i + ", collumn : " +k + "  " + ex.Message + "\n";
                                            error_list.Add(error);
                                            if (k >= 9) break;
                                        }
                                        k++;
                                    }

                                    i++;
                                    k = 1;
                                }
                                if (!flag_error)
                                {
                                    k = 1;
                                    i = 2;
                                    while (true)
                                    {
                                        var check_table = workSheet.Cell(i, k);
                                        if (check_table.GetValue<String>() == "") break;
                                        tb_m_employee_structure_organization ExcelObject11 = new tb_m_employee_structure_organization();

                                        while (true)
                                        {

                                            var cellString = workSheet.Cell(i, k);
                                            if (k == 1) ExcelObject11.division = (cellString.GetValue<string>());
                                            else if (k == 2 && cellString.GetValue<string>() != "") ExcelObject11.division_head = cellString.GetValue<string>();
                                            else if (k == 3 && cellString.GetValue<string>() != "") ExcelObject11.sub_directorate = cellString.GetValue<string>();
                                            else if (k == 4 && cellString.GetValue<string>() != "") ExcelObject11.egm = cellString.GetValue<string>();
                                            else if (k == 5 && cellString.GetValue<string>() != "") ExcelObject11.directorate = cellString.GetValue<string>();
                                            else if (k == 6 && cellString.GetValue<string>() != "") ExcelObject11.directorate_head = cellString.GetValue<string>();
                                            else if (k == 7 && cellString.GetValue<string>() != "") ExcelObject11.fd_local = cellString.GetValue<string>();
                                            else if (k == 8 && cellString.GetValue<string>() != "") ExcelObject11.fd_japan = cellString.GetValue<string>();
                                            else if (k == 9 && cellString.GetValue<string>() != "") ExcelObject11.vp = cellString.GetValue<string>();
                                            else if (k == 10 && cellString.GetValue<string>() != "") ExcelObject11.pd = (cellString.GetValue<string>());
                                            if (k >= 11)
                                            {
                                                break;
                                            }

                                            k++;
                                        }
                                        // ExcelObject11.create_date = DateTime.Now;
                                        ExcelObject11.active_flag = true;
                                        ExcelObject11.user_created = Convert.ToInt32(model.no_reg);
                                        ExcelObject11.start_date = DateTime.Now;

                                        await Utility.OrganizationStructure(ExcelObject11);
                                        i++;
                                        k = 1;
                                    }
                                    flag = true;
                                }
                                else if (flag_error)
                                {
                                    ViewBag.errorlist = error_list;
                                    return View("Index", model);
                                }
                            }
                            else if (model.excell_type == "Master Budget Source Data")
                            {
                                int k = 1, i = 2;
                                while (true)
                                {
                                    var check_table = workSheet.Cell(i, k);
                                    if (check_table.GetValue<String>() == "")
                                    {
                                        if (i == 2)
                                        {
                                            ViewBag.error_list = "Not Valid Excell or Excell not starting from Collumn 2";
                                            return View("Index", model);
                                        }
                                        break;
                                    }
                                    tb_m_budget_source_data ExcelObject12 = new tb_m_budget_source_data();


                                    while (true)
                                    {
                                        try
                                        {
                                            var cellString = workSheet.Cell(i, k);
                                            if (cellString.GetValue<string>() == "" && k > 4 && k<10) throw new System.ArgumentException("There's no Value in this Cell");
                                            else if (k == 1) ExcelObject12.Division = cellString.GetValue<string>();
                                            else if (k == 2 && cellString.GetValue<string>() != "") ExcelObject12.Employee_Position = cellString.GetValue<string>();
                                            else if (k == 3 && cellString.GetValue<string>() != "") ExcelObject12.Employee_Name = cellString.GetValue<string>();
                                            else if (k == 4 && cellString.GetValue<string>() != "") ExcelObject12.Department = cellString.GetValue<string>();
                                            else if (k == 5) ExcelObject12.Description = cellString.GetValue<string>();
                                            else if (k == 6) ExcelObject12.Travel_Type = cellString.GetValue<string>();
                                            else if (k == 7) ExcelObject12.Cost_Center = cellString.GetValue<string>();
                                            else if (k == 8) ExcelObject12.WBS_Element = (cellString.GetValue<string>());
                                            else if (k == 9) ExcelObject12.Amount = Convert.ToDouble(cellString.GetValue<string>());
                                            if (k >= 10)
                                            {
                                                if (cellString.GetValue<string>() != "") throw new System.ArgumentException("Invalid Format Excell");
                                                else break;
                                            }


                                        }
                                        catch (Exception ex)
                                        {
                                            flag_error = true;
                                            error = "on row : " + i + ", collumn : " +k + "  " + ex.Message + "\n";
                                            error_list.Add(error);
                                            if (k >= 10) break;
                                        }
                                        k++;
                                    }

                                    i++;
                                    k = 1;
                                }
                                if (!flag_error)
                                {
                                    k = 1;
                                    i = 2;
                                    while (true)
                                    {
                                        var check_table = workSheet.Cell(i, k);
                                        if (check_table.GetValue<String>() == "") break;
                                        tb_m_budget_source_data ExcelObject12 = new tb_m_budget_source_data();

                                        while (true)
                                        {

                                            var cellString = workSheet.Cell(i, k);
                                            if (k == 1) ExcelObject12.Division = cellString.GetValue<string>();
                                            else if (k == 2 && cellString.GetValue<string>() != "") ExcelObject12.Employee_Position = cellString.GetValue<string>();
                                            else if (k == 3 && cellString.GetValue<string>() != "") ExcelObject12.Employee_Name = cellString.GetValue<string>();
                                            else if (k == 4 && cellString.GetValue<string>() != "") ExcelObject12.Department = cellString.GetValue<string>();
                                            else if (k == 5) ExcelObject12.Description = cellString.GetValue<string>();
                                            else if (k == 6) ExcelObject12.Travel_Type = cellString.GetValue<string>();
                                            else if (k == 7) ExcelObject12.Cost_Center = cellString.GetValue<string>();
                                            else if (k == 8) ExcelObject12.WBS_Element = (cellString.GetValue<string>());
                                            else if (k == 9) ExcelObject12.Amount = Convert.ToDouble(cellString.GetValue<string>());
                                            if (k >= 10)
                                            {
                                                break;
                                            }


                                            k++;
                                        }
                                        // ExcelObject9.create_date = DateTime.Now;
                                        ExcelObject12.active_flag = true;
                                        ExcelObject12.user_created = Convert.ToInt32(model.no_reg);
                                        ExcelObject12.start_date = DateTime.Now;

                                        await Utility.BudgetSourceData(ExcelObject12);
                                        i++;
                                        k = 1;
                                    }
                                    flag = true;
                                }
                                else if (flag_error)
                                {
                                    ViewBag.errorlist = error_list;
                                    return View("Index", model);
                                }
                            }
                            if (flag)
                            {
                                tb_r_statusUploadMaster upload = new tb_r_statusUploadMaster();
                                upload.DATE = DateTime.Now;
                                upload.EXCEL_NAME = model.excell_type;
                                upload.STATUS_UPLOAD = true;

                                await InsertData.StatusUpload(upload);

                                ViewBag.done = "Upload " + model.excell_type + " Succeed";
                            }
                        }
                    }
                }
            }
            else if (submit == "" && download == "Submit")
            {
                XLWorkbook CreateExcell = new XLWorkbook();
                if (model.excell_type == "Master Rate Hotel")
                {
                    var InsertData = CreateExcell.Worksheets.Add("Current Rate Hotel Data");
                    List<tb_m_rate_hotel> dbObject1 = await GetData.RateHotelList();


                    InsertData.Cell(1, 1).Value = "Class";
                    InsertData.Cell(1, 2).Value = "Star(s)";
                    InsertData.Cell(1, 3).Value = "Domestic";
                    InsertData.Cell(1, 4).Value = "OverSeas";

                    for (int i = 0; i < dbObject1.Count(); i++)
                    {
                        InsertData.Cell(i + 2, 1).Value = dbObject1[i].kelas;
                        InsertData.Cell(i + 2, 2).Value = dbObject1[i].bintang;
                        InsertData.Cell(i + 2, 3).Value = dbObject1[i].domestik;
                        InsertData.Cell(i + 2, 4).Value = dbObject1[i].overseas;
                    }
                    MemoryStream excelStream = new MemoryStream();
                    CreateExcell.SaveAs(excelStream);
                    excelStream.Position = 0;
                    return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Current Rate Hotel.xlsx");

                }
                else if (model.excell_type == "Master Rate Flight")
                {
                    var InsertData = CreateExcell.Worksheets.Add("Current Rate Flight Data");
                    List<tb_m_rate_flight> dbObject1 = await GetData.RateFlightList();

                    InsertData.Cell(1, 1).Value = "Destination";
                    InsertData.Cell(1, 2).Value = "Region";
                    InsertData.Cell(1, 3).Value = "Economy";
                    InsertData.Cell(1, 4).Value = "Business";
                    InsertData.Cell(1, 8).Value = "Region Flag";
                    InsertData.Cell(1, 9).Value = "Region Meaning";

                    InsertData.Cell(2, 8).Value = "0";
                    InsertData.Cell(3, 8).Value = "1";
                    InsertData.Cell(4, 8).Value = "2";
                    InsertData.Cell(5, 8).Value = "3";
                    InsertData.Cell(6, 8).Value = "4";

                    InsertData.Cell(2, 9).Value = "Destinasi Tidak Terdaftar";
                    InsertData.Cell(3, 9).Value = "Jepang";
                    InsertData.Cell(4, 9).Value = "Asia";
                    InsertData.Cell(5, 9).Value = "Eropa";
                    InsertData.Cell(6, 9).Value = "Domestik";

                    List<tb_m_destination> region_list = await GetData.DestinationActive();
                    for (int i = 0; i < dbObject1.Count(); i++)
                    {
                        tb_m_destination region = new tb_m_destination();
                        int value = 0;
                        try
                        {
                            region = region_list.Where(b => b.destination_name.Contains(dbObject1[i].destination)).First();
                            value = Convert.ToInt32(region.id_region);
                        }
                        catch (Exception x)
                        {
                            value = 0;
                        }
                        InsertData.Cell(i + 2, 1).Value = dbObject1[i].destination;
                        InsertData.Cell(i + 2, 2).Value = value;
                        InsertData.Cell(i + 2, 3).Value = dbObject1[i].economy;
                        InsertData.Cell(i + 2, 4).Value = dbObject1[i].business;
                    }
                    MemoryStream excelStream = new MemoryStream();
                    CreateExcell.SaveAs(excelStream);
                    excelStream.Position = 0;
                    return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Current Rate Flight.xlsx");

                }
                else if (model.excell_type == "Master Rate Visa")
                {
                    var InsertData = CreateExcell.Worksheets.Add("Current Rate Visa Data");
                    List<tb_m_rate_visa> dbObject1 = await GetData.RateVisaList();

                    InsertData.Cell(1, 1).Value = "Destination";
                    InsertData.Cell(1, 2).Value = "Visa Rate";

                    for (int i = 0; i < dbObject1.Count(); i++)
                    {
                        InsertData.Cell(i + 2, 1).Value = dbObject1[i].destination;
                        InsertData.Cell(i + 2, 2).Value = dbObject1[i].visa;
                    }
                    MemoryStream excelStream = new MemoryStream();
                    CreateExcell.SaveAs(excelStream);
                    excelStream.Position = 0;
                    return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Current Rate Visa.xlsx");

                }
                else if (model.excell_type == "Master Rate Passport")
                {
                    var InsertData = CreateExcell.Worksheets.Add("Current Rate Passport Data");
                    List<tb_m_rate_passport> dbObject1 = await GetData.RatePassportList();

                    InsertData.Cell(1, 1).Value = "Passport Type";
                    InsertData.Cell(1, 2).Value = "Rate Passport";

                    for (int i = 0; i < dbObject1.Count(); i++)
                    {
                        InsertData.Cell(i + 2, 1).Value = dbObject1[i].passport_type;
                        InsertData.Cell(i + 2, 2).Value = dbObject1[i].rate_passport;
                    }
                    MemoryStream excelStream = new MemoryStream();
                    CreateExcell.SaveAs(excelStream);
                    excelStream.Position = 0;
                    return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Current Rate Passport.xlsx");

                }
                else if (model.excell_type == "Master Vendor Employee")
                {
                    var InsertData = CreateExcell.Worksheets.Add("Current Vendor Employee Data");
                    List<tb_m_vendor_employee> dbObject1 = await GetData.VendorEmployeeList();

                    InsertData.Cell(1, 1).Value = "Employee Code";
                    InsertData.Cell(1, 2).Value = "Vendor Code";
                    InsertData.Cell(1, 3).Value = "Bank Number";
                    InsertData.Cell(1, 4).Value = "Bank Type";
                    InsertData.Cell(1, 5).Value = "Bank Name";
                    InsertData.Cell(1, 6).Value = "Bank Account";
                    InsertData.Cell(1, 7).Value = "Employee Name";
                    InsertData.Cell(1, 8).Value = "Beneficiary Name";


                    for (int i = 0; i < dbObject1.Count(); i++)
                    {
                        InsertData.Cell(i + 2, 1).Value = dbObject1[i].no_reg;
                        InsertData.Cell(i + 2, 2).Value = dbObject1[i].vendor_code_employee;
                        InsertData.Cell(i + 2, 3).Value = dbObject1[i].bank_number;
                        InsertData.Cell(i + 2, 4).Value = dbObject1[i].bank_type;
                        InsertData.Cell(i + 2, 5).Value = dbObject1[i].Bank_Name;
                        InsertData.Cell(i + 2, 6).Value = dbObject1[i].account_number;
                        InsertData.Cell(i + 2, 7).Value = dbObject1[i].employee_name;
                        InsertData.Cell(i + 2, 8).Value = dbObject1[i].beneficiary_name;
                    }
                    MemoryStream excelStream = new MemoryStream();
                    CreateExcell.SaveAs(excelStream);
                    excelStream.Position = 0;
                    return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Current Vendor Employee.xlsx");

                }
                else if (model.excell_type == "Master Vendor Travel")
                {
                    var InsertData = CreateExcell.Worksheets.Add("Current Vendor Travel Data");
                    List<tb_m_vendor> dbObject1 = await GetData.VendorTravelList();

                    InsertData.Cell(1, 1).Value = "Vendor Code";
                    InsertData.Cell(1, 2).Value = "Vendor Name";
                    InsertData.Cell(1, 3).Value = "Vendor Type";
                    InsertData.Cell(1, 4).Value = "Bank Account";
                    InsertData.Cell(1, 5).Value = "Account Holder";
                    InsertData.Cell(1, 6).Value = "Name of Bank";
                    InsertData.Cell(1, 7).Value = "Swift Code";
                    InsertData.Cell(1, 8).Value = "SKN";
                    InsertData.Cell(1, 9).Value = "E-Mail";


                    for (int i = 0; i < dbObject1.Count(); i++)
                    {
                        InsertData.Cell(i + 2, 1).Value = dbObject1[i].vendor_code;
                        InsertData.Cell(i + 2, 2).Value = dbObject1[i].vendor_name;
                        InsertData.Cell(i + 2, 3).Value = dbObject1[i].type;
                        InsertData.Cell(i + 2, 4).Value = dbObject1[i].bank_account;
                        InsertData.Cell(i + 2, 5).Value = dbObject1[i].account_holder;
                        InsertData.Cell(i + 2, 6).Value = dbObject1[i].name_of_bank;
                        InsertData.Cell(i + 2, 7).Value = dbObject1[i].swift_code;
                        InsertData.Cell(i + 2, 8).Value = dbObject1[i].skn;
                        InsertData.Cell(i + 2, 9).Value = dbObject1[i].email;
                    }
                    MemoryStream excelStream = new MemoryStream();
                    CreateExcell.SaveAs(excelStream);
                    excelStream.Position = 0;
                    return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Current Vendor Travel.xlsx");

                }
                else if (model.excell_type == "Master Budget")
                {
                    var InsertData = CreateExcell.Worksheets.Add("Current Budget Data");
                    List<tb_m_budget> dbObject1 = await GetData.BudgetList();

                    InsertData.Cell(1, 1).Value = "Fiscal Year";
                    InsertData.Cell(1, 2).Value = "EOA No";
                    InsertData.Cell(1, 3).Value = "EOA Item New";
                    InsertData.Cell(1, 4).Value = "Cost Center";
                    InsertData.Cell(1, 5).Value = "WBS No New";
                    InsertData.Cell(1, 6).Value = "EOA WBS No";
                    InsertData.Cell(1, 7).Value = "EOA WBS Description";
                    InsertData.Cell(1, 8).Value = "Travel Type";
                    InsertData.Cell(1, 9).Value = "Available Amount";
                    InsertData.Cell(1, 10).Value = "Amount New";
                    InsertData.Cell(1, 11).Value = "Available Status";
                    InsertData.Cell(1, 12).Value = "Initial Budget";
                    InsertData.Cell(1, 13).Value = "Remaining Budget";
                    InsertData.Cell(1, 14).Value = "Reduction Option";
                    InsertData.Cell(1, 15).Value = "BOD Date";
                    InsertData.Cell(1, 16).Value = "Unit Code";
                    InsertData.Cell(1, 17).Value = "Division Name";


                    for (int i = 0; i < dbObject1.Count(); i++)
                    {
                        InsertData.Cell(i + 2, 1).Value = dbObject1[i].fiscal_year;
                        InsertData.Cell(i + 2, 2).Value = dbObject1[i].eoa_no;
                        InsertData.Cell(i + 2, 3).Value = dbObject1[i].eoa_item_new;
                        InsertData.Cell(i + 2, 4).Value = dbObject1[i].cost_center;
                        InsertData.Cell(i + 2, 5).Value = dbObject1[i].wbs_no_new;
                        InsertData.Cell(i + 2, 6).Value = dbObject1[i].eoa_wbs_no;
                        InsertData.Cell(i + 2, 7).Value = dbObject1[i].eoa_wbs_description;
                        InsertData.Cell(i + 2, 8).Value = dbObject1[i].travel_type;
                        InsertData.Cell(i + 2, 9).Value = dbObject1[i].available_amount;
                        InsertData.Cell(i + 2, 10).Value = dbObject1[i].amount_new;
                        InsertData.Cell(i + 2, 11).Value = dbObject1[i].available_status;
                        InsertData.Cell(i + 2, 12).Value = dbObject1[i].initial_budget;
                        InsertData.Cell(i + 2, 13).Value = dbObject1[i].budget_remining;
                        InsertData.Cell(i + 2, 14).Value = dbObject1[i].reduction_option;
                        InsertData.Cell(i + 2, 15).Value = dbObject1[i].bod_date;
                        InsertData.Cell(i + 2, 16).Value = dbObject1[i].unit_code;
                        InsertData.Cell(i + 2, 17).Value = dbObject1[i].division_name;
                    }
                    MemoryStream excelStream = new MemoryStream();
                    CreateExcell.SaveAs(excelStream);
                    excelStream.Position = 0;
                    return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Budget Source Data.xlsx");

                }
                else if (model.excell_type == "Travel Procedure")
                {
                    var InsertData = CreateExcell.Worksheets.Add("Current Travel Procedures Data");
                    List<tb_m_travel_procedures> dbObject1 = await GetData.TravelProcedureList();

                    InsertData.Cell(1, 1).Value = "Class";
                    InsertData.Cell(1, 2).Value = "Destination Type";
                    InsertData.Cell(1, 3).Value = "Region ID";
                    InsertData.Cell(1, 4).Value = "Approval Lv 1";
                    InsertData.Cell(1, 5).Value = "Approval Lv 2";
                    InsertData.Cell(1, 6).Value = "Approval Lv 3";
                    InsertData.Cell(1, 7).Value = "Approval Lv 4";
                    InsertData.Cell(1, 8).Value = "Approval Lv 5";
                    InsertData.Cell(1, 9).Value = "Extend Day";
                    InsertData.Cell(1, 10).Value = "Reduction Day";
                    InsertData.Cell(1, 11).Value = "Destination Revision";
                    InsertData.Cell(1, 12).Value = "Member Revision";
                    InsertData.Cell(1, 13).Value = "Diversion Approval DH";
                    InsertData.Cell(1, 14).Value = "Lead Time Submission";
                    InsertData.Cell(1, 15).Value = "Lead Time Visa Submission";
                    InsertData.Cell(1, 16).Value = "Standart Flight Allowance";
                    InsertData.Cell(1, 17).Value = "Upgrade Class Request";
                    InsertData.Cell(1, 18).Value = "Flight Reservation Lead Time";
                    InsertData.Cell(1, 19).Value = "Issued Ticket Lead Time";
                    InsertData.Cell(1, 20).Value = "Issued Ticket Req Doc";
                    InsertData.Cell(1, 21).Value = "Airlines Priority";
                    InsertData.Cell(1, 22).Value = "Airline Option";
                    InsertData.Cell(1, 23).Value = "Standart Hotel Allowance";
                    InsertData.Cell(1, 24).Value = "Upgrade Star Hotel";
                    InsertData.Cell(1, 25).Value = "Room Type";
                    InsertData.Cell(1, 26).Value = "Hotel Reservation Lead Time";
                    InsertData.Cell(1, 27).Value = "Issued Hotel Req Doc";
                    InsertData.Cell(1, 28).Value = "Meal Allowance";
                    InsertData.Cell(1, 29).Value = "Deduction";
                    InsertData.Cell(1, 30).Value = "Pre Allowance 1-14 days";
                    InsertData.Cell(1, 31).Value = "Pre Allowance 15-90 days";
                    InsertData.Cell(1, 32).Value = "Pre Allowance >90 days";
                    InsertData.Cell(1, 33).Value = "Extra Dep Arr";
                    InsertData.Cell(1, 34).Value = "Extra Weekend";
                    InsertData.Cell(1, 35).Value = "Winter Allowance";
                    InsertData.Cell(1, 36).Value = "Cash Lead Time Submission";
                    InsertData.Cell(1, 37).Value = "Communication Allowance";
                    InsertData.Cell(1, 38).Value = "Laundry Allowance";
                    InsertData.Cell(1, 39).Value = "Land Transport";

                    for (int i = 0; i < dbObject1.Count(); i++)
                    {
                        InsertData.Cell(i + 2, 1).Value = dbObject1[i].@class;
                        InsertData.Cell(i + 2, 2).Value = dbObject1[i].destination_type;
                        InsertData.Cell(i + 2, 3).Value = dbObject1[i].id_region;
                        InsertData.Cell(i + 2, 4).Value = dbObject1[i].apprv_by_lvl1;
                        InsertData.Cell(i + 2, 5).Value = dbObject1[i].apprv_by_lvl2;
                        InsertData.Cell(i + 2, 6).Value = dbObject1[i].apprv_by_lvl3;
                        InsertData.Cell(i + 2, 7).Value = dbObject1[i].apprv_by_lvl4;
                        InsertData.Cell(i + 2, 8).Value = dbObject1[i].apprv_by_lvl5;
                        InsertData.Cell(i + 2, 9).Value = dbObject1[i].extend_day;
                        InsertData.Cell(i + 2, 10).Value = dbObject1[i].reduction_day;
                        InsertData.Cell(i + 2, 11).Value = dbObject1[i].destination_revision;
                        InsertData.Cell(i + 2, 12).Value = dbObject1[i].member_revision;
                        InsertData.Cell(i + 2, 13).Value = dbObject1[i].diversion_approval_dh;
                        InsertData.Cell(i + 2, 14).Value = dbObject1[i].lt_submission;
                        InsertData.Cell(i + 2, 15).Value = dbObject1[i].lt_submission_visa;
                        InsertData.Cell(i + 2, 16).Value = dbObject1[i].flight_standard_allowance;
                        InsertData.Cell(i + 2, 17).Value = dbObject1[i].upgrade_class_req;
                        InsertData.Cell(i + 2, 18).Value = dbObject1[i].flight_reservation_lt;
                        InsertData.Cell(i + 2, 19).Value = dbObject1[i].issued_ticket_lt;
                        InsertData.Cell(i + 2, 20).Value = dbObject1[i].issued_ticket_req_doc;
                        InsertData.Cell(i + 2, 21).Value = dbObject1[i].airlines_priority;
                        InsertData.Cell(i + 2, 22).Value = dbObject1[i].airlines_option;
                        InsertData.Cell(i + 2, 23).Value = dbObject1[i].hotel_standard_allowance;
                        InsertData.Cell(i + 2, 24).Value = dbObject1[i].upgrade_star_hotel;
                        InsertData.Cell(i + 2, 25).Value = dbObject1[i].room_type;
                        InsertData.Cell(i + 2, 26).Value = dbObject1[i].hotel_reservation_lt;
                        InsertData.Cell(i + 2, 27).Value = dbObject1[i].issued_hotel_req_doc;
                        InsertData.Cell(i + 2, 28).Value = dbObject1[i].meal_allowance;
                        InsertData.Cell(i + 2, 29).Value = dbObject1[i].deduction;
                        InsertData.Cell(i + 2, 30).Value = dbObject1[i].pre_allowance_1_to_14_day;
                        InsertData.Cell(i + 2, 31).Value = dbObject1[i].pre_allowance_15_to_90_day;
                        InsertData.Cell(i + 2, 32).Value = dbObject1[i].pre_allowance_90_more;
                        InsertData.Cell(i + 2, 33).Value = dbObject1[i].extra_dep_arr;
                        InsertData.Cell(i + 2, 34).Value = dbObject1[i].extra_weekend;
                        InsertData.Cell(i + 2, 35).Value = dbObject1[i].winter_allowance;
                        InsertData.Cell(i + 2, 36).Value = dbObject1[i].cash_lt_submission;
                        InsertData.Cell(i + 2, 37).Value = dbObject1[i].communication_allowance;
                        InsertData.Cell(i + 2, 38).Value = dbObject1[i].laundry_allowance;
                        InsertData.Cell(i + 2, 39).Value = dbObject1[i].land_transport;
                    }
                    MemoryStream excelStream = new MemoryStream();
                    CreateExcell.SaveAs(excelStream);
                    excelStream.Position = 0;
                    return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Current Travel Procedures.xlsx");

                }
                else if (model.excell_type == "Organization Structure")
                {
                    var InsertData = CreateExcell.Worksheets.Add("Organization Structure Data");
                    List<tb_m_employee_structure_organization> dbObject1 = await GetData.EmployeeStructureOrganizationList();

                    InsertData.Cell(1, 1).Value = "Division";
                    InsertData.Cell(1, 2).Value = "Division Head";
                    InsertData.Cell(1, 3).Value = "Sub Directorate";
                    InsertData.Cell(1, 4).Value = "EGM";
                    InsertData.Cell(1, 5).Value = "Directorate";
                    InsertData.Cell(1, 6).Value = "Directorate Head";
                    InsertData.Cell(1, 7).Value = "FD Local";
                    InsertData.Cell(1, 8).Value = "FD Japan";
                    InsertData.Cell(1, 9).Value = "VP";
                    InsertData.Cell(1, 10).Value = "PD";


                    for (int i = 0; i < dbObject1.Count(); i++)
                    {
                        InsertData.Cell(i + 2, 1).Value = dbObject1[i].division;
                        InsertData.Cell(i + 2, 2).Value = dbObject1[i].division_head;
                        InsertData.Cell(i + 2, 3).Value = dbObject1[i].sub_directorate;
                        InsertData.Cell(i + 2, 4).Value = dbObject1[i].egm;
                        InsertData.Cell(i + 2, 5).Value = dbObject1[i].directorate;
                        InsertData.Cell(i + 2, 6).Value = dbObject1[i].directorate_head;
                        InsertData.Cell(i + 2, 7).Value = dbObject1[i].fd_local;
                        InsertData.Cell(i + 2, 8).Value = dbObject1[i].fd_japan;
                        InsertData.Cell(i + 2, 9).Value = dbObject1[i].vp;
                        InsertData.Cell(i + 2, 10).Value = dbObject1[i].pd;
                    }
                    MemoryStream excelStream = new MemoryStream();
                    CreateExcell.SaveAs(excelStream);
                    excelStream.Position = 0;
                    return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Current Organization Structure.xlsx");

                }
                else if (model.excell_type == "Master Budget Source Data")
                {
                    var InsertData = CreateExcell.Worksheets.Add("Current Budget Data");
                    List<tb_m_budget_source_data> dbObject1 = await GetData.BudgetSourceDataList();

                    InsertData.Cell(1, 1).Value = "Division";
                    InsertData.Cell(1, 2).Value = "Employee Position";
                    InsertData.Cell(1, 3).Value = "Employee Name";
                    InsertData.Cell(1, 4).Value = "Department";
                    InsertData.Cell(1, 5).Value = "Description";
                    InsertData.Cell(1, 6).Value = "Travel Type";
                    InsertData.Cell(1, 7).Value = "Cost Center";
                    InsertData.Cell(1, 8).Value = "WBS Element";
                    InsertData.Cell(1, 9).Value = "Amount";


                    for (int i = 0; i < dbObject1.Count(); i++)
                    {
                        InsertData.Cell(i + 2, 1).Value = dbObject1[i].Division;
                        InsertData.Cell(i + 2, 2).Value = dbObject1[i].Employee_Position;
                        InsertData.Cell(i + 2, 3).Value = dbObject1[i].Employee_Name;
                        InsertData.Cell(i + 2, 4).Value = dbObject1[i].Department;
                        InsertData.Cell(i + 2, 5).Value = dbObject1[i].Description;
                        InsertData.Cell(i + 2, 6).Value = dbObject1[i].Travel_Type;
                        InsertData.Cell(i + 2, 7).Value = dbObject1[i].Cost_Center;
                        InsertData.Cell(i + 2, 8).Value = dbObject1[i].WBS_Element;
                        InsertData.Cell(i + 2, 9).Value = dbObject1[i].Amount;
                    }
                    MemoryStream excelStream = new MemoryStream();
                    CreateExcell.SaveAs(excelStream);
                    excelStream.Position = 0;
                    return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Current Budget Source Data.xlsx");

                }

            }
            else if (division_input == "Submit")
            {
                XLWorkbook CreateExcell = new XLWorkbook();
                var InsertData = CreateExcell.Worksheets.Add("Current Division Data");
                List<string> dbObject1 = await GetData.DivisionNameList();



                InsertData.Cell(1, 1).Value = "Division Name";

                for (int i = 0; i < dbObject1.Count(); i++)
                {
                    InsertData.Cell(i + 2, 1).Value = dbObject1[i].Replace("  ", "");

                }
                MemoryStream excelStream = new MemoryStream();

                CreateExcell.SaveAs(excelStream);
                excelStream.Position = 0;
                return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Current Division.xlsx");

            }
            return View("Index", model);
        }
    }
}
