namespace ShoeStoreApp.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public ApplicationUser Customer { get; set; }
        public int ProductVariantItemId { get; set; }
        public ProductVariantItem ProductVariantItem { get; set; }
        public int Quantity { get; set; }
    }
}
