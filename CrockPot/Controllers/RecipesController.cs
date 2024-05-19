using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrockPot.Models;
using Microsoft.AspNetCore.Authorization;
using CrockPot.Services.IServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using CrockPot.ViewModels.Recipes;

namespace CrockPot.Controllers
{
    [Authorize]
    public class RecipesController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly ICategoryService _categoryService;
        private readonly IIngredientService _ingredientService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRatingService _ratingService;
        private readonly IBlobService _blobService;


        public RecipesController(IRecipeService recipeService, ICategoryService categoryService, IIngredientService ingredientService, UserManager<IdentityUser> userManager, IRatingService ratingService, IBlobService blobService)
        {
            _recipeService = recipeService;
            _categoryService = categoryService;
            _ingredientService = ingredientService;
            _ratingService = ratingService;
            _userManager = userManager;
            _blobService = blobService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Recipe>? recipes = await _recipeService.GetRecipesAsync();
            IndexRecipeViewModel viewModel = new IndexRecipeViewModel
            {
                Recipes = recipes,
                AuthorsNames = await _recipeService.GetAuthorsNames(recipes)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (TempData["MSErrorsFromCommentsRedirect"] != null)
            {
                ModelState.AddModelError(string.Empty, (string)TempData["MSErrorsFromCommentsRedirect"]);
            }
            DetailsRecipeViewModel viewModel = await _recipeService.GetDetailsViewModelByRecipeId(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateRecipeViewModel viewModel = new CreateRecipeViewModel
            {
                AllCategories = await _categoryService.GetCategoriesAsync(),
                AllIngredients = await _ingredientService.GetIngredientsAsync()
            };
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRecipeViewModel viewModel)
        {
            if(await _recipeService.CreateRecipeAsync(viewModel, User.FindFirstValue(ClaimTypes.NameIdentifier), ModelState))
            {
                viewModel.AllCategories = await _categoryService.GetCategoriesAsync();
                viewModel.AllIngredients = await _ingredientService.GetIngredientsAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if(!_recipeService.RecipeExists(id))
            {
                return NotFound();
            }
            Recipe? recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != recipe.AuthorId)
            {
                return StatusCode(403);
            }

            EditRecipeViewModel viewModel = await _recipeService.GetEditViewModelByRecipeId(id);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRecipeViewModel viewModel)
        {
            if(await _recipeService.UpdateRecipeAsync(viewModel, User.FindFirstValue(ClaimTypes.NameIdentifier), ModelState))
            {
                viewModel.AllIngredients = await _ingredientService.GetIngredientsAsync();
                viewModel.AllCategories = await _categoryService.GetCategoriesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!_recipeService.RecipeExists(id))
            {
                return NotFound();
            }

            Recipe recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != recipe.AuthorId && !User.IsInRole("Admin"))
            {
                return StatusCode(403);
            }

            return View(recipe);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await _recipeService.DeleteRecipeAsync(id, ModelState))
            {
                return BadRequest("Failed to delete the recipe. Please try again.");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> RecipeSearch()
        {
            SearchRecipeViewModel viewModel = new SearchRecipeViewModel
            {
                AllCategories = await _categoryService.GetCategoriesAsync(),
                AllIngredients = await _ingredientService.GetIngredientsAsync()
            };
            return View("RecipeSearch", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> SearchByFilter(string name, int[] selectedCategories, int[] selectedIngredients)
        {
            List<Recipe>? recipes = await _recipeService.GetAllRecipesByFilterAsync(name, selectedCategories, selectedIngredients);
            IndexRecipeViewModel viewModel = new IndexRecipeViewModel
            {
                Recipes = recipes,
                AuthorsNames = await _recipeService.GetAuthorsNames(recipes)
            };
            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> HighestRatedRecipes()
        {
            List<(Recipe, float)> highestRatedRecipes = await _ratingService.GetHighestRatedRecipesAsync();
            return View("HighestRatedRecipes", highestRatedRecipes);
        }

        
    }
}
