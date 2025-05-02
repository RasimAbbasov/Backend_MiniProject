using JuanApp.Data;
using JuanApp.Models;
using JuanApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace JuanApp.Controllers
{
    public class HomeController(JuanDbContext juanDbContext) : Controller
    {
        public IActionResult Index()
        {
            HomeVm homeVm = new HomeVm
            {
                Sliders = juanDbContext.Sliders.ToList(),
                Features = juanDbContext.Features.ToList(),
                NewProducts = juanDbContext.Products
                .Where(x => x.IsNew)
                .Include(x => x.ProductImages.Where(b => b.IsMain == true))
                .ToList(),
                TopProducts = juanDbContext.Products
                .Where(x => x.IsTopSeller)
                .Include(x => x.ProductImages.Where(b => b.IsMain == true))
                .ToList()
            };
            return View(homeVm);
        }
    }
}
