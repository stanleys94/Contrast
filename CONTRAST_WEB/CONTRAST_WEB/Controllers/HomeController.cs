using CONTRAST_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Owin.Security;
using CONTRAST_WEB.CustomValidator;

namespace CONTRAST_WEB.Controllers
{
     
    public class HomeController : Controller
    {

        private CONTRASTEntities db = new CONTRASTEntities();

        [Authorize(Roles = "contrast.user")]       
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            Utility.Logger(identity.Name);
            string[] claims = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            ViewBag.Privillege = claims;

            tb_m_employee model = await GetData.EmployeeInfo(identity.Name);


            //tb_m_employee model = await GetData.EmployeeInfo(identity.Name);
            //AP
            //tb_m_employee model = await GetData.EmployeeInfo("101419");
            //staff ga
            //tb_m_employee model = await GetData.EmployeeInfo("101495");
            // dph ga 100354  
            //tb_m_employee model = await GetData.EmployeeInfo("100354");
            //percobaan
            //tb_m_employee model = await GetData.EmployeeInfo("100626");
            //tb_m_employee model = await GetData.EmployeeInfo("101795");

            ViewBag.photo = await GetData.PhotoEmployeeInfo(identity.Name);
            ViewBag.Username = model.name;
        
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "contrast.user")]     
        public async System.Threading.Tasks.Task<ActionResult> Back(tb_m_employee model)
        {
            ViewBag.Username = model.name;
            return View("Index",model);
        }
        
       
        [HttpPost]
        [Authorize(Roles = "contrast.user")]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Submitted(TravelRequestHelper[] ListModel)
        {
            for (int k = 0; k < ListModel.Count(); k++)
            {              

                await InsertData.TravelRequest(ListModel[k]);
                if (ListModel[k].travel_request.participants_flag == true)
                {
                    if (k <= ListModel.Count())
                    {
                        for (int i = 0; i < ListModel[k].participants.Count; i++)
                            await InsertData.TravelParticipant(ListModel[k].participants[i]);
                    }
                }
            }
            ViewBag.Username = ListModel[0].employee_info.name;
            //return View("Index", ListModel[0].employee_info);
            return RedirectToAction("Index");
        }
         
    }
}
