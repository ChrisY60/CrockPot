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
            comment.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                var result = await _commentService.CreateCommentAsync(comment);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create the comment. Please try again.");
                    return View(comment);
                }
            }
            else
            {
                Debug.WriteLine("Errors are tuk :) : ");
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {

                        Debug.WriteLine(error.ErrorMessage);
                    }
                }

            }

            return RedirectToAction("Details", "Recipes", new { id = comment.RecipeId });
        }


        public async Task<IActionResult> Edit(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
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
            if (ModelState.IsValid)
            {
                var existingComment = await _commentService.GetCommentByIdAsync(comment.Id);
                if (existingComment == null)
                {
                    return NotFound();
                }

                if (User.FindFirstValue(ClaimTypes.NameIdentifier) != existingComment.AuthorId)
                {
                    return StatusCode(403);
                }

                existingComment.Content = comment.Content;

                var result = await _commentService.UpdateCommentAsync(existingComment);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update the comment. Please try again.");
                    return View(existingComment);
                }

                return RedirectToAction("Details", "Recipes", new { id = existingComment.RecipeId });
            }
            else
            {
                Debug.WriteLine("Errors are tuk :) : ");
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        Debug.WriteLine(error.ErrorMessage);
                    }
                }
            }

            return View(comment);
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || !_commentService.CommentExists(id.Value))
            {
                return NotFound();
            }

            var comment = await _commentService.GetCommentByIdAsync(id.Value);
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
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var recipeId = comment.RecipeId;

            if (!_commentService.CommentExists(id))
            {
                return NotFound();
            }

            var result = await _commentService.DeleteCommentAsync(id);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Failed to delete the comment. Please try again.");
                return View(await _commentService.GetCommentByIdAsync(id)); 
            }

            return RedirectToAction("Details", "Recipes", new { id = recipeId });
        }
    }
}
