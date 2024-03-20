using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CrockPot.Models;
using CrockPot.Services.IServices;
using System.Security.Claims;

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
            rating.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                var result = await _ratingService.SubmitRatingAsync(rating);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create the rating. Please try again.");
                }
            }

            return RedirectToAction("Details", "Recipes", new { id = rating.RecipeId });
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var rating = await _ratingService.GetRatingByIdAsync(id);

            if (rating == null)
            {
                return NotFound();
            }

            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != rating.AuthorId)
            {
                return StatusCode(403);
            }
            var result = await _ratingService.DeleteRatingAsync(id);
            if (!result)
            {
                return Problem("Failed to delete the rating.");
            }

            return RedirectToAction("Details", "Recipes", new { id = rating.RecipeId });
        }

    }
}
