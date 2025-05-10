using JuanApp.Data;
using JuanApp.Models;
using JuanApp.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace JuanApp.Controllers
{
    public class BasketController(JuanDbContext juanDbContext,
        UserManager<AppUser> userManager) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddtoBasket(int id)
        {
            if(id==null) return NotFound();
            var product= juanDbContext.Products
                .Include(x=>x.ProductImages)
                .FirstOrDefault(x=>x.Id==id);
            if (product == null) return NotFound(); 
            List<BasketItemVm> list;
            var basket = HttpContext.Request.Cookies["basket"];
            if (basket != null)
            {
                list = JsonConvert.DeserializeObject<List<BasketItemVm>>(basket);
            }
            else
            {
               list = new List<BasketItemVm>();
            }
            var existProduct = list.FirstOrDefault(x => x.ProductId == id);
            if (existProduct != null)
            {
                existProduct.Count++;
            }
            else
            {
                list.Add(new BasketItemVm
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    MainImage = product.ProductImages.FirstOrDefault(x=>x.IsMain==true).Image,
                    Price = product.Price,
                    Count = 1
                });
            }
            if(User.Identity.IsAuthenticated)
            {
                var user= userManager.Users.Include(x=>x.DbBasketItems).FirstOrDefault(x => x.UserName == User.Identity.Name);
                var existUserBasketItem = user.DbBasketItems.FirstOrDefault(x => x.ProductId == id);
                if (existUserBasketItem != null)
                {
                    existUserBasketItem.Count++;
                }
                else
                {
                    user.DbBasketItems.Add(new DbBasketItem
                    {
                        ProductId = product.Id,
                        Count = 1,
                        AppUserId = user.Id
                    });
                }
                juanDbContext.SaveChanges();
            }
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(list));
            return PartialView("_BasketPartial", list);
        }
    }
}
