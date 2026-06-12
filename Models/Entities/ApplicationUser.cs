using Microsoft.AspNetCore.Identity;

namespace FoodDeliverySystem.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        // Navigation Properties
        public ICollection<Restaurant>? OwnedRestaurants { get; set; }
        public ICollection<Order> ?CustomerOrders { get; set; }
      
    }
}
