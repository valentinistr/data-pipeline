namespace Database.Seeding;

public interface IDatabaseSeed
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
