using Microsoft.AspNetCore.Identity;

namespace ShoeStoreApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<DeliveryAddress> DeliveryAddresses { get; set; } = new List<DeliveryAddress>();

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
