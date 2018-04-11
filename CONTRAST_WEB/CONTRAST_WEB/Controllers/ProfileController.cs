using CONTRAST_WEB.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace CONTRAST_WEB.Controllers
{
    public class ProfileController : Controller
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

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public ActionResult DownloadUG()
        {
            string file = HostingEnvironment.MapPath("~/Files/User_Manual_Contrast.docx");
            string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            return File(file, contentType, Path.GetFileName(file));
        }

    }
}
