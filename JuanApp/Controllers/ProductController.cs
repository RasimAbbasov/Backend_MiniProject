using JuanApp.Data;
using JuanApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JuanApp.Controllers
{
    public class ProductController(JuanDbContext juanDbContext) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail(int? id)
        {
            if (id == null)
                return NotFound();
            var existProduct = juanDbContext.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.ProductTags)
                .ThenInclude(x => x.Tag)
                .FirstOrDefault(x => x.Id == id);
            if (existProduct == null)
                return NotFound();
            ProductDetailVm productDetailVm = new ProductDetailVm()
            {
                Product = existProduct,
                RelatedProducts = juanDbContext.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Where(b => b.CategoryId == existProduct.CategoryId)
                .ToList()
            };
            return View(productDetailVm);
        }
    }
}
