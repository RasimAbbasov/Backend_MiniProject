using JuanApp.Areas.Manage.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using JuanApp.Data;
using Microsoft.EntityFrameworkCore;
using JuanApp.Models;
using JuanApp.Areas.Manage.Helpers;
using JuanApp.Services;
using Microsoft.Extensions.Options;
using JuanApp.Settings;

namespace JuanApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController(JuanDbContext juanDbContext,
        EmailService emailService,
        IOptions<EmailSetting> emailOptions
        ) : Controller
    {

        public IActionResult Index(int page = 1, int take = 2)
        {
            var query = juanDbContext.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .AsQueryable();
            PaginationVm<Product> data = PaginationVm<Product>.Paginate(query, page, take);
            return View(data);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var product = juanDbContext.Products
                .Include(x => x.ProductImages)
                .FirstOrDefault(x => x.Id == id);
            if (product == null)
                return NotFound();
            foreach (var productImage in product.ProductImages)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img/product", productImage.Image);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            juanDbContext.Products.Remove(product);
            juanDbContext.SaveChanges();
            return Ok();
        }
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(juanDbContext.Categories.ToList(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {
            var emails = juanDbContext.Users
                .Select(x => x.Email)
                .ToList();

            ViewBag.Categories = new SelectList(juanDbContext.Categories.ToList(), "Id", "Name");
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!juanDbContext.Categories.Any(x => x.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Category is not valid");
                return View();
            }
            if (product.Files == null)
            {
                ModelState.AddModelError("Files", "Files is not valid");
                return View();
            }
            if (product.MainFile == null)
            {
                ModelState.AddModelError("MainFile", "MainFile is not valid");
            }
            foreach (var file in product.Files)
            {
                ProductImage productImage = new ProductImage();
                productImage.Image = file.Save("wwwroot/assets/img/product");
                productImage.ProductId = product.Id;
                productImage.IsMain = false;
                product.ProductImages.Add(productImage);
            }
            ProductImage productMainImage = new ProductImage();
            productMainImage.IsMain = true;
            productMainImage.Image = product.MainFile.Save("wwwroot/assets/img/product");
            product.ProductImages.Add(productMainImage);


            juanDbContext.Products.Add(product);
            juanDbContext.SaveChanges();
            
            var url = Url.Action("Detail", "Product", new { id = product.Id }, Request.Scheme);
            var body = $"<h1>New Product</h1><p>Product Name: {product.Name}</p><p>Product Price: {product.Price}</p><p>Product Url: <a href='{url}'>Click Here</a></p>";
            emailService.SendEmails(emails, "New Product", body, emailOptions.Value);
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int? id)
        {
            ViewBag.Categories = new SelectList(juanDbContext.Categories.ToList(), "Id", "Name");

            if (id == null)
                return NotFound();
            var product = juanDbContext.Products.FirstOrDefault(x => x.Id == id);
            if (product == null)
                return NotFound();
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            ViewBag.Categories = new SelectList(juanDbContext.Categories.ToList(), "Id", "Name");
            if (!ModelState.IsValid)
                return View(product);

            var existProduct = juanDbContext.Products
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == product.Id);

            if (existProduct == null)
                return NotFound();

            if (product.MainFile != null)
            {
                var oldMainImage = existProduct.ProductImages.FirstOrDefault(pi => pi.IsMain);
                if (oldMainImage != null)
                {
                    string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img/product/", oldMainImage.Image);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);

                    juanDbContext.ProductImages.Remove(oldMainImage);
                }

                string newMainFileName = product.MainFile.Save("wwwroot/assets/img/product");
                existProduct.ProductImages.Add(new ProductImage
                {
                    Image = newMainFileName,
                    IsMain = true
                });
            }

            if (product.Files != null && product.Files.Any())
            {
                foreach (var file in product.Files)
                {
                    string newFileName = file.Save("wwwroot/assets/img/product");
                    existProduct.ProductImages.Add(new ProductImage
                    {
                        Image =  newFileName,
                        IsMain = false
                    });
                }
            }

            existProduct.Name = product.Name;
            existProduct.Description = product.Description;
            existProduct.StockCount = product.StockCount;
            existProduct.InStock = product.InStock;
            existProduct.IsTopSeller = product.IsTopSeller;
            existProduct.IsNew = product.IsNew;
            existProduct.Price = product.Price;
            existProduct.DiscountPercentage = product.DiscountPercentage;
            existProduct.Rate = product.Rate;
            existProduct.CategoryId = product.CategoryId;
            existProduct.UpdateDate = DateTime.Now;

            juanDbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
