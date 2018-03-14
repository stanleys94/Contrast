using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;

[assembly: OwinStartupAttribute(typeof(CONTRAST_WEB.Startup))]
namespace CONTRAST_WEB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                ExpireTimeSpan = new TimeSpan(60000000000)
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            ConfigureAuth(app);
        }

    }
}
