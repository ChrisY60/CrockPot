using CrockPot.Models;
using CrockPot.Services.IServices;
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

        

        public async Task<IActionResult> Index()
        {
            
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return NotFound();
            }
            List<SharedRecipe> sharedRecipes = await _sharedRecipeService.GetSharedRecipesByReceiverAsync(currentUser.Id);
            Dictionary<string, string> senderNames = await GetSenderNames(sharedRecipes);
            ViewData["SenderNames"] = senderNames;
            return View(sharedRecipes);
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
                Timestamp = DateTime.Now
            };

            var result = await _sharedRecipeService.CreateSharedRecipeAsync(newSharedRecipe);

            if (result)
            {
                return RedirectToAction("Details", "Recipes", new { id = RecipeId });
            }
            return NotFound();
        }

        private async Task<Dictionary<string, string>> GetSenderNames(List<SharedRecipe> sharedRecipes)
        {
            var senderNames = new Dictionary<string, string>();

            foreach (var sharedRecipe in sharedRecipes)
            {
                var senderUser = await _userManager.FindByIdAsync(sharedRecipe.SenderId);
                var senderName = senderUser?.UserName;

                senderNames[sharedRecipe.SenderId] = senderName;
            }

            return senderNames;
        }

    }
}
