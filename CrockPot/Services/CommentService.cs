using CrockPot.Data;
using CrockPot.Models;
using CrockPot.Services.IServices;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CrockPot.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetCommentsAsync()
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments
                .Include(r => r.Recipe)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> CreateCommentAsync(Comment comment, string currentUser, ModelStateDictionary modelState)
        { 
            comment.AuthorId = currentUser;
            if(comment.Content == null || comment.Content.Length > 500)
            {
                modelState.AddModelError(string.Empty,"The comment must be between 1 and 500 symbols.");
            }
            try
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                modelState.AddModelError(string.Empty, "Failed to create the comment. Please try again.");
                return false;
            }
        }

        public async Task<bool> UpdateCommentAsync(int id, string content,string currentUser, ModelStateDictionary modelState)
        {
            try
            {
                Comment? comment = await GetCommentByIdAsync(id);
                if (comment == null){
                    modelState.AddModelError(string.Empty, "This comment does not exist.");
                    return false;
                }

                if (comment.AuthorId == currentUser)
                {
                    comment.Content = content;
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                    return true;
                }else{
                    modelState.AddModelError(string.Empty, "You do not have access to edit this comment.");
                    return false;
                }
                
            }
            catch (DbUpdateException)
            {
                modelState.AddModelError(string.Empty, "Failed to update the comment. Please try again.");
                return false;
            }
        }

        public async Task<bool> DeleteCommentAsync(int id, string currentUser, bool isAdmin, ModelStateDictionary modelState)
        {
            try
            {
                Comment? comment = await _context.Comments.FindAsync(id);
                if (comment == null){
                    modelState.AddModelError(string.Empty,"This comment doesn't exist");
                    return false;
                }
                if (comment.AuthorId != currentUser && !isAdmin)
                {
                    modelState.AddModelError(string.Empty, "You don't have access to delete this comment.");
                    return false;
                }
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                modelState.AddModelError(string.Empty, "Failed to update the comment. Please try again.");
                return false;
            }
        }

        public bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }

    }
}
