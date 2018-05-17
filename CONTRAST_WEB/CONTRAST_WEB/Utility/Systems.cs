using CONTRAST_WEB.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace CONTRAST_WEB.Models
{
    public static class Systems
    {
        public static async Task<IdentityModel> Identity(HttpContextBase httpContext)
        {
            IdentityModel model = new IdentityModel();
            model.ClaimedIdentity = (ClaimsIdentity)httpContext.User.Identity;            
            model.Privillege = model.ClaimedIdentity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();

            return model;
        }
    }
}