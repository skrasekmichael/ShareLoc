using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShareLoc.Client.DAL;

internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
		optionsBuilder.UseSqlite("Data Source=sqlite.3db");

		return new ApplicationDbContext(optionsBuilder.Options)
		{
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
			Places = null,
			Guesses = null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
		};
	}
}
