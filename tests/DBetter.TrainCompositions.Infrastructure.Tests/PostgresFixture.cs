using Testcontainers.PostgreSql;

namespace DBetter.TrainCompositions.Infrastructure.Tests;

public class PostgresFixture: IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:16-alpine")
        .Build();
    
    public string ConnectionString => _container.GetConnectionString();
    
    public async Task InitializeAsync() => await _container.StartAsync();
    public async Task DisposeAsync() => await _container.DisposeAsync();
}