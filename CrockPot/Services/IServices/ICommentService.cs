using CrockPot.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CrockPot.Services.IServices
{
    public interface ICommentService
    {
        Task<List<Comment>> GetCommentsAsync();
        Task<Comment> GetCommentByIdAsync(int id);
        Task<bool> CreateCommentAsync(Comment comment, string currentUser, ModelStateDictionary modelState);
        Task<bool> UpdateCommentAsync(int id, string content, string currentUser, ModelStateDictionary modelState);
        Task<bool> DeleteCommentAsync(int id, string currentUser, bool isAdmin, ModelStateDictionary modelState);
        bool CommentExists(int id);
    }
}
