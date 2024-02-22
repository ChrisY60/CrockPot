using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrockPot.Models;
using Microsoft.AspNetCore.Authorization;
using CrockPot.Services.IServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using CrockPot.ViewModels;
using System.Diagnostics;
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

        public async Task<IActionResult> Index()
        {
            var recipes = await _recipeService.GetRecipesAsync();
            var authorsNames = await GetAuthorsNames(recipes);

            var viewModel = new IndexRecipeViewModel
            {
                Recipes = recipes,
                AuthorNames = authorsNames
            };

            return View(viewModel);
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

            var comments = await _commentService.GetCommentsByRecipeIdAsync(recipe.Id);

            var author = await _userManager.FindByIdAsync(recipe.AuthorId);
            var authorName = author != null ? author.UserName : "Unknown";
            var averageRating = await _ratingService.GetAverageRatingByRecipeIdAsync(recipe.Id);
            var allUsers = await _userManager.Users.ToListAsync();
            var currentRating = await GetUserRatingOnRecipeAsync(recipe.Id);

            var viewModel = new DetailsRecipeViewModel
            {
                Recipe = recipe,
                AuthorName = authorName,
                Comments = comments,
                AverageRating = averageRating,
                AllUsers = allUsers,
                CurrentRating = currentRating
            };

            return View(viewModel);
        }


        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateRecipeViewModel
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
            string authorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Recipe recipe = new Recipe
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                AuthorId = authorId,
            };

            if (ModelState.IsValid)
            {
                if (viewModel.ImageFile != null)
                {
                    var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".svg" };

                    var fileExtension = Path.GetExtension(viewModel.ImageFile.FileName).ToLower();

                    if (allowedExtensions.Contains(fileExtension))
                    {
                        var imageUrl = await _blobService.UploadImageAsync(viewModel.ImageFile);
                        recipe.ImageUrl = imageUrl;
                    }
                    else
                    {
                        ModelState.AddModelError("Image", "Invalid file type. Please upload a PNG, JPG, JPEG, or SVG image.");
                        return View(viewModel);
                    }
                }

                var result = await _recipeService.CreateRecipeAsync(recipe, viewModel.SelectedCategories, viewModel.SelectedIngredients);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create the recipe. Please try again.");
                    return View(viewModel);
                }

                return RedirectToAction(nameof(Index));
            }
            viewModel.AllCategories = await _categoryService.GetCategoriesAsync();
            viewModel.AllIngredients = await _ingredientService.GetIngredientsAsync();
            return View(viewModel);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            var categories = await _categoryService.GetCategoriesAsync();
            var ingredients = await _ingredientService.GetIngredientsAsync();

            var viewModel = new EditRecipeViewModel
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
            if (ModelState.IsValid)
            {
                var recipe = await _recipeService.GetRecipeByIdAsync(viewModel.Id);
                if (recipe == null)
                {
                    return NotFound();
                }

                recipe.Name = viewModel.Name;
                recipe.Description = viewModel.Description;

                var categories = await _categoryService.GetCategoriesAsync();
                recipe.Categories = categories.Where(c => viewModel.SelectedCategories.Contains(c.Id)).ToList();

                var ingredients = await _ingredientService.GetIngredientsAsync();
                recipe.Ingredients = ingredients.Where(i => viewModel.SelectedIngredients.Contains(i.Id)).ToList();

                var result = await _recipeService.UpdateRecipeAsync(recipe);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update the recipe. Please try again.");
                    return View(viewModel);
                }

                return RedirectToAction(nameof(Index));
            }
            Debug.WriteLine("Something went wrong");
            viewModel.AllIngredients = await _ingredientService.GetIngredientsAsync();
            viewModel.AllCategories = await _categoryService.GetCategoriesAsync();
            return View(viewModel);
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
            Recipe recipe = await _recipeService.GetRecipeByIdAsync(id);
            if(recipe == null)
            {
                return NotFound();
            }

            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != recipe.AuthorId && !User.IsInRole("Admin"))
            {
                return StatusCode(403);
            }

            if (_recipeService.RecipeExists(id))
            {
                var result = await _recipeService.DeleteRecipeAsync(id);
                if (!result)
                {
                    return BadRequest("Failed to delete the recipe. Please try again.");
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RecipeSearch()
        {
            var viewModel = new SearchRecipeViewModel
            {
                AllCategories = await _categoryService.GetCategoriesAsync(),
                AllIngredients = await _ingredientService.GetIngredientsAsync()
            };
            return View("RecipeSearch", viewModel);
        }

        public async Task<IActionResult> SearchByFilter(string name, int[] selectedCategories, int[] selectedIngredients)
        {
            var recipes = await _recipeService.GetAllRecipesByFilterAsync(name, selectedCategories, selectedIngredients);
            var authorsNames = await GetAuthorsNames(recipes);

            var viewModel = new IndexRecipeViewModel
            {
                Recipes = recipes,
                AuthorNames = authorsNames
            };

            return View("Index", viewModel);
        }


        public async Task<IActionResult> HighestRatedRecipes()
        {
            ViewBag.HighestRatedRecipes = await _ratingService.GetHighestRatedRecipesAsync();
            return View("HighestRatedRecipes");
        }

        public async Task<Dictionary<string, string>> GetAuthorsNames(List<Recipe>Recipes){
            var authorsNames = new Dictionary<string, string>();

            foreach(Recipe r in Recipes){
                var authorUser = await _userManager.FindByIdAsync(r.AuthorId);
                var authorName = authorUser?.UserName;

                authorsNames[r.AuthorId] = authorName;
            }

            return authorsNames;
        }

        public async Task<Rating> GetUserRatingOnRecipeAsync(int recipeId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _ratingService.GetUserRatingOnRecipeAsync(userId, recipeId);
        }



    }
}
