using CONTRAST_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace CONTRAST_WEB.Controllers
{
    public class PhotoEmployeeController : Controller
    {
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [CustomValidator.NoCache] 
        // GET: PhotoEmployee
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);  // HTTP 1.1.
            Response.Cache.AppendCacheExtension("no-store, must-revalidate");
            Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
            Response.AppendHeader("Expires", "0"); // Proxies.

            var identity = (ClaimsIdentity)User.Identity;
            tb_m_photo_employee photo_info = new tb_m_photo_employee();
            photo_info = await GetData.PhotoEmployee(identity.Name);
            return View(photo_info);
        }

        [CustomValidator.NoCache]
        public async System.Threading.Tasks.Task<ActionResult> Edit(tb_m_photo_employee model)
        {
            var identity = (ClaimsIdentity)User.Identity;
            tb_m_photo_employee photo_info = new tb_m_photo_employee();
            photo_info = await GetData.PhotoEmployee(identity.Name);

            return View(photo_info);
        }

        [CustomValidator.NoCache]
        public async System.Threading.Tasks.Task<ActionResult> Upload()
        {
            var identity = (ClaimsIdentity)User.Identity;
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["file"];
                if (file.ContentType != "image/jpeg")
                {
                     
                    ViewBag.error_list = "Upload jpeg image ONLY";
                    return View("Edit");
                }
                //string path_file = Utility.UploadPhoto(file, identity.Name);
                string path_file = Utility.UploadPhoto(file, identity.Name);
                path_file = Constant.PhotoFolder+path_file;
                tb_m_photo_employee photo_info = new tb_m_photo_employee();
                photo_info = await GetData.PhotoEmployee(identity.Name);

                //photo_info.photo_emp.Trim();
                //photo_info.status_photo.Trim();
                photo_info.insert_date = DateTime.Now;
                //photo_info.code.Trim();

                photo_info.photo_emp = path_file;

                await UpdateData.PhotoIcon(photo_info);
                ModelState.Clear();

            }
            return RedirectToAction ("Index");
        }

    }
}