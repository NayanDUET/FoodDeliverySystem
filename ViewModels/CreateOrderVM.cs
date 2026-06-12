namespace FoodDeliverySystem.ViewModels
{
    public class CreateOrderVM
    {
        public int FoodItemId { get; set; }

        public string? FoodName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string? DeliveryAddress { get; set; }

        public string? PaymentMethod { get; set; }

        public string? PaymentNumber { get; set; }
    }
}
