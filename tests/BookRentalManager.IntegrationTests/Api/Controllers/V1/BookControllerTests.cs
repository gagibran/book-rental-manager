namespace BookRentalManager.IntegrationTests.Api.Controllers.V1;

public sealed class BookControllerTests(IntegrationTestsWebApplicationFactory integrationTestsWebApplicationFactory)
    : IntegrationTest(integrationTestsWebApplicationFactory)
{
    private static readonly List<GetBookDto> s_expectedBooks =
    [
        new(
            It.IsAny<Guid>(),
            "Clean Code: A Handbook of Agile Software Craftsmanship",
            [
                new("Bob Martin")
            ],
            1,
            "978-0132350884",
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>(),
            new("Rosanne Johnson", "rosane.johnson@email.com")),
        new(
            It.IsAny<Guid>(),
            "Design Patterns: Elements of Reusable Object-Oriented Software",
            [
                new("Erich Gamma"),
                new("John Vlissides"),
                new("Ralph Johnson"),
                new("Richard Helm")
            ],
            1,
            "0-201-63361-2",
            null,
            null,
            null),
        new(
            It.IsAny<Guid>(),
            "The Call Of Cthulhu",
            [
                new("Howard Lovecraft")
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
                new("Howard Lovecraft")
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
                    .OrderByDescending(book => book.Edition)
                    .ThenBy(book => book.Isbn)
                    .Skip(1)
                    .Take(1)
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
            Assert.True(expectedBooks[bookIndex].Authors.SequenceEqual(
                actualBooks!.ElementAt(bookIndex).Authors.OrderBy(author => author.FullName)));
            Assert.Equal(expectedBooks[bookIndex].Edition, actualBooks!.ElementAt(bookIndex).Edition);
            Assert.Equal(expectedBooks[bookIndex].Isbn, actualBooks!.ElementAt(bookIndex).Isbn);
            Assert.Equal(expectedBooks[bookIndex].RentedBy, actualBooks!.ElementAt(bookIndex).RentedBy);
        }
    }

    [Fact]
    public async Task GetBooksByQueryParametersAsync_WithIncorrectParameterType_Returns400WithErrors()
    {
        // Arrange:
        var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Errors = new Dictionary<string, string[]>
            {
                { "PageSize", ["The value 'notANumber' is not valid for PageSize."]}
            },
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "One or more validation errors occurred.",
            Status = 400
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{BookBaseUri}?pageSize=notANumber");

        // Assert:
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails!.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails!.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails!.Status);
    }

    [Fact]
    public async Task GetBooksByQueryParametersAsync_WithNonexistingQueryParameter_Returns422WithError()
    {
        // Arrange:
        var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Errors = new Dictionary<string, string[]>
            {
                { "invalidProperty", ["The property 'notAValidParameter' does not exist for 'GetBookDto'."]}
            },
            Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
            Title = "One or more validation errors occurred.",
            Status = 422
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{BookBaseUri}?sortBy=notAValidParameter");

        // Assert:
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails!.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails!.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails!.Status);
    }

    [Theory]
    [InlineData("", "{\"totalAmountOfItems\":4,\"pageIndex\":1,\"pageSize\":50,\"totalAmountOfPages\":1}")]
    [InlineData("?sortBy=BookTitle", "{\"totalAmountOfItems\":4,\"pageIndex\":1,\"pageSize\":50,\"totalAmountOfPages\":1}")]
    [InlineData("?sortBy=BookTitle&pageSize=1&pageIndex=2", "{\"totalAmountOfItems\":4,\"pageIndex\":2,\"pageSize\":1,\"totalAmountOfPages\":4}")]
    public async Task GetBooksByQueryParametersAsync_WithHead_Returns200WithXPaginationHeaders(
        string queryParameters,
        string expectedReturnedXPaginationHeaders)
    {
        // Arrange:
        var expectedXPaginationHeaders = new List<string> { expectedReturnedXPaginationHeaders };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Head,
            $"{BookBaseUri}{queryParameters}"));

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        IEnumerable<string> actualXPaginationHeaders = httpResponseMessage.Headers.GetValues("x-pagination");
        Assert.Equal(expectedXPaginationHeaders, actualXPaginationHeaders);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetBookByIdAsync_WithMediaTypeVendorSpecific_Returns200WithHateoasLinks(int currentBookIndex)
    {
        // Arrange:
        Guid expectedId = await GetIdOrderedByConditionAsync<GetBookDto>(
            currentBookIndex,
            BookBaseUri,
            getAuthorDto => getAuthorDto.BookTitle);
        HttpClient.DefaultRequestHeaders.Add("Accept", CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{BookBaseUri}/{expectedId}");

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        BookWithHateoasLinks bookWithLinks = JsonSerializer.Deserialize<BookWithHateoasLinks>(responseContent, jsonSerializerOptions)!;
        Assert.Contains(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            httpResponseMessage.Content.Headers.ContentType!.ToString());
        Assert.Equal(s_expectedBooks[currentBookIndex].BookTitle, bookWithLinks.BookTitle);
        Assert.True(s_expectedBooks[currentBookIndex].Authors.SequenceEqual(
            s_expectedBooks[currentBookIndex].Authors.OrderBy(author => author.FullName)));
        Assert.Equal(s_expectedBooks[currentBookIndex].Edition, bookWithLinks.Edition);
        Assert.Equal(s_expectedBooks[currentBookIndex].Isbn, bookWithLinks.Isbn);
        Assert.Equal(s_expectedBooks[currentBookIndex].RentedBy, bookWithLinks.RentedBy);
        Assert.Equal("self", bookWithLinks.Links[0].Rel);
        Assert.Equal("patch_book", bookWithLinks.Links[1].Rel);
        Assert.Equal("delete_book", bookWithLinks.Links[2].Rel);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetBookByIdAsync_WithMediaTypeNotVendorSpecific_Returns200WithObject(int currentBookIndex)
    {
        // Arrange:
        Guid expectedId = await GetIdOrderedByConditionAsync<GetBookDto>(
            currentBookIndex,
            BookBaseUri,
            getAuthorDto => getAuthorDto.BookTitle);
        HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{BookBaseUri}/{expectedId}");

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        GetBookDto? actualBook = await httpResponseMessage.Content.ReadFromJsonAsync<GetBookDto>();
        Assert.Equal(s_expectedBooks[currentBookIndex].BookTitle, actualBook!.BookTitle);
        Assert.True(s_expectedBooks[currentBookIndex].Authors.SequenceEqual(actualBook.Authors.OrderBy(
            author => author.FullName)));
        Assert.Equal(s_expectedBooks[currentBookIndex].Edition, actualBook.Edition);
        Assert.Equal(s_expectedBooks[currentBookIndex].Isbn, actualBook.Isbn);
        Assert.Equal(s_expectedBooks[currentBookIndex].RentedBy, actualBook.RentedBy);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetBookByIdAsync_WithHeadAndMediaTypeVendorSpecific_Returns200WithContentTypeHeaders(int currentBookIndex)
    {
        // Arrange:
        Guid id = await GetIdOrderedByConditionAsync<GetBookDto>(
            currentBookIndex,
            BookBaseUri,
            getAuthorDto => getAuthorDto.BookTitle);
        HttpClient.DefaultRequestHeaders.Add("Accept", CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson);
        var expectedContentTypeHeaders = new List<string>
        {
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson + "; charset=utf-8"
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Head,
            $"{BookBaseUri}/{id}"));

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        IEnumerable<string> actualContentTypeHeaders = httpResponseMessage.Content.Headers.GetValues("content-type");
        Assert.Equal(expectedContentTypeHeaders, actualContentTypeHeaders);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetBookByIdAsync_WithHeadAndMediaTypeNotVendorSpecific_Returns200WithContentTypeHeaders(int currentBookIndex)
    {
        // Arrange:
        Guid id = await GetIdOrderedByConditionAsync<GetBookDto>(
            currentBookIndex,
            BookBaseUri,
            getAuthorDto => getAuthorDto.BookTitle);
        HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
        var expectedContentTypeHeaders = new List<string>
        {
            MediaTypeNames.Application.Json + "; charset=utf-8"
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Head,
            $"{BookBaseUri}/{id}"));

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        IEnumerable<string> actualContentTypeHeaders = httpResponseMessage.Content.Headers.GetValues("content-type");
        Assert.Equal(expectedContentTypeHeaders, actualContentTypeHeaders);
    }

    [Fact]
    public async Task CreateBookAsync_WithMediaTypeVendorSpecific_Returns201WithResponseBody()
    {
        // Arrange:
        const string ExpectedBookTitle = "A Cool Book";
        const int ExpectedEdition = 1;
        const string ExpectedIsbn = "0-201-63361-1";
        AuthorCreatedDto expectedAuthor = await CreateAsync<AuthorCreatedDto>(
            $"{{\"firstName\": \"John\", \"lastName\": \"Doe\"}}",
            MediaTypeNames.Application.Json,
            AuthorBaseUri);
        var expectedHateoasLinks = new List<HateoasLinkDto>
        {
            new(It.IsAny<string>(), "self", "GET"),
            new(It.IsAny<string>(), "patch_book", "PATCH"),
            new(It.IsAny<string>(), "delete_book", "DELETE"),
        };
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, BookBaseUri)
        {
            Content = new StringContent(
                $"{{\"authorIds\": [\"{expectedAuthor.Id}\"],\"bookTitle\": \"{ExpectedBookTitle}\",\"edition\": {ExpectedEdition},\"isbn\": \"{ExpectedIsbn}\"}}",
                Encoding.UTF8,
                CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson)
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        Guid actualCreatedBookId = JsonSerializer.Deserialize<BookCreatedDto>(responseContent, jsonSerializerOptions)!.Id;
        var actualBookWithLinks = await GetAsync<BookWithHateoasLinks>(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            $"{BookBaseUri}/{actualCreatedBookId}");
        Assert.Equal(ExpectedBookTitle, actualBookWithLinks.BookTitle);
        Assert.Equal(ExpectedEdition, actualBookWithLinks.Edition);
        Assert.Equal(ExpectedIsbn, actualBookWithLinks.Isbn);
        Assert.Equal(actualCreatedBookId, actualBookWithLinks.Id);
        Assert.Equal(expectedAuthor.FirstName + " " + expectedAuthor.LastName, actualBookWithLinks.Authors[0].FullName);
        Assert.Equal(expectedHateoasLinks[0].Rel, actualBookWithLinks.Links[0].Rel);
        Assert.Equal(expectedHateoasLinks[1].Rel, actualBookWithLinks.Links[1].Rel);
        Assert.Equal(expectedHateoasLinks[2].Rel, actualBookWithLinks.Links[2].Rel);
        Assert.Equal(expectedHateoasLinks[0].Method, actualBookWithLinks.Links[0].Method);
        Assert.Equal(expectedHateoasLinks[1].Method, actualBookWithLinks.Links[1].Method);
        Assert.Equal(expectedHateoasLinks[2].Method, actualBookWithLinks.Links[2].Method);

        // Clean up:
        await HttpClient.DeleteAsync($"{BookBaseUri}/{actualCreatedBookId}");
        await HttpClient.DeleteAsync($"{AuthorBaseUri}/{expectedAuthor.Id}");
    }

    [Fact]
    public async Task CreateBookAsync_WithMediaTypeNotVendorSpecific_Returns201WithResponseBody()
    {
        // Arrange:
        const string ExpectedBookTitle = "A Cool Book";
        const int ExpectedEdition = 1;
        const string ExpectedIsbn = "0-201-63361-1";
        AuthorCreatedDto expectedAuthor = await CreateAsync<AuthorCreatedDto>(
            $"{{\"firstName\": \"John\", \"lastName\": \"Doe\"}}",
            MediaTypeNames.Application.Json,
            AuthorBaseUri);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, BookBaseUri)
        {
            Content = new StringContent(
                $"{{\"authorIds\": [\"{expectedAuthor.Id}\"],\"bookTitle\": \"{ExpectedBookTitle}\",\"edition\": {ExpectedEdition},\"isbn\": \"{ExpectedIsbn}\"}}",
                Encoding.UTF8,
                MediaTypeNames.Application.Json)
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        Guid actualCreatedBookId = JsonSerializer.Deserialize<BookCreatedDto>(responseContent, jsonSerializerOptions)!.Id;
        GetBookDto actualBook = await GetAsync<GetBookDto>(
            MediaTypeNames.Application.Json,
            $"{BookBaseUri}/{actualCreatedBookId}");
        Assert.Equal(ExpectedBookTitle, actualBook.BookTitle);
        Assert.Equal(ExpectedEdition, actualBook.Edition);
        Assert.Equal(ExpectedIsbn, actualBook.Isbn);
        Assert.Equal(actualCreatedBookId, actualBook.Id);
        Assert.Equal(expectedAuthor.FirstName + " " + expectedAuthor.LastName, actualBook.Authors[0].FullName);

        // Clean up:
        await HttpClient.DeleteAsync($"{BookBaseUri}/{actualCreatedBookId}");
        await HttpClient.DeleteAsync($"{AuthorBaseUri}/{expectedAuthor.Id}");
    }

    [Theory]
    [InlineData(null, 1, "0-201-63361-1", "The title can't be empty.")]
    [InlineData("A Cool Book", 0, "0-201-63361-1", "The edition number can't be smaller than 1.")]
    [InlineData("A Cool Book", 1, null, "Invalid ISBN format.")]
    [InlineData(null, 1, null, "bookTitle\":[\"The title can't be empty.\"],\"isbnFormat\":[\"Invalid ISBN format.\"]")]
    public async Task CreateBookAsync_WithInvalidParameters_Returns422WithErrorMessage(
        string? bookTitle,
        int? edition,
        string? isbn,
        string expectedErrorMessage)
    {
        // Arrange:
        Guid authorId = (await CreateAsync<AuthorCreatedDto>(
            $"{{\"firstName\": \"John\", \"lastName\": \"Doe\"}}",
            MediaTypeNames.Application.Json,
            AuthorBaseUri)).Id;
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, BookBaseUri)
        {
            Content = new StringContent(
                $"{{\"authorIds\": [\"{authorId}\"],\"bookTitle\": \"{bookTitle}\",\"edition\": {edition},\"isbn\": \"{isbn}\"}}",
                Encoding.UTF8,
                MediaTypeNames.Application.Json)
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.UnprocessableEntity, httpResponseMessage.StatusCode);
        Assert.Contains(expectedErrorMessage, responseContent);

        // Clean up:
        await HttpClient.DeleteAsync($"{AuthorBaseUri}/{authorId}");
    }

    [Fact]
    public async Task PatchBookTitleEditionAndIsbnByIdAsync_WithNonexistingBook_Returns404WithErrorMessage()
    {
        // Arrange:
        Guid nonexistingBookId = Guid.NewGuid();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, $"{BookBaseUri}/{nonexistingBookId}")
        {
            Content = new StringContent(
                $"[{{\"op\": \"replace\", \"path\": \"/booktitle\", \"value\": \"A Cool New Title\"}}, {{\"op\": \"replace\", \"path\": \"/edition\", \"value\": \"3\"}}, {{\"op\": \"replace\", \"path\": \"/isbn\", \"value\": \"978-0132350884\"}}]",
                Encoding.UTF8,
                MediaTypeNames.Application.JsonPatch)
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        string errorMessage = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Contains($"No book with the ID of '{nonexistingBookId}' was found.", errorMessage);
    }

    [Theory]
    [InlineData("  ", 1, "0-301-64361-2", "The title can't be empty.")]
    [InlineData("A Cool New Title", -1, "0-301-64361-2", "The edition number can't be smaller than 1.")]
    [InlineData("A Cool New Title", 1, "0-301-6436", "Invalid ISBN format.")]
    [InlineData("  ", 1, "24", "{\"bookTitle\":[\"The title can't be empty.\"],\"isbnFormat\":[\"Invalid ISBN format.\"]}")]
    public async Task PatchBookTitleEditionAndIsbnByIdAsync_WithValidationErrors_Returns422WithErrorMessage(
        string bookTitle,
        int edition,
        string isbn,
        string expectedErrorMessage)
    {
        // Arrange:
        Guid authorId = (await CreateAsync<AuthorCreatedDto>(
            "{\"firstName\": \"John\", \"lastName\": \"Doe\"}",
            MediaTypeNames.Application.Json,
            AuthorBaseUri)).Id;
        BookCreatedDto book = await CreateAsync<BookCreatedDto>(
            $"{{\"authorIds\": [\"{authorId}\"], \"bookTitle\": \" A Cool Title\", \"edition\": 1, \"isbn\": \"0-301-64361-2\"}}",
            MediaTypeNames.Application.Json,
            BookBaseUri);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, $"{BookBaseUri}/{book.Id}")
        {
            Content = new StringContent(
                $"[{{\"op\": \"replace\", \"path\": \"/booktitle\", \"value\": \"{bookTitle}\"}}, {{\"op\": \"replace\", \"path\": \"/edition\", \"value\": \"{edition}\"}}, {{\"op\": \"replace\", \"path\": \"/isbn\", \"value\": \"{isbn}\"}}]",
                Encoding.UTF8,
                MediaTypeNames.Application.JsonPatch)
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, httpResponseMessage.StatusCode);
        string errorMessage = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Contains(expectedErrorMessage, errorMessage);

        // Clean up:
        await HttpClient.DeleteAsync($"{BookBaseUri}/{book.Id}");
        await HttpClient.DeleteAsync($"{AuthorBaseUri}/{authorId}");
    }

    [Fact]
    public async Task PatchBookTitleEditionAndIsbnByIdAsync_WithValidParameters_Returns204AndUpdatesBook()
    {
        // Arrange:
        const string ExpectedNewTitle = "A Cool New Title";
        const int ExpectedNewEdition = 2;
        const string ExpectedNewIsbn = "0-341-61361-2";
        Guid authorId = (await CreateAsync<AuthorCreatedDto>(
            "{\"firstName\": \"John\", \"lastName\": \"Doe\"}",
            MediaTypeNames.Application.Json,
            AuthorBaseUri)).Id;
        BookCreatedDto book = await CreateAsync<BookCreatedDto>(
            $"{{\"authorIds\": [\"{authorId}\"], \"bookTitle\": \" A Cool Title\", \"edition\": 1, \"isbn\": \"0-301-64361-2\"}}",
            MediaTypeNames.Application.Json,
            BookBaseUri);
        var uriWithBookThatWillBeUpdatedId = $"{BookBaseUri}/{book.Id}";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uriWithBookThatWillBeUpdatedId)
        {
            Content = new StringContent(
                $"[{{\"op\": \"replace\", \"path\": \"/booktitle\", \"value\": \"{ExpectedNewTitle}\"}}, {{\"op\": \"replace\", \"path\": \"/edition\", \"value\": \"{ExpectedNewEdition}\"}}, {{\"op\": \"replace\", \"path\": \"/isbn\", \"value\": \"{ExpectedNewIsbn}\"}}]",
                Encoding.UTF8,
                MediaTypeNames.Application.JsonPatch)
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        GetBookDto updatedBook = await GetAsync<GetBookDto>(
            MediaTypeNames.Application.Json,
            uriWithBookThatWillBeUpdatedId);
        Assert.Equal(ExpectedNewTitle, updatedBook.BookTitle);
        Assert.Equal(ExpectedNewEdition, updatedBook.Edition);
        Assert.Equal(ExpectedNewIsbn, updatedBook.Isbn);

        // Clean up:
        await HttpClient.DeleteAsync($"{BookBaseUri}/{book.Id}");
        await HttpClient.DeleteAsync($"{AuthorBaseUri}/{authorId}");
    }
}
