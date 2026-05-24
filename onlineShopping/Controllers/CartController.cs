using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineShopping.Data;
using onlineShopping.Models;
using onlineShopping.Data;
using onlineShopping.Models;
using System.Security.Claims;

[Authorize] // Requires login for ALL cart actions
public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    public CartController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var cartItems = await _context.CartItems.Include(c => c.Product)
                                    .Where(c => c.UserId == userId).ToListAsync();
        return View(cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _context.CartItems.Add(new CartItem
            {
                ProductId = productId,
                UserId = userId,
                Quantity = quantity
            });
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Remove(int id)
    {
        var item = await _context.CartItems.FindAsync(id);
        if (item != null) { _context.CartItems.Remove(item); await _context.SaveChangesAsync(); }
        return RedirectToAction("Index");
    }
}