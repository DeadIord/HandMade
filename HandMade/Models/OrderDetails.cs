using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HandMade.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsId { get; set; }

        [Required(ErrorMessage = "Укажите имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Укажите фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Укажите отчество")]
        public string Patronymic { get; set; }

        [Required(ErrorMessage = "Укажите номер телефона")]
        public string NumberPhone { get; set; }

        [Required(ErrorMessage = "Укажите адрес")]
        public string Addres { get; set; }

        public ICollection< Orders> Orders { get; set; }
    }
}
