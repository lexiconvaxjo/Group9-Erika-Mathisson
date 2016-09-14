using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    /// <summary>
    /// Class for holding properties of an order
    /// </summary>
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public User UserBuyer { get; set; }
        [Required]
        public List<OrderRow> OrderRows { get; set; }

    }
}