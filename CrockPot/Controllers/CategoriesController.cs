using Microsoft.AspNetCore.Mvc;
using CrockPot.Models;
using Microsoft.AspNetCore.Authorization;
using CrockPot.Services.IServices;
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _categoryService.GetCategoriesAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int Id, string Name)
        {
            Category category = new Category(Id, Name);

            if (!await _categoryService.IsCategoryNameUniqueAsync(category.Name))
            {
                ModelState.AddModelError("Name", "A category with this name already exists.");
            }

            if (ModelState.IsValid)
            {
                var result = await _categoryService.CreateCategoryAsync(category);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create the category. Please try again.");
                    return View(category);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        [HttpGet]
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
                var result = await _categoryService.UpdateCategoryAsync(category);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update the category. Please try again.");
                    return View(category);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || !_categoryService.CategoryExists(id.Value))
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!_categoryService.CategoryExists(id))
            {
                return NotFound();
            }

            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
            {
                return BadRequest("Failed to delete the category. Please try again.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
