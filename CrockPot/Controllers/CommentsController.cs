using CrockPot.Models;
using CrockPot.Services;
using CrockPot.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace CrockPot.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Comment comment)
        {
            await _commentService.CreateCommentAsync(comment, User.FindFirstValue(ClaimTypes.NameIdentifier), ModelState);
            TempData["MSErrorsFromCommentsRedirect"] = string.Join("\n ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));

            return RedirectToAction("Details", "Recipes", new { id = comment.RecipeId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Comment? comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != comment.AuthorId)
            {
                return StatusCode(403);
            }
            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Comment comment)
        {
            if (await _commentService.UpdateCommentAsync(comment.Id, comment.Content, User.FindFirstValue(ClaimTypes.NameIdentifier), ModelState)){
                    return RedirectToAction("Details", "Recipes", new { id = comment.RecipeId });
            }
            return View(comment);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Comment? comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != comment.AuthorId && !User.IsInRole("Admin"))
            {
                return StatusCode(403);
            }
            return View(comment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Comment deltedComment = await _commentService.GetCommentByIdAsync(id);
            if (!await _commentService.DeleteCommentAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier), User.IsInRole("Admin"), ModelState))
            {
                return BadRequest("Failed to delete the comment. Please try again.");
            }
            return RedirectToAction("Details", "Recipes", new { id = deltedComment.RecipeId });
        }
    }
}
