namespace BookRentalManager.IntegrationTests.Api.Controllers.V1;

public sealed class BookControllerTests(IntegrationTestsWebApplicationFactory integrationTestsWebApplicationFactory)
    : IntegrationTest(integrationTestsWebApplicationFactory)
{
    private static readonly List<GetBookDto> s_expectedBooks =
    [
        new(
            It.IsAny<Guid>(),
            "Design Patterns: Elements of Reusable Object-Oriented Software",
            [
                new(FullName.Create("Erich", "Gamma").Value!),
                new(FullName.Create("Ralph", "Johnson").Value!),
                new(FullName.Create("Richard", "Helm").Value!),
                new(FullName.Create("John", "Vlissides").Value!)
            ],
            1,
            "0-201-63361-2",
            null,
            null,
            null),
        new(
            It.IsAny<Guid>(),
            "Clean Code: A Handbook of Agile Software Craftsmanship",
            [
                new(FullName.Create("Bob", "Martin").Value!)
            ],
            1,
            "978-0132350884",
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>(),
            new("Rosanne Johnson", "rosane.johnson@email.com")),
        new(
            It.IsAny<Guid>(),
            "The Call Of Cthulhu",
            [
                new(FullName.Create("Howard", "Lovecraft").Value!)
            ],
            1,
            "978-1515424437",
            null,
            null,
            null),
        new(
            It.IsAny<Guid>(),
            "The Shadow Over Innsmouth",
            [
                new(FullName.Create("Howard", "Lovecraft").Value!)
            ],
            1,
            "978-1878252180",
            null,
            null,
            null)
    ];

    public static TheoryData<string, List<GetBookDto>, IEnumerable<string>> GetBooksByQueryParametersAsyncTestData()
    {
        return new()
        {
            {
                "sortBy=IsbnDesc",
                s_expectedBooks.OrderByDescending(book => book.Isbn).ToList(),
                new List<string> { "{\"totalAmountOfItems\":4,\"pageIndex\":1,\"pageSize\":50,\"totalAmountOfPages\":1}" }
            },
            {
                "sortBy=EditionDesc,Isbn&pageSize=1&pageIndex=2",
                s_expectedBooks
                    .Skip(1)
                    .Take(1)
                    .OrderByDescending(book => book.Edition)
                    .ThenBy(book => book.Isbn)
                    .ToList(),
                new List<string> { "{\"totalAmountOfItems\":4,\"pageIndex\":2,\"pageSize\":1,\"totalAmountOfPages\":4}" }
            },
        };
    }

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

    [Theory]
    [MemberData(nameof(GetBooksByQueryParametersAsyncTestData))]
    public async Task GetBooksByQueryParametersAsync_WithMediaTypeNotVendorSpecific_Returns200WithObjectAndXPaginationHeaders(
        string queryParameters,
        List<GetBookDto> expectedBooks,
        IEnumerable<string> expectedXPaginationHeaders)
    {
        // Arrange:
        HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{BookBaseUri}?{queryParameters}");

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        IEnumerable<string> actualXPaginationHeaders = httpResponseMessage.Headers.GetValues("x-pagination");
        List<GetBookDto>? actualBooks = await httpResponseMessage.Content.ReadFromJsonAsync<List<GetBookDto>>();
        Assert.Equal(expectedXPaginationHeaders, actualXPaginationHeaders);
        for (int bookIndex = 0; bookIndex < expectedBooks.Count; bookIndex++)
        {
            Assert.Equal(expectedBooks[bookIndex].BookTitle, actualBooks!.ElementAt(bookIndex).BookTitle);
            Assert.Equal(expectedBooks[bookIndex].Authors.Count, actualBooks!.ElementAt(bookIndex).Authors.Count);
            Assert.Equal(expectedBooks[bookIndex].Edition, actualBooks!.ElementAt(bookIndex).Edition);
            Assert.Equal(expectedBooks[bookIndex].Isbn, actualBooks!.ElementAt(bookIndex).Isbn);
            Assert.Equal(expectedBooks[bookIndex].RentedBy, actualBooks!.ElementAt(bookIndex).RentedBy);
        }
    }
}
