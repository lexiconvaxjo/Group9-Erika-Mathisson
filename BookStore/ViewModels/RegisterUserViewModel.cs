using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.ViewModels
{
    // viewmodel for register a user
    public class RegisterUserViewModel
    {
        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        public string UserName { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]        
        public string FirstName { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]       
        public string LastName { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]       
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]       
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [MinLength(7)]
        [MaxLength(30)]       
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]        
        public string Address { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]       
        public string City { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(10)]      
        public string ZipCode { get; set; }

        [Required]
        [MinLength(9)]
        [MaxLength(15)]       
        public string PhoneNumber { get; set; }

        public bool Admin { get; set; }

    }
}