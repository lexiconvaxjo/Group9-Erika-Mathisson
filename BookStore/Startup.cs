using BookStore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static BookStore.App_Start.IdentityConfig;
using static BookStore.App_Start.IdentityConfig.AppUserManager;

[assembly: OwinStartup(typeof(BookStore.Startup))]

namespace BookStore
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            app.CreatePerOwinContext(Models.DB.CreateConnection);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.CreateUserManagerInstance);
            app.CreatePerOwinContext<AppSignIn>(AppSignIn.CreateSignInInstance);
            app.CreatePerOwinContext<AppRole>(AppRole.CreateRoleInstance);

            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity =
                    SecurityStampValidator.OnValidateIdentity<AppUserManager, User>(
                         validateInterval: TimeSpan.FromMinutes(30),
                         regenerateIdentity: (manager, user) => user.GenerateUser(manager))   
                }
            });
        }
    }
}