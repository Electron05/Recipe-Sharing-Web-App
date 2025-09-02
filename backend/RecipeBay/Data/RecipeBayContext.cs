using Microsoft.EntityFrameworkCore;
using RecipeBay.Models;

//https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-9.0&tabs=visual-studio-code

namespace RecipeBay.Data
{
	public class RecipeBayContext : DbContext
	{
		public RecipeBayContext(DbContextOptions<RecipeBayContext> options)
			: base(options)
		{
		}

		public DbSet<Recipe> Recipes { get; set; }

		public DbSet<User> Users { get; set; }

		public DbSet<Comment> Comments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Recipe → User (Author)
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.Author)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Comment → User (Author of comment)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Comment → Recipe
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Recipe)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

			

		}

	}
}
