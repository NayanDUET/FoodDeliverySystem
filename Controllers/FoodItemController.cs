using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Controllers
{
    public class FoodItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FoodItemController(ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int restaurantId)
        {
            ViewBag.RestaurantId = restaurantId;

            var userId = _userManager.GetUserId(User);

            var restaurant = _context.Restaurants
                .FirstOrDefault(r =>
                    r.Id == restaurantId &&
                    r.OwnerId == userId);

            if (restaurant == null)
                return Unauthorized();

            var foods = _context.FoodItems
                .Where(f => f.RestaurantId == restaurantId)
                .ToList();

            return View(foods);

           
        }

        public IActionResult Create(int restaurantId)
        {
            var model = new FoodItem
            {
                RestaurantId = restaurantId
            };

            return View(model);

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
        FoodItem foodItem,

        IFormFile? imageFile)
        { 
            var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == foodItem.RestaurantId);

            if (restaurant == null)
            {
                return Content($"Restaurant {foodItem.RestaurantId} not found");
            }
            if (ModelState.IsValid)
            {
                foodItem.CreatedAt = DateTime.UtcNow;
                foodItem.IsAvailable = true;

                if (imageFile != null)
                {
                    string fileName =
                        Guid.NewGuid().ToString()
                        + Path.GetExtension(imageFile.FileName);

                    string folder =
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot/images/foods");

                    string filePath =
                        Path.Combine(folder, fileName);

                    using (var stream =
                        new FileStream(filePath,
                        FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    foodItem.ImageUrl =
                        "/images/foods/" + fileName;
                }

                _context.FoodItems.Add(foodItem);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { restaurantId = foodItem.RestaurantId });
            }

            return View(foodItem);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var food = await _context.FoodItems.FindAsync(id);

            if (food == null)
                return NotFound();

            return View(food);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
        FoodItem foodItem,

        IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return View(foodItem);

            var existingFood = await _context.FoodItems.FindAsync(foodItem.Id);

            if (existingFood == null)
                return NotFound();

            existingFood.Name = foodItem.Name;
            existingFood.Description = foodItem.Description;
            existingFood.Price = foodItem.Price;

            if (imageFile != null)
            {
                string fileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(imageFile.FileName);

                string folderPath =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images/foods");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath =
                    Path.Combine(folderPath, fileName);

                using (var stream =
                    new FileStream(filePath,
                    FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                existingFood.ImageUrl =
                    "/images/foods/" + fileName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { restaurantId = existingFood.RestaurantId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var food = await _context.FoodItems.FindAsync(id);
            if (food == null)
                return NotFound();

            return View(food);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var food = await _context.FoodItems.FindAsync(id);

            int restaurantId = food.RestaurantId;

            _context.FoodItems.Remove(food);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { restaurantId });
        }
    }
}
