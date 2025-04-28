using JuanApp.Data;
using JuanApp.Models;
using JuanApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
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
                Features = juanDbContext.Features.ToList()
            };
            return View(homeVm);
        }
    }
}
