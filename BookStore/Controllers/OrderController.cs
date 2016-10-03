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
        DB context = new DB();
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

        /// <summary>
        /// function for adding an order to a user
        /// </summary>
        /// <param name="addOrder">order to be added</param>
        /// <returns>Json</returns>
        [Authorize]
        [HttpPost]
        public JsonResult AddOrder(List<OrderViewModel> addOrder)
        {
            // check if the model is valid
            if (ModelState.IsValid)
            {
                // fetch username
                var buyer = User.Identity.Name;
                // create a new list for order rows
                var orderRows = new List<OrderRow>();
                // fetch user who order should be added to
                var user = context.Users.FirstOrDefault(x => x.UserName == buyer);
                // go through all items in the order
                for (int i = 0; i < addOrder.Count; i++)
                {
                    // fetch bookId from order
                    var bookId = addOrder[i].Id;
                    // fetch book who should be added to an order row, from database
                    var currentBook = context.Books.FirstOrDefault(x => x.Id == bookId);
                    // create a new OrderRow
                    var addRow = new OrderRow
                    {
                        Price = addOrder[i].Price,
                        NoOfItem = addOrder[i].NoOfItem,
                        BookPurchase = currentBook
                    };
                    // add the order row to the order row list
                    orderRows.Add(addRow);
                }
                // create a date for the order
                var orderDate = DateTime.Now;

                // create a new order and add the date, buyer and order rows
                Order newOrder = new Order
                {
                    OrderDate = orderDate,
                    UserBuyer = user,
                    OrderRows = orderRows
                };

                // try and save the created order to the database
                try
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    var addedOrder = context.Orders.Add(newOrder);
                    context.SaveChanges();

                    // return status to page along with the newly added order
                    return Json(new { status = "Success", data = newOrder });

                }
                catch (Exception e)
                {
                    //something went wrong when saving to database, send info to page
                    return Json(new { status = "DBFailure" });
                }
            }
            else
            {
                // model isn't valid, take all errors and save in a string, send to page
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return Json(new { status = "Failure", data = message });
            }
        }

        /// <summary>
        /// function for getting a users order history
        /// </summary>
        /// <returns>Json</returns>
        [Authorize]
        [HttpPost]
        public JsonResult GetOrderHistory(GetOrderHistoryViewModel vm)
        {
            var user = new User();
            // fetch user
            if (vm.UserName == null)
            {
                var buyer = User.Identity.Name;
                user = context.Users.FirstOrDefault(x => x.UserName == buyer);
            }
            else
            {
                user = context.Users.FirstOrDefault(x => x.UserName == vm.UserName);
            }

            // fetch orders from database including orderrows and purchased books
            List<Order> orderList = context.Orders.Include("OrderRows").Include("OrderRows.BookPurchase").ToList().FindAll(x => x.UserBuyer == user);

            // return status to page along with the list of orders
            return Json(new { status = "Success", data = orderList });

        }

        /// <summary>
        /// Function for removing a row from an order
        /// </summary>
        /// <param name="deleteRow">row that should be removed</param>
        /// <returns>Json</returns>
        public JsonResult RemoveRowFromOrder(DeleteRowViewModel deleteRow)
        { 
            // fetch the row that should be removed           
            var rowToRemove = context.OrderRows.FirstOrDefault(x => x.Id == deleteRow.RowId);
            // check so that the row isn't empty
            if (rowToRemove != null)
            {
                // remove the row 
                context.OrderRows.Remove(rowToRemove);
                // fetch the order whom the row is removed from
                var orderRemove = context.Orders.Include("OrderRows").FirstOrDefault(x => x.Id == deleteRow.OrderId);
                // check if the order contains any more order rows, if not remove the actual order
                if (orderRemove.OrderRows == null)
                {
                    context.Orders.Remove(orderRemove);
                }
                // save the changes to the database
                context.SaveChanges();
            }
            // return info to the page
            return Json(new { status = "Success" });
        }
    }
}