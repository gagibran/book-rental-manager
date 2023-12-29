namespace BookRentalManager.IntegrationTests.Api.Controllers.V1;

public class AuthorControllerTests(IntegrationTestsWebApplicationFactory integrationTestsWebbApplicationFactory) : IntegrationTest(integrationTestsWebbApplicationFactory)
{
    [Fact]
    public async Task CreateAuthorAsync_WithMediaTypeVendorSpecific_Returns200WithHateoasLinks()
    {
        // Act
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync("api/v1/author");

        // Assert
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
    }
}
