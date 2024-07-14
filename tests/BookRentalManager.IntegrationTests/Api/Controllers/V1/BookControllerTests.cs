namespace BookRentalManager.IntegrationTests.Api.Controllers.V1;

public sealed class BookControllerTests(IntegrationTestsWebApplicationFactory integrationTestsWebApplicationFactory)
    : IntegrationTest(integrationTestsWebApplicationFactory)
{
    [Fact]
    public async Task GetBooksByQueryParametersAsync_WithMediaTypeVendorSpecific_Returns200OkWithHateoasLinks()
    {
        // Arrange:
        HttpClient.DefaultRequestHeaders.Add("Accept", CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(BookBaseUri);

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        BooksWithHateoasLinks booksWithHateoasLinks = JsonSerializer.Deserialize<BooksWithHateoasLinks>(responseContent, jsonSerializerOptions)!;
        Assert.Contains(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            httpResponseMessage.Content.Headers.ContentType!.ToString());
        Assert.NotEqual(Guid.Empty, booksWithHateoasLinks.Values[0].Id);
        Assert.Equal("Design Patterns: Elements of Reusable Object-Oriented Software", booksWithHateoasLinks.Values[0].BookTitle);
        Assert.NotEmpty(booksWithHateoasLinks.Values[0].Authors);
        Assert.Equal(1, booksWithHateoasLinks.Values[0].Edition);
        Assert.Equal("0-201-63361-2", booksWithHateoasLinks.Values[0].Isbn);
        Assert.Null(booksWithHateoasLinks.Values[0].RentedAt);
        Assert.Null(booksWithHateoasLinks.Values[0].DueDate);
        Assert.Null(booksWithHateoasLinks.Values[0].RentedBy);
        Assert.NotEmpty(booksWithHateoasLinks.Values[0].Links);
        Assert.Empty(booksWithHateoasLinks.Links);
    }
}
