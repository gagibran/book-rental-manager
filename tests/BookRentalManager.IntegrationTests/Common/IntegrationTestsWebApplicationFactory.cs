using BookRentalManager.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace BookRentalManager.IntegrationTests.Common;

public sealed class IntegrationTestsWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;

    public IntegrationTestsWebApplicationFactory()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("BookRentalManager")
            .WithUsername("admin")
            .WithPassword("admin")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder webHostBuilder)
    {
        webHostBuilder.ConfigureServices(webHostBuilderContext =>
        {
            ServiceDescriptor? serviceDescriptor = webHostBuilderContext.SingleOrDefault(
                serviceDescriptor => serviceDescriptor.ServiceType == typeof(DbContextOptions<BookRentalManagerDbContext>));
            if (serviceDescriptor is not null)
            {
                webHostBuilderContext.Remove(serviceDescriptor);
            }
            webHostBuilderContext.AddDbContext<BookRentalManagerDbContext>(dbContextOptionsBuilder =>
            {
                dbContextOptionsBuilder.UseNpgsql(_postgreSqlContainer.GetConnectionString());
            });
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
    }
}
