using FoodDeliverySystem.Models.Entities;

namespace FoodDeliverySystem.ViewModels
{
    public class SearchResultVM
    {
        public List<FoodItem> Foods { get; set; }
            = new();

        public List<Restaurant> Restaurants { get; set; }
            = new();

        public string? Keyword { get; set; }
    }
}