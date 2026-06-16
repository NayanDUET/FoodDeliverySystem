using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FoodDeliverySystem.Controllers
{
    [Authorize(Roles = "RestaurantOwner")]
    public class RestaurantController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RestaurantController(ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            

            var userId = _userManager.GetUserId(User);

            var restaurants = _context.Restaurants
                .Where(r => r.OwnerId == userId)
                .ToList();

            return View(restaurants);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                restaurant.CreatedAt = DateTime.UtcNow;
                restaurant.IsActive = true;

                //for role

                var userId = _userManager.GetUserId(User);

                restaurant.OwnerId = userId;

                //

                _context.Restaurants.Add(restaurant);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(restaurant);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
                return NotFound();

            return View(restaurant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Restaurant restaurant)
        {
            if (id != restaurant.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                
                var existingRestaurant = await _context.Restaurants.FindAsync(id);

                if (existingRestaurant == null)
                {
                    return NotFound();
                }

               
                var userId = _userManager.GetUserId(User);

                if (existingRestaurant.OwnerId != userId)
                {
                    return Forbid(); 
                }

                
                existingRestaurant.Name = restaurant.Name;
                existingRestaurant.Description = restaurant.Description;
                existingRestaurant.Address = restaurant.Address;
                existingRestaurant.PhoneNumber = restaurant.PhoneNumber;

                try
                {
                    _context.Restaurants.Update(existingRestaurant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Restaurants.Any(e => e.Id == restaurant.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(restaurant);
        }

        // GET: Restaurant/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
                return NotFound();

            return View(restaurant);
        }


        // POST: Restaurant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.FoodItems)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant == null)
                return NotFound();

            var foodIds = restaurant.FoodItems
                .Select(f => f.Id)
                .ToList();

            var orderItems = _context.OrderItems
                .Where(oi => foodIds.Contains(oi.FoodItemId));

            _context.OrderItems.RemoveRange(orderItems);

            _context.FoodItems.RemoveRange(restaurant.FoodItems);

            _context.Restaurants.Remove(restaurant);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
