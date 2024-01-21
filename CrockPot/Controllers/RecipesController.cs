using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrockPot.Models;
using Microsoft.AspNetCore.Authorization;
using CrockPot.Services.IServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

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
            
            ViewBag.Author = await _userManager.FindByIdAsync(recipe.AuthorId);
            if(ViewBag.Author == null)
            {
                ViewBag.Author = "Unknown";
            }

            var comments = await _commentService.GetCommentsByRecipeIdAsync(recipe.Id);

            var formattedComments = new List<string>();

            foreach (var comment in comments)
            {
                var author = await _userManager.FindByIdAsync(comment.AuthorId);
                string authorName = author != null ? author.UserName : "Unknown";
                formattedComments.Add($"{authorName}: {comment.Content}");
            }

            ViewBag.Comments = formattedComments;

            ViewBag.AverageRating = await _ratingService.GetAverageRatingByRecipeIdAsync(recipe.Id);

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
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Recipe recipe, int[] selectedCategories, int[] selectedIngredients, IFormFile imageFile)
        {
            recipe.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".svg"}; 

                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

                    if (allowedExtensions.Contains(fileExtension))
                    {

                        var imageUrl = await _blobService.UploadImageAsync(imageFile);
                        recipe.ImageUrl = imageUrl;
                    }
                    else
                    {
                        ModelState.AddModelError("Image", "Invalid file type. Please upload a PNG, JPG, JPEG or SVG image.");
                        ViewBag.allCategories = await _categoryService.GetCategoriesAsync();
                        ViewBag.allIngredients = await _ingredientService.GetIngredientsAsync();
                        return View(recipe);
                    }
                }

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
