using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CrockPot.Models;
using CrockPot.Services.IServices;
using System.Security.Claims;
using System.Diagnostics;

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

        public async Task<IActionResult> Index()
        {
            var ratings = await _ratingService.GetRatingsAsync();
            return View(ratings);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _ratingService.GetRatingByIdAsync(id.Value);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rating rating)
        {
            rating.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                await _ratingService.CreateRatingAsync(rating);
            }

            return RedirectToAction("Details", "Recipes", new { id = rating.RecipeId });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _ratingService.GetRatingByIdAsync(id.Value);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _ratingService.DeleteRatingAsync(id);
            if (!success)
            {
                return Problem("Failed to delete the rating.");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RatingExists(int id)
        {
            return _ratingService.RatingExists(id);
        }
    }
}
