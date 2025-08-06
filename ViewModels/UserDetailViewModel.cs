using Microsoft.AspNetCore.Identity;

namespace MyClassesShop.ViewModels
{
    public class UserDetailViewModel
    {
        public IdentityUser User { get; set; }
        public List<string> Roles { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
