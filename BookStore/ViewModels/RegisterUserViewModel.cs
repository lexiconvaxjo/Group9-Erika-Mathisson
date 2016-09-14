using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        [Display(Name = "User name: ")]
        public string UserName { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        [Display(Name ="First name: ")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        [Display(Name ="Last name: ")]
        public string LastName { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        [Display(Name ="Password: ")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        [Display(Name ="Confirm password: ")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [MinLength(7)]
        [MaxLength(30)]
        [Display(Name ="Email: ")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        [Display(Name ="Address: ")]
        public string Address { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        [Display(Name ="City: ")]
        public string City { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(10)]
        [Display(Name ="Zip code: ")]
        public string ZipCode { get; set; }

        [Required]
        [MinLength(9)]
        [MaxLength(15)]
        [Display(Name ="Phone number: ")]
        public string PhoneNumber { get; set; }

        public bool Admin { get; set; }

    }
}