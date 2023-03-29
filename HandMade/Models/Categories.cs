using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HandMade.Models
{
    public class Categories
    {
        [Key]

        public int CategoriesId { get; set; }

        [Required(ErrorMessage = "Укажите название категории!")]
        public string NameOfTheCategories { get; set; }

        [Required(ErrorMessage = "Укажите статус категории!")]
        public string ActiveCategories { get; set; }


        public ICollection<Products> Products { get; set; }
    }
}
