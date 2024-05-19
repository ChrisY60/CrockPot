using CrockPot.Data;
using CrockPot.Models;
using CrockPot.Services.IServices;
using CrockPot.ViewModels.SharedRecipes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CrockPot.Services
{
    public class SharedRecipeService : ISharedRecipeService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public SharedRecipeService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> CreateSharedRecipeAsync(SharedRecipe sharedRecipe)
        {
            try
            {
                _context.SharedRecipes.Add(sharedRecipe);
                await _context.SaveChangesAsync();
            }catch (DbUpdateException)
            {
                return false;
            }
            
            return true;
        }

        public async Task<List<SharedRecipe>> GetSharedRecipesAsync()
        {
            return await _context.SharedRecipes.ToListAsync();
        }

        public async Task<List<SharedRecipe>> GetSharedRecipesByReceiverAsync(string receiverId)
        {
            return await _context.SharedRecipes
                .Include(sr => sr.Recipe)
                .Where(sr => sr.ReceiverId == receiverId)
                .ToListAsync();
        }

        public async Task<IndexSharedRecipeViewModel> GetIndexViewModel(string currentUser)
        {
            List<SharedRecipe> sharedRecipes = await GetSharedRecipesByReceiverAsync(currentUser);
            Dictionary<string, string> senderNames = await GetSenderNames(sharedRecipes);

            Dictionary<int, string> timeDifference = new Dictionary<int, string>();
            foreach (SharedRecipe sharedRecipe in sharedRecipes)
            {
                timeDifference[sharedRecipe.Id] = CalculateTimeDifference(sharedRecipe.TimeOfSending, DateTime.Now);
            }

            IndexSharedRecipeViewModel viewModel = new IndexSharedRecipeViewModel
            {
                SharedRecipes = sharedRecipes,
                SendersNames = senderNames,
                TimeDifference = timeDifference
            };
            return viewModel;
        }

        public string CalculateTimeDifference(DateTime startDateTime, DateTime endDateTime)
        {
            TimeSpan timeDifference = endDateTime - startDateTime;

            if (timeDifference.TotalDays >= 1)
            {
                int days = (int)timeDifference.TotalDays;
                return days == 1 ? "1 day ago" : $"{days} days ago";
            }
            else if (timeDifference.TotalHours >= 1)
            {
                int hours = (int)timeDifference.TotalHours;
                return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
            }
            else if (timeDifference.TotalMinutes >= 1)
            {
                int minutes = (int)timeDifference.TotalMinutes;
                return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
            }
            else
            {
                return "Just now";
            }
        }

        public async Task<Dictionary<string, string>> GetSenderNames(List<SharedRecipe> sharedRecipes)
        {
            Dictionary<string, string> senderNames = new Dictionary<string, string>();

            foreach (SharedRecipe sharedRecipe in sharedRecipes)
            {
                IdentityUser? senderUser = await _userManager.FindByIdAsync(sharedRecipe.SenderId);
                string? senderName = senderUser?.UserName;
                if (string.IsNullOrEmpty(senderName))
                {
                    senderName = "Unknown";
                }

                senderNames[sharedRecipe.SenderId] = senderName;
            }

            return senderNames;
        }
    }
}
