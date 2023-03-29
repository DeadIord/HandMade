using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.Collections.Generic;

namespace HandMade.Models
{
    public class ApplicationUser : IdentityUser
    {
     
        public ICollection<Orders> Orders { get; set; }
        public ICollection<Carts> Carts { get; set; }
    }
}
