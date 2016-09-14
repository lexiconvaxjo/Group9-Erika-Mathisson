using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.ViewModels
{
    public class LogInPersonViewModel
    {
        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        public string UserName { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}