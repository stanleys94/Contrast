using CONTRAST_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CONTRAST_WEB.Controllers
{
    public class InvokeCubeController : Controller
    {
        // GET: InvokeCube
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            await Utility.InvokeCube();
            return View();
        }
    }
}