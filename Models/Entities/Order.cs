using FoodDeliverySystem.Models.Enums;

namespace FoodDeliverySystem.Models.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public string? CustomerId { get; set; }
        public ApplicationUser? Customer { get; set; }

        public int RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }

        public string? DeliveryManId { get; set; }
        public ApplicationUser? DeliveryMan { get; set; }

        public decimal TotalAmount { get; set; }
        public string? DeliveryAddress { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }

        public Delivery? Delivery { get; set; }

        //last added

        public string? PaymentMethod { get; set; }

        public string? PaymentNumber { get; set; }
    }
}
