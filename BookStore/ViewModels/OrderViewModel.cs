using BookStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.ViewModels
{
    // viewmodel for an order
    public class OrderViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public long ISBN { get; set; }
        [Required]
        public int NumberInStock { get; set; }
        [Required]
        public double Price { get; set; }

        [Required]
        public int NoOfItem { get; set; }
    }

    // view model for getting orderhistory for a user
    public class GetOrderHistoryViewModel
    {
        public string UserName { get; set; }
    }

    // view model for deleting a row in an order
    public class DeleteRowViewModel
    {
        public int OrderId { get; set; }

        public int RowId { get; set; }
    }
}