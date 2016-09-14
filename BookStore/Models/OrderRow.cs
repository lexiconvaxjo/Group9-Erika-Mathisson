using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    /// <summary>
    /// Class for holding properties for an orderrow
    /// </summary>
    public class OrderRow
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Order order { get; set; }
        [Required]
        public Book BookPurchase { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int NoOfItem { get; set; }
    }
}