using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineShopping.Data;

[Authorize(Roles = "Admin")] // Use this if you have roles set up
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    public AdminController(ApplicationDbContext context) => _context = context;

    public IActionResult Dashboard() => View();

    public async Task<IActionResult> Users()
    {
        return View(await _context.Users.ToListAsync());
    }

    public async Task<IActionResult> Orders() => View(await _context.Orders.Include(o => o.User).ToListAsync());
}