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
        private readonly ICommentService _commentService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRatingService _ratingService;
        private readonly IBlobService _blobService;


        public RecipesController(IRecipeService recipeService, ICategoryService categoryService, IIngredientService ingredientService, ICommentService commentService, UserManager<IdentityUser> userManager, IRatingService ratingService, IBlobService blobService)
        {
            _recipeService = recipeService;
            _categoryService = categoryService;
            _ingredientService = ingredientService;
            _commentService = commentService;
            _ratingService = ratingService;
            _userManager = userManager;
            _blobService = blobService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Recipe>? recipes = await _recipeService.GetRecipesAsync();
            Dictionary<string, string>? authorsNames = await GetAuthorsNames(recipes);

            IndexRecipeViewModel viewModel = new IndexRecipeViewModel
            {
                Recipes = recipes,
                AuthorsNames = authorsNames
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

            Recipe? recipe = await _recipeService.GetRecipeByIdAsync(id);
            IdentityUser? author = await _userManager.FindByIdAsync(recipe.AuthorId);
            string? authorName = author != null ? author.UserName : "Unknown";
            double averageRating = await _ratingService.GetAverageRatingByRecipeIdAsync(recipe.Id);
            List<IdentityUser>? allUsers = await _userManager.Users.ToListAsync();
            Rating? currentRating = await _ratingService.GetUserRatingOnRecipeAsync(User.FindFirstValue(ClaimTypes.NameIdentifier), recipe.Id);

            DetailsRecipeViewModel viewModel = new DetailsRecipeViewModel
            {
                Recipe = recipe,
                AuthorName = authorName,
                AverageRating = averageRating,
                AllUsers = allUsers,
                CurrentRating = currentRating
            };

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
            List<Category>? categories = await _categoryService.GetCategoriesAsync();
            List<Ingredient>? ingredients = await _ingredientService.GetIngredientsAsync();

            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != recipe.AuthorId)
            {
                return StatusCode(403);
            }

            EditRecipeViewModel viewModel = new EditRecipeViewModel
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                SelectedCategories = recipe.Categories.Select(c => c.Id).ToArray(),
                SelectedIngredients = recipe.Ingredients.Select(i => i.Id).ToArray(),
                AllCategories = categories,
                AllIngredients = ingredients
            };

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
                AuthorsNames = await GetAuthorsNames(recipes)
            };
            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> HighestRatedRecipes()
        {
            List<(Recipe, float)> highestRatedRecipes = await _ratingService.GetHighestRatedRecipesAsync();
            return View("HighestRatedRecipes", highestRatedRecipes);
        }

        private async Task<Dictionary<string, string>> GetAuthorsNames(List<Recipe>Recipes){
            Dictionary<string, string> authorsNames = new Dictionary<string, string>();
            foreach(Recipe r in Recipes){
                IdentityUser? authorUser = await _userManager.FindByIdAsync(r.AuthorId);
                string? authorName = authorUser?.UserName;
                if(authorName != null) {authorsNames[r.AuthorId] = authorName;}
            }
            return authorsNames;
        }
    }
}
