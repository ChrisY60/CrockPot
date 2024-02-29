using CrockPot.Models;
using CrockPot.Services.IServices;
using CrockPot.ViewModels.SharedRecipes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CrockPot.Controllers
{
    [Authorize]
    public class SharedRecipesController : Controller
    {
        private readonly ISharedRecipeService _sharedRecipeService;
        private readonly UserManager<IdentityUser> _userManager;

        public SharedRecipesController(ISharedRecipeService sharedRecipeService, UserManager<IdentityUser> userManager)
        {
            _sharedRecipeService = sharedRecipeService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return NotFound();
            }

            List<SharedRecipe> sharedRecipes = await _sharedRecipeService.GetSharedRecipesByReceiverAsync(currentUser.Id);
            Dictionary<string, string> senderNames = await GetSenderNames(sharedRecipes);

            var viewModel = new IndexSharedRecipeViewModel
            {
                SharedRecipes = sharedRecipes,
                SenderNames = senderNames
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string ReceiverId, int RecipeId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return NotFound();
            }

            var newSharedRecipe = new SharedRecipe()
            {
                SenderId = currentUser.Id,
                ReceiverId = ReceiverId,
                RecipeId = RecipeId,
                TimeOfSending = DateTime.Now
            };
            if(ModelState.IsValid)
            {
                var result = await _sharedRecipeService.CreateSharedRecipeAsync(newSharedRecipe);

                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to share the recipe, please try again");
                    return View(newSharedRecipe);
                }
                return RedirectToAction("Details", "Recipes", new { id = RecipeId });
            }
            return View(newSharedRecipe);
        }

        private async Task<Dictionary<string, string>> GetSenderNames(List<SharedRecipe> sharedRecipes)
        {
            var senderNames = new Dictionary<string, string>();

            foreach (var sharedRecipe in sharedRecipes)
            {
                var senderUser = await _userManager.FindByIdAsync(sharedRecipe.SenderId);
                var senderName = senderUser?.UserName;
                if (string.IsNullOrEmpty(senderName))
                {
                    senderName = "Unknown";
                }

                senderNames[sharedRecipe.SenderId] = senderName;
            }

            return senderNames;
        }

    }
}
