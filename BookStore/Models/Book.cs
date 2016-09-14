using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    /// <summary>
    /// Class for holding properties of a book
    /// </summary>
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Desctiption { get; set; }
        [Required]
        public int ISBN { get; set; }
        [Required]
        public int NumberInStock { get; set; }
        [Required]
        public double Price { get; set; }
    }
}