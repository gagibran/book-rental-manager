using BookRentalManager.Api.Constants;

namespace BookRentalManager.IntegrationTests.Api.Controllers.V1;

public sealed class AuthorControllerTests(IntegrationTestsWebApplicationFactory integrationTestsWebbApplicationFactory) : IntegrationTest(integrationTestsWebbApplicationFactory)
{
    [Fact]
    public async Task CreateAuthorAsync_WithMediaTypeVendorSpecific_Returns200WithHateoasLinks()
    {
        // Arrange:
        HttpClient.DefaultRequestHeaders.Add("Accept", CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync("api/v1/author");

        // Assert:
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        httpResponseMessage.EnsureSuccessStatusCode();
        Assert.Contains(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            httpResponseMessage.Content.Headers.ContentType!.ToString());
    }
}
