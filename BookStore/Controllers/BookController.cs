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
        DB context = new DB();
        // GET: Book
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Function for getting books from database
        /// </summary>
        /// <returns>Json</returns>
        public JsonResult GetBooks()
        {           
            // fetch books from database
            List<Book> books = context.Books.ToList();
            // return books, allowing the method to be HttpGet
            return Json(books, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Function for adding a book to the database
        /// This function can only be used if authorized
        /// </summary>
        /// <param name="book">book that should be added</param>
        /// <returns>Json</returns>
        [Authorize]
        [HttpPost]
        public JsonResult AddBook([Bind(Include = "Title, Author, ISBN, Description, Price, NumberInStock")]Book book)
        {
            if (ModelState.IsValid)
            {               
                // check if added entered book already exists in database
                var checkBook = context.Books.FirstOrDefault(x => x.ISBN == book.ISBN);
                // the book already exist return info to page
                if (checkBook != null)
                {
                    return Json(new { status = "ISBNExist" });
                }

                // create a new book
                Book newBook = new Book
                {
                    Title = book.Title,
                    Author = book.Author,
                    Description = book.Description,
                    ISBN = book.ISBN,
                    Price = book.Price,
                    NumberInStock = book.NumberInStock
                };

                //try and add the new book to the database
                try
                {
                    context.Books.Add(newBook);
                    context.SaveChanges();

                    //fetch newly added book including Id and send to page                 
                    var addedBook = context.Books.FirstOrDefault(x => x.ISBN == newBook.ISBN);
                    return Json(new { status = "Success", addedBook = addedBook });
                }
                catch (Exception e)
                {
                    //something went wrong when saving to database, send info to page
                    return Json(new { status = "DBFailure", message = e });
                }
            }
            else
            {
                // the model isn't valid send what requirements that aren't fulfilled to the page
                var message = string.Join(" | ", ModelState.Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage));
                return Json(new { status = "Failure", message = message });
            }
        }


        /// <summary>
        /// Function for editing an exising book
        /// </summary>
        /// <param name="book">book that should be updated</param>
        /// <returns>Json</returns>
        [Authorize]
        [HttpPost]
        public JsonResult EditBook([Bind(Include = "Id, Title, Author, ISBN, Description, Price, NumberInStock")]Book book)
        {
            //check if model is valid
            if (ModelState.IsValid)
            {                
                //try to fetch a book via Id
                var currentBook = context.Books.FirstOrDefault(x => x.ISBN == book.ISBN);
                // check if entered ISBN already exist and doesn't belong to book who should be updated
                if (currentBook != null && book.Id != currentBook.Id)
                {
                    // ISBN already exist send info to page
                    return Json(new { status = "ISBNExist" });
                }                

                //try to add the changes on the book to the database               
                try
                {
                    currentBook.Title = book.Title;
                    currentBook.Author = book.Author;
                    currentBook.ISBN = book.ISBN;
                    currentBook.Description = book.Description;
                    currentBook.Price = book.Price;
                    currentBook.NumberInStock = book.NumberInStock;
                    context.SaveChanges();
                    // changes updated ok, send info to page
                    return Json(new { status = "Success" });
                }
                catch (Exception e)
                {
                    // something went wrong, error code returned
                    return Json(new { status = "DBFailure", message = e });
                }
            }
            else
            {
                // model isn't valid, fetch error message and send to page
                var message = string.Join(" | ", ModelState.Values
                  .SelectMany(v => v.Errors)
                  .Select(e => e.ErrorMessage));
                return Json(new { status = "Failure", message = message });
            }            
        }
    }
}