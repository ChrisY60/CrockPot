using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrockPot.Models;
using Microsoft.AspNetCore.Authorization;
using CrockPot.Services.IServices;

namespace CrockPot.Controllers
{
    [Authorize]
    public class IngredientsController : Controller
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        public async Task<IActionResult> Index()
        {
            var ingredients = await _ingredientService.GetIngredientsAsync();
            return View(ingredients);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || !_ingredientService.IngredientExists(id.Value))
            {
                return NotFound();
            }

            var ingredient = await _ingredientService.GetIngredientByIdAsync(id.Value);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                await _ingredientService.CreateIngredientAsync(ingredient);
                return RedirectToAction(nameof(Index));
            }

            return View(ingredient);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || !_ingredientService.IngredientExists(id.Value))
            {
                return NotFound();
            }

            var ingredient = await _ingredientService.GetIngredientByIdAsync(id.Value);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Ingredient ingredient)
        {
            if (id != ingredient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _ingredientService.UpdateIngredientAsync(ingredient);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_ingredientService.IngredientExists(ingredient.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(ingredient);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || !_ingredientService.IngredientExists(id.Value))
            {
                return NotFound();
            }

            var ingredient = await _ingredientService.GetIngredientByIdAsync(id.Value);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!_ingredientService.IngredientExists(id))
            {
                return Problem("Entity set 'ApplicationDbContext.Ingredients' is null.");
            }

            await _ingredientService.DeleteIngredientAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
