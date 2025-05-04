using JuanApp.Areas.Manage.Helpers;
using JuanApp.Areas.Manage.ViewModel;
using JuanApp.Data;
using JuanApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace JuanApp.Areas.Manage.Controllers
{
    [Area("Manage")]

    public class SliderController(JuanDbContext juanDbContext) : Controller
    {
        public IActionResult Index(int page = 1, int take = 2)
        {
            var query = juanDbContext.Sliders.AsQueryable();
            var paginatedList = PaginationVm<Slider>.Paginate(query, page, take);
            return View(paginatedList);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var slider = juanDbContext.Sliders.FirstOrDefault(x => x.Id == id);
            if (slider == null)
                return NotFound();
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img/slider/", slider.Image);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            juanDbContext.Sliders.Remove(slider);
            juanDbContext.SaveChanges();
            return Ok();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Slider slider)
        {

            if (!ModelState.IsValid)
                return View();
            var file = slider.File;
            if (file == null)
            {
                ModelState.AddModelError("File", "Please select a file");
                return View();
            }

            slider.Image = file.Save("wwwroot/assets/img/slider");
            slider.CreateDate = DateTime.Now;
            juanDbContext.Sliders.Add(slider);
            juanDbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var slider = juanDbContext.Sliders.FirstOrDefault(x => x.Id == id);
            if (slider == null)
                return NotFound();
            return View(slider);
        }
        [HttpPost]
        public IActionResult Edit(Slider slider)
        {
            var existSlider = juanDbContext.Sliders.FirstOrDefault(x => x.Id == slider.Id);
            if (existSlider == null)
                return NotFound();
            if (!ModelState.IsValid)
                return View(slider);

            var file = slider.File;
            string oldImageName = existSlider.Image;
            if (file != null)
            {
                existSlider.Image = file.Save("wwwroot/assets/img/slider");

                string path2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img/slider/", oldImageName);

                if (System.IO.File.Exists(path2))
                {
                    System.IO.File.Delete(path2);
                }
            }
            existSlider.Subtitle = slider.Subtitle;
            existSlider.Title = slider.Title;
            existSlider.Description = slider.Description;
            existSlider.ButtonText = slider.ButtonText;
            existSlider.ButtonLink = slider.ButtonLink;
            existSlider.Order = slider.Order;




            existSlider.UpdateDate = DateTime.Now;
            juanDbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
