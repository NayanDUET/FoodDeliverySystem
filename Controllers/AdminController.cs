using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliverySystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Restaurants()
        {
            var restaurants = _context.Restaurants.ToList();

            return View(restaurants);
        }

        public IActionResult RestaurantFoods(int id)
        {
            var foods = _context.FoodItems
                .Where(f => f.RestaurantId == id)
                .ToList();

            return View(foods);
        }

        public IActionResult Customers()
        {
            var customers = _context.Users.ToList();

            return View(customers);
        }
        public IActionResult Orders()
        {
            var orders = _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(orders);
        }

        public async Task<IActionResult> DeliveryMen()
        {
            var users = _userManager.Users.ToList();

            var deliveryMen = new List<ApplicationUser>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "DeliveryMan"))
                {
                    deliveryMen.Add(user);
                }
            }

            return View(deliveryMen);
        }

        public IActionResult DeliveryActivity(string id)
        {
            var orders = _context.Orders
                .Where(o => o.DeliveryManId == id)
                .ToList();

            return View(orders);
        }
    }
}