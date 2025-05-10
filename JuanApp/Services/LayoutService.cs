using Humanizer.Localisation;
using JuanApp.Data;
using JuanApp.Models;
using JuanApp.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace JuanApp.Services
{
    public class LayoutService(JuanDbContext juanDbContext,IHttpContextAccessor httpContextAccessor,UserManager<AppUser> userManager)
    {
        public Dictionary<string, string> GetSettings()
        {
            return juanDbContext.Settings
                .ToDictionary(x => x.Key, x => x.Value);
        }
        public List<Category> GetCategories()
        {
            return juanDbContext.Categories.ToList();
        }
        public List<BasketItemVm> GetBasketItems()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var basket = httpContext.Request.Cookies["basket"];
            var list = new List<BasketItemVm>();
            if (basket != null)
            {
                list = JsonConvert.DeserializeObject<List<BasketItemVm>>(basket);
            }
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var user = userManager.Users
                    .Include(x => x.DbBasketItems)
                    .ThenInclude(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .FirstOrDefault(x => x.UserName == httpContext.User.Identity.Name);
                foreach (var item in user.DbBasketItems)
                {
                    if (!list.Any(b => b.ProductId == item.ProductId))
                    {
                        list.Add(new BasketItemVm
                        {
                            ProductId = item.ProductId,
                            Name = item.Product.Name,
                            MainImage = item.Product.ProductImages.FirstOrDefault(x => x.IsMain == true).Image,
                            Price = item.Product.Price,
                            Count = item.Count
                        });
                    }
                }
                httpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(list));
            }
            return list;
        }
    }
}
