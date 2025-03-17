namespace ShoeStoreApp.Models
{
    public class ProductVariantItem
    {
        public int Id { get; set; }
        public string Size { get; set; }
        public int StockQuantity { get; set; }
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
