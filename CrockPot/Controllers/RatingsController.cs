using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CrockPot.Models;
using CrockPot.Services.IServices;
using System.Security.Claims;
using System.Diagnostics;
using System.Collections.Generic;

namespace CrockPot.Controllers
{
    [Authorize]
    public class RatingsController : Controller
    {
        private readonly IRatingService _ratingService;

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }


        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(Rating rating)
        {
            await _ratingService.SubmitRatingAsync(rating, User.FindFirstValue(ClaimTypes.NameIdentifier), ModelState);
            return RedirectToAction("Details", "Recipes", new { id = rating.RecipeId });
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Rating? rating = await _ratingService.GetRatingByIdAsync(id);
            if (rating == null)
            {
                return NotFound();
            }
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != rating.AuthorId)
            {
                return StatusCode(403);
            }

            await _ratingService.DeleteRatingAsync(id);
            return RedirectToAction("Details", "Recipes", new { id = rating.RecipeId });
        }

        [HttpGet]
        public async Task<IActionResult> GetRecommendedRecipes()
        {
            List<(Recipe, float)>? recipes = await _ratingService.GetRecommendedRecipesAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Debug.WriteLine("tukaaa");
            for(int i = 0; i < recipes.Count; i++)
            {
                Debug.WriteLine(recipes[i].Item1.Name);   
            }
            Debug.WriteLine("tukaaa");
            return View("~/Views/Recipes/RecommendedRecipes.cshtml", recipes);
        }

    }
}
