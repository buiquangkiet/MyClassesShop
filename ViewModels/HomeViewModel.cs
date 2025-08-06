using MyClassesShop.Models;
using System.Collections.Generic;

namespace MyClassesShop.ViewModels
{
    public class HomeViewModel
    {
        public List<Product> FeaturedProducts { get; set; }
        public List<Product> AllProducts { get; set; }
        // Danh sách để hiển thị các lựa chọn lọc
        public List<Category> Categories { get; set; }
        public List<string> Colors { get; set; }
        public List<string> Types { get; set; }

        // Giá trị lọc đã chọn
        public int? SelectedCategoryId { get; set; }
        public string SelectedColor { get; set; }
        public string SelectedType { get; set; }
    }
}
