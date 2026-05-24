using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineShopping.Data;
using System.Security.Claims;

public class CartCountViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public CartCountViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = UserClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int count = 0;

        if (userId != null)
        {
            int id = int.Parse(userId);

            count = await _context.CartItems
                .Where(c => c.UserId == id)
                .CountAsync();
        }

        return View(count);
    }
}