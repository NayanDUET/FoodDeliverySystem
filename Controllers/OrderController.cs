using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models.Entities;
using FoodDeliverySystem.Models.Enums;
using FoodDeliverySystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Controllers
{
    [Authorize(Roles = "Customer")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int foodItemId)
        {
            var food = await _context.FoodItems
                .Include(f => f.Restaurant)
                .FirstOrDefaultAsync(f => f.Id == foodItemId);

            if (food == null)
                return NotFound();

            var vm = new CreateOrderVM
            {
                FoodItemId = food.Id,
                Quantity = 1
            };

            ViewBag.FoodName = food.Name;
            ViewBag.Price = food.Price;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderVM model)
        {
            var user = await _userManager.GetUserAsync(User);

            var food = await _context.FoodItems
                .FirstOrDefaultAsync(f => f.Id == model.FoodItemId);

            if (food == null)
                return NotFound();


            var order = new Order
            {
                CustomerId = user?.Id,

                RestaurantId = food.RestaurantId,

                DeliveryAddress = model.DeliveryAddress,

                PaymentMethod = model.PaymentMethod,

                PaymentNumber = model.PaymentNumber,

                Status = OrderStatus.Pending,

                CreatedAt = DateTime.Now,

                TotalAmount = food.Price * model.Quantity
            };

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                FoodItemId = food.Id,
                Quantity = model.Quantity,
                Price = food.Price
            };

            _context.OrderItems.Add(orderItem);

            await _context.SaveChangesAsync();

             TempData["Success"] =
            "Order placed successfully!";

            return RedirectToAction("MyOrders");
        }

        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);

            var orders = await _context.Orders
                .Include(o => o.Restaurant)
                .Where(o => o.CustomerId == user!.Id)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.FoodItem)
                .Include(o => o.Restaurant)
                .FirstOrDefaultAsync(o =>
                    o.Id == id &&
                    o.CustomerId == user!.Id);

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}
