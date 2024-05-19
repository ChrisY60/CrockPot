using CrockPot.Data;
using CrockPot.Models;
using CrockPot.Services.IServices;
using CrockPot.ViewModels.Recipes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CrockPot.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobService _blobService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRatingService _ratingService;

        public RecipeService(ApplicationDbContext context, IBlobService blobService, UserManager<IdentityUser> userManager, IRatingService ratingService)
        {
            _context = context;
            _blobService = blobService;
            _userManager = userManager;
            _ratingService = ratingService;
        }

        public async Task<List<Recipe>> GetRecipesAsync()
        {
            return await _context.Recipes.ToListAsync();
        }

        public async Task<Recipe> GetRecipeByIdAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.Categories)
                .Include(r => r.Ingredients)
                .Include(r => r.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> CreateRecipeAsync(CreateRecipeViewModel viewModel, string currentUser, ModelStateDictionary modelState)
        {
            try
            {
                Recipe recipe = new Recipe
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    AuthorId = currentUser,
                };

                string[] allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".svg" };

                string? fileExtension = Path.GetExtension(viewModel.ImageFile.FileName).ToLower();

                if (allowedExtensions.Contains(fileExtension))
                {
                    string? imageUrl = await _blobService.UploadImageAsync(viewModel.ImageFile);
                    recipe.ImageUrl = imageUrl;
                }
                else
                {
                    modelState.AddModelError("Image", "Invalid file type. Please upload a PNG, JPG, JPEG, or SVG image.");
                    return false;
                }
                if (viewModel.SelectedCategories != null)
                {
                    recipe.Categories = await _context.Categories.Where(c => viewModel.SelectedCategories.Contains(c.Id)).ToListAsync();
                }

                if (viewModel.SelectedIngredients != null)
                {
                    recipe.Ingredients = await _context.Ingredients.Where(i => viewModel.SelectedIngredients.Contains(i.Id)).ToListAsync();
                }

                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                modelState.AddModelError(string.Empty, "Could not create the recipe. Please try again.");
                return false;
            }
        }

        public async Task<bool> UpdateRecipeAsync(EditRecipeViewModel viewModel, string currentUser, ModelStateDictionary modelState)
        {
            try
            {
                Recipe? recipe = await GetRecipeByIdAsync(viewModel.Id);
                
                if (recipe == null)
                {
                    modelState.AddModelError(string.Empty, "Recipe not found");
                    return false;
                }
                if(currentUser != recipe.AuthorId)
                {
                    modelState.AddModelError(string.Empty, "You do not have access to update this recipe");
                }

                recipe.Name = viewModel.Name;
                recipe.Description = viewModel.Description;
                recipe.Categories = await _context.Categories.Where(c => viewModel.SelectedCategories.Contains(c.Id)).ToListAsync();
                recipe.Ingredients = await _context.Ingredients.Where(i => viewModel.SelectedIngredients.Contains(i.Id)).ToListAsync();

                _context.Update(recipe);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                modelState.AddModelError(string.Empty, "Failed to update the recipe. Please try again.");
                return false;
            }
        }


        public async Task<bool> DeleteRecipeAsync(int id, ModelStateDictionary modelState)
        {
            try
            {
                Recipe? recipe = await _context.Recipes.FindAsync(id);
                if (recipe != null)
                {
                    _context.Recipes.Remove(recipe);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (DbUpdateException)
            {
                modelState.AddModelError(string.Empty, "An error occurred while deleting this recipe. Please try again.");
                return false;
            }
        }

        public bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }

        public async Task<List<Recipe>> GetAllRecipesByFilterAsync(string name, int[] selectedCategories, int[] selectedIngredients)
        {
            IQueryable<Recipe> query = _context.Recipes;

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(recipe => recipe.Name.Contains(name));
            }

            if (selectedCategories != null && selectedCategories.Length > 0)
            {
                query = query.Where(recipe => recipe.Categories.Any(category => selectedCategories.Contains(category.Id)));
            }

            if (selectedIngredients != null && selectedIngredients.Length > 0)
            {
                query = query.Where(recipe => recipe.Ingredients.All(ingredient => selectedIngredients.Contains(ingredient.Id)));
            }

            return await query.ToListAsync();

        }

        public async Task<Dictionary<string, string>> GetAuthorsNames(List<Recipe> Recipes)
        {
            Dictionary<string, string> authorsNames = new Dictionary<string, string>();
            foreach (Recipe r in Recipes)
            {
                IdentityUser? authorUser = await _userManager.FindByIdAsync(r.AuthorId);
                string? authorName = authorUser?.UserName;
                if (authorName != null) { authorsNames[r.AuthorId] = authorName; }
            }
            return authorsNames;
        }

        public async Task<DetailsRecipeViewModel> GetDetailsViewModelByRecipeId(int recipeId,string currentUser){
            Recipe? recipe = await GetRecipeByIdAsync(recipeId);
            IdentityUser? author = await _userManager.FindByIdAsync(recipe.AuthorId);
            string? authorName = author != null ? author.UserName : "Unknown";
            double averageRating = await _ratingService.GetAverageRatingByRecipeIdAsync(recipe.Id);
            List<IdentityUser>? allUsers = await _userManager.Users.ToListAsync();
            Rating? currentRating = await _ratingService.GetUserRatingOnRecipeAsync(currentUser, recipe.Id);

            DetailsRecipeViewModel viewModel = new DetailsRecipeViewModel
            {
                Recipe = recipe,
                AuthorName = authorName,
                AverageRating = averageRating,
                AllUsers = allUsers,
                CurrentRating = currentRating
            };
            return viewModel;

        }

        public async Task<EditRecipeViewModel> GetEditViewModelByRecipeId(int recipeId)
        {
            Recipe? recipe = await GetRecipeByIdAsync(recipeId);
            EditRecipeViewModel viewModel = new EditRecipeViewModel
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                SelectedCategories = recipe.Categories.Select(c => c.Id).ToArray(),
                SelectedIngredients = recipe.Ingredients.Select(i => i.Id).ToArray(),
                AllCategories = await _context.Categories.ToListAsync(),
                AllIngredients = await _context.Ingredients.ToListAsync()
            };
            return viewModel;
        }
    }
}
