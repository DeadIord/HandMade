namespace HandMade.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductsId { get; set; }
        public int Quantity { get; set; }

        public virtual Products Product { get; set; }
        public virtual Orders Orders { get; set; }
    }
}
