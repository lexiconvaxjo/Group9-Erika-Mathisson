using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace BookStore.Models
{
    public class User : IdentityUser
    {
        /// <summary>
        /// function for generating a user async
        /// </summary>
        /// <param name="manager">usermanager</param>
        /// <returns>identity of the user</returns>
        public async Task<ClaimsIdentity> GenerateUser(UserManager<User> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(
                user: this, authenticationType: DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
        // added properties for a user appart from Identity properites       
        [Required]
        public string FirstName { get; set; }
        [Required]       
        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public string City { get; set; }
    }
}