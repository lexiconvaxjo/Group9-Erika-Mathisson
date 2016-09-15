using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        // GET: Book
        public ActionResult Index()
        {
            return View();
        }

       
        public JsonResult GetBooks()
        {
            DB context = new DB();
            List<Book> books = context.Books.ToList();           

            // return peoples, allowing the method to be HttpGet
            return Json(books, JsonRequestBehavior.AllowGet);
        }
    }
}