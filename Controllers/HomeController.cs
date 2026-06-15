using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Models.Entities;
using FoodDeliverySystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace FoodDeliverySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction(
                        "Index",
                        "Admin");
                }

                if (await _userManager.IsInRoleAsync(user!, "RestaurantOwner"))
                {
                    return RedirectToAction("Index", "Restaurant");
                }

                if (await _userManager.IsInRoleAsync(user!, "DeliveryMan"))
                {
                    return RedirectToAction("Index", "Delivery");
                }
            }

            var foodItems = _context.FoodItems
                        .Include(f => f.Restaurant)
                        .Where(f => f.IsAvailable)
                        .ToList();

            return View(foodItems);
        }

        public IActionResult Search(string keyword)
        {
            var foods = _context.FoodItems

                .Include(f => f.Restaurant)

                .Where(f =>

                    f.Name!.Contains(keyword)

                    ||

                    f.Restaurant!.Name!.Contains(keyword)

                    ||

                    f.Restaurant.Address!.Contains(keyword)

                )

                .ToList();



            var restaurants = _context.Restaurants

                .Where(r =>

                    r.Name!.Contains(keyword)

                    ||

                    r.Address!.Contains(keyword)

                )

                .ToList();


            var model = new SearchResultVM
            {
                Foods = foods,

                Restaurants = restaurants,

                Keyword = keyword
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
