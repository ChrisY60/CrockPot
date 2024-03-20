using CrockPot.Data.Migrations;
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

            var timeDifference = new Dictionary<int, string>();
            foreach (var sharedRecipe in sharedRecipes){
                timeDifference[sharedRecipe.Id] = CalculateTimeDifference(sharedRecipe.TimeOfSending, DateTime.Now);
            }


            var viewModel = new IndexSharedRecipeViewModel
            {
                SharedRecipes = sharedRecipes,
                SendersNames = senderNames,
                TimeDifference = timeDifference
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
                }
                
            }
            return RedirectToAction("Details", "Recipes", new { id = RecipeId });
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
        private string CalculateTimeDifference(DateTime startDateTime, DateTime endDateTime)
        {
            TimeSpan timeDifference = endDateTime - startDateTime;

            if (timeDifference.TotalDays >= 1)
            {
                int days = (int)timeDifference.TotalDays;
                return days == 1 ? "1 day ago" : $"{days} days ago";
            }
            else if (timeDifference.TotalHours >= 1)
            {
                int hours = (int)timeDifference.TotalHours;
                return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
            }
            else if (timeDifference.TotalMinutes >= 1)
            {
                int minutes = (int)timeDifference.TotalMinutes;
                return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
            }
            else
            {
                return "Just now";
            }
        }


    }
}
