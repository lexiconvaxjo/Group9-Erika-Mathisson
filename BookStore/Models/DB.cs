using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    public class DB : IdentityDbContext<User>
    {
        /// <summary>
        /// empty constructor for dbcontext
        /// </summary>
        public DB() : base("BookStore")
        {

        }

        /// <summary>
        /// Static method for creating an instance of the databasecontext so you don't need to create an instance every time
        /// </summary>
        /// <returns>new instance of dbcontext</returns>
        public static DB CreateConnection()
        {
            return new DB();
        }

        //For creating table Book
        public DbSet<Book> Books { get; set; }
        //For creating table Order
        public DbSet<Order> Orders { get; set; }
        //For creating table OrderRows
        public DbSet<OrderRow> OrderRows { get; set; }
    }
}