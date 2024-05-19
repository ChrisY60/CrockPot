using CrockPot.Data.Migrations;
using CrockPot.Models;
using CrockPot.Services.IServices;
using CrockPot.ViewModels.SharedRecipes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CrockPot.Controllers
{
    [Authorize]
    public class SharedRecipesController : Controller
    {
        private readonly ISharedRecipeService _sharedRecipeService;

        public SharedRecipesController(ISharedRecipeService sharedRecipeService)
        {
            _sharedRecipeService = sharedRecipeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IndexSharedRecipeViewModel viewModel = await _sharedRecipeService.GetIndexViewModel(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string ReceiverId, int RecipeId)
        { 
            bool result = await _sharedRecipeService.CreateSharedRecipeAsync(new SharedRecipe(){SenderId = User.FindFirstValue(ClaimTypes.NameIdentifier),ReceiverId = ReceiverId,RecipeId = RecipeId,TimeOfSending = DateTime.Now});
            if (!result){
                ModelState.AddModelError(string.Empty, "Failed to share the recipe, please try again");
            }
            return RedirectToAction("Details", "Recipes", new { id = RecipeId });
        }

    }
}
