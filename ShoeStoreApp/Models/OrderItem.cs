namespace ShoeStoreApp.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductVariantItemId { get; set; }
        public ProductVariantItem ProductVariantItem { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

    }
}
