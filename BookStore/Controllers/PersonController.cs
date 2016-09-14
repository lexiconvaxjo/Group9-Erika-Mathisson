using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static BookStore.App_Start.IdentityConfig;
using static BookStore.App_Start.IdentityConfig.AppUserManager;

namespace BookStore.Controllers
{
    public class PersonController : Controller
    {
        /// <summary>
        /// empty constructor for controller
        /// </summary>
        public PersonController()
        {

        }


        /// <summary>
        /// setting parameters
        /// </summary>
        /// <param name="role">role for adding to a user</param>
        /// <param name="userManager">usermanager for a user</param>
        /// <param name="signIn">signin manager</param>
        public PersonController(AppRole role, AppUserManager userManager, AppSignIn signIn)
        {
            _role = role;
            _signIn = signIn;
            _userManager = userManager;
        }

        //property for rolemanager and get and set method
        private AppRole _role;
        public AppRole RoleManager
        {
            //?? check if object is null in that case get role from OwinContext
            get { return _role ?? HttpContext.GetOwinContext().Get<AppRole>(); }
            set { _role = value; }
        }

        //property for usermanager and get and set method
        private AppUserManager _userManager;
        public AppUserManager UserManager
        {
            // check if object is null in that case get usermanager from OwinContext            
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
            set { _userManager = value; }
        }

        // property for signInManager and get and set method
        private AppSignIn _signIn;
        public AppSignIn SignIn
        {
            // check if object is null in that case get signinmanager from owincontext
            get { return _signIn ?? HttpContext.GetOwinContext().Get<AppSignIn>(); }
            set { _signIn = value; }
        }

        // GET: Person
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// function for register a person to the database
        /// </summary>
        /// <param name="user">person whom should be registered</param>
        /// <returns>Json</returns>
        [AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> RegisterPerson([Bind(Include = "UserName, FirstName, LastName, PassWord, ConfirmPassword, Email, Address, ZipCode, City, PhoneNumber")] RegisterUserViewModel user)
        {
            // check if all information added ok
            if (ModelState.IsValid)
            {
                // check if entered email already exist, email should be unique
                var checkEmail = await UserManager.FindByEmailAsync(user.Email);
                // email already exist in database return error code
                if (checkEmail != null)
                {
                    return Json("EmailExists");
                }

                //check if entered UserName already exist, UserName should be unique
                var userNameCheck = await UserManager.FindByNameAsync(user.UserName);
                // userName already exist in database return error code
                if (userNameCheck != null)
                {
                    return Json("UserNameExists");
                }

                // create a new user
                var person = new User
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Address = user.Address,
                    ZipCode = user.ZipCode,
                    City = user.City,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };

                //try and save the user to database
                var result = await UserManager.CreateAsync(person, user.Password);

                //the user was not saved ok               
                if (!result.Succeeded)
                {
                    // retrieve information about what went wrong and send message to page
                    string message = "";
                    if (result.Errors != null)
                    {
                        foreach (var error in result.Errors)
                        {
                            message = message + error.ToString() + " ";
                        }
                    }
                    return Json(message);
                }
                else
                {
                    // fetch the registered user and assign a role
                    var registeredUser = await UserManager.FindByNameAsync(user.UserName);

                    // fetch all people that exist in database
                    var Peoples = UserManager.Users.ToList();
                    // if only one person exist in the database this person should be assigned the role admin
                    if (Peoples.Count == 1)
                    {
                        // check if the role Admin exist in database, if it doesn't create it
                        if (!RoleManager.RoleExists("Admin"))
                        {
                            RoleManager.Create(new IdentityRole("Admin"));
                        }
                        // check if the role User exist in database, if it doesn't create it
                        if (!RoleManager.RoleExists("User"))
                        {
                            RoleManager.Create(new IdentityRole("User"));
                        }
                        // add the role admin to the registered user since it's the first user the role assigned
                        // should be admin
                        await UserManager.AddToRoleAsync(registeredUser.Id, "Admin");
                        user.Admin = true;
                    }
                    else
                    {
                        // check if user should have role "admin" or "user" and assign role to user
                        if (user.Admin == true)
                        {
                            await UserManager.AddToRoleAsync(registeredUser.Id, "Admin");
                        }
                        else
                        {
                            await UserManager.AddToRoleAsync(registeredUser.Id, "User");
                        }
                    }

                    //the user is registered and saved ok to database, log in automatically
                    //LogInViewModel vm = new LogInViewModel();
                    //vm.UserName = model.UserName;
                    //vm.Password = model.Password;
                    //await Login(vm);
                    //return RedirectToAction("Index", "Home");

                    return Json("Success");
                }
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return Json(message);

            }

            //    return Json("NonValid");

        }

        /// <summary>
        /// function for logging in a person
        /// </summary>
        /// <param name="user">User whom should be logged in</param>
        /// <returns>Json</returns>
        public async System.Threading.Tasks.Task<JsonResult> LogInPerson([Bind(Include = "UserName, PassWord")] LogInPersonViewModel user)
        {
            //check if information entered ok
            if (ModelState.IsValid)
            {
                // trying to log in the user
                var status = await SignIn.PasswordSignInAsync(user.UserName, user.Password,
                    isPersistent: false, shouldLockout: true);
                // check if user is logged in ok
                switch (status)
                {
                    //user logged in, return to home page
                    case SignInStatus.Success:
                        string role = (User.IsInRole("Admin")) ? "Admin" : "User";
                        return Json(new { status = "Success", role = role });
                    //user not logged in, show information for the user
                    case SignInStatus.Failure:
                        return Json(new { status = "Failure", role = "" });
                    default:
                        break;
                }
                return Json("Failure");
            }
            else
            {
                // user didn't enter information correct
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return Json(message);
            }

        }

        /// <summary>
        /// Action for logging out a user and redirect to home page
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOutUser()
        {
            SignIn.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// function for getting all the people in the database
        /// </summary>
        /// <param name="user">User whom should be logged in</param>
        /// <returns>Json</returns>
        public JsonResult GetPeople()
        {
            //fetch peoples with attributes as EditUserVIewModel           
            var people = UserManager.Users.ToList().Select(x => new EditUserVIewModel
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName,
                Address = x.Address,
                City = x.City,
                ZipCode = x.ZipCode,
                Email = x.Email,
                Id = x.Id
            }).ToList();

            // return peoples, allowing the method to be HttpGet
            return Json(people, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult IsLoggedIn()
        {
            if (User.Identity.IsAuthenticated)
            {
                var role = (User.IsInRole("Admin")) ? "Admin" : "User";
                return Json(new { status = true, role = role });

            }
            return Json(null);

        }
    }
}