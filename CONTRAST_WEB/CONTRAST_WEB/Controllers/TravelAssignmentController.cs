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
        public async System.Threading.Tasks.Task<ActionResult> Index(string participant, TravelAssignmentDTO posted,string delete)
        {
            TravelAssignmentDTO model = new TravelAssignmentDTO();
            model.SetIdentity(HttpContext);
            await model.AutoFillEmployeeInfo();

            if (participant != null && delete == null)
            {
                posted.Identity = model.Identity;
                model = posted;
                model.AddParticipant(participant);
                ModelState.Remove(ModelState.FirstOrDefault(ms => ms.Key.ToString().StartsWith("participant")));
            }
            else
            if (delete != null)
            {
                posted.Identity = model.Identity;
                model = posted;
                model.DeleteParticipant(Convert.ToInt32(delete));
                ModelState.Clear();
            }
            
            List<TravelAssignmentDTO> model2 = new List<TravelAssignmentDTO>();
            model2.Add(model);
            
            return View(model2);
        }

        [Authorize]
        [Authorize(Roles = "contrast.user")]
        public async System.Threading.Tasks.Task<ActionResult> Validate(TravelAssignmentDTO model)
        {
            
            return View(model);
        }


    }
}