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

        public DbSet<Ingredient> Ingredients { get; set; }

        public DbSet<IngredientEntry> IngredientEntries { get; set; }

        public DbSet<IngredientAlias> IngredientAliases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Recipe -> User (Author)
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.Author)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Comment -> User (Author of comment)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Comment -> Recipe
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Recipe)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Recipe -> RecipeIngredientEntry -> Ingredient -> IngredientAlias
            modelBuilder.Entity<IngredientEntry>()
                .HasOne(rie => rie.Recipe)
                .WithMany(r => r.IngredientEntries)
                .HasForeignKey(rie => rie.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IngredientEntry>()
                .HasOne(rie => rie.Ingredient)
                .WithMany(i => i.RecipeEntries)
                .HasForeignKey(rie => rie.IngredientId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IngredientEntry>()
                .HasOne(rie => rie.IngredientAlias)
                .WithMany()
                .HasForeignKey(rie => rie.IngredientAliasId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IngredientAlias>()
                .HasOne(iap => iap.Ingredient)
                .WithMany(i => i.Aliases)
                .HasForeignKey(iap => iap.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ingredient part relationship
            modelBuilder.Entity<Ingredient>()
                .HasOne(i => i.IsPartOf)
                .WithMany()
                .HasForeignKey(i => i.ParentIngredientId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if it has parts
        }

	}
}
