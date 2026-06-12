using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models.Entities;
using FoodDeliverySystem.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Controllers
{
    public class RestaurantOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RestaurantOrdersController(ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index(int restaurantId)
        {
            //var restaurant = _context.Restaurants
            // .FirstOrDefault(r => r.Id == restaurantId);

            //if (restaurant == null)
            //{
            //    return NotFound();
            //}

            //ViewBag.RestaurantName = restaurant.Name;
            //var orders = _context.Orders
            //    .Where(o => o.RestaurantId == restaurantId)
            //    .OrderByDescending(o => o.CreatedAt)
            //    .ToList();

            //return View(orders);

            var userId = _userManager.GetUserId(User);

            var restaurant = _context.Restaurants
                .FirstOrDefault(r =>
                    r.Id == restaurantId &&
                    r.OwnerId == userId);

            if (restaurant == null)
                return Unauthorized();

            ViewBag.RestaurantName = restaurant.Name;

            var orders = _context.Orders
                .Where(o => o.RestaurantId == restaurantId)
                .ToList();

            return View(orders);
        }

        public IActionResult Details(int id)
        {

            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.FoodItem)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        public IActionResult Accept(int id)
        {
            var order = _context.Orders.Find(id);

            if (order == null)
                return NotFound();

            order.Status = OrderStatus.Confirmed;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index),
                new { restaurantId = order.RestaurantId });
        }

        public IActionResult Reject(int id)
        {
            var order = _context.Orders.Find(id);

            if (order == null)
                return NotFound();

            order.Status = OrderStatus.Cancelled;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index),
                new { restaurantId = order.RestaurantId });
        }
    }
}
