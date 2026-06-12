using FoodDeliverySystem.Models.Enums;

namespace FoodDeliverySystem.Models.Entities
{
    public class Delivery
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int DeliveryBoyId { get; set; }

        public DateTime? AssignedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        public DeliveryStatus Status { get; set; }
    }
}
