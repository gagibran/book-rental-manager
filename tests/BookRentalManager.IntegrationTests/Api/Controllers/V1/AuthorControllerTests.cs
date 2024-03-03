using BookRentalManager.Api.Constants;

namespace BookRentalManager.IntegrationTests.Api.Controllers.V1;

public sealed class AuthorControllerTests(IntegrationTestsWebApplicationFactory integrationTestsWebbApplicationFactory) : IntegrationTest(integrationTestsWebbApplicationFactory)
{
    [Fact]
    public async Task GetAuthorsByQueryParametersAsync_WithMediaTypeVendorSpecific_Returns200WithHateoasLinks()
    {
        // Arrange:
        HttpClient.DefaultRequestHeaders.Add("Accept", CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync("api/v1/author");

        // Assert:
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        (List<object> values, List<object> links) = GetValuesAndLinksFromHateoasResponse(responseContent);
        httpResponseMessage.EnsureSuccessStatusCode();
        Assert.Contains(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            httpResponseMessage.Content.Headers.ContentType!.ToString());
        Assert.NotEqual(string.Empty, ((dynamic)values.ElementAt(0)).id);
        Assert.Equal("Erich Gamma", ((dynamic)values.ElementAt(0)).fullName);
        Assert.NotEmpty(((dynamic)values.ElementAt(0)).books);
        Assert.NotEmpty(((dynamic)values.ElementAt(0)).links);
        Assert.Empty(links);
    }
}
