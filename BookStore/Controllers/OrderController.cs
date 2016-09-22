using BookStore.Models;
using BookStore.ViewModels;
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
    [Authorize]
    public class OrderController : Controller
    {

        public OrderController()
        {

        }

        /// <summary>
        /// setting parameters
        /// </summary>
        /// <param name="role">role for adding to a user</param>
        /// <param name="userManager">usermanager for a user</param>
        /// <param name="signIn">signin manager</param>
        public OrderController(AppRole role, AppUserManager userManager, AppSignIn signIn)
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

        // GET: Order
        public ActionResult Index()
        {
            return View();
        }




        [Authorize]
        [HttpPost]
        public JsonResult AddOrder(List<OrderViewModel> addOrder)
        {
            if (ModelState.IsValid)
            {
                DB context = new DB();
                // fetch username
                var buyer = User.Identity.Name;
              
                var orderRows = new List<OrderRow>();              

                var user = context.Users.FirstOrDefault(x => x.UserName == buyer);

                for (int i = 0; i < addOrder.Count; i++)
                {
                    var currentBook = context.Books.FirstOrDefault(x => x.Id == addOrder[i].Id);                   

                    // create a new OrderRow
                    var addRow = new OrderRow
                    {
                        Price = addOrder[i].Price,
                        NoOfItem = addOrder[i].NoOfItem,
                        BookPurchase = currentBook
                    };

                    orderRows.Add(addRow);
                }

                var orderDate = DateTime.Now;

                // create a new order
                Order newOrder = new Order
                {
                    OrderDate = orderDate,
                    UserBuyer = user,
                    OrderRows = orderRows

                };


                try
                {
                    context.Orders.Add(newOrder);
                    context.SaveChanges();


                    //using (context)
                    //{
                    //    var order = context.Orders
                    //        .Where(x => x.UserBuyer == user);




                    //}

                    //todo check this query
                    var addedOrder = context.Orders
                        .Where(x => x.UserBuyer == user && x.OrderDate == orderDate);

                    return Json(new { status = "Success" });
                }
                catch (Exception e)
                {
                    //something went wrong when saving to database, send info to page
                    return Json(new { status = "DBFailure", message = e });
                }


            }

            return Json(new { status = "Failure" });


        }
    }
}