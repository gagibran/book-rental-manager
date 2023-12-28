namespace BookRentalManager.IntegrationTests;

public abstract class IntegrationTest : IClassFixture<IntegrationTestsWebbApplicationFactory>
{
    protected HttpClient HttpClient { get; }

    protected IntegrationTest(IntegrationTestsWebbApplicationFactory integrationTestsWebbApplicationFactory)
    {
        using IServiceScope serviceScope = integrationTestsWebbApplicationFactory.Services.CreateScope();
        IServiceProvider serviceProvider = serviceScope.ServiceProvider;
        BookRentalManagerDbContext bookRentalManagerDbContext = serviceProvider.GetRequiredService<BookRentalManagerDbContext>();
        bookRentalManagerDbContext.Database.Migrate();
        HttpClient = integrationTestsWebbApplicationFactory.CreateClient();
    }
}
