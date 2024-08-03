namespace BookRentalManager.IntegrationTests.Api.Controllers.V1;

public sealed class AuthorControllerTests(IntegrationTestsWebApplicationFactory integrationTestsWebApplicationFactory)
    : IntegrationTest(integrationTestsWebApplicationFactory)
{
    private static readonly List<GetAuthorDto> s_expectedAuthors =
    [
        new(
            It.IsAny<Guid>(),
            new(FullName.Create("Bob", "Martin").Value!.ToString()),
            [
                new("Clean Code: A Handbook of Agile Software Craftsmanship", 1, "978-0132350884")
            ]),
        new(
            It.IsAny<Guid>(),
            new(FullName.Create("Edgar Allan", "Poe").Value!.ToString()),
            []),
        new(
            It.IsAny<Guid>(),
            new(FullName.Create("Erich", "Gamma").Value!.ToString()),
            [
                new("Design Patterns: Elements of Reusable Object-Oriented Software", 1, "0-201-63361-2")
            ]),
        new(
            It.IsAny<Guid>(),
            new(FullName.Create("Howard", "Lovecraft").Value!.ToString()),
            [
                new("The Call Of Cthulhu", 1, "978-1515424437"),
                new("The Shadow Over Innsmouth", 1, "978-1878252180")
            ]),
        new(
            It.IsAny<Guid>(),
            new(FullName.Create("John", "Vlissides").Value!.ToString()),
            [
                new("Design Patterns: Elements of Reusable Object-Oriented Software", 1, "0-201-63361-2")
            ]),
        new(
            It.IsAny<Guid>(),
            new(FullName.Create("Ralph", "Johnson").Value!.ToString()),
            [
                new("Design Patterns: Elements of Reusable Object-Oriented Software", 1, "0-201-63361-2")
            ]),
        new(
            It.IsAny<Guid>(),
            new(FullName.Create("Richard", "Helm").Value!.ToString()),
            [
                new("Design Patterns: Elements of Reusable Object-Oriented Software", 1, "0-201-63361-2")
            ]),
    ];

    public static TheoryData<string, List<GetAuthorDto>, IEnumerable<string>> GetAuthorsByQueryParametersAsyncTestData()
    {
        return new()
        {
            {
                "sortBy=FullName",
                s_expectedAuthors.OrderBy(author => author.FullName).ToList(),
                new List<string> { "{\"totalAmountOfItems\":7,\"pageIndex\":1,\"pageSize\":50,\"totalAmountOfPages\":1}" }
            },
            {
                "sortBy=FullName&pageSize=1&pageIndex=2",
                s_expectedAuthors.OrderBy(author => author.FullName).Skip(1).Take(1).ToList(),
                new List<string> { "{\"totalAmountOfItems\":7,\"pageIndex\":2,\"pageSize\":1,\"totalAmountOfPages\":7}" }
            },
        };
    }

    public static TheoryData<string?, string, ValidationProblemDetails> CreateAuthorAsyncWithInvalidParametersTestData()
    {
        return new()
        {
            {
                "John",
                "",
                new()
                {
                    Errors = new Dictionary<string, string[]>
                    {
                        { "lastName", ["Last name cannot be empty."]}
                    },
                    Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                    Title = "One or more validation errors occurred.",
                    Status = 422
                }
            },
            {
                null,
                "Doe",
                new()
                {
                    Errors = new Dictionary<string, string[]>
                    {
                        { "firstName", ["First name cannot be empty."]}
                    },
                    Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                    Title = "One or more validation errors occurred.",
                    Status = 422
                }
            },
            {
                "",
                "",
                new()
                {
                    Errors = new Dictionary<string, string[]>
                    {
                        { "firstName", ["First name cannot be empty."]},
                        { "lastName", ["Last name cannot be empty."]}
                    },
                    Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                    Title = "One or more validation errors occurred.",
                    Status = 422
                }
            }
        };
    }

    [Fact]
    public async Task GetAuthorsByQueryParametersAsync_WithMediaTypeVendorSpecific_Returns200WithHateoasLinks()
    {
        // Arrange:
        HttpClient.DefaultRequestHeaders.Add("Accept", CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(AuthorBaseUri);

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        AuthorsWithHateoasLinks authorsWithHateoasLinks = JsonSerializer.Deserialize<AuthorsWithHateoasLinks>(responseContent, jsonSerializerOptions)!;
        Assert.Contains(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            httpResponseMessage.Content.Headers.ContentType!.ToString());
        Assert.NotEqual(Guid.Empty, authorsWithHateoasLinks.Values[0].Id);
        Assert.Equal("Erich Gamma", authorsWithHateoasLinks.Values[0].FullName);
        Assert.NotEmpty(authorsWithHateoasLinks.Values[0].Books);
        Assert.NotEmpty(authorsWithHateoasLinks.Values[0].Links);
        Assert.Empty(authorsWithHateoasLinks.Links);
    }

    [Theory]
    [MemberData(nameof(GetAuthorsByQueryParametersAsyncTestData))]
    public async Task GetAuthorsByQueryParametersAsync_WithMediaTypeNotVendorSpecific_Returns200WithObjectAndXPaginationHeaders(
        string queryParameters,
        List<GetAuthorDto> expectedAuthors,
        IEnumerable<string> expectedXPaginationHeaders)
    {
        // Arrange:
        HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{AuthorBaseUri}?{queryParameters}");

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        IEnumerable<string> actualXPaginationHeaders = httpResponseMessage.Headers.GetValues("x-pagination");
        List<GetAuthorDto>? actualAuthors = await httpResponseMessage.Content.ReadFromJsonAsync<List<GetAuthorDto>>();
        Assert.Equal(expectedXPaginationHeaders, actualXPaginationHeaders);
        for (int authorIndex = 0; authorIndex < expectedAuthors.Count; authorIndex++)
        {
            Assert.Equal(expectedAuthors[authorIndex].FullName, actualAuthors!.ElementAt(authorIndex).FullName);
            Assert.True(expectedAuthors[authorIndex].Books.SequenceEqual(
                actualAuthors!.ElementAt(authorIndex).Books.OrderBy(book => book.BookTitle)));
        }
    }

    [Fact]
    public async Task GetAuthorsByQueryParametersAsync_WithIncorrectParameterType_Returns400WithErrors()
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
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{AuthorBaseUri}?pageSize=notANumber");

        // Assert:
        Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails.Status);
        Assert.NotNull(actualValidationProblemDetails.Extensions["traceId"]);
    }

    [Fact]
    public async Task GetAuthorsByQueryParametersAsync_WithNonexistingQueryParameter_Returns422WithError()
    {
        // Arrange:
        var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Errors = new Dictionary<string, string[]>
            {
                { "invalidProperty", ["The property 'notAValidParameter' does not exist for 'GetAuthorDto'."]}
            },
            Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
            Title = "One or more validation errors occurred.",
            Status = 422
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{AuthorBaseUri}?sortBy=notAValidParameter");

        // Assert:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, httpResponseMessage.StatusCode);
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails.Status);
        Assert.NotNull(actualValidationProblemDetails.Extensions["traceId"]);
    }

    [Theory]
    [InlineData("", "{\"totalAmountOfItems\":7,\"pageIndex\":1,\"pageSize\":50,\"totalAmountOfPages\":1}")]
    [InlineData("?sortBy=FullName", "{\"totalAmountOfItems\":7,\"pageIndex\":1,\"pageSize\":50,\"totalAmountOfPages\":1}")]
    [InlineData("?sortBy=FullName&pageSize=1&pageIndex=2", "{\"totalAmountOfItems\":7,\"pageIndex\":2,\"pageSize\":1,\"totalAmountOfPages\":7}")]
    public async Task GetAuthorsByQueryParametersAsync_WithHead_Returns200WithXPaginationHeaders(
        string queryParameters,
        string expectedReturnedXPaginationHeaders)
    {
        // Arrange:
        var expectedXPaginationHeaders = new List<string> { expectedReturnedXPaginationHeaders };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Head,
            $"{AuthorBaseUri}{queryParameters}"));

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        IEnumerable<string> actualXPaginationHeaders = httpResponseMessage.Headers.GetValues("x-pagination");
        Assert.Equal(expectedXPaginationHeaders, actualXPaginationHeaders);
    }

    [Fact]
    public async Task GetAuthorById_WithNonexistingAuthor_Returns404WithErrorMessage()
    {
        // Arrange:
        Guid nonexistingAuthorId = Guid.NewGuid();
        var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Errors = new Dictionary<string, string[]>
            {
                { "idNotFound", [$"No author with the ID of '{nonexistingAuthorId}' was found."]}
            },
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            Title = "One or more validation errors occurred.",
            Status = 404
        };
        HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{AuthorBaseUri}/{nonexistingAuthorId}");

        // Assert:
        Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails.Status);
        Assert.NotNull(actualValidationProblemDetails.Extensions["traceId"]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task GetAuthorByIdAsync_WithMediaTypeVendorSpecific_Returns200WithHateoasLinks(int currentAuthorIndex)
    {
        // Arrange:
        Guid expectedId = await GetIdOrderedByConditionAsync<GetAuthorDto>(
            currentAuthorIndex,
            AuthorBaseUri,
            getAuthorDto => getAuthorDto.FullName);
        HttpClient.DefaultRequestHeaders.Add("Accept", CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{AuthorBaseUri}/{expectedId}");

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        AuthorWithHateoasLinks authorWithLinks = JsonSerializer.Deserialize<AuthorWithHateoasLinks>(responseContent, jsonSerializerOptions)!;
        Assert.Contains(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            httpResponseMessage.Content.Headers.ContentType!.ToString());
        Assert.Equal(s_expectedAuthors[currentAuthorIndex].FullName, authorWithLinks.FullName);
        Assert.True(s_expectedAuthors[currentAuthorIndex].Books.SequenceEqual(
            s_expectedAuthors[currentAuthorIndex].Books.OrderBy(book => book.BookTitle)));
        Assert.Equal("self", authorWithLinks.Links[0].Rel);
        Assert.Equal("add_existing_books_to_author", authorWithLinks.Links[1].Rel);
        Assert.Equal("delete_author", authorWithLinks.Links[2].Rel);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task GetAuthorByIdAsync_WithMediaTypeNotVendorSpecific_Returns200WithObject(int currentAuthorIndex)
    {
        // Arrange:
        Guid expectedId = await GetIdOrderedByConditionAsync<GetAuthorDto>(
            currentAuthorIndex,
            AuthorBaseUri,
            getAuthorDto => getAuthorDto.FullName);
        HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{AuthorBaseUri}/{expectedId}");

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        GetAuthorDto? actualAuthor = await httpResponseMessage.Content.ReadFromJsonAsync<GetAuthorDto>();
        Assert.Equal(s_expectedAuthors[currentAuthorIndex].FullName, actualAuthor!.FullName);
        Assert.True(s_expectedAuthors[currentAuthorIndex].Books.SequenceEqual(
                actualAuthor.Books.OrderBy(book => book.BookTitle)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task GetAuthorByIdAsync_WithHeadAndMediaTypeVendorSpecific_Returns200WithContentTypeHeaders(int currentAuthorIndex)
    {
        // Arrange:
        Guid id = await GetIdOrderedByConditionAsync<GetAuthorDto>(
            currentAuthorIndex,
            AuthorBaseUri,
            getAuthorDto => getAuthorDto.FullName);
        HttpClient.DefaultRequestHeaders.Add("Accept", CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson);
        var expectedContentTypeHeaders = new List<string>
        {
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson + "; charset=utf-8"
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Head,
            $"{AuthorBaseUri}/{id}"));

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
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task GetAuthorByIdAsync_WithHeadAndMediaTypeNotVendorSpecific_Returns200WithContentTypeHeaders(int currentAuthorIndex)
    {
        // Arrange:
        Guid id = await GetIdOrderedByConditionAsync<GetAuthorDto>(
            currentAuthorIndex,
            AuthorBaseUri,
            getAuthorDto => getAuthorDto.FullName);
        HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
        var expectedContentTypeHeaders = new List<string>
        {
            MediaTypeNames.Application.Json + "; charset=utf-8"
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Head,
            $"{AuthorBaseUri}/{id}"));

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        IEnumerable<string> actualContentTypeHeaders = httpResponseMessage.Content.Headers.GetValues("content-type");
        Assert.Equal(expectedContentTypeHeaders, actualContentTypeHeaders);
    }

    [Fact]
    public async Task CreateAuthorAsync_WithMediaTypeVendorSpecific_Returns201WithResponseBody()
    {
        // Arrange:
        const string ExpectedFirstName = "John";
        const string ExpectedLastName = "Doe";
        var expectedHateoasLinks = new List<HateoasLinkDto>
        {
            new(It.IsAny<string>(), "self", "GET"),
            new(It.IsAny<string>(), "add_existing_books_to_author", "PATCH"),
            new(It.IsAny<string>(), "delete_author", "DELETE"),
        };
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, AuthorBaseUri)
        {
            Content = new StringContent(
                $"{{\"firstName\": \"{ExpectedFirstName}\", \"lastName\": \"{ExpectedLastName}\"}}",
                Encoding.UTF8,
                CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson)
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        Guid actualCreatedAuthorId = JsonSerializer.Deserialize<AuthorCreatedDto>(responseContent, jsonSerializerOptions)!.Id;
        AuthorWithHateoasLinks actualAuthorWithLinks = await GetAsync<AuthorWithHateoasLinks>(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            $"{AuthorBaseUri}/{actualCreatedAuthorId}");
        Assert.Equal(ExpectedFirstName + " " + ExpectedLastName, actualAuthorWithLinks.FullName);
        Assert.Equal(actualCreatedAuthorId, actualAuthorWithLinks.Id);
        Assert.Empty(actualAuthorWithLinks.Books);
        Assert.Equal(expectedHateoasLinks[0].Rel, actualAuthorWithLinks.Links[0].Rel);
        Assert.Equal(expectedHateoasLinks[1].Rel, actualAuthorWithLinks.Links[1].Rel);
        Assert.Equal(expectedHateoasLinks[2].Rel, actualAuthorWithLinks.Links[2].Rel);
        Assert.Equal(expectedHateoasLinks[0].Method, actualAuthorWithLinks.Links[0].Method);
        Assert.Equal(expectedHateoasLinks[1].Method, actualAuthorWithLinks.Links[1].Method);
        Assert.Equal(expectedHateoasLinks[2].Method, actualAuthorWithLinks.Links[2].Method);

        // Clean up:
        await HttpClient.DeleteAsync($"{AuthorBaseUri}/{actualCreatedAuthorId}");
    }

    [Fact]
    public async Task CreateAuthorAsync_WithMediaTypeNotVendorSpecific_Returns201WithResponseBody()
    {
        // Arrange:
        const string ExpectedFirstName = "John";
        const string ExpectedLastName = "Doe";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, AuthorBaseUri)
        {
            Content = new StringContent(
                $"{{\"firstName\": \"{ExpectedFirstName}\", \"lastName\": \"{ExpectedLastName}\"}}",
                Encoding.UTF8,
                MediaTypeNames.Application.Json)
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        Guid actualCreatedAuthorId = JsonSerializer.Deserialize<AuthorCreatedDto>(responseContent, jsonSerializerOptions)!.Id;
        var uriWithCreatedAuthorId = $"{AuthorBaseUri}/{actualCreatedAuthorId}";
        GetAuthorDto actualAuthor = await GetAsync<GetAuthorDto>(
            MediaTypeNames.Application.Json,
            uriWithCreatedAuthorId);
        Assert.Equal(ExpectedFirstName + " " + ExpectedLastName, actualAuthor.FullName);
        Assert.Equal(actualCreatedAuthorId, actualAuthor.Id);
        Assert.Empty(actualAuthor.Books);

        // Clean up:
        await HttpClient.DeleteAsync(uriWithCreatedAuthorId);
    }

    [Theory]
    [MemberData(nameof(CreateAuthorAsyncWithInvalidParametersTestData))]
    public async Task CreateAuthorAsync_WithInvalidParameters_Returns422WithErrorMessage(
        string? firstName,
        string lastName,
        ValidationProblemDetails expectedValidationProblemDetails)
    {
        // Arrange:
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, AuthorBaseUri)
        {
            Content = new StringContent(
                $"{{\"firstName\": \"{firstName}\", \"lastName\": \"{lastName}\"}}",
                Encoding.UTF8,
                MediaTypeNames.Application.Json)
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, httpResponseMessage.StatusCode);
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails.Status);
        Assert.NotNull(actualValidationProblemDetails.Extensions["traceId"]);
    }

    [Fact]
    public async Task AddExistingBooksToAuthorAsync_WithExistingAuthorAndBook_Returns204AndAddsBookToAuthor()
    {
        // Arrange:
        Guid authorId = (await CreateAsync<AuthorCreatedDto>(
            "{\"firstName\": \"John\", \"lastName\": \"Doe\"}",
            MediaTypeNames.Application.Json,
            AuthorBaseUri)).Id;
        Guid authorThatBookWillBeAddedToId = (await CreateAsync<AuthorCreatedDto>(
            "{\"firstName\": \"Jane\", \"lastName\": \"Doe\"}",
            MediaTypeNames.Application.Json,
            AuthorBaseUri)).Id;
        BookCreatedDto book = await CreateAsync<BookCreatedDto>(
            $"{{\"authorIds\": [\"{authorId}\"], \"bookTitle\": \"Title\", \"edition\": 1, \"isbn\": \"0-301-64361-2\"}}",
            MediaTypeNames.Application.Json,
            BookBaseUri);
        var uriWithAuthorThatBookWillBeAddedToId = $"{AuthorBaseUri}/{authorThatBookWillBeAddedToId}";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, $"{uriWithAuthorThatBookWillBeAddedToId}/AddBooks")
        {
            Content = new StringContent(
                $"[{{\"op\": \"add\", \"path\": \"/bookIds\", \"value\": [\"{book.Id}\"]}}]",
                Encoding.UTF8,
                MediaTypeNames.Application.JsonPatch)
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        GetAuthorDto updatedAuthor = await GetAsync<GetAuthorDto>(
            MediaTypeNames.Application.Json,
            uriWithAuthorThatBookWillBeAddedToId);
        Assert.Equal(book.BookTitle, updatedAuthor.Books[0].BookTitle);

        // Clean up:
        await HttpClient.DeleteAsync($"{BookBaseUri}/{book.Id}");
        await HttpClient.DeleteAsync(uriWithAuthorThatBookWillBeAddedToId);
        await HttpClient.DeleteAsync($"{AuthorBaseUri}/{authorId}");
    }

    [Fact]
    public async Task AddExistingBooksToAuthorAsync_WithNonexistingAuthor_Returns404WithErrorMessage()
    {
        // Arrange:
        Guid nonexistingAuthorId = Guid.NewGuid();
        BookCreatedDto book = await CreateAsync<BookCreatedDto>(
            $"{{\"authorIds\": [\"{nonexistingAuthorId}\"], \"bookTitle\": \"Title\", \"edition\": 1, \"isbn\": \"0-301-64361-2\"}}",
            MediaTypeNames.Application.Json,
            BookBaseUri);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, $"{AuthorBaseUri}/{nonexistingAuthorId}/AddBooks")
        {
            Content = new StringContent(
                $"[{{\"op\": \"add\", \"path\": \"/bookIds\", \"value\": [\"{book.Id}\"]}}]",
                Encoding.UTF8,
                MediaTypeNames.Application.JsonPatch)
        };
        var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Errors = new Dictionary<string, string[]>
            {
                { "idNotFound", [$"No author with the ID of '{nonexistingAuthorId}' was found."]}
            },
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            Title = "One or more validation errors occurred.",
            Status = 404
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails.Status);
        Assert.NotNull(actualValidationProblemDetails.Extensions["traceId"]);

        // Clean up:
        await HttpClient.DeleteAsync($"{BookBaseUri}/{book.Id}");
    }

    [Fact]
    public async Task AddExistingBooksToAuthorAsync_WithNonexistingBook_Returns422WithErrorMessage()
    {
        // Arrange:
        Guid nonexistingBookId = Guid.NewGuid();
        Guid authorId = (await CreateAsync<AuthorCreatedDto>(
            "{\"firstName\": \"James\", \"lastName\": \"Smith\"}",
            MediaTypeNames.Application.Json,
            AuthorBaseUri)).Id;
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, $"{AuthorBaseUri}/{authorId}/AddBooks")
        {
            Content = new StringContent(
                $"[{{\"op\": \"add\", \"path\": \"/bookIds\", \"value\": [\"{nonexistingBookId}\"]}}]",
                Encoding.UTF8,
                MediaTypeNames.Application.JsonPatch)
        };
        var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Errors = new Dictionary<string, string[]>
            {
                { "bookIds", ["Could not find some of the books for the provided IDs."]}
            },
            Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
            Title = "One or more validation errors occurred.",
            Status = 422
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, httpResponseMessage.StatusCode);
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails.Status);
        Assert.NotNull(actualValidationProblemDetails.Extensions["traceId"]);

        // Clean up:
        await HttpClient.DeleteAsync($"{BookBaseUri}/{authorId}");
    }

    [Fact]
    public async Task DeleteAuthorByIdAsync_WithExistingAuthor_Returns204AndDeletesAuthor()
    {
        // Arrange:
        Guid createdAuthorId = (await CreateAsync<AuthorCreatedDto>(
            "{\"firstName\": \"John\", \"lastName\": \"Doe\"}",
            MediaTypeNames.Application.Json,
            AuthorBaseUri)).Id;
        var uriWithCreatedAuthorId = $"{AuthorBaseUri}/{createdAuthorId}";

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            uriWithCreatedAuthorId));

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        GetAuthorDto author = await GetAsync<GetAuthorDto>(MediaTypeNames.Application.Json, uriWithCreatedAuthorId);
        Assert.Equal(Guid.Empty, author.Id);
    }

    [Fact]
    public async Task DeleteAuthorByIdAsync_WithNonexistingAuthor_Returns404WithErrorMessage()
    {
        // Arrange:
        Guid nonexistingAuthorId = Guid.NewGuid();
        var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Errors = new Dictionary<string, string[]>
            {
                { "idNotFound", [$"No author with the ID of '{nonexistingAuthorId}' was found."]}
            },
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            Title = "One or more validation errors occurred.",
            Status = 404
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            $"{AuthorBaseUri}/{nonexistingAuthorId}"));

        // Assert:
        Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails.Status);
        Assert.NotNull(actualValidationProblemDetails.Extensions["traceId"]);
    }

    [Fact]
    public async Task DeleteAuthorByIdAsync_WithAuthorWithBooks_Returns422WithErrorMessage()
    {
        // Arrange:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(AuthorBaseUri);
        string authors = await httpResponseMessage.Content.ReadAsStringAsync();
        Guid existingAuthorWithBooksId = JsonSerializer.Deserialize<List<GetAuthorDto>>(authors, jsonSerializerOptions)!.First().Id;
        var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Errors = new Dictionary<string, string[]>
            {
                { "authorHasBooks", ["This author has books. Please, delete the books before deleting the author."]}
            },
            Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
            Title = "One or more validation errors occurred.",
            Status = 422
        };

        // Act:
        httpResponseMessage = await HttpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            $"{AuthorBaseUri}/{existingAuthorWithBooksId}"));

        // Assert:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, httpResponseMessage.StatusCode);
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails.Status);
        Assert.NotNull(actualValidationProblemDetails.Extensions["traceId"]);
    }

    [Fact]
    public async Task GetAuthorOptions_WithoutParameters_Returns200WithHeaders()
    {
        // Arrange:
        var expectedAllowHeader = new List<string>
        {
            "GET",
            "HEAD",
            "POST",
            "DELETE",
            "OPTIONS"
        };
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Options, AuthorBaseUri);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        Assert.Equal(expectedAllowHeader, httpResponseMessage.Content.Headers.GetValues("allow"));
    }

    [Fact]
    public async Task GetAuthorAddBooksOptionsAsync_WithExistingAuthor_Returns200WithHeaders()
    {
        // Arrange:
        var expectedAllowHeader = new List<string> { "PATCH", "OPTIONS" };
        Guid existingAuthorId = (await GetAsync<List<GetAuthorDto>>(MediaTypeNames.Application.Json, AuthorBaseUri))
            .First()
            .Id;
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Options, $"{AuthorBaseUri}/{existingAuthorId}/AddBooks");

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        Assert.Equal(expectedAllowHeader, httpResponseMessage.Content.Headers.GetValues("allow"));
    }

    [Fact]
    public async Task GetAuthorAddBooksOptionsAsync_WithNonexistingAuthor_Returns404WithErrorMessage()
    {
        // Arrange:
        Guid nonexistingAuthorId = Guid.NewGuid();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Options, $"{AuthorBaseUri}/{nonexistingAuthorId}/AddBooks");
        var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Errors = new Dictionary<string, string[]>
            {
                { "idNotFound", [$"No author with the ID of '{nonexistingAuthorId}' was found."]}
            },
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            Title = "One or more validation errors occurred.",
            Status = 404
        };

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage);

        // Assert:
        Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails.Status);
        Assert.NotNull(actualValidationProblemDetails.Extensions["traceId"]);
    }
}
