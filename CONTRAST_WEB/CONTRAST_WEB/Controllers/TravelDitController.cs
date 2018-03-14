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
using ClosedXML.Excel;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Hosting;
using System.Security.Claims;

namespace CONTRAST_WEB.Controllers
{
    public class TravelDitController : Controller
    {
        private CONTRASTEntities db = new CONTRASTEntities();

        // GET: TravelDit
        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(tb_m_employee model)
        {
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DetailsView(TravelDitHelper model)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();

            tb_m_employee login_id = await GetData.EmployeeInfo(identity.Name);
            List<tb_m_employee> div_employees = await GetData.EmployeeDivision(login_id.unit_code_code);

            List<tb_r_travel_dit> RequestObject = new List<tb_r_travel_dit>();
            List<tb_r_travel_dit> RequestObjectTemp = new List<tb_r_travel_dit>();
            List<string> name = new List<string>();
            List<int> unit_code = new List<int>();
            RequestObjectTemp = await GetData.TravelDitList();


            for (int k = 0; k < RequestObjectTemp.Count(); k++)
            {
                for (int i = 0; i < div_employees.Count(); i++)
                {
                    if (RequestObjectTemp[k].no_reg==Convert.ToInt32(div_employees[i].code))
                    {
                        RequestObject.Add(RequestObjectTemp[k]);
                        break;
                    }
                }
            }

            tb_m_employee info = new tb_m_employee();
            foreach (var item in RequestObject)
            {
                info = await GetData.EmployeeInfo(item.no_reg.ToString());
                name.Add(info.name);
                unit_code.Add(Convert.ToInt32(info.unit_code_id));
            }
            ViewBag.name = name;
            ViewBag.unit_code = unit_code;
            return View(RequestObject);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<TravelDitHelper> CompleteData(TravelDitHelper model)
        {

            using (var client = new HttpClient())
            {
                vw_travel_allowance EmpInfo = new vw_travel_allowance();
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/TravelAllowance?rank=" + model.employee_info.@class + "&reg=" + model.travel_dit.destination_code + "&dt=" + ((model.travel_dit.overseas_flag == true) ? 1 : 0).ToString());

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    EmpInfo = JsonConvert.DeserializeObject<vw_travel_allowance>(EmpResponse);

                }
                model.travel_dit.allowance_meal_idr = EmpInfo.meal_allowance * model.travel_dit.periode;

                // cek winter gak?

                if (model.travel_dit.start_date.Value.Month == 12 || model.travel_dit.start_date.Value.Month == 1 || model.travel_dit.start_date.Value.Month == 0)
                {
                    model.travel_dit.allowance_winter = (int)EmpInfo.winter_allowance;
                }
                else
                    model.travel_dit.allowance_winter = 0;                
            }

            using (var client = new HttpClient())
            {
                tb_m_rate_flight EmpInfo = new tb_m_rate_flight();
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/RateFlight/" + model.travel_dit.id_destination_city);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    EmpInfo = JsonConvert.DeserializeObject<tb_m_rate_flight>(EmpResponse);
                }
                model.travel_dit.allowance_ticket = (EmpInfo.economy) * 2;
            }

            using (var client = new HttpClient())
            {
                tb_m_rate_hotel EmpInfo = new tb_m_rate_hotel();
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/RateHotel?rank=" + model.employee_info.@class);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    EmpInfo = JsonConvert.DeserializeObject<tb_m_rate_hotel>(EmpResponse);
                }
                if (model.travel_dit.overseas_flag == true) model.travel_dit.allowance_hotel = EmpInfo.overseas * model.travel_dit.periode;
                else
                    model.travel_dit.allowance_hotel = EmpInfo.domestik * model.travel_dit.periode;
            }

            return model;
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task InsertData(TravelDitHelper model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage response = await client.PostAsJsonAsync("api/TravelDit", model.travel_dit);

                HttpResponseMessage response = await client.PostAsync("api/TravelDit", new StringContent(
                                new JavaScriptSerializer().Serialize(model.travel_dit), Encoding.UTF8, "application/json"));
            }
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(tb_m_employee model)
        {
            List<string> error = new List<string>();
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if (file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    error.Add("Wrong file type inserted, please enter xlsx file only");
                    ViewBag.error_list = error;
                    return View("Index", model);
                }
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName) && file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    string fileContentType = file.ContentType;
                    String string3 = "";
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                    bool flag = false, flag_error = false;

                    List<TravelDitHelper> TravelDit = new List<TravelDitHelper>();
                    List<TravelDitHelper> test = new List<TravelDitHelper>();
                    //Open the Excel file using ClosedXML.
                    using (XLWorkbook workBook = new XLWorkbook(file.InputStream))
                    {
                        //Read the first Sheet from Excel file.
                        IXLWorksheet workSheet = workBook.Worksheet(1);
                        int i = 17, k = 2;

                        string fileName = file.FileName;

                        while (true)
                        {
                            var cellStringc = workSheet.Cell(i, k);
                            if (cellStringc.GetValue<String>() == "")
                            {
                                if (i == 17 && k == 2)
                                {
                                    error.Add("Wrong format template file, please use correct template file");
                                    ViewBag.error_list = error;
                                }
                                break;
                            }
                            test.Add(new TravelDitHelper());
                            test[i - 17].travel_dit = new tb_r_travel_dit();
                            test[i - 17].employee_info = new tb_m_employee();
                            test[i - 17].employee_info = model;

                            //test[i-17].travel_dit.Add(new tb_r_travel_dit());
                            while (true)
                            {
                                var cellString = workSheet.Cell(i, k);
                                string3 = cellString.GetValue<String>();
                                try
                                {
                                    if (cellString.GetValue<string>() == "" && k < 9) throw new System.ArgumentException("There's no Value in this Cell");

                                    else if (k == 2) test[i - 17].travel_dit.no_reg = Convert.ToInt32(string3);
                                    else if (k == 3) test[i - 17].travel_dit.travel_month = Convert.ToInt32(string3);
                                    else if (k == 4) test[i - 17].travel_dit.travel_purpose = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(string3);

                                    else if (k == 5) test[i - 17].travel_dit.periode = Convert.ToInt32(string3);
                                    else if (k == 6) test[i - 17].travel_dit.id_destination_city = Convert.ToInt32(string3);
                                    else if (k == 7) test[i - 17].travel_dit.destination_code = string3;
                                    else if (k == 8) test[i - 17].travel_dit.destination_city = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(string3);


                                    if (k == 9)
                                    {
                                        if (string3 == "")
                                        {
                                            test[i - 17].travel_dit.active_flag = true;
                                            test[i - 17].travel_dit.user_created = model.code;
                                            test[i - 17].travel_dit.create_date = DateTime.Now;
                                            test[i - 17].travel_dit.comments = "Comments";
                                            test[i - 17].travel_dit.overseas_flag = false;
                                            test[i - 17].travel_dit.end_date = Convert.ToDateTime("05/05/2005");
                                            test[i - 17].travel_dit.start_date = Convert.ToDateTime("05/05/2005");
                                            break;
                                            //test[i - 17].travel_dit.room_night = (test[i - 17].travel_dit.end_date - test[i - 17].travel_dit.start_date).Value.Days;
                                            //test[i - 17].travel_dit.periode = test[i - 17].travel_dit.room_night + 1;

                                            // TimeSpan duration = ((DateTime)model.travel_request.end_date - (DateTime)model.travel_request.start_date);
                                        }
                                        else throw new System.ArgumentException("Invalid Excell Format");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string error_temp = "in Row: " + i + ", Collumn: " + k + ":" + ex.Message;
                                    error.Add(error_temp);
                                    flag_error = true;
                                    if (k >= 9) break;
                                }
                                k++;
                            }
                            k = 2;
                            i++;
                        }
                        k = 2;
                        i = 17;
                        if (flag_error)
                        {
                            ViewBag.error_list = error;
                            return View("Index", model);
                        }
                        else if (!flag_error)
                        {
                            while (true)
                            {
                                var cellStringc = workSheet.Cell(i, k);
                                if (cellStringc.GetValue<String>() == "") break;
                                TravelDit.Add(new TravelDitHelper());
                                TravelDit[i - 17].travel_dit = new tb_r_travel_dit();
                                TravelDit[i - 17].employee_info = new tb_m_employee();
                                TravelDit[i - 17].employee_info = model;

                                //travelDit[i-17].travel_dit.Add(new tb_r_travel_dit());
                                while (true)
                                {
                                    var cellString = workSheet.Cell(i, k);
                                    string3 = cellString.GetValue<String>();

                                    if (k == 2) TravelDit[i - 17].travel_dit.no_reg = Convert.ToInt32(string3);
                                    else if (k == 3) TravelDit[i - 17].travel_dit.travel_month = Convert.ToInt32(string3);
                                    else if (k == 4) TravelDit[i - 17].travel_dit.travel_purpose = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(string3);
                                    else if (k == 5) TravelDit[i - 17].travel_dit.periode = Convert.ToInt32(string3);
                                    else if (k == 6) TravelDit[i - 17].travel_dit.id_destination_city = Convert.ToInt32(string3);
                                    else if (k == 7) TravelDit[i - 17].travel_dit.destination_code = string3;
                                    else if (k == 8) TravelDit[i - 17].travel_dit.destination_city = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(string3);
                                    if (k == 9)
                                    {
                                        TravelDit[i - 17].travel_dit.active_flag = true;
                                        TravelDit[i - 17].travel_dit.user_created = model.code;
                                        TravelDit[i - 17].travel_dit.create_date = DateTime.Now;
                                        TravelDit[i - 17].travel_dit.comments = "Comments";
                                        TravelDit[i - 17].travel_dit.overseas_flag = false;
                                        TravelDit[i - 17].travel_dit.end_date = Convert.ToDateTime("05/05/2005");
                                        TravelDit[i - 17].travel_dit.start_date = Convert.ToDateTime("05/05/2005");
                                        break;
                                        //TravelDit[i - 17].travel_dit.room_night = (TravelDit[i - 17].travel_dit.end_date - TravelDit[i - 17].travel_dit.start_date).Value.Days;
                                        //TravelDit[i - 17].travel_dit.periode = TravelDit[i - 17].travel_dit.room_night + 1;

                                        // TimeSpan duration = ((DateTime)model.travel_request.end_date - (DateTime)model.travel_request.start_date);
                                    }
                                    k++;
                                }
                                k = 2;
                                i++;
                                flag = true;
                            }

                        }
                    }
                    if (flag)
                    {
                        ViewBag.Done = "Upload Succeed!!";
                        for (int i = 0; i < TravelDit.Count(); i++)
                        {
                            TravelDit[i] = await this.CompleteData(TravelDit[i]);
                            await this.InsertData(TravelDit[i]);
                        }
                    }
                }
            }
            return View("Index", model);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public ActionResult Download()
        {
            string file = HostingEnvironment.MapPath("~/Files/DIT_DOMESTIC_TEMPLATE.xlsx");
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(file, contentType, Path.GetFileName(file));
        }

        //[HttpPost]
        //[Authorize]
        //[Authorize(Roles = "contrast.user")]
        //[ValidateAntiForgeryToken]
        // GET: TravelDit/Delete/5
        public async Task<ActionResult> Delete(List<tb_r_travel_dit> model,string delete="")
        {
            
            
            tb_r_travel_dit tb_r_travel_dit = await GetData.TravelDitInfo(model[Convert.ToInt32(delete)].id_dit);


            if (tb_r_travel_dit == null)
            {
                return HttpNotFound();
            }
            return View(tb_r_travel_dit);
        }

        //[HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        //// POST: TravelDit/Delete/5
        //[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(tb_r_travel_dit model)
        {
            await UpdateData.TravelDitDel(model.id_dit);
            List<tb_r_travel_dit> RequestObject = new List<tb_r_travel_dit>();
            List<string> name = new List<string>();
            List<int> unit_code = new List<int>();
            RequestObject = await GetData.TravelDitList();
            return View("DetailsView", RequestObject);
        }
    }
}