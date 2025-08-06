using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyClassesShop.Data;
using MyClassesShop.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // DASHBOARD
    public async Task<IActionResult> Dashboard()
    {
        var viewModel = new DashboardViewModel
        {
            TotalOrders = await _context.Orders.CountAsync(),
            TotalProducts = await _context.Products.CountAsync(),
            TotalUsers = await _userManager.Users.CountAsync()
        };

        return View(viewModel);
    }

    // QUẢN LÝ ĐƠN HÀNG
    public async Task<IActionResult> Orders()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
            .Include(o => o.User)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders); // Views/Admin/Orders.cshtml
    }

    // CẬP NHẬT TRẠNG THÁI
    public async Task<IActionResult> EditOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        return View(order); // Views/Admin/EditOrder.cshtml
    }

    [HttpPost]
    public async Task<IActionResult> EditOrder(int id, string status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.Status = status;
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công!";
        return RedirectToAction("Orders");      


    }

    // IN HÓA ĐƠN PDF
    public async Task<IActionResult> ExportInvoice(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null) return NotFound();

        using var stream = new MemoryStream();
        var doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
        PdfWriter.GetInstance(doc, stream).CloseStream = false;

        doc.Open();
        doc.Add(new Paragraph("🕶️ HÓA ĐƠN BÁN HÀNG"));
        doc.Add(new Paragraph($"Mã đơn: {order.OrderId}"));
        doc.Add(new Paragraph($"Khách hàng: {order.User.Email}"));
        doc.Add(new Paragraph($"Trạng thái: {order.Status}"));
        doc.Add(new Paragraph(" "));

        var table = new PdfPTable(4);
        table.AddCell("Sản phẩm");
        table.AddCell("Giá");
        table.AddCell("SL");
        table.AddCell("Thành tiền");

        foreach (var item in order.OrderDetails)
        {
            table.AddCell(item.Product.Name);
            table.AddCell(item.UnitPrice.ToString("N0"));
            table.AddCell(item.Quantity.ToString());
            table.AddCell((item.UnitPrice * item.Quantity).ToString("N0"));
        }

        doc.Add(table);
        doc.Add(new Paragraph($"Tổng: {order.TotalAmount.ToString("N0")} ₫"));
        doc.Close();

        var fileBytes = stream.ToArray();
        return File(fileBytes, "application/pdf", $"HoaDon_{order.OrderId}.pdf");
    }
}
