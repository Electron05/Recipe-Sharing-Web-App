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

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.Author)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);
        }

	}
}
