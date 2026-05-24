using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineShopping.Data;
using onlineShopping.Models;
using System.Security.Claims;

[Authorize]
public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    public OrderController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> CheckoutAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var cartItems = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return View(cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(string shippingAddress)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var cartItems = await _context.CartItems.Include(c => c.Product)
                                     .Where(c => c.UserId == userId).ToListAsync();

        if (!cartItems.Any()) return RedirectToAction("Index", "Cart");

        foreach (var item in cartItems)
        {
            // Optional: Check if there is enough stock before proceeding
            if (item.Product.StockQuantity < item.Quantity)
            {
                TempData["Error"] = $"Sorry, {item.Product.Name} only has {item.Product.StockQuantity} items left in stock.";
                return RedirectToAction("Index", "Cart");
            }

            // Subtract the quantity from the product stock
            item.Product.StockQuantity -= item.Quantity;

            // Note: EF Core tracks 'item.Product', so it knows it needs to update the Product table
        }

        // 1. Create the Order
        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            ShippingAddress = shippingAddress,
            Status = "Pending",
            TotalAmount = cartItems.Sum(ci => ci.Quantity * ci.Product.Price)
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // 2. Clear Cart
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        return RedirectToAction("Success");
    }

    public IActionResult Success() => View();

    public async Task<IActionResult> MyOrders()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var orders = await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
        return View(orders);
    }
}