namespace ShoeStoreApp.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public string Color { get; set; }
        public string ImgPath { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public ICollection<ProductVariantItem> Items { get; set; }

    }
}
