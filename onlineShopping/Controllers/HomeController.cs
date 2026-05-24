using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineShopping.Data;
using onlineShopping.Models;
using System.Diagnostics;

namespace onlineShopping.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context) => _context = context;


        public async Task<IActionResult> Index()

        {
            // Fetch products with their categories for the homepage grid
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
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
