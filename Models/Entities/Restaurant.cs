namespace FoodDeliverySystem.Models.Entities
{
    public class Restaurant
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

        public string? OwnerId { get; set; }
        public ApplicationUser? Owner { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // Navigation
        public ICollection<FoodItem> ?FoodItems { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
