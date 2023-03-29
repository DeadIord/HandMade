using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HandMade.Models
{
    public class Products
    {
        [Key]

        public int ProductsId { get; set; }

        [Required(ErrorMessage = "Укажите название товара")]
        public string NameOfTheProduct { get; set; }

        [Required(ErrorMessage = "Укажите описание товара")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Укажите цену товара")]
        public double Price { get; set; }
        [Required(ErrorMessage = "Укажите колличество товара")]
        public int Quantity { get; set; }

        public byte[] ImageProduct { get; set; }

        public int CategoriesId { get; set; }

 
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public virtual ICollection<Carts> Carts { get; set; }
        public Categories Categories { get; set; }
    }
}
