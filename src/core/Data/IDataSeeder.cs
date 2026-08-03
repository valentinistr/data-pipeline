namespace Server.Data;

public interface IDatabaseSeed
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
