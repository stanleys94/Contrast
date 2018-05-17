using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace CONTRAST_WEB.Helper
{
    public class IdentityModel
    {
        public ClaimsIdentity ClaimedIdentity { get; set; }
        public string[] Privillege { get; set; }
    }
}