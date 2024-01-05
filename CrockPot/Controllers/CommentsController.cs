using CrockPot.Models;
using CrockPot.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

       public async Task<IActionResult> Index()
        {
            var comments = await _commentService.GetCommentsAsync();
            return View(comments);
        }

        public async Task<IActionResult> Details(int? id)
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

            return View(comment);
        }

        public async Task<IActionResult> GetCommentsByRecipeId(int recipeId)
        {
            var comments = await _commentService.GetCommentsByRecipeIdAsync(recipeId);
            return View("Index", comments); 
        }

        public async Task<IActionResult> GetCommentsByAuthorId()
        {
            var authorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comments = await _commentService.GetCommentsByAuthorIdAsync(authorId);
            return View("Index", comments); 
        }


        public IActionResult Create(int recipeId)
        {
            ViewBag.RecipeIdComment = recipeId;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Comment comment)
        {
            comment.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                await _commentService.CreateCommentAsync(comment);
                return RedirectToAction("Details", "Recipes", new { id = comment.RecipeId });
            }

            return View(comment);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            comment.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                await _commentService.UpdateCommentAsync(comment);
                return RedirectToAction("Details", "Recipes", new { id = comment.RecipeId });
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

            return View(comment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int recipeId)
        {
            if (_commentService.CommentExists(id))
            {
                await _commentService.DeleteCommentAsync(id);
            }

            return RedirectToAction("Details", "Recipes", new { id = recipeId });
        }
    }
}
