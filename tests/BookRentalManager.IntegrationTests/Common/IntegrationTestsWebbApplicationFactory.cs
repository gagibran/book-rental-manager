using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace BookRentalManager.IntegrationTests.Common;

public class IntegrationTestsWebbApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;

    public IntegrationTestsWebbApplicationFactory()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithName("book_rental_manager_integration_tests")
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
