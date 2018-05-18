using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CONTRAST_WEB.Models
{
    public static class GetData
    {
        public static async Task<tb_m_employee> EmployeeInfo(tb_m_employee model)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Employee/" + model.code);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    model = JsonConvert.DeserializeObject<tb_m_employee>(EmpResponse);
                }
            }
            return model;
        }

        public static async Task<List<tb_m_employee>> EmployeeDivision(string unitcode)
        {
            List<tb_m_employee> model = new List<tb_m_employee>();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Employee/Division?unitcode=" + unitcode);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    model = JsonConvert.DeserializeObject<List<tb_m_employee>>(EmpResponse);
                }
            }
            return model;
        }

        public static async Task<string> PhotoEmployeeInfo(string noreg)
        {
            string model = "";
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/PhotoEmployee/GetPhoto?noreg=" + noreg);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    model = JsonConvert.DeserializeObject < string>(EmpResponse);
                }
            }
            return model;
        }

        public static async Task<tb_m_photo_employee> PhotoEmployee(string noreg)
        {
            tb_m_photo_employee model =  new tb_m_photo_employee();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/PhotoEmployee/GetPhotoComplete?noreg=" + noreg);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    model = JsonConvert.DeserializeObject<tb_m_photo_employee>(EmpResponse);
                }
            }
            return model;
        }

        public static async Task<tb_m_employee_source_data> GetDivisionSource(int code)
        {
            tb_m_employee_source_data division_name= new tb_m_employee_source_data();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/EmployeeSourceData/GetDivision?id=" + code);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    division_name = JsonConvert.DeserializeObject<tb_m_employee_source_data>(EmpResponse);
                }
            }
            division_name.Departemen = division_name.Departemen.Replace("&", "and1");
            division_name.Divisi = division_name.Divisi.Replace("&", "and1");

            return division_name;
        }

        public static async Task<string> GetDivisionSourceName(int code)
        {
            tb_m_employee_source_data division_name = new tb_m_employee_source_data();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/EmployeeSourceData/GetDivision?id=" + code);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    division_name = JsonConvert.DeserializeObject<tb_m_employee_source_data>(EmpResponse);
                }
            }
            
            return division_name.Divisi;
        }

        public static async Task<tb_m_budget> GetCostWbs(bool overseas_flag,string division)
        {
            tb_m_budget division_name = new tb_m_budget();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Budget/GetCostWbs?div="+division.Trim() + "&type=" +overseas_flag);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    division_name = JsonConvert.DeserializeObject<tb_m_budget>(EmpResponse);
                }
            }
            return division_name;
        }

        public static async Task<bool> GetDivisionDoubleCheck(string division, string departement)
        {
            List<string> division_name = new List<string>();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/BudgetDivisionMapping/CheckDouble?div=" + division.Trim() + "&department=" + departement.Trim());

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    division_name = JsonConvert.DeserializeObject<List<string>>(EmpResponse);
                }
            }
            //division_name = division_name.Replace("&", "and1");
            if (division_name.Count() > 1) return true;
            else
            return false;
        }

        public static async Task<string> GetDivisionMapping(string division, string departement)
        {
            string division_name="";
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/BudgetDivisionMapping/GetDivision?div=" + division.Trim() +"&department="+departement.Trim());

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    division_name = JsonConvert.DeserializeObject<string>(EmpResponse);
                }
            }
            division_name = division_name.Replace("&", "and1");
            return division_name;
        }

        public static async Task<string> GetDivMapping(string code)
        {
            string division_name = "";
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/BudgetDivMapping/GetDivision?noreg=" + code.Trim());

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    division_name = JsonConvert.DeserializeObject<string>(EmpResponse);
                }
            }
            division_name = division_name.Replace("&", "and1");
            return division_name;
        }


        public static async Task<tb_m_budget> Budget(string wbs_no,string cost_center)
        {
            tb_m_budget division_name = new tb_m_budget();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Budget/GetData?wbs="+wbs_no+"&costcenter="+cost_center);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    division_name = JsonConvert.DeserializeObject<tb_m_budget>(EmpResponse);
                }
            }
            return division_name;
        }


        internal static Task TravelRequestDateTimeList(int? no_reg)
        {
            throw new NotImplementedException();
        }

        public static async Task<List<tb_r_travel_dit>> TravelDitList()
        {

            List<tb_r_travel_dit> ListItem = new List<tb_r_travel_dit>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TravelDit/Employee");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_r_travel_dit> ResponseList = new List<tb_r_travel_dit>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_r_travel_dit>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new tb_r_travel_dit();
                        listItem = item;

                        ListItem.Add(listItem);
                        k++;
                    }
                }
            }

            return ListItem;
        }

        public static async Task<tb_r_travel_dit> TravelDitListId(int id)
        {

            tb_r_travel_dit ListItem = new tb_r_travel_dit();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TravelDit/" + id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    ListItem = JsonConvert.DeserializeObject<tb_r_travel_dit>(str);
                                    }
            }

            return ListItem;
        }

        public static async Task<List<Class1>> SearchName(string search)
        {
            List<Class1> ListItem2 = new List<Class1>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/Employee/Search?name=" + search);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<Class1> ResponseList = new List<Class1>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<Class1>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new Class1();
                        listItem = item;
                        listItem.name = "(" + listItem.code.Trim() + ") " + listItem.name;
                        ListItem2.Add(listItem);

                    }
                }
            }

            return ListItem2;
        }

        public static async Task<List<Class1>> SearchNameDiv(string search)
        {
            List<Class1> ListItem2 = new List<Class1>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/EmployeeCoordinated/Search?name=" + search);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<Class1> ResponseList = new List<Class1>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<Class1>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new Class1();
                        listItem = item;
                        listItem.name = "(" + item.code.Trim() + ") " + item.name.Trim();
                        ListItem2.Add(listItem);

                    }
                }
            }

            return ListItem2;
        }

        public static async Task<tb_m_employee> EmployeeInfo(string noreg)
        {
            tb_m_employee model = new tb_m_employee();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Employee/" + noreg);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    model = JsonConvert.DeserializeObject<tb_m_employee>(EmpResponse);
                }
            }
            return model;
        }

        public static async Task<tb_r_travel_actualcost> ActualCostOrigin(int model)
        {
            tb_r_travel_actualcost Response = new tb_r_travel_actualcost();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/ActualCost/" + model);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    Response = JsonConvert.DeserializeObject<tb_r_travel_actualcost>(EmpResponse);
                }
            }
            return Response;
        }

        public static async Task<List<tb_r_travel_actualcost>> ActualCostOrigins(string group_code)
        {
            List<tb_r_travel_actualcost> Response = new List<tb_r_travel_actualcost>();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/ActualCost/GetAll?gcode=" + group_code);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    Response = JsonConvert.DeserializeObject<List<tb_r_travel_actualcost>>(EmpResponse);
                }
            }
            return Response;
        }

        public static async Task<List<vw_actualcost_generate_file>> GenerateFileData()
        {
            List<vw_actualcost_generate_file> ResponseList = new List<vw_actualcost_generate_file>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/ActualCostGenerateFile/");
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_actualcost_generate_file>>(str);
                }

            }
            return ResponseList;
        }

        public static async Task<List<vw_actualcost_generate_file>> GenerateFileDataFiltered(string search, DateTime? start, DateTime? end)
        {
            List<vw_actualcost_generate_file> ResponseList = new List<vw_actualcost_generate_file>();
            List<vw_actualcost_generate_file> FilteredList = new List<vw_actualcost_generate_file>();
            List<vw_actualcost_generate_file> ReturnList = new List<vw_actualcost_generate_file>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/ActualCostGenerateFile/");
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_actualcost_generate_file>>(str);
                }
                foreach (var item in ResponseList)
                {
                    if (item.BTR_NO.ToLower().Contains(search) ||
                        item.EMPLOYEE_NAME.ToLower().Contains(search) ||
                        item.DESTINATION.ToLower().Contains(search) ||
                        item.ID_CITY.ToString().Contains(search) ||
                        item.COST_CENTER_.ToLower().Contains(search) ||
                        item.WBS_ELEMENT_.ToLower().Contains(search) ||
                        item.TRAVEL_TYPE.ToLower().Contains(search) ||
                        item.BUDGET.ToLower().Contains(search)
                        )
                    {
                        FilteredList.Add(item);
                    }
                }

                if (start.HasValue && end.HasValue)
                {
                    foreach (var item in FilteredList)
                    {
                        DateTime start_date = new DateTime();
                        DateTime end_date = new DateTime();
                        start_date = DateTime.ParseExact(item.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        end_date = DateTime.ParseExact(item.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        if (start_date >= start && end_date <= end) ReturnList.Add(item);
                    }
                }
                else if (start.HasValue)
                {
                    foreach (var item in FilteredList)
                    {
                        DateTime start_date = new DateTime();
                        DateTime end_date = new DateTime();
                        start_date = DateTime.ParseExact(item.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        end_date = DateTime.ParseExact(item.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        if (start_date >= start) ReturnList.Add(item);
                    }

                }
                else if (end.HasValue)
                {
                    foreach (var item in FilteredList)
                    {
                        DateTime start_date = new DateTime();
                        DateTime end_date = new DateTime();
                        start_date = DateTime.ParseExact(item.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        end_date = DateTime.ParseExact(item.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        if (end_date <= end) ReturnList.Add(item);
                    }
                }
                else ReturnList = FilteredList;
            }
            return ReturnList;
        }



        public static async Task<tb_m_verifier_employee> ActualCostPosition(int model)
        {
            tb_m_verifier_employee Response = new tb_m_verifier_employee();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/ActualCost/" + model);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    Response = JsonConvert.DeserializeObject<tb_m_verifier_employee>(EmpResponse);
                }
            }
            return Response;
        }

        public static async Task<List<tb_r_travel_request_comment>> Comment(string group_code)
        {
            List<tb_r_travel_request_comment> employee_object = new List<tb_r_travel_request_comment>();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/TravelRequestComment/GetComment?id=" + group_code);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    employee_object = JsonConvert.DeserializeObject<List<tb_r_travel_request_comment>>(EmpResponse);
                }
            }
            return employee_object;
        }

        public static async Task<string> EmployeeNameInfo(int? model)
        {
            tb_m_employee employee_object = new tb_m_employee();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Employee/Name?noreg=" + model);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    employee_object = JsonConvert.DeserializeObject<tb_m_employee>(EmpResponse);
                }
            }
            return employee_object.name;
        }

        public static async Task<tb_m_verifier_employee> EmployeeVerifier(int? no_reg)
        {
            tb_m_verifier_employee employee_object = new tb_m_verifier_employee();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/VerifierEmployee/" + no_reg);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    employee_object = JsonConvert.DeserializeObject<tb_m_verifier_employee>(EmpResponse);
                }
            }
            return employee_object;
        }


        public static async Task<List<vw_rejected_actualcost_verification>> ActualCostRejected()
        {

            List<vw_rejected_actualcost_verification> ListItem = new List<vw_rejected_actualcost_verification>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployxees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/RejectedActualcostVerification");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                { 
                    var str = response.Content.ReadAsStringAsync().Result;
                    ListItem = JsonConvert.DeserializeObject<List<vw_rejected_actualcost_verification>>(str); 
                }
            }

            return ListItem;
        }

        public static async Task<tb_r_record_rejected_verification> RejectedComment(int? no_reg)
        {
            tb_r_record_rejected_verification employee_object = new tb_r_record_rejected_verification();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/RecordRejectedVerification/Comment?id=" + no_reg);

                //Checking the response is successful or not which is sent using HttpClient  vendor
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    employee_object = JsonConvert.DeserializeObject<tb_r_record_rejected_verification>(EmpResponse);
                }
            }
            return employee_object;
        }



        public static async Task<List<tb_m_vendor_employee>> VendorEmployee(int? no_reg)
        {
            List<tb_m_vendor_employee> ResponseList = new List<tb_m_vendor_employee>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/VendorEmployee/noreg?id=" + no_reg.ToString());
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject < List <tb_m_vendor_employee>>(str);
                }

            }
            return ResponseList;
        }

        public static List<tb_m_vendor_employee> VendorEmployeeValidate(int noreg)
        {

            List<tb_m_vendor_employee> ListItem = new List<tb_m_vendor_employee>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployxees using HttpClient  
                //HttpResponseMessage response = await client.GetAsync("api/TravelRequest/Activity?noreg="+Convert.ToInt32(noreg));
                var response = client.GetAsync("api/VendorEmployee/noreg?id=" + Convert.ToInt32(noreg)).Result;

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_m_vendor_employee> ResponseList = new List<tb_m_vendor_employee>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_m_vendor_employee>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new tb_m_vendor_employee();
                        listItem = item;

                        ListItem.Add(listItem);
                        k++;
                    }
                }
            }

            return ListItem;
        }

        public static async Task<double> TaxValueInfo(string model)
        {
            tb_m_tax employee_object = new tb_m_tax();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Tax/Value?id=" + model);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    employee_object = JsonConvert.DeserializeObject<tb_m_tax>(EmpResponse);
                }
            }
            return employee_object.value_tax;
        }

        public static async Task<string> DestinationNameInfo(int? model)
        {
            tb_m_destination employee_object = new tb_m_destination();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Destination/Name?destination=" + model);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    employee_object = JsonConvert.DeserializeObject<tb_m_destination>(EmpResponse);
                }
            }
            return employee_object.destination_name;
        }


        public static async Task<vw_summary_settlement> SummarySettlementInfo(string model)
        {
            vw_summary_settlement employee_object = new vw_summary_settlement();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/SummarySettlement/gid?gid=" + model);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    employee_object = JsonConvert.DeserializeObject<vw_summary_settlement>(EmpResponse);
                }
            }
            return employee_object;
        }

        public static async Task<List<vw_request_summary>> RequestSummaryListInfo(string no_reg)
        {
            List<vw_request_summary> ResponseList = new List<vw_request_summary>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);


                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/RequestSummary/noreg?id=" + no_reg);
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_request_summary>>(str);
                }

                return ResponseList;
            }
        }

        public static async Task<vw_request_summary> RequestSummaryInfo(string group_code)
        {
            vw_request_summary ResponseList = new vw_request_summary();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);


              
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/RequestSummary/group_code?id=" + group_code);
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<vw_request_summary>(str);
                }

                return ResponseList;
            }
        }


        public static async Task<List<vw_travel_execution_list>> ExecutionList(string no_reg)
        {
            List<vw_travel_execution_list> ResponseList = new List<vw_travel_execution_list>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/ExecutionList/noreg?id=" + no_reg);
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_travel_execution_list>>(str);
                }

                return ResponseList;
            }
        }

        public static async Task<List<tb_r_travel_dit>> TravelDitListInfo(string no_reg)
        {
            List<tb_r_travel_dit> ResponseList = new List<tb_r_travel_dit>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);


                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TravelDit/Employee?Reg=" + no_reg);
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_r_travel_dit>>(str);
                }

                return ResponseList;
            }
        }


        public static async Task <tb_r_travel_dit> TravelDitInfo(int? id)
        {
            tb_r_travel_dit ResponseList = new tb_r_travel_dit();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);


                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TravelDit/" + id);
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<tb_r_travel_dit>(str);
                }

                return ResponseList;
            }
        }

        public static async Task<List<SelectListItem>> DestinationInfo()
        {
            List<SelectListItem> ListItem1 = new List<SelectListItem>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/Destination/Destination_List");
                if (response.IsSuccessStatusCode)
                {
                    List<tb_m_destination> ResponseList = new List<tb_m_destination>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_m_destination>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new SelectListItem();
                        listItem.Text = item.destination_name;
                        listItem.Value = item.id_destination.ToString();
                        ListItem1.Add(listItem);
                        k++;
                    }
                }
            }
            return ListItem1;
        }

        public static async Task<tb_m_destination> DestinationCityInfo(string city)
        {
            tb_m_destination employee_object = new tb_m_destination();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Destination/");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                    List<tb_m_destination> Result = new List<tb_m_destination>();
                    //Deserializing the response recieved from web api and storing into the Employee list  
                    Result = JsonConvert.DeserializeObject<List<tb_m_destination>>(EmpResponse);
                    foreach (var temp in Result)
                    {
                        if (temp.destination_name.Contains(city) && temp.active_flag == true) employee_object = temp;
                    }
                }
            }
            return employee_object;
        }
        public static async Task<List<SelectListItem>> TaxInfo()
        {
            List<SelectListItem> ListItem1 = new List<SelectListItem>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/Tax/Tax_List");
                if (response.IsSuccessStatusCode)
                {
                    List<String> ResponseList = new List<String>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<String>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new SelectListItem();
                        listItem.Text = item;
                        listItem.Value = item;
                        ListItem1.Add(listItem);
                        k++;
                    }
                }
            }
            return ListItem1;
        }

        public static async Task<List<SelectListItem>> PurposeInfo()
        {
            List<SelectListItem> ListItem = new List<SelectListItem>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/Purpose/");
                if (response.IsSuccessStatusCode)
                {
                    List<String> ResponseList = new List<String>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<String>>(str);
                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new SelectListItem();
                        listItem.Text = item;
                        listItem.Value = item;
                        ListItem.Add(listItem);
                        k++;
                    }
                }
                //ViewBag.ResponseList = new List<SelectListItem> { }; 
            }

            return ListItem;
        }

        public static async Task<List<SelectListItem>> VendorInfo()
        {
            List<SelectListItem> ListItem = new List<SelectListItem>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/Vendor/");
                if (response.IsSuccessStatusCode)
                {
                    List<tb_m_vendor> ResponseList = new List<tb_m_vendor>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_m_vendor>>(str);
                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new SelectListItem();
                        listItem.Text = item.vendor_name;
                        listItem.Value = item.vendor_name;
                        ListItem.Add(listItem);
                        k++;
                    }
                }
                //ViewBag.ResponseList = new List<SelectListItem> { }; 
            }

            return ListItem;
        }

        public static async Task<List<SelectListItem>> VendorTicketInfo()
        {
            List<SelectListItem> ListItem = new List<SelectListItem>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/Vendor/TicketList");
                if (response.IsSuccessStatusCode)
                {
                    List<tb_m_vendor> ResponseList = new List<tb_m_vendor>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_m_vendor>>(str);
                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new SelectListItem();
                        listItem.Text = item.vendor_name;
                        listItem.Value = item.vendor_name;
                        ListItem.Add(listItem);
                        k++;
                    }
                }
                //ViewBag.ResponseList = new List<SelectListItem> { }; 
            }

            return ListItem;
        }

        public static async Task<List<SelectListItem>> VendorHotelInfo()
        {
            List<SelectListItem> ListItem = new List<SelectListItem>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/Vendor/HotelList");
                if (response.IsSuccessStatusCode)
                {
                    List<tb_m_vendor> ResponseList = new List<tb_m_vendor>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_m_vendor>>(str);
                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new SelectListItem();
                        listItem.Text = item.vendor_name;
                        listItem.Value = item.vendor_name;
                        ListItem.Add(listItem);
                        k++;
                    }
                }
                //ViewBag.ResponseList = new List<SelectListItem> { }; 
            }

            return ListItem;
        }

        public static async Task<string> VendorCodeInfo(string model)
        {
            tb_m_vendor EmpInfo = new tb_m_vendor();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Vendor/Code?name=" + model);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    EmpInfo = JsonConvert.DeserializeObject<tb_m_vendor>(EmpResponse);

                }
            }

            return EmpInfo.vendor_code;
        }

        public static async Task<short?> RegionInfo(int? id_destination_city)
        {
            tb_m_destination EmpInfo = new tb_m_destination();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Destination/Region?reg=" + id_destination_city);
                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    EmpInfo = JsonConvert.DeserializeObject<tb_m_destination>(EmpResponse);
                }

            }

            return EmpInfo.id_region;
        }

        public static async Task<vw_travel_allowance> RateMealWinterInfo(TravelRequestHelper model)
        {
            vw_travel_allowance EmpInfo = new vw_travel_allowance();
            using (var client = new HttpClient())
            {
               var rank = model.employee_info.@class.Replace("B.","BOD");
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/TravelAllowance?rank=" + rank + "&reg=" + model.travel_request.destination_code + "&dt=" + ((model.travel_request.overseas_flag == true) ? 1 : 0).ToString());

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    EmpInfo = JsonConvert.DeserializeObject<vw_travel_allowance>(EmpResponse);

                }
            }

            return EmpInfo;
        }



        public static async Task<List<tb_r_travel_request>> TravelRequestList()
        {
            List<tb_r_travel_request> ListItem = new List<tb_r_travel_request>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TravelRequest");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_r_travel_request> ResponseList = new List<tb_r_travel_request>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_r_travel_request>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new tb_r_travel_request();
                        listItem = item;

                        ListItem.Add(listItem);
                        k++;
                    }
                }
            }

            return ListItem;
        }

        public static async Task<tb_r_travel_request> TravelRequest(int id)
        {
            tb_r_travel_request Entity = new tb_r_travel_request();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TravelRequest/" + id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    Entity = JsonConvert.DeserializeObject<tb_r_travel_request>(str);
                }
            }

            return Entity;
        }

        public static async Task<List<vw_actualcost_preparation>> ActualcostPreparation()
        {
            List<vw_actualcost_preparation> Entity = new List<vw_actualcost_preparation>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync(" api/ActualcostPreparation");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    Entity = JsonConvert.DeserializeObject<List<vw_actualcost_preparation>>(str);
                }
            }

            return Entity;
        }
        public static async Task<List<tb_r_travel_request>> TravelRequestActiveList()
        {

            List<tb_r_travel_request> ListItem = new List<tb_r_travel_request>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployxees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TravelRequest/Active");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_r_travel_request> ResponseList = new List<tb_r_travel_request>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_r_travel_request>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new tb_r_travel_request();
                        listItem = item;

                        ListItem.Add(listItem);
                        k++;
                    }
                }
            }

            return ListItem;
        }

        public static List<tb_r_travel_request> TravelRequestDateTimeList(string noreg)
        {

            List<tb_r_travel_request> ListItem = new List<tb_r_travel_request>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployxees using HttpClient  
                //HttpResponseMessage response = await client.GetAsync("api/TravelRequest/Activity?noreg="+Convert.ToInt32(noreg));
                var response = client.GetAsync("api/TravelRequest/Activity?noreg=" + Convert.ToInt32(noreg)).Result;

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_r_travel_request> ResponseList = new List<tb_r_travel_request>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_r_travel_request>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new tb_r_travel_request();
                        listItem = item;

                        ListItem.Add(listItem);
                        k++;
                    }
                }
            }

            return ListItem;
        }

        public static async Task<List<tb_r_travel_request>> TravelRequestGCList(string group_code)
        {

            List<tb_r_travel_request> ListItem = new List<tb_r_travel_request>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployxees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TravelRequest/Sort?group_code=" + group_code);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_r_travel_request> ResponseList = new List<tb_r_travel_request>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_r_travel_request>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new tb_r_travel_request();
                        listItem = item;

                        ListItem.Add(listItem);
                        k++;
                    }
                }
            }

            return ListItem;
        }

        public static async Task<List<tb_r_travel_request_participant>> TravelRequestParticipant(vw_request_summary model)
        {

            List<tb_r_travel_request_participant> ListItem = new List<tb_r_travel_request_participant>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TravelRequestActiveParticipant?reg=" + model.no_reg + "&gcode=" + model.group_code);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_r_travel_request_participant> ResponseList = new List<tb_r_travel_request_participant>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_r_travel_request_participant>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new tb_r_travel_request_participant();
                        listItem = item;

                        ListItem.Add(listItem);
                        k++;
                    }
                }
            }

            return ListItem;
        }

        //overload
        public static async Task<List<tb_r_travel_request_participant>> TravelRequestParticipant(string no_reg,string group_code)
        {

            List<tb_r_travel_request_participant> ListItem = new List<tb_r_travel_request_participant>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TravelRequestActiveParticipant?reg=" + no_reg + "&gcode=" + group_code);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_r_travel_request_participant> ResponseList = new List<tb_r_travel_request_participant>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_r_travel_request_participant>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new tb_r_travel_request_participant();
                        listItem = item;

                        ListItem.Add(listItem);
                        k++;
                    }
                }
            }

            return ListItem;
        }

        public static async Task<tb_m_rate_flight> RateFlightInfo(TravelRequestHelper model)
        {
            tb_m_rate_flight EmpInfo = new tb_m_rate_flight();
            string Destination = await GetData.DestinationNameInfo(model.travel_request.id_destination_city);
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/RateFlight/Destination?city=" + Destination);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    EmpInfo = JsonConvert.DeserializeObject<tb_m_rate_flight>(EmpResponse);
                }
            }

            return EmpInfo;
        }

        public static async Task<tb_m_rate_hotel> RateHotelInfo(TravelRequestHelper model)
        {
            tb_m_rate_hotel EmpInfo = new tb_m_rate_hotel();
            using (var client = new HttpClient())
            {
                var rank = model.employee_info.@class.Replace("BOD", "B.");

                model.employee_info.@class = model.employee_info.@class.Replace("BOD","B.");
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);
                model.employee_info.@class = model.employee_info.@class.Replace("BOD", "B.");
                client.DefaultRequestHeaders.Clear();
                model.employee_info.@class = model.employee_info.@class.Replace("BOD", "B.");
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient                  
                HttpResponseMessage Res = await client.GetAsync("api/RateHotel?rank=" + rank);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    EmpInfo = JsonConvert.DeserializeObject<tb_m_rate_hotel>(EmpResponse);
                }

            }

            return EmpInfo;
        }

        public static async Task<List<vw_travel_for_settlement>> TravelSettlementList(int? no_reg)
        {

            List<vw_travel_for_settlement> ListItem = new List<vw_travel_for_settlement>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TravelForSettlement/Employee?reg=" + no_reg);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_travel_for_settlement> ResponseList = new List<vw_travel_for_settlement>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_travel_for_settlement>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new vw_travel_for_settlement();
                        listItem = item;

                        ListItem.Add(listItem);
                        k++;
                    }
                }
            }

            return ListItem;
        }

        public static async Task<List<vw_actualCost_verified>> ActualCostVerifiedList(string position)
        {

            List<vw_actualCost_verified> ListItem = new List<vw_actualCost_verified>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/ActualCostVerified/div?name=" + position.Trim());

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_actualCost_verified> ResponseList = new List<vw_actualCost_verified>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_actualCost_verified>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new vw_actualCost_verified();
                        listItem = item;

                        ListItem.Add(listItem);
                        k++;
                    }
                }
            }

            return ListItem;
        }

        public static async Task<tb_m_div_approval_mapping> AssignedBy(string unitcode)
        {

            tb_m_div_approval_mapping model = new tb_m_div_approval_mapping();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/ApprovalMapping/GetAssignedBy?name=" + unitcode);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    model = JsonConvert.DeserializeObject<tb_m_div_approval_mapping>(EmpResponse);
                }
            }
            return model;
        }

        public static async Task<tb_m_travel_procedures> Procedures(string code)
        {

            tb_m_travel_procedures model = new tb_m_travel_procedures();
            using (var client = new HttpClient())
            {
                code = code.Replace("B.","BOD");
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/TravelProcedures/GetApprv?code=" + code);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    model = JsonConvert.DeserializeObject<tb_m_travel_procedures>(EmpResponse);
                }
            }
            return model;
        }

         
        public static async Task<List<vw_settlement_verified>> SettlementVerifiedList(string position)
        {

            List<vw_settlement_verified> ListItem = new List<vw_settlement_verified>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/SettlementVerified/div?name=" + position.Trim());

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_settlement_verified> ResponseList = new List<vw_settlement_verified>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_settlement_verified>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new vw_settlement_verified();
                        listItem = item;

                        ListItem.Add(listItem);
                        k++;
                    }
                }
            }

            return ListItem;
        }

        public static async Task<List<vw_BPD_verified>> FixedCostVerifiedList(string position)
        {

            List<vw_BPD_verified> ListItem = new List<vw_BPD_verified>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/FixedCostVerified/div?name=" + position.Trim());

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_BPD_verified> ResponseList = new List<vw_BPD_verified>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_BPD_verified>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new vw_BPD_verified();
                        listItem = item;

                        ListItem.Add(listItem);
                        k++;
                    }
                }
            }

            return ListItem;
        }

        public static async Task<List<tb_r_travel_actualcost>> ActualCostList()
        {

            List<tb_r_travel_actualcost> ListItem = new List<tb_r_travel_actualcost>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/ActualCost");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_r_travel_actualcost> ResponseList = new List<tb_r_travel_actualcost>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_r_travel_actualcost>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new tb_r_travel_actualcost();
                        listItem = item;

                        ListItem.Add(listItem);
                        k++;
                    }
                }
            }

            return ListItem;
        }
        public static async Task<List<vw_actualCost_verified>> ActualCostVerifiedListFiltered(string position, string search)
        {
            string lower_search = search.ToLower();
            List<vw_actualCost_verified> ListItem = new List<vw_actualCost_verified>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/ActualCostVerified/div?name=" + position.Trim());

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_actualCost_verified> ResponseList = new List<vw_actualCost_verified>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_actualCost_verified>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new vw_actualCost_verified();
                        if (item.name.ToLower().Contains(search) || item.destination_name.ToLower().Contains(search) || item.group_code.ToLower().Contains(search) || item.jenis_transaksi.ToLower().Contains(search))
                        {
                            listItem = item;

                            ListItem.Add(listItem);
                            k++;
                        }
                    }
                }
            }

            return ListItem;
        }

        public static async Task<List<vw_settlement_verified>> SettlementVerifiedListFiltered(string position, string search)
        {
            string lower_search = search.ToLower();
            List<vw_settlement_verified> ListItem = new List<vw_settlement_verified>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/SettlementVerified/div?name=" + position.Trim());

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_settlement_verified> ResponseList = new List<vw_settlement_verified>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_settlement_verified>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new vw_settlement_verified();
                        if (item.name.ToLower().Contains(search) || item.destination_name.ToLower().Contains(search) || item.group_code.ToLower().Contains(search) || item.jenis_transaksi.ToLower().Contains(search))
                        {
                            listItem = item;

                            ListItem.Add(listItem);
                            k++;
                        }
                    }
                }
            }

            return ListItem;
        }

        public static async Task<List<vw_BPD_verified>> FixedCostVerifiedListFiltered(string position, string search)
        {
            string lower_search = search.ToLower();
            List<vw_BPD_verified> ListItem = new List<vw_BPD_verified>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/FixedCostVerified/div?name=" + position.Trim());

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_BPD_verified> ResponseList = new List<vw_BPD_verified>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_BPD_verified>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new vw_BPD_verified();
                        if (item.name.ToLower().Contains(search) || item.destination_name.ToLower().Contains(search) || item.group_code.ToLower().Contains(search) || item.jenis_transaksi.ToLower().Contains(search))
                        {
                            listItem = item;

                            ListItem.Add(listItem);
                            k++;
                        }
                    }
                }
            }

            return ListItem;
        }

        public static async Task<List<vw_rejected_travel_for_settlement>> RejectTravelSettlementList(int? no_reg)
        {

            List<vw_rejected_travel_for_settlement> ListItem = new List<vw_rejected_travel_for_settlement>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/RejectedTraverForSettlement/Employee?reg=" + no_reg);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_rejected_travel_for_settlement> ResponseList = new List<vw_rejected_travel_for_settlement>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_rejected_travel_for_settlement>>(str);

                    int k = 1;
                    foreach (var item in ResponseList)
                    {
                        var listItem = new vw_rejected_travel_for_settlement();
                        listItem = item;

                        ListItem.Add(listItem);
                        k++;
                    }
                }
            }

            return ListItem;
        }

        //15/3 add 5 list tracking
        public static async Task<List<vw_tracking_transaction_data_new>> TrackingListAll()
        {
            List<vw_tracking_transaction_data_new> ListItem = new List<vw_tracking_transaction_data_new>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TrackingTransactionDataNew");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_tracking_transaction_data_new> ResponseList = new List<vw_tracking_transaction_data_new>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_tracking_transaction_data_new>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        var temp = new vw_tracking_transaction_data_new();

                        temp = item;

                        ListItem.Add(temp);
                        k++;

                    }
                }
            }

            return ListItem;
        }

        public static async Task<List<vw_tracking_transaction_data_new>> TrackingListDivisonAll(string code)
        {
            List<vw_tracking_transaction_data_new> ListItem = new List<vw_tracking_transaction_data_new>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TrackingTransactionDataNew/Division/?code=" + code);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_tracking_transaction_data_new> ResponseList = new List<vw_tracking_transaction_data_new>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_tracking_transaction_data_new>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        var temp = new vw_tracking_transaction_data_new();

                        temp = item;

                        ListItem.Add(temp);
                        k++;

                    }
                }
            }

            return ListItem;
        }

        public static async Task<List<vw_tracking_transaction_data_new>> TrackingListIndividual(string code)
        {
            List<vw_tracking_transaction_data_new> ListItem = new List<vw_tracking_transaction_data_new>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TrackingTransactionDataNew/Single/?code=" + code);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_tracking_transaction_data_new> ResponseList = new List<vw_tracking_transaction_data_new>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ListItem = JsonConvert.DeserializeObject<List<vw_tracking_transaction_data_new>>(str);
                }
            }

            return ListItem;
        }

        public static async Task<List<vw_tracking_transaction_data_new>> TrackingListAllSearch(string search, DateTime? start = null, DateTime? end = null)
        {
            string lower_search = search.ToLower();
            List<vw_tracking_transaction_data_new> ListItem = new List<vw_tracking_transaction_data_new>();
            List<vw_tracking_transaction_data_new> LastFilter = new List<vw_tracking_transaction_data_new>();

            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TrackingTransactionDataNew");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_tracking_transaction_data_new> ResponseList = new List<vw_tracking_transaction_data_new>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_tracking_transaction_data_new>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        var temp = new vw_tracking_transaction_data_new();
                        //if (item.name.ToLower().Contains(lower_search) || item.destination_name.ToLower().Contains(lower_search) || item.group_code.ToLower().Contains(lower_search) || item.jenis_transaksi.ToLower().Contains(lower_search) || item.group_code.ToLower().Contains(lower_search) || item.jenis_transaksi.ToLower().Contains(search) || item.TYPES_OF_TRANSACTIONS.ToLower().Contains(search) || item.verified_flag.ToLower().Contains(search))
                        //{
                        //    temp = item;

                        //    ListItem.Add(temp);
                        //    k++;
                        //}

                        if (item.jenis_transaksi != null)
                        {
                            if (item.name.ToLower().Contains(lower_search) || item.destination_name.ToLower().Contains(lower_search) || item.no_reg.ToString().ToLower().Contains(lower_search) || item.jenis_transaksi.ToLower().Contains(lower_search) || item.group_code.ToLower().Contains(lower_search) || item.TYPES_OF_TRANSACTIONS.ToLower().Contains(search) || item.verified_flag.ToLower().Contains(search))
                            {
                                temp = item;

                                ListItem.Add(temp);
                                k++;
                            }
                        }
                        else
                        {
                            if (item.name.ToLower().Contains(lower_search) || item.destination_name.ToLower().Contains(lower_search) || item.no_reg.ToString().ToLower().Contains(lower_search) || item.group_code.ToLower().Contains(lower_search) || item.TYPES_OF_TRANSACTIONS.ToLower().Contains(search) || item.verified_flag.ToLower().Contains(search))
                            {
                                temp = item;

                                ListItem.Add(temp);
                                k++;
                            }
                        }
                    }

                    if (start.HasValue && end.HasValue)
                    {
                        foreach (var item in ListItem)
                        {
                            var temp = new vw_tracking_transaction_data_new();
                            if (item.create_date >= start && item.create_date <= end)
                            {
                                temp = item;

                                LastFilter.Add(temp);
                                k++;
                            }
                        }
                    }
                    else if (start.HasValue)
                    {
                        foreach (var item in ListItem)
                        {
                            var temp = new vw_tracking_transaction_data_new();
                            if (item.create_date >= start)
                            {
                                temp = item;

                                LastFilter.Add(temp);
                                k++;
                            }
                        }
                    }
                    else if (end.HasValue)
                    {
                        foreach (var item in ListItem)
                        {
                            var temp = new vw_tracking_transaction_data_new();
                            if (item.create_date <= end)
                            {
                                temp = item;

                                LastFilter.Add(temp);
                                k++;
                            }
                        }
                    }
                    else LastFilter = ListItem;
                }
            }

            return LastFilter;
        }

        public static async Task<List<vw_tracking_transaction_data_new>> TrackingListDivisonAllSearch(string code, string search, DateTime? start = null, DateTime? end = null)
        {
            string lower_search = search.ToLower();
            List<vw_tracking_transaction_data_new> ListItem = new List<vw_tracking_transaction_data_new>();
            List<vw_tracking_transaction_data_new> LastFilter = new List<vw_tracking_transaction_data_new>();

            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TrackingTransactionDataNew/Division/?code=" + code);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_tracking_transaction_data_new> ResponseList = new List<vw_tracking_transaction_data_new>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_tracking_transaction_data_new>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        var temp = new vw_tracking_transaction_data_new();
                         
                        if (item.jenis_transaksi != null)
                        {
                            if (item.name.ToLower().Contains(lower_search) || item.destination_name.ToLower().Contains(lower_search) || item.no_reg.ToString().ToLower().Contains(lower_search) || item.jenis_transaksi.ToLower().Contains(lower_search) || item.group_code.ToLower().Contains(lower_search) || item.TYPES_OF_TRANSACTIONS.ToLower().Contains(search) || item.verified_flag.ToLower().Contains(search))
                            {
                                temp = item;

                                ListItem.Add(temp);
                                k++;
                            }
                        }
                        else
                        {
                            if (item.name.ToLower().Contains(lower_search) || item.destination_name.ToLower().Contains(lower_search) || item.no_reg.ToString().ToLower().Contains(lower_search) || item.group_code.ToLower().Contains(lower_search) || item.TYPES_OF_TRANSACTIONS.ToLower().Contains(search) || item.verified_flag.ToLower().Contains(search))
                            {
                                temp = item;

                                ListItem.Add(temp);
                                k++;
                            }
                        }
                    }

                    if (start.HasValue && end.HasValue)
                    {
                        foreach (var item in ListItem)
                        {
                            var temp = new vw_tracking_transaction_data_new();
                            if (item.create_date >= start && item.create_date <= end)
                            {
                                temp = item;

                                LastFilter.Add(temp);
                                k++;
                            }
                        }
                    }
                    else if (start.HasValue)
                    {
                        foreach (var item in ListItem)
                        {
                            var temp = new vw_tracking_transaction_data_new();
                            if (item.create_date >= start)
                            {
                                temp = item;

                                LastFilter.Add(temp);
                                k++;
                            }
                        }
                    }
                    else if (end.HasValue)
                    {
                        foreach (var item in ListItem)
                        {
                            var temp = new vw_tracking_transaction_data_new();
                            if (item.create_date <= end)
                            {
                                temp = item;

                                LastFilter.Add(temp);
                                k++;
                            }
                        }
                    }
                    else LastFilter = ListItem;
                }
            }

            return LastFilter;
        }

        public static async Task<List<vw_tracking_transaction_data_new>> TrackingListIndividualSearch(string code, string search, DateTime? start = null, DateTime? end = null)
        {
            string lower_search = search.ToLower();
            List<vw_tracking_transaction_data_new> ListItem = new List<vw_tracking_transaction_data_new>();
            List<vw_tracking_transaction_data_new> LastFilter = new List<vw_tracking_transaction_data_new>();

            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TrackingTransactionDataNew/Single/?code=" + code);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_tracking_transaction_data_new> ResponseList = new List<vw_tracking_transaction_data_new>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_tracking_transaction_data_new>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        var temp = new vw_tracking_transaction_data_new();
                        //if (item.name.ToLower().Contains(lower_search) || item.destination_name.ToLower().Contains(lower_search) || item.group_code.ToLower().Contains(lower_search) || item.jenis_transaksi.ToLower().Contains(lower_search) || item.group_code.ToLower().Contains(lower_search) || item.jenis_transaksi.ToLower().Contains(search) || item.TYPES_OF_TRANSACTIONS.ToLower().Contains(search) || item.verified_flag.ToLower().Contains(search))
                        //{
                        //    temp = item;

                        //    ListItem.Add(temp);
                        //    k++;
                        //}
                        if (item.jenis_transaksi != null)
                        {
                            if (item.name.ToLower().Contains(lower_search) || item.destination_name.ToLower().Contains(lower_search) || item.no_reg.ToString().ToLower().Contains(lower_search) || item.jenis_transaksi.ToLower().Contains(lower_search) || item.group_code.ToLower().Contains(lower_search) || item.TYPES_OF_TRANSACTIONS.ToLower().Contains(search) || item.verified_flag.ToLower().Contains(search))
                            {
                                temp = item;

                                ListItem.Add(temp);
                                k++;
                            }
                        }
                        else
                        {
                            if (item.name.ToLower().Contains(lower_search) || item.destination_name.ToLower().Contains(lower_search) || item.no_reg.ToString().ToLower().Contains(lower_search) || item.group_code.ToLower().Contains(lower_search) || item.TYPES_OF_TRANSACTIONS.ToLower().Contains(search) || item.verified_flag.ToLower().Contains(search))
                            {
                                temp = item;

                                ListItem.Add(temp);
                                k++;
                            }
                        }
                    }

                    if (start.HasValue && end.HasValue)
                    {
                        foreach (var item in ListItem)
                        {
                            var temp = new vw_tracking_transaction_data_new();
                            if (item.create_date >= start && item.create_date <= end)
                            {
                                temp = item;

                                LastFilter.Add(temp);
                                k++;
                            }
                        }
                    }
                    else if (start.HasValue)
                    {
                        foreach (var item in ListItem)
                        {
                            var temp = new vw_tracking_transaction_data_new();
                            if (item.create_date >= start)
                            {
                                temp = item;

                                LastFilter.Add(temp);
                                k++;
                            }
                        }
                    }
                    else if (end.HasValue)
                    {
                        foreach (var item in ListItem)
                        {
                            var temp = new vw_tracking_transaction_data_new();
                            if (item.create_date <= end)
                            {
                                temp = item;

                                LastFilter.Add(temp);
                                k++;
                            }
                        }
                    }
                    else LastFilter = ListItem;
                }
            }

            return LastFilter;
        }

        //rate
        public static async Task<List<tb_m_rate_hotel>> RateHotelList()
        {
            List<tb_m_rate_hotel> ListItem = new List<tb_m_rate_hotel>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/RateHotel");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_m_rate_hotel> ResponseList = new List<tb_m_rate_hotel>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_m_rate_hotel>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        if (item.active_flag == true)
                        {
                            var temp = new tb_m_rate_hotel();

                            temp = item;

                            ListItem.Add(temp);
                        }
                        k++;

                    }
                }
            }

            return ListItem;
        }

        public static async Task<List<tb_m_rate_visa>> RateVisaList()
        {
            List<tb_m_rate_visa> ListItem = new List<tb_m_rate_visa>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/RateVisa");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_m_rate_visa> ResponseList = new List<tb_m_rate_visa>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_m_rate_visa>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        if (item.active_flag == true)
                        {
                            var temp = new tb_m_rate_visa();

                            temp = item;

                            ListItem.Add(temp);
                        }
                        k++;

                    }
                }
            }
            return ListItem;
        }

        public static async Task<List<tb_m_rate_passport>> RatePassportList()
        {
            List<tb_m_rate_passport> ListItem = new List<tb_m_rate_passport>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/RatePassport");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_m_rate_passport> ResponseList = new List<tb_m_rate_passport>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_m_rate_passport>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        if (item.active_flag == true)
                        {
                            var temp = new tb_m_rate_passport();

                            temp = item;

                            ListItem.Add(temp);
                        }
                        k++;

                    }
                }
            }
            return ListItem;
        }

        public static async Task<List<tb_m_vendor_employee>> VendorEmployeeList()
        {
            List<tb_m_vendor_employee> ListItem = new List<tb_m_vendor_employee>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/VendorEmployee");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_m_vendor_employee> ResponseList = new List<tb_m_vendor_employee>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_m_vendor_employee>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        if (item.active_flag == true)
                        {
                            var temp = new tb_m_vendor_employee();

                            temp = item;

                            ListItem.Add(temp);
                        }

                        k++;

                    }
                }
            }
            return ListItem;
        }

        public static async Task<List<tb_m_vendor>> VendorTravelList()
        {
            List<tb_m_vendor> ListItem = new List<tb_m_vendor>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/VendorEmployee");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_m_vendor> ResponseList = new List<tb_m_vendor>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_m_vendor>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        if (item.active_flag == true)
                        {
                            var temp = new tb_m_vendor();

                            temp = item;

                            ListItem.Add(temp);
                        }
                        k++;

                    }
                }
            }
            return ListItem;
        }

        public static async Task<List<tb_m_budget>> BudgetList()
        {
            List<tb_m_budget> ListItem = new List<tb_m_budget>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/Budget");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_m_budget> ResponseList = new List<tb_m_budget>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_m_budget>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        if (item.active_flag == "1")
                        {
                            var temp = new tb_m_budget();
                            temp = item;
                            ListItem.Add(temp);
                        }
                        k++;
                    }
                }
            }
            return ListItem;
        }

        public static async Task<List<tb_m_rate_flight>> RateFlightList()
        {
            List<tb_m_rate_flight> ListItem = new List<tb_m_rate_flight>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/RateFlight");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_m_rate_flight> ResponseList = new List<tb_m_rate_flight>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_m_rate_flight>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        if (item.active_flag == true)
                        {
                            var temp = new tb_m_rate_flight();

                            temp = item;

                            ListItem.Add(temp);
                        }
                        k++;

                    }
                }
            }

            return ListItem;
        }

        public static async Task<List<tb_m_travel_procedures>> TravelProcedureList()
        {
            List<tb_m_travel_procedures> ListItem = new List<tb_m_travel_procedures>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TravelProcedures");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_m_travel_procedures> ResponseList = new List<tb_m_travel_procedures>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_m_travel_procedures>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        if (item.active_flag == true)
                        {
                            var temp = new tb_m_travel_procedures();
                            temp = item;
                            ListItem.Add(temp);
                        }
                        k++;
                    }
                }
            }
            return ListItem;
        }
        
        public static async Task<List<tb_m_employee_structure_organization>> EmployeeStructureOrganizationList()
        {
            List<tb_m_employee_structure_organization> ListItem = new List<tb_m_employee_structure_organization>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/EmployeeStructureOrganization");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_m_employee_structure_organization> ResponseList = new List<tb_m_employee_structure_organization>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_m_employee_structure_organization>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        if (item.active_flag == true)
                        {
                            var temp = new tb_m_employee_structure_organization();
                            temp = item;
                            ListItem.Add(temp);
                        }
                        k++;

                    }
                }
            }
            return ListItem;
        }

        public static async Task<List<tb_m_budget_source_data>> BudgetSourceDataList()
        {
            List<tb_m_budget_source_data> ListItem = new List<tb_m_budget_source_data>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/BudgetSourceData");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_m_budget_source_data> ResponseList = new List<tb_m_budget_source_data>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_m_budget_source_data>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        if (item.active_flag == true)
                        {
                            var temp = new tb_m_budget_source_data();
                            temp = item;
                            ListItem.Add(temp);
                        }
                        k++;
                    }
                }
            }
            return ListItem;
        }

        public static async Task<List<string>> DivisionNameList()
        {
            List<string> ListItem = new List<string>();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/EmployeeSourceData/Distinct");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<string> ResponseList = new List<string>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<string>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {

                        ListItem.Add(item);


                    }
                }
            }
            return ListItem;
        }

        public static async Task<List<vw_invoice_actualcost>> InvoiceActualCost()
        {
            using (var client = new HttpClient())
            {
                List<vw_invoice_actualcost> ListItem = new List<vw_invoice_actualcost>();
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/InvoiceActualcost");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_invoice_actualcost> ResponseList = new List<vw_invoice_actualcost>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_invoice_actualcost>>(str);

                    foreach (var item in ResponseList)
                    {

                        var temp = new vw_invoice_actualcost();
                        temp = item;
                        ListItem.Add(temp);

                    }
                }
                return ListItem;
            }
        }

        public static async Task<List<vw_invoice_actualcost_new>> InvoiceActualCostNew()
        {
            using (var client = new HttpClient())
            {
                List<vw_invoice_actualcost_new> ListItem = new List<vw_invoice_actualcost_new>();
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/InvoiceActualcostNew");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_invoice_actualcost_new> ResponseList = new List<vw_invoice_actualcost_new>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_invoice_actualcost_new>>(str);

                    foreach (var item in ResponseList)
                    {

                        var temp = new vw_invoice_actualcost_new();
                        temp = item;
                        ListItem.Add(temp);

                    }
                }
                return ListItem;
            }
        }
        
        public static async Task<List<tb_r_invoice_actualcost>> TableInvoiceActualcostAll()
        {
            using (var client = new HttpClient())
            {
                List<tb_r_invoice_actualcost> ListItem = new List<tb_r_invoice_actualcost>();
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TableInvoiceActualcost");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_r_invoice_actualcost> ResponseList = new List<tb_r_invoice_actualcost>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_r_invoice_actualcost>>(str);

                    foreach (var item in ResponseList)
                    {
                        ListItem.Add(item);
                    }
                }
                if (ListItem.Count > 0) return ListItem.OrderBy(m => m.GR_issued_flag).ToList();
                else return ListItem;

            }

        }

        public static async Task<List<tb_r_invoice_actualcost>> TableInvoiceActualcostSingle(string bta, string transaction)
        {
            using (var client = new HttpClient())
            {
                List<tb_r_invoice_actualcost> ListItem = new List<tb_r_invoice_actualcost>();
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TableInvoiceActualcost/BTA?bta=" + bta + "&transaction=" + transaction);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_r_invoice_actualcost> ResponseList = new List<tb_r_invoice_actualcost>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_r_invoice_actualcost>>(str);
                    ListItem = ResponseList;
                }
                return ListItem;
            }

        }

        public static async Task<vw_invoice_actualcost_new> InvoiceActualCostNewID(int id)
        {
            using (var client = new HttpClient())
            {
                vw_invoice_actualcost_new ListItem = new vw_invoice_actualcost_new();
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/InvoiceActualcostNew");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_invoice_actualcost_new> ResponseList = new List<vw_invoice_actualcost_new>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_invoice_actualcost_new>>(str);

                    foreach (var item in ResponseList)
                    {
                        if (item.id_data == id) ListItem = item;
                    }
                }
                return ListItem;
            }

        }

        public static async Task<List<vw_invoice_actualcost_new>> InvoiceActualCostNewBTA(string bta, string transaction)
        {
            using (var client = new HttpClient())
            {
                List<vw_invoice_actualcost_new> ListItem = new List<vw_invoice_actualcost_new>();
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/InvoiceActualcostNew/BTA?bta=" + bta + "&transaction=" + transaction);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    ListItem = JsonConvert.DeserializeObject<List<vw_invoice_actualcost_new>>(str);
                }
                return ListItem;
            }

        }

        public static async Task<List<tb_r_travel_actualcost>> ActualCostBTA(string group_code)
        {
            List<tb_r_travel_actualcost> Response = new List<tb_r_travel_actualcost>();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/ActualCost/GetBTA?gcode=" + group_code);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    Response = JsonConvert.DeserializeObject<List<tb_r_travel_actualcost>>(EmpResponse);
                }
            }
            return Response;
        }

        // 18/5/2018 adding ActualCostBTAActive (for getting active actual cost on certain BTA)
        public static async Task<List<tb_r_travel_actualcost>> ActualCostBTAActive(string group_code)
        {
            List<tb_r_travel_actualcost> Response = new List<tb_r_travel_actualcost>();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/ActualCost/GetBTA?gcode=" + group_code);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    List<tb_r_travel_actualcost> newList = new List<tb_r_travel_actualcost>();
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    newList = JsonConvert.DeserializeObject<List<tb_r_travel_actualcost>>(EmpResponse);
                    foreach (var item in newList)
                    {
                        if (item.final_status == null) Response.Add(item);
                    }
                }
            }
            return Response;
        }

        public static async Task<List<tb_r_travel_execution>> TravelExecution(string bta)
        {
            using (var client = new HttpClient())
            {
                List<tb_r_travel_execution> ListItem = new List<tb_r_travel_execution>();
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TravelExecution/BTA?bta=" + bta);

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<tb_r_travel_execution> ResponseList = new List<tb_r_travel_execution>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<tb_r_travel_execution>>(str);
                    foreach (var item in ResponseList)
                    {
                        if (item.group_code.Contains(bta.Trim(' '))) ListItem.Add(item);
                    }
                }
                return ListItem;
            }


        }

        public static async Task<List<vw_payment_list>> PaymentListData()
        {
            List<vw_payment_list> ResponseList = new List<vw_payment_list>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/PaymentList/");
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_payment_list>>(str);
                }

            }
            return ResponseList;
        }

        public static async Task<List<vw_payment_list>> PaymentListDataFiltered(string search, DateTime? start, DateTime? end)
        {
            List<vw_payment_list> ResponseList = new List<vw_payment_list>();
            List<vw_payment_list> FilteredList = new List<vw_payment_list>();
            List<vw_payment_list> ReturnList = new List<vw_payment_list>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/PaymentList");
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_payment_list>>(str);
                }
                foreach (var item in ResponseList)
                {
                    if (item.BTR_NO.ToLower().Contains(search) ||
                        item.EMPLOYEE_NAME.ToLower().Contains(search) ||
                        item.DESTINATION.ToLower().Contains(search) ||
                        item.ID_CITY.ToString().Contains(search) ||
                        item.COST_CENTER_.ToLower().Contains(search) ||
                        item.WBS_ELEMENT_.ToLower().Contains(search) ||
                        item.TRAVEL_TYPE.ToLower().Contains(search) ||
                        item.BUDGET.ToLower().Contains(search)
                        )
                    {
                        FilteredList.Add(item);
                    }
                }

                if (start.HasValue && end.HasValue)
                {
                    foreach (var item in FilteredList)
                    {
                        DateTime start_date = new DateTime();
                        DateTime end_date = new DateTime();
                        start_date = DateTime.ParseExact(item.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        end_date = DateTime.ParseExact(item.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        if (start_date >= start && end_date <= end) ReturnList.Add(item);
                    }
                }
                else if (start.HasValue)
                {
                    foreach (var item in FilteredList)
                    {
                        DateTime start_date = new DateTime();
                        DateTime end_date = new DateTime();
                        start_date = DateTime.ParseExact(item.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        end_date = DateTime.ParseExact(item.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        if (start_date >= start) ReturnList.Add(item);
                    }

                }
                else if (end.HasValue)
                {
                    foreach (var item in FilteredList)
                    {
                        DateTime start_date = new DateTime();
                        DateTime end_date = new DateTime();
                        start_date = DateTime.ParseExact(item.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        end_date = DateTime.ParseExact(item.PV_DATE, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        if (end_date <= end) ReturnList.Add(item);
                    }
                }
                else ReturnList = FilteredList;
            }
            return ReturnList;
        }

        public static async Task<List<vw_payment_proposal>> PaymentProposalData()
        {
            List<vw_payment_proposal> ResponseList = new List<vw_payment_proposal>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/PaymentProposal/");
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_payment_proposal>>(str);
                }

            }
            return ResponseList;
        }

        public static async Task<List<vw_payment_proposal>> PaymentProposalDataFiltered(string search, DateTime? start, DateTime? end)
        {
            List<vw_payment_proposal> ResponseList = new List<vw_payment_proposal>();
            List<vw_payment_proposal> FilteredList = new List<vw_payment_proposal>();
            List<vw_payment_proposal> ReturnList = new List<vw_payment_proposal>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constant.Baseurl);
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/PaymentProposal");
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_payment_proposal>>(str);
                }
                foreach (var item in ResponseList)
                {
                    if (//item.id_data.Contains(search) ||
                        item.vendor_code.ToLower().Contains(search) ||
                        item.currency.ToLower().Contains(search) ||
                        item.total_amount.ToString().Contains(search) ||
                        item.beneficiary_name.ToLower().Contains(search) ||
                        item.account_number.ToLower().Contains(search) ||
                        item.employee_name.ToLower().Contains(search) ||
                        item.refference.ToLower().Contains(search)
                        )
                    {
                        FilteredList.Add(item);
                    }
                }

                if (start.HasValue && end.HasValue)
                {
                    foreach (var item in FilteredList)
                    {
                        DateTime start_date = new DateTime();
                        DateTime end_date = new DateTime();
                        start_date = DateTime.ParseExact(item.vendor_code, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        end_date = DateTime.ParseExact(item.vendor_code, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        if (start_date >= start && end_date <= end) ReturnList.Add(item);
                    }
                }
                else if (start.HasValue)
                {
                    foreach (var item in FilteredList)
                    {
                        DateTime start_date = new DateTime();
                        DateTime end_date = new DateTime();
                        start_date = DateTime.ParseExact(item.vendor_code, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        end_date = DateTime.ParseExact(item.vendor_code, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        if (start_date >= start) ReturnList.Add(item);
                    }

                }
                else if (end.HasValue)
                {
                    foreach (var item in FilteredList)
                    {
                        DateTime start_date = new DateTime();
                        DateTime end_date = new DateTime();
                        start_date = DateTime.ParseExact(item.vendor_code, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        end_date = DateTime.ParseExact(item.vendor_code, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        if (end_date <= end) ReturnList.Add(item);
                    }
                }
                else ReturnList = FilteredList;
            }
            return ReturnList;
        }


        public static async Task<vw_tracking_transaction_data_new> TrackingListID(int id)
        {
            vw_tracking_transaction_data_new ListItem = new vw_tracking_transaction_data_new();
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage response = await client.GetAsync("api/TrackingTransactionDataNew");

                //Checking the response is successful or not which is sent using HttpClient  
                if (response.IsSuccessStatusCode)
                {
                    List<vw_tracking_transaction_data_new> ResponseList = new List<vw_tracking_transaction_data_new>();
                    var str = response.Content.ReadAsStringAsync().Result;
                    ResponseList = JsonConvert.DeserializeObject<List<vw_tracking_transaction_data_new>>(str);

                    int k = 1;

                    foreach (var item in ResponseList)
                    {
                        var temp = new vw_tracking_transaction_data_new();

                        if (item.id_data == id)
                        {
                            ListItem = item;
                        }
                        k++;
                    }
                }
            }

            return ListItem;
        }

        public static async Task<List<tb_m_special_employee>> SpecialEmployee(string no_reg)
        {
            List<tb_m_special_employee> model = new List<tb_m_special_employee>();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/SpecialEmployee/GetEmployee?id=" + no_reg);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    model = JsonConvert.DeserializeObject<List<tb_m_special_employee>>(EmpResponse);
                }
            }
            return model;
        }

        public static async Task<vw_payment_list> PaymentList(int id)
        {
            vw_payment_list Response = new vw_payment_list();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  

                HttpResponseMessage Res = await client.GetAsync("api/PaymentList/" + id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    Response = JsonConvert.DeserializeObject<vw_payment_list>(EmpResponse);
                }
            }
            return Response;
        }

        public static async Task<vw_payment_proposal> PaymentProposal(int id)
        {
            vw_payment_proposal Response = new vw_payment_proposal();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/PaymentProposal/" + id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    Response = JsonConvert.DeserializeObject<vw_payment_proposal>(EmpResponse);

                }
            }
            return Response;
        }

        public static async Task<vw_actualcost_generate_file> GenerateFile(int id)
        {
            vw_actualcost_generate_file Response = new vw_actualcost_generate_file();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/ActualcostGenerateFile/" + id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    Response = JsonConvert.DeserializeObject<vw_actualcost_generate_file>(EmpResponse);
                }
            }
            return Response;
        }

        public static async Task<vw_tracking_transaction_data_new> DownloadFileTrack(int id)
        {
            vw_tracking_transaction_data_new Response = new vw_tracking_transaction_data_new();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/TrackingTransactionDataNew/" + id);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    Response = JsonConvert.DeserializeObject<vw_tracking_transaction_data_new>(EmpResponse);
                }
            }
            return Response;
        }

        public static async Task<tb_m_special_employee_new> GetSpecialNoreg(string id)
        {
            tb_m_special_employee_new Response = new tb_m_special_employee_new();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Constant.Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/SpecialEmployeeNew/GetID?key=" + id.Trim(' '));

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    Response = JsonConvert.DeserializeObject<tb_m_special_employee_new>(EmpResponse);
                }
            }
            return Response;
        }

    }
}
    