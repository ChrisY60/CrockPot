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
        public async Task<IActionResult> Create(int Id, string Name)
        {
            Ingredient ingredient = new Ingredient(Id, Name);

            if (!await _ingredientService.IsIngredientNameUniqueAsync(ingredient.Name))
            {
                ModelState.AddModelError("Name", "An ingredient with that name already exists.");
            }
            if (ModelState.IsValid)
            {
                var result = await _ingredientService.CreateIngredientAsync(ingredient);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create the ingredient. Please try again.");
                    return View(ingredient);
                }
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
        public async Task<IActionResult> Edit(int Id, string Name)
        {
            Ingredient ingredient = new(Id, Name);

            if (!await _ingredientService.IsIngredientNameUniqueAsync(ingredient.Name))
            {
                ModelState.AddModelError("Name", "An ingredient with that name already exists.");
            }

            if (ModelState.IsValid)
            {
                var result = await _ingredientService.UpdateIngredientAsync(ingredient);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update the ingredient. Please try again.");
                    return View(ingredient);
                }
                
                return RedirectToAction("Index");
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
            Debug.WriteLine("Got here 1 :)");
            if (!_ingredientService.IngredientExists(id))
            {
                return NotFound();
            }

            var result = await _ingredientService.DeleteIngredientAsync(id);
            if (!result)
            {
                return BadRequest("Failed to delete the ingredient. Please try again.");
            }
            Debug.WriteLine("Got here 3 :)");
            return RedirectToAction(nameof(Index));
        }
    }
}
