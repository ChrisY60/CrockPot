﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrockPot.Models;
using Microsoft.AspNetCore.Authorization;
using CrockPot.Services.IServices;

namespace CrockPot.Controllers
{
    [Authorize]
    public class RecipesController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly ICategoryService _categoryService;
        private readonly IIngredientService _ingredientService;

        public RecipesController(IRecipeService recipeService, ICategoryService categoryService, IIngredientService ingredientService)
        {
            _recipeService = recipeService;
            _categoryService = categoryService;
            _ingredientService = ingredientService;
        }

        public async Task<IActionResult> Index()
        {
            var recipes = await _recipeService.GetRecipesAsync();
            ViewBag.allCategories = await _categoryService.GetCategoriesAsync();
            ViewBag.allIngredients = await _ingredientService.GetIngredientsAsync();

            return View(recipes);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || !_recipeService.RecipeExists(id.Value))
            {
                return NotFound();
            }

            var recipe = await _recipeService.GetRecipeByIdAsync(id.Value);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        public IActionResult Create()
        {
            ViewBag.allCategories = _categoryService.GetCategoriesAsync().Result;
            ViewBag.allIngredients = _ingredientService.GetIngredientsAsync().Result;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,AuthorId")] Recipe recipe, int[] selectedCategories, int[] selectedIngredients)
        {
            if (ModelState.IsValid)
            {
                await _recipeService.CreateRecipeAsync(recipe, selectedCategories, selectedIngredients);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.allCategories = await _categoryService.GetCategoriesAsync();
            ViewBag.allIngredients = await _ingredientService.GetIngredientsAsync();

            return View(recipe);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || !_recipeService.RecipeExists(id.Value))
            {
                return NotFound();
            }

            var recipe = await _recipeService.GetRecipeByIdAsync(id.Value);
            if (recipe == null)
            {
                return NotFound();
            }
            ViewBag.allCategories = await _categoryService.GetCategoriesAsync();
            ViewBag.allIngredients = await _ingredientService.GetIngredientsAsync();

            return View(recipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,AuthorId")] Recipe recipe, int[] selectedCategories, int[] selectedIngredients)
        {
            if (id != recipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRecipe = await _recipeService.GetRecipeByIdAsync(id);

                    existingRecipe.Name = recipe.Name;
                    existingRecipe.Description = recipe.Description;
                    existingRecipe.AuthorId = recipe.AuthorId;

                    var categories = await _categoryService.GetCategoriesAsync();
                    existingRecipe.Categories = categories.Where(c => selectedCategories.Contains(c.Id)).ToList();

                    var ingredients = await _ingredientService.GetIngredientsAsync();
                    existingRecipe.Ingredients = ingredients.Where(i => selectedIngredients.Contains(i.Id)).ToList();

                    await _recipeService.UpdateRecipeAsync(existingRecipe);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_recipeService.RecipeExists(recipe.Id))
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

            ViewBag.allCategories = await _categoryService.GetCategoriesAsync();
            ViewBag.allIngredients = await _ingredientService.GetIngredientsAsync();

            return View(recipe);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || !_recipeService.RecipeExists(id.Value))
            {
                return NotFound();
            }

            var recipe = await _recipeService.GetRecipeByIdAsync(id.Value);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_recipeService.RecipeExists(id))
            {
                await _recipeService.DeleteRecipeAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id)
        {
            return _recipeService.RecipeExists(id);
        }


        public async Task<IActionResult> RecipeSearch()
        {
            ViewBag.allCategories = await _categoryService.GetCategoriesAsync();
            ViewBag.allIngredients = await _ingredientService.GetIngredientsAsync();
            return View("RecipeSearch");
        }
        public async Task<IActionResult> SearchByFilter(string name, int[] selectedCategories, int[] selectedIngredients)
        {
            var recipes = await _recipeService.GetAllRecipesByFilterAsync(name, selectedCategories, selectedIngredients);
            return View("Index", recipes);
        }



    }
}
