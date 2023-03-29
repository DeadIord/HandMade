using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HandMade.Models
{
    public class Orders
    {
        [Key]
        public int OrdersId { get; set; }

        public string OrderNo { get; set; }
        public string ApplicationUserId { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }

        public int OrderDetailsId { get; set; }
        public OrderDetails OrderDetails { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}
