using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CONTRAST_WEB.Controllers
{
    public class LinkController : Controller
    {
        // GET: Link
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Link(string mstr = "")
        {
            string website = "";
            if (mstr == "ipad") website = "mstr://?url=https%3A%2F%2Fdashboard.toyota.astra.co.id%3A443%2FMicroStrategyMobile%2Fasp%2FTaskProc.aspx%3FtaskId%3DgetMobileConfiguration%26taskEnv%3Dxml%26taskContentType%3Dxmlanf%26configurationID%3Da384e6f7-3e6f-4e2e-a9a8-c34190565525&authMode=1&dt=2";
            else if (mstr == "iphone") website = "mstr://?url=https%3A%2F%2Fdashboard.toyota.astra.co.id%3A443%2FMicroStrategyMobile%2Fasp%2FTaskProc.aspx%3FtaskId%3DgetMobileConfiguration%26taskEnv%3Dxml%26taskContentType%3Dxmlanf%26configurationID%3Db422b9ef-b584-477a-bda5-c865db45b8b0&authMode=1&dt=1";
            else if (mstr == "android") website = "mstr://?url=https%3A%2F%2Fdashboard.toyota.astra.co.id%3A443%2FMicroStrategyMobile%2Fasp%2FTaskProc.aspx%3FtaskId%3DgetMobileConfiguration%26taskEnv%3Dxhr%26taskContentType%3Djson%26configurationID%3D31ec2e4a-ea6c-49e4-93c5-ab77b1e11844%26blockTransform%3Dflatten%26blockVersion%3D1&authMode=1&dt=3";
            else if (mstr == "website") website = "http://dashboard.toyota.astra.co.id/MicroStrategy/asp/Main.aspx?evt=2048001&src=Main.aspx.2048001&documentID=FC1D8A7C4B5E950637932592195FBF7D&currentViewMedia=1&visMode=0&Server=VSV-C003-016102&Project=CONTRAST&Port=0&share=1";
            return Redirect(website);
        }
    }
}