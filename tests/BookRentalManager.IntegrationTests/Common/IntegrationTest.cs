namespace BookRentalManager.IntegrationTests;

public abstract class IntegrationTest : IClassFixture<IntegrationTestsWebApplicationFactory>
{
    protected HttpClient HttpClient { get; }

    protected IntegrationTest(IntegrationTestsWebApplicationFactory integrationTestsWebbApplicationFactory)
    {
        using IServiceScope serviceScope = integrationTestsWebbApplicationFactory.Services.CreateScope();
        IServiceProvider serviceProvider = serviceScope.ServiceProvider;
        BookRentalManagerDbContext bookRentalManagerDbContext = serviceProvider.GetRequiredService<BookRentalManagerDbContext>();
        bookRentalManagerDbContext.Database.Migrate();
        HttpClient = integrationTestsWebbApplicationFactory.CreateClient();
    }
}
