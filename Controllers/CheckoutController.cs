using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyClassesShop.Data;
using MyClassesShop.Models;

[Authorize]
public class CheckoutController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CheckoutController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return View(cartItems);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(string fullName, string address, string phone, string discountCode, decimal total)
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
        {
            TempData["Error"] = "Giỏ hàng đang trống.";
            return RedirectToAction("Index", "Cart");
        }

        // Tính tổng tiền trước giảm
        decimal totalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity);

        // Kiểm tra mã giảm giá
        Discount? discount = null;
        if (!string.IsNullOrWhiteSpace(discountCode))
        {
            discount = await _context.Discounts
                .FirstOrDefaultAsync(d => d.Code == discountCode && d.ExpiryDate >= DateTime.Now);

            if (discount != null)
            {
                totalAmount *= (1 - (discount.Percentage / 100m));
            }
            else
            {
                TempData["Warning"] = "Mã giảm giá không hợp lệ hoặc đã hết hạn!";
            }
        }

        // Tạo đơn hàng
        var order = new Order
        {
            UserId = userId,
            TotalAmount = totalAmount,
            DiscountCode = discountCode,
            OrderDate = DateTime.Now,
            Status = "Chờ xử lý",
            OrderDetails = cartItems.Select(item => new OrderDetail
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price
            }).ToList()
        };



        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        return RedirectToAction("Success");
    }

    public IActionResult Success()
    {
        return View();
    }
}
