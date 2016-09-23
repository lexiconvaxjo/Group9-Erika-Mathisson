using BookStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.ViewModels
{
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

    public class AddedOrderViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public User UserBuyer { get; set; }

    }

}