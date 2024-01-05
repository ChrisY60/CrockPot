using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CrockPot.Models;
using CrockPot.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CrockPot.Controllers
{
    [Authorize]
    public class RatingsController : Controller
    {
        private readonly IRatingService _ratingService;
        private readonly UserManager<IdentityUser> _userManager;

        public RatingsController(IRatingService ratingService, UserManager<IdentityUser> userManager)
        {
            _ratingService = ratingService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _ratingService.GetRatingsAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _ratingService.GetRatingsAsync == null)
            {
                return NotFound();
            }

            var ratings = await _ratingService.GetRatingsAsync();
            var rating = ratings.FirstOrDefault(m => m.Id == id);

            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        public IActionResult Create(int recipeId)
        {
            ViewBag.RecipeIdRating = recipeId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AuthorId,RecipeId,RatingValue")] Rating rating)
        {
            rating.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                await _ratingService.CreateRatingAsync(rating);
                return RedirectToAction("Details", "Recipes", new { id = rating.RecipeId });
            }

            return View(rating);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (_ratingService.GetRatingsAsync == null)
            {
                return NotFound();
            }

            var rating = await _ratingService.GetRatingByIdAsync(id);

            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AuthorId,RecipeId,RatingValue")] Rating rating)
        {
            if (id != rating.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _ratingService.UpdateRatingAsync(rating);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_ratingService.RatingExists(rating.Id))
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

            return View(rating);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _ratingService.GetRatingsAsync == null)
            {
                return NotFound();
            }

            var ratings = await _ratingService.GetRatingsAsync();
            var rating = ratings.FirstOrDefault(m => m.Id == id);

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
            if (_ratingService.GetRatingsAsync == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Ratings' is null.");
            }

            await _ratingService.DeleteRatingAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool RatingExists(int id)
        {
            return _ratingService.RatingExists(id);
        }
    }
}
