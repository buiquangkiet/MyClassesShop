using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyClassesShop.Data;
using MyClassesShop.Models;
using MyClassesShop.ViewModels;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    //public async Task<IActionResult> Index()
    //{
    //    var products = await _context.Products.Include(p => p.Category).ToListAsync();
    //    return View(products); // View: Views/Home/Index.cshtml
    //}

    public async Task<IActionResult> Index(int? categoryId)
    {
        var products = _context.Products.Include(p => p.Category).AsQueryable();

        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId.Value);
        }

        var productListViewModel = new ProductListViewModel
        {
            Products = await products.ToListAsync(),
            Categories = await _context.Categories.ToListAsync(),
            SelectedCategoryId = categoryId
        };

        var homeViewModel = new HomeViewModel
        {
            FeaturedProducts = await _context.Products
                .Where(p => p.StockQuantity > 0)
                .OrderByDescending(p => p.Price)
                .Take(4)
                .ToListAsync(),

            AllProducts = await _context.Products
                .Where(p => p.StockQuantity > 0)
                .Take(8)
                .ToListAsync()
        };

        return View(homeViewModel);
    }

}
