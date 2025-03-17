using System.ComponentModel.DataAnnotations;

namespace ShoeStoreApp.Models
{
    public class Product
    {
        public int Id { get; set; }
      
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Range(0,long.MaxValue,ErrorMessage ="Giá không phải số")]
        public long Price { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        public string Description { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Category { get; set; }
        public ICollection<ProductVariant>? Variants { get; set; }
    }
}
