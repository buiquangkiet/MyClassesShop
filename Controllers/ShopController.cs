using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyClassesShop.Data;
using MyClassesShop.Models;
using MyClassesShop.ViewModels;

public class ShopController : Controller
{
    private readonly ApplicationDbContext _context;
   

    public ShopController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? categoryId = null, string color = null, string type = null)
    {
        var products = _context.Products.AsQueryable();

        if (categoryId.HasValue)
            products = products.Where(p => p.CategoryId == categoryId);

        if (!string.IsNullOrEmpty(color))
            products = products.Where(p => p.Color == color);

        if (!string.IsNullOrEmpty(type))
            products = products.Where(p => p.Type == type);

        var viewModel = new HomeViewModel
        {
            AllProducts = await products.Include(p => p.Category).ToListAsync(),
            FeaturedProducts = await _context.Products.Where(p => p.IsFeatured).Take(5).ToListAsync(),
            Categories = await _context.Categories.ToListAsync(),
            Colors = await _context.Products
                                   .Where(p => p.Color != null)
                                   .Select(p => p.Color)
                                   .Distinct()
                                   .ToListAsync(),
            Types = await _context.Products
                                  .Where(p => p.Type != null)
                                  .Select(p => p.Type)
                                  .Distinct()
                                  .ToListAsync(),
            SelectedCategoryId = categoryId,
            SelectedColor = color,
            SelectedType = type
        };

        return View(viewModel);
    }



    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
            return NotFound();

        return View(product); // <-- trả về View với model
    }

}
