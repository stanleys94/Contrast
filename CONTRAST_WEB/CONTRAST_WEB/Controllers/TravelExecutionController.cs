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

namespace CONTRAST_WEB.Controllers
{

    public class TravelExecutionController : Controller
    {

        // GET: TravelExecution
        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Index(tb_m_employee model)
        {
            
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