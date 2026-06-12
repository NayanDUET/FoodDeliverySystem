namespace FoodDeliverySystem.Models.Entities
{
    public class FoodItem
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }

        public int RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }
    }
}
