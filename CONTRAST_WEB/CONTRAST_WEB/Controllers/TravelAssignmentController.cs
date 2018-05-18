using CONTRAST_WEB.DTO;
using CONTRAST_WEB.Models; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CONTRAST_WEB.Controllers
{
    public class TravelAssignmentController : Controller
    {
        // GET: TravelAssignment
        [Authorize]
        [Authorize(Roles = "contrast.user")]
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            TravelAssignmentDTO model = new TravelAssignmentDTO();
            model.SetIdentity(HttpContext);
            model.AutoFillEmployeeInfo(await GetData.EmployeeInfo(model.Identity.ClaimedIdentity.Name), await model.GetEmployeeInfoDivision(model.Identity.ClaimedIdentity.Name));
            //

            return View(model);
        }
    }
}