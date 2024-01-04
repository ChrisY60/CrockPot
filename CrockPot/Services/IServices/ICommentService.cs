using CrockPot.Models;

namespace CrockPot.Services.IServices
{
    public interface ICommentService
    {
        Task<List<Comment>> GetCommentsAsync();
        Task<Comment> GetCommentByIdAsync(int id);
        Task<List<Comment>> GetCommentsByRecipeIdAsync(int recipeId);
        Task<List<Comment>> GetCommentsByAuthorIdAsync(string authorId);
        Task<bool> CreateCommentAsync(Comment comment);
        Task<bool> UpdateCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(int id);
        bool CommentExists(int id);
    }
}
