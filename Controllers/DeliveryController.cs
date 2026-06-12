using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodDeliverySystem.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace FoodDeliverySystem.Controllers
{
    [Authorize(Roles = "DeliveryMan")]
    public class DeliveryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeliveryController(
          ApplicationDbContext context,
          UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var orders = _context.Orders
                .Where(o => o.Status == OrderStatus.Confirmed)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(orders);
        }

        public async Task<IActionResult> Pick(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            order.DeliveryManId = userId;
            order.Status = OrderStatus.OutForDelivery;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PickedOrders));
        }

        public IActionResult PickedOrders()
        {
            var userId = _userManager.GetUserId(User);

            var orders = _context.Orders
                .Where(o =>
                    o.DeliveryManId == userId &&
                    o.Status == OrderStatus.OutForDelivery)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(orders);
        }

        public IActionResult DeliveredOrders()
        {
            var userId = _userManager.GetUserId(User);

            var orders = _context.Orders
                .Where(o =>
                    o.DeliveryManId == userId &&
                    o.Status == OrderStatus.Delivered)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(orders);
        }

        public async Task<IActionResult> Delivered(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound();

            order.Status = OrderStatus.Delivered;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(DeliveredOrders));
        }
    }
}
