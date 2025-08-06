using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MyClassesShop.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}