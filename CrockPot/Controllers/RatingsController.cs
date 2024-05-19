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

    }
}
