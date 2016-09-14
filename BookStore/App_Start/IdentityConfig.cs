using BookStore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.App_Start
{
    public class IdentityConfig
    {
        public class AppUserManager : UserManager<User>
        {
            public AppUserManager(IUserStore<User> store) : base(store)
            {

            }
            /// <summary>
            /// function for creating an instance of a usermanager without creating a new one every time
            /// setting properties for password
            /// </summary>
            /// <param name="options"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            public static AppUserManager CreateUserManagerInstance(IdentityFactoryOptions<AppUserManager> options, 
                IOwinContext context)
            {
                var um = new AppUserManager(new UserStore<User> (context.Get<DB>()));
                // username can use alphanumeric, email needs to be unique for every user
                um.UserValidator = new UserValidator<User>(um)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = true
                };
                //properties for password
                um.PasswordValidator = new PasswordValidator
                {
                    RequireDigit = false,
                    RequiredLength = 4,                  
                    RequireLowercase = false,
                    RequireNonLetterOrDigit = false,
                    RequireUppercase = false                    
                };
                //user should be locked out after 5 faulty tries and locked out for 5 minutes
                um.UserLockoutEnabledByDefault = true;
                um.MaxFailedAccessAttemptsBeforeLockout = 5;
                um.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
                return um;
            }

            /// <summary>
            /// Class for creating a SignInManager
            /// </summary>
            public class AppSignIn : SignInManager<User, string>
            {
                public AppSignIn(AppUserManager userManager, IAuthenticationManager authenticationManager): 
                    base(userManager, authenticationManager)
                {

                }

                // creating an instance of SignInManager so that it don't need to be instantiated every time
                public static AppSignIn CreateSignInInstance(IdentityFactoryOptions<AppSignIn> options,
                    IOwinContext context)
                {
                    return new AppSignIn(context.GetUserManager<AppUserManager>(), context.Authentication);
                }
            }           
        }

        //class for application roles
        public class AppRole : RoleManager<IdentityRole>
        {
            public AppRole(IRoleStore<IdentityRole, string> store) : base(store)
            {
            }
            //creating an instans of a rolestore so it don't need to be instantiated every time
            public static AppRole CreateRoleInstance(IdentityFactoryOptions<AppRole> options, IOwinContext context)
            {
                var rs = new RoleStore<IdentityRole>(context.Get<DB>());
                return new AppRole(rs);
            } 
        }
    }
}