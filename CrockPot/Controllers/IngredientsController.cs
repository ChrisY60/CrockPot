using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrockPot.Models;
using Microsoft.AspNetCore.Authorization;
using CrockPot.Services.IServices;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using CrockPot.Services;

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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Ingredient>? ingredients = await _ingredientService.GetIngredientsAsync();
            return View(ingredients);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            Ingredient? ingredient = await _ingredientService.GetIngredientByIdAsync(id.Value);
            if (ingredient == null)
            {
                return NotFound();
            }
            return View(ingredient);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ingredient ingredient)
        {
            if(await _ingredientService.CreateIngredientAsync(ingredient, ModelState)){
                return RedirectToAction(nameof(Index));
            }
            return View(ingredient);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Ingredient? ingredient = await _ingredientService.GetIngredientByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            return View(ingredient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Ingredient ingredient)
        {
            if(await _ingredientService.UpdateIngredientAsync(ingredient, ModelState))
            {
                return RedirectToAction(nameof(Index));
            }
            return View(ingredient);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Ingredient? ingredient = await _ingredientService.GetIngredientByIdAsync(id);
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
            if (!await _ingredientService.DeleteIngredientAsync(id))
            {
                return BadRequest("Failed to delete the ingredient. Please try again.");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
