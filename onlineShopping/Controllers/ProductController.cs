using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using onlineShopping.Data;
using onlineShopping.Models;


public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    public ProductController(ApplicationDbContext context) => _context = context;

    // Admin List
    public async Task<IActionResult> Index() => View(await _context.Products.Include(p => p.Category).ToListAsync());

    // GET: /Product/Shop
    // This is the public-facing catalog for customers
    public async Task<IActionResult> Shop(string category)
    {
        // Start with all products and include the category data
        var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

        // Optional: Filter by category if a category name is passed in the URL
        if (!string.IsNullOrEmpty(category))
        {
            productsQuery = productsQuery.Where(p => p.Category.Name == category);
        }

        var products = await productsQuery.ToListAsync();
        return View(products); // This looks for a file named Shop.cshtml
    }

    // Public Details
    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        return product == null ? NotFound() : View(product);
    }

    public IActionResult Create()
    {
        ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
        return View();
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // TEMP DEBUG
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine(error.ErrorMessage);
        }

        ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");

        return View(product);
    }

    // Edit and Delete methods follow the same pattern...

    // 5. EDIT: Show the form (GET)
    // GET: /Product/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        return View(product);
    }

    // 6. EDIT: Update the data (POST)
    // POST: /Product/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,ImageUrl,StockQuantity,CategoryId")] Product product)
    {
        if (id != product.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }

        ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        return View(product);
    }

    // 7. DELETE: Show confirmation (GET)
    // GET: /Product/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (product == null) return NotFound();

        return View(product);
    }

    // 8. DELETE: Perform removal (POST)
    // POST: /Product/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Helper method to check product existence
    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}