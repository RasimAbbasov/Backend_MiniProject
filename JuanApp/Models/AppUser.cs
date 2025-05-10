using Microsoft.AspNetCore.Identity;

namespace JuanApp.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
        public List<DbBasketItem> DbBasketItems { get; set; }

    }
}
