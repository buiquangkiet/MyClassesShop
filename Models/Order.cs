using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyClassesShop.Models
{
    public class Order
    {
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }

        public int OrderId { get; set; }

        public string UserId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Chờ xử lý"; // hoặc enum

        public decimal TotalAmount { get; set; }

        public string? DiscountCode { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }

}
