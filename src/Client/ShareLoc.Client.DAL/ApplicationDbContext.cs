using Microsoft.EntityFrameworkCore;

using ShareLoc.Client.DAL.Entities;

namespace ShareLoc.Client.DAL;

public sealed class ApplicationDbContext : DbContext
{
	public required DbSet<PlaceEntity> Places { get; init; }
	public required DbSet<GuessEntity> Guesses { get; init; }

	public ApplicationDbContext(DbContextOptions options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

		base.OnModelCreating(modelBuilder);
	}
}
