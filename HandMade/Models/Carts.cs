using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HandMade.Models
{
    public class Carts
    {
        [Key]

        public int CartsId { get; set; }
        public int Quantity { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Products Products { get; set; }

    }
}
