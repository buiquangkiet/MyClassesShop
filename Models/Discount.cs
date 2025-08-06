using System.ComponentModel.DataAnnotations;

namespace MyClassesShop.Models
{
    public class Discount
    {
        public int DiscountId { get; set; }

        [Required]
        public string Code { get; set; }

        [Range(0, 100)]
        public int Percentage { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsActive => ExpiryDate >= DateTime.Now;
    }

}
