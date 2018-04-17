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
using System.Globalization;
using System.Security.Claims;

namespace CONTRAST_WEB.Controllers
{

    public class TravelExecutionController : Controller
    {

        public async Task<JsonResult> GetSearchValue2(string search, string code)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            tb_m_employee_source_data div = await GetData.GetDivisionSource(Convert.ToInt32(identity.Name));
            if (div.Divisi.Contains("and1"))
            {
                div.Divisi = div.Divisi.Replace("and1", "&");
            }
            List<Class1> list = new List<Class1>();
            list = await GetData.SearchNameDiv(search, div.Divisi);
            //list = await GetData.SearchName(search);
            List<Class1> filtered = new List<Class1>();
            foreach (var item in list)
            {
                if (!item.code.Contains(code)) filtered.Add(item);
            }

            return new JsonResult { Data = filtered, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public async Task<JsonResult> GetSearchValue(string search, string code)
        {
            List<Class1> list = new List<Class1>();
            list = await GetData.SearchName(search);
            List<Class1> filtered = new List<Class1>();
            foreach (var item in list)
            {
                if (!item.code.Contains(code)) filtered.Add(item);
            }

            return new JsonResult { Data = filtered, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string applied)
        {
            var identity = (ClaimsIdentity)User.Identity;
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;
            tb_m_employee model = new tb_m_employee();

            if (applied != null) model = await GetData.EmployeeInfo(applied);
            else model = await GetData.EmployeeInfo(identity.Name);

            ViewBag.Employee = model;
            ViewBag.applied = model.code;

            List<vw_travel_execution_list> RequestSummary = new List<vw_travel_execution_list>();
            RequestSummary = await GetData.ExecutionList(model.code);
            return View(RequestSummary);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Details(vw_travel_execution_list model)
        {
            TravelExecutionHelper TravelExecutionHelperObject = new TravelExecutionHelper();
            TravelExecutionHelperObject.TravelRequest = model;
            return View(TravelExecutionHelperObject);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(TravelExecutionHelper model)
        {
            string path=null;
            model.TravelExecution = new tb_r_travel_execution();
            model.TravelExecution.group_code = model.TravelRequest.group_code.Trim();
            model.TravelExecution.desc_upload = "description";
            model.TravelExecution.execution_date = DateTime.Now;
            model.TravelExecution.user_created = model.TravelRequest.no_reg.ToString();

            HttpPostedFileBase file=null;
            

            if (Request != null)
            {
                file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    fileName = model.TravelRequest.no_reg + "_" + DateTime.Now.ToLongDateString() + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + "_" + fileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];

                    path = Path.Combine(Server.MapPath(Constant.TravelExecutionReceiptFolder), fileName);
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                    model.TravelExecution.pic_path = path;                    
                    model.TravelExecution.uploaded = 1;
                    model.TravelExecution.numbers = 0;                                    
                }
            }

            if (model.Templat != null&&model.Templong!=null)
            {
                model.TravelExecution.latitude = float.Parse(model.Templat, CultureInfo.InvariantCulture.NumberFormat);
                model.TravelExecution.longitude = float.Parse(model.Templong, CultureInfo.InvariantCulture.NumberFormat);
            }


            if (path != null || (model.TravelExecution.latitude != null && model.TravelExecution.longitude != null))
            {
                if (model.TravelExecution.latitude == null) model.TravelExecution.latitude = 0.0d;
                if (model.TravelExecution.longitude == null) model.TravelExecution.longitude = 0.0d;

                model.TravelExecution.status = "1";
                await InsertData.TravelExecution(model.TravelExecution);
                if (path != null) file.SaveAs(path);
            }
            else
            {
                model.error_string = "Invalid entry : You need to at least upload a picture proof or tag valid travel location";
                return View("Details",model);
            }

            List<vw_travel_execution_list> RequestSummary = new List<vw_travel_execution_list>();
            RequestSummary = await GetData.ExecutionList(model.TravelRequest.no_reg.ToString());
            return View("Index", RequestSummary);
        }
    }
}