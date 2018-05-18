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
            model.AutoFillEmployeeInfo(
                //get employee info
                await GetData.EmployeeInfo(model.Identity.ClaimedIdentity.Name),
                //get employee division
                await model.GetEmployeeInfoDivision(model.Identity.ClaimedIdentity.Name),
                //get employee assigned by
                await Utility.AssignedBy(await GetData.EmployeeInfo(model.Identity.ClaimedIdentity.Name))
                );

            List<TravelAssignmentDTO> model2 = new List<TravelAssignmentDTO>();
            model2.Add(model);
            //

            return View(model2);
        }
    }
}