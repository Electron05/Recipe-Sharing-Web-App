using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RecipeBay.Data;


// Design-time DbContext factory for EF Core migrations
// It enables the creation of DbContext instances at design time without running the application
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RecipeBayContext>
{
	public RecipeBayContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<RecipeBayContext>();
		optionsBuilder.UseNpgsql("Host=localhost;Database=recipebay;Username=postgres;Password=yourpassword");
		return new RecipeBayContext(optionsBuilder.Options);
	}
}
