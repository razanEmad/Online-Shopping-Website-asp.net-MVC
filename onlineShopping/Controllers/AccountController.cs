using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineShopping.Data;
using onlineShopping.Models;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    public AccountController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(User user)
    {
        if (ModelState.IsValid)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == user.Email);

            if (exists)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(user);
            }

            user.Role = "User"; // default role

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        return View(user);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password)
    {
        // 1. Use .ToLower() to prevent case sensitivity issues in email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.Password == password);

        if (user != null)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role) // Ensure user.Role is "Admin" in DB
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 2. Add AuthenticationProperties for a "clean" persistent sign-in
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Keeps user logged in after browser closes
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            };

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                authProperties);

            // 3. Force check for Admin role (Case-Insensitive)
            if (string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Invalid email or password.");
        return View();
    }

    [HttpPost] // Changed to Post for security
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // 4. Explicitly sign out of the specific scheme
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Optional: Clear session if you use it
        //HttpContext.Session.Clear();
        Response.Cookies.Delete(".AspNetCore.Cookies");

        return RedirectToAction("Login", "Account");
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        // 5. Use the correct claim type to find the ID
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return RedirectToAction("Login");

        var userId = int.Parse(userIdClaim);
        var user = await _context.Users.Include(u => u.Orders).FirstOrDefaultAsync(u => u.Id == userId);
        return View(user);
    }
}