using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrockPot.Models;
using Microsoft.AspNetCore.Authorization;
using CrockPot.Services.IServices;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;

namespace CrockPot.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        
        public async Task<IActionResult> Index()
        {
            return View(await _categoryService.GetCategoriesAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _categoryService.CategoryExists(id.Value))
            {
                return NotFound();
            }
            var categories = await _categoryService.GetCategoriesAsync();

            var category = categories.FirstOrDefault(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int Id, string Name)
        {
            Category category = new Category(Id, Name);

            if(!await _categoryService.IsCategoryNameUniqueAsync(category.Name)){
                ModelState.AddModelError("Name", "A category with this name already exists.");
            }

            if (ModelState.IsValid)
            {
                await _categoryService.CreateCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || !_categoryService.CategoryExists(id.Value))
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, string Name)
        {
            Category category = new Category(Id, Name);

            if (!await _categoryService.IsCategoryNameUniqueAsync(category.Name))
            {
                ModelState.AddModelError("Name", "A category with this name already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryService.UpdateCategoryAsync(category);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    await ex.Entries.Single().ReloadAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _categoryService.CategoryExists(id.Value))
            {
                return NotFound();
            }

            var categories = await _categoryService.GetCategoriesAsync();
            var category = categories.FirstOrDefault(m => m.Id == id);
            if (category == null)
            {
                Debug.WriteLine("Got here");
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_categoryService.GetCategoriesAsync == null)
            {
                return Problem("This category does not exist");
            }
            await _categoryService.DeleteCategoryAsync(id);

            return RedirectToAction(nameof(Index));
        }

    }
}
