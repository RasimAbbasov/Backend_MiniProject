using Humanizer.Localisation;
using JuanApp.Areas.Manage.ViewModel;
using JuanApp.Data;
using JuanApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace JuanApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CategoryController(JuanDbContext juanDbContext) : Controller
    {
        public IActionResult Index(int page = 1, int take = 2)
        {
            var query = juanDbContext.Categories.AsQueryable();
            PaginationVm<Category> paginatedList = PaginationVm<Category>.Paginate(query, page, take);
            return View(paginatedList);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var category = juanDbContext.Categories.FirstOrDefault(x => x.Id == id);
            if (category == null)
                return NotFound();
            juanDbContext.Categories.Remove(category);
            juanDbContext.SaveChanges();
            return Ok();
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (juanDbContext.Categories.Any(g => g.Name.ToUpper() == category.Name.ToUpper()))
            {
                ModelState.AddModelError("Name", "Bu adli category movcuddur.");
                return View();
            }

            juanDbContext.Categories.Add(category);
            juanDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var category = juanDbContext.Categories.FirstOrDefault(x => x.Id == id);
            if (category == null)
                return NotFound();

            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View();
            var existCategory = juanDbContext.Categories.FirstOrDefault(x => x.Id == category.Id);
            if (existCategory == null)
                return NotFound();
            if (existCategory.Name != category.Name
                &&
                juanDbContext.Categories.Any(g => g.Name.ToUpper() == category.Name.ToUpper() && g.Id != existCategory.Id)
                )
            {
                ModelState.AddModelError("Name", "There is a genre named like that");
                return View();
            }
            existCategory.Name = category.Name;
            juanDbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Detail(int? id)
        {
            if (id == null)
                return NotFound();
            var category = juanDbContext.Categories.FirstOrDefault(x => x.Id == id);
            if (category == null)
                return NotFound();

            return View(category);
        }

    }
}
