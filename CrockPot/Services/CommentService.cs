using CrockPot.Data;
using CrockPot.Models;
using CrockPot.Services.IServices;
using Microsoft.EntityFrameworkCore;

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
            return await _context.Comments.FindAsync(id);
        }

        public async Task<List<Comment>> GetCommentsByRecipeIdAsync(int recipeId)
        {
            return await _context.Comments
                .Where(comment => comment.RecipeId == recipeId)
                .ToListAsync();
        }

        public async Task<List<Comment>> GetCommentsByAuthorIdAsync(string authorId)
        {
            return await _context.Comments
                .Where(comment => comment.AuthorId == authorId)
                .ToListAsync();
        }

        public async Task<bool> CreateCommentAsync(Comment comment)
        {
            try
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCommentAsync(Comment comment)
        {
            try
            {
                _context.Update(comment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteCommentAsync(int id)
        {
            try
            {
                var comment = await _context.Comments.FindAsync(id);
                if (comment != null)
                {
                    _context.Comments.Remove(comment);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }

    }
}