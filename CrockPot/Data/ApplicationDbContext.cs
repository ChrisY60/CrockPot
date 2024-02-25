using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CrockPot.Models;

namespace CrockPot.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        public DbSet<SharedRecipe> SharedRecipes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Recipe>()
                .HasMany(r => r.Categories)
                .WithMany(c => c.Recipes)
                .UsingEntity(j => j.ToTable("RecipeCategory"));

            modelBuilder.Entity<Recipe>()
                .HasMany(r => r.Ingredients)
                .WithMany(i => i.Recipes)
                .UsingEntity(j => j.ToTable("RecipeIngredient"));

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Recipe)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.RecipeId);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Recipe)
                .WithMany(recipe => recipe.Ratings)
                .HasForeignKey(r => r.RecipeId);

            modelBuilder.Entity<SharedRecipe>()
                .HasOne(sr => sr.Recipe)
                .WithMany()
                .HasForeignKey(sr => sr.RecipeId);

        }





    }
}
