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
                    //user registered ok, send info to page
                    return Json("Success");
                }
            }
            else
            {
                // modelstate isn't valid, take all errors and save in a string, send to page
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return Json(message);
            }
        }

        /// <summary>
        /// Action for editing a person
        /// </summary>
        /// <param name="user">the person whom should be edited</param>
        /// <returns>Json</returns>
        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> EditPerson(
            [Bind(Include = "Id, UserName, FirstName, LastName, PassWord, ConfirmPassword, Email, Address, City, ZipCode, PhoneNumber, Admin")]
            EditUserVIewModel user)
        {
            //check if model is valid
            if (ModelState.IsValid)
            {
                // fetch user from database
                var CurrentUser = await UserManager.FindByEmailAsync(user.Email);

                //try to add the changes on the person to the database   
                CurrentUser.FirstName = user.FirstName;
                CurrentUser.LastName = user.LastName;
                CurrentUser.Email = user.Email;
                CurrentUser.Address = user.Address;
                CurrentUser.City = user.City;
                CurrentUser.ZipCode = user.ZipCode;
                CurrentUser.PhoneNumber = user.PhoneNumber;

                //try and save the user to database
                var result = await UserManager.UpdateAsync(CurrentUser);

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
                    return Json(new { status = "Failure", message = message });
                }
                else
                {
                    // fetch the registered user and assign a role
                    var registeredUser = await UserManager.FindByNameAsync(user.UserName);
                    string role = null;
                    // check what type of role the user should have, Admin or User, and assign the correct one.
                    if (user.Admin == true)
                    {
                        await UserManager.AddToRoleAsync(registeredUser.Id, "Admin");
                        role = "Admin";
                    }
                    else
                    {
                        await UserManager.AddToRoleAsync(registeredUser.Id, "User");
                        role = "User";
                    }

                    // return status, edit went well and what role the user have
                    return Json(new { status = "Success", role = role });                   
                }
            }
            else
            {
                //modelstate isn't valid, fetch error messages and send to page
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                
                return Json(new { status = "Failure", message = message });
            }
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
                        string role = null;
                        // await a little bit slow needs to fetch user for getting correct information about user role 
                        User currentUser = UserManager.FindByName(user.UserName);
                        //check if the user has the role "Admin"
                        bool admin = UserManager.IsInRole(currentUser.Id, "Admin");
                        // assign role with correct role
                        role = (admin) ? "Admin" : "User";
                        return Json(new { status = "Success", role = role, userName = currentUser.UserName });

                    //user not logged in, show information for the user
                    case SignInStatus.Failure:
                        return Json(new { status = "Failure", role = "", userName = "" });
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
                PhoneNumber = x.PhoneNumber,               
                Id = x.Id
            }).ToList();
            // for every person in the list, check what type of role the user has and assign the role.
            foreach (var person in people)
            {
                var role = UserManager.GetRoles(person.Id);
                // check if user has the role Admin and assign it to each person
                if (role.Contains("Admin"))
                {
                    person.Admin = true;
                }
                else
                {
                    person.Admin = false;
                }
            }

            // return peoples, allowing the method to be HttpGet
            return Json(people, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// function for getting information about a single user
        /// </summary>
        /// <param name="userName">user name of user whom should be fetched</param>
        /// <returns>Json</returns>       
        [HttpPost]
        public JsonResult GetPerson()
        {            
            User person = UserManager.FindByName(User.Identity.Name);
            return Json(person);
        }

        /// <summary>
        /// Function for checking if if user is logged in and what role the user has.
        /// </summary>
        /// <returns>Json</returns>
        [HttpPost]
        public JsonResult IsLoggedIn()
        {
            //check if the user is authenticated that is logged in
            if (User.Identity.IsAuthenticated)
            {
                // check what role the user has admin or user
                var role = (User.IsInRole("Admin")) ? "Admin" : "User";
                // get the logged in users user name
                var userName = User.Identity.Name;
                // return that user is logged in, what role the user has and the user name
                return Json(new { status = true, role = role, userName = userName });

            }
            // the user isn't logged in
            return Json(null);
        }

        /// <summary>
        /// function for changing password
        /// </summary>
        /// <param name="user">user whom should change password</param>
        /// <returns>Json</returns>
        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> ChangePassword([Bind(Include = "CurrentPassword, NewPassword, ConfirmNewPassword")] ChangePasswordViewModel user)
        {
            // check if the model is ok
            if (ModelState.IsValid)
            {
                //try and change the users password by entering both old and new password
                var status = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), user.CurrentPassword, user.NewPassword);
                // the password changed ok
                if (status.Succeeded)
                {
                    // return the success status to the page
                    return Json(new { status = "Success" });
                }
                else
                {
                    // retrieve information about what went wrong when trying to change the password
                    // and send message to page
                    string message = "";
                    if (status.Errors != null)
                    {
                        // set a message with all error messages
                        foreach (var error in status.Errors)
                        {
                            message = message + error.ToString() + " ";
                        }
                    }
                    return Json(new { status = "Failure", message = message });
                }
            }
            else
            {
                // the modelstate isn't valid, fetch the errors and send to page
                var message = string.Join(" | ", ModelState.Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage));

                return Json(new { status = "Failure", message = message });
            }
        }
    }
}