using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using BookRentalManager.Api.Constants;
using BookRentalManager.Application.Common;
using BookRentalManager.Application.Dtos;
using BookRentalManager.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookRentalManager.IntegrationTests.Api.Controllers.V1;

public sealed class AuthorControllerTests(IntegrationTestsWebApplicationFactory integrationTestsWebbApplicationFactory)
    : IntegrationTest(integrationTestsWebbApplicationFactory)
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

    public static TheoryData<string, Func<GetAuthorDto, string>, List<GetAuthorDto>, IEnumerable<string>> GetAuthorsByQueryParametersAsyncTestData()
    {
        return new()
        {
            {
                "",
                getAuthorDto => getAuthorDto.FullName,
                s_expectedAuthors,
                new List<string> { "{\"totalAmountOfItems\":7,\"pageIndex\":1,\"pageSize\":50,\"totalAmountOfPages\":1}" }
            },
            {
                "?sortBy=FullName",
                getAuthorDto => getAuthorDto.FullName,
                s_expectedAuthors,
                new List<string> { "{\"totalAmountOfItems\":7,\"pageIndex\":1,\"pageSize\":50,\"totalAmountOfPages\":1}" }
            },
            {
                "?sortBy=FullName&pageSize=1&pageIndex=2",
                getAuthorDto => getAuthorDto.FullName,
                s_expectedAuthors.Skip(1).Take(1).ToList(),
                new List<string> { "{\"totalAmountOfItems\":7,\"pageIndex\":2,\"pageSize\":1,\"totalAmountOfPages\":7}" }
            },
        };
    }

    [Fact]
    public async Task GetAuthorsByQueryParametersAsync_WithMediaTypeVendorSpecific_Returns200OkWithHateoasLinks()
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
        Assert.NotEqual(Guid.Empty, authorsWithHateoasLinks.Values.ElementAt(0).Id);
        Assert.Equal("Erich Gamma", authorsWithHateoasLinks.Values.ElementAt(0).FullName);
        Assert.NotEmpty(authorsWithHateoasLinks.Values.ElementAt(0).Books);
        Assert.NotEmpty(authorsWithHateoasLinks.Values.ElementAt(0).Links);
        Assert.Empty(authorsWithHateoasLinks.Links);
    }

    [Theory]
    [MemberData(nameof(GetAuthorsByQueryParametersAsyncTestData))]
    public async Task GetAuthorsByQueryParametersAsync_WithMediaTypeNotVendorSpecific_Returns200OkWithObjectAndXPaginationHeaders(
        string queryParameters,
        Func<GetAuthorDto, string> orderBy,
        List<GetAuthorDto> expectedAuthors,
        IEnumerable<string> expectedXPaginationHeaders)
    {
        // Arrange:
        HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{AuthorBaseUri}{queryParameters}");

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        IEnumerable<string> actualXPaginationHeaders = httpResponseMessage.Headers.GetValues("x-pagination");
        IOrderedEnumerable<GetAuthorDto> actualAuthors = (await httpResponseMessage.Content.ReadFromJsonAsync<List<GetAuthorDto>>())!
            .OrderBy(orderBy);
        Assert.Equal(expectedXPaginationHeaders, actualXPaginationHeaders);
        for (int authorIndex = 0; authorIndex < expectedAuthors.Count; authorIndex++)
        {
            Assert.Equal(expectedAuthors.ElementAt(authorIndex).FullName, actualAuthors.ElementAt(authorIndex).FullName);
            Assert.True(expectedAuthors.ElementAt(authorIndex).Books.SequenceEqual(
                actualAuthors.ElementAt(authorIndex).Books.OrderBy(book => book.BookTitle)));
        }
    }

    [Fact]
    public async Task GetAuthorsByQueryParametersAsync_WithIncorrectParameterType_Returns400BadRequest()
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
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails!.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails!.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails!.Status);
    }

    [Fact]
    public async Task GetAuthorsByQueryParametersAsync_WithNonexistingQueryParameter_Returns422UnprocessableEntity()
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
        ValidationProblemDetails? actualValidationProblemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Equal(expectedValidationProblemDetails.Errors, actualValidationProblemDetails!.Errors);
        Assert.Equal(expectedValidationProblemDetails.Type, actualValidationProblemDetails!.Type);
        Assert.Equal(expectedValidationProblemDetails.Title, actualValidationProblemDetails!.Title);
        Assert.Equal(expectedValidationProblemDetails.Status, actualValidationProblemDetails!.Status);
    }

    [Theory]
    [InlineData("", "{\"totalAmountOfItems\":7,\"pageIndex\":1,\"pageSize\":50,\"totalAmountOfPages\":1}")]
    [InlineData("?sortBy=FullName", "{\"totalAmountOfItems\":7,\"pageIndex\":1,\"pageSize\":50,\"totalAmountOfPages\":1}")]
    [InlineData("?sortBy=FullName&pageSize=1&pageIndex=2", "{\"totalAmountOfItems\":7,\"pageIndex\":2,\"pageSize\":1,\"totalAmountOfPages\":7}")]
    public async Task GetAuthorsByQueryParametersAsync_WithHead_Returns200OkWithXPaginationHeaders(
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

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task GetAuthorByIdAsync_WithMediaTypeVendorSpecific_Returns200OkWithHateoasLinks(int currentAuthorIndex)
    {
        // Arrange:
        Guid expectedId = await GetAuthorIdOrderedByFullNameAsync(currentAuthorIndex);
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
        Assert.Equal(s_expectedAuthors.ElementAt(currentAuthorIndex).FullName, authorWithLinks.FullName);
        Assert.True(s_expectedAuthors.ElementAt(currentAuthorIndex).Books.SequenceEqual(
            s_expectedAuthors.ElementAt(currentAuthorIndex).Books.OrderBy(book => book.BookTitle)));
        Assert.Equal("self", authorWithLinks.Links.ElementAt(0).Rel);
        Assert.Equal("add_existing_books_to_author", authorWithLinks.Links.ElementAt(1).Rel);
        Assert.Equal("delete_author", authorWithLinks.Links.ElementAt(2).Rel);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task GetAuthorByIdAsync_WithMediaTypeNotVendorSpecific_Returns200OkWithObject(int currentAuthorIndex)
    {
        // Arrange:
        Guid expectedId = await GetAuthorIdOrderedByFullNameAsync(currentAuthorIndex);
        HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"{AuthorBaseUri}/{expectedId}");

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        GetAuthorDto? actualAuthor = await httpResponseMessage.Content.ReadFromJsonAsync<GetAuthorDto>();
        Assert.Equal(s_expectedAuthors.ElementAt(currentAuthorIndex).FullName, actualAuthor!.FullName);
        Assert.True(s_expectedAuthors.ElementAt(currentAuthorIndex).Books.SequenceEqual(
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
    public async Task GetAuthorByIdAsync_WithHeadAndMediaTypeVendorSpecific_Returns200OkWithContentTypeHeaders(int currentAuthorIndex)
    {
        // Arrange:
        Guid id = await GetAuthorIdOrderedByFullNameAsync(currentAuthorIndex);
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
    public async Task GetAuthorByIdAsync_WithHeadAndMediaTypeNotVendorSpecific_Returns200OkWithContentTypeHeaders(int currentAuthorIndex)
    {
        // Arrange:
        Guid id = await GetAuthorIdOrderedByFullNameAsync(currentAuthorIndex);
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
        var actualAuthorWithLinks = await GetAsync<AuthorWithHateoasLinks>(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            $"{AuthorBaseUri}/{actualCreatedAuthorId}");
        Assert.Equal(ExpectedFirstName + " " + ExpectedLastName, actualAuthorWithLinks.FullName);
        Assert.Equal(actualCreatedAuthorId, actualAuthorWithLinks.Id);
        Assert.Empty(actualAuthorWithLinks.Books);
        Assert.Equal(expectedHateoasLinks.ElementAt(0).Rel, actualAuthorWithLinks.Links.ElementAt(0).Rel);
        Assert.Equal(expectedHateoasLinks.ElementAt(1).Rel, actualAuthorWithLinks.Links.ElementAt(1).Rel);
        Assert.Equal(expectedHateoasLinks.ElementAt(2).Rel, actualAuthorWithLinks.Links.ElementAt(2).Rel);
        Assert.Equal(expectedHateoasLinks.ElementAt(0).Method, actualAuthorWithLinks.Links.ElementAt(0).Method);
        Assert.Equal(expectedHateoasLinks.ElementAt(1).Method, actualAuthorWithLinks.Links.ElementAt(1).Method);
        Assert.Equal(expectedHateoasLinks.ElementAt(2).Method, actualAuthorWithLinks.Links.ElementAt(2).Method);

        // Clean up:
        await HttpClient.DeleteAsync(new Uri($"{AuthorBaseUri}/{actualCreatedAuthorId}", UriKind.Relative));
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
        var actualAuthor = await GetAsync<GetAuthorDto>(
            MediaTypeNames.Application.Json,
            uriWithCreatedAuthorId);
        Assert.Equal(ExpectedFirstName + " " + ExpectedLastName, actualAuthor.FullName);
        Assert.Equal(actualCreatedAuthorId, actualAuthor.Id);
        Assert.Empty(actualAuthor.Books);

        // Clean up:
        await HttpClient.DeleteAsync(new Uri(uriWithCreatedAuthorId, UriKind.Relative));
    }

    [Theory]
    [InlineData("John", "", "Last name cannot be empty.")]
    [InlineData("", "Doe", "First name cannot be empty.")]
    [InlineData("", "", "name cannot be empty.")]
    public async Task CreateAuthorAsync_WithMissingParameter_Returns422WithErrorMessage(string firstName, string lastName, string expectedErrorMessage)
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
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.UnprocessableEntity, httpResponseMessage.StatusCode);
        Assert.Contains(expectedErrorMessage, responseContent);
    }

    [Fact]
    public async Task DeleteAuthorByIdAsync_WithExistingAuthor_DeletesAuthorAndReturns204()
    {
        // Arrange:
        Guid createdAuthorId = (await CreateAsync<AuthorCreatedDto>(
            $"{{\"firstName\": \"John\", \"lastName\": \"Doe\"}}",
            MediaTypeNames.Application.Json,
            AuthorBaseUri)).Id;
        var uriWithCreatedAuthorId = $"{AuthorBaseUri}/{createdAuthorId}";

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            uriWithCreatedAuthorId));

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        var author = await GetAsync<GetAuthorDto>(MediaTypeNames.Application.Json, uriWithCreatedAuthorId);
        Assert.Equal(Guid.Empty, author.Id);
    }

    [Fact]
    public async Task DeleteAuthorByIdAsync_WithNonexistingAuthor_Returns404WithErrorMessage()
    {
        // Arrange:
        Guid nonexistingAuthorId = Guid.NewGuid();

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            $"{AuthorBaseUri}/{nonexistingAuthorId}"));

        // Assert:
        Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Contains($"No author with the ID of '{nonexistingAuthorId}' was found.", responseContent);
    }

    [Fact]
    public async Task DeleteAuthorByIdAsync_WithAuthorWithBooks_Returns422WithErrorMessage()
    {
        // Arrange:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(new Uri(AuthorBaseUri, UriKind.Relative));
        string authors = await httpResponseMessage.Content.ReadAsStringAsync();
        Guid existingAuthorWithBooksId = JsonSerializer.Deserialize<List<GetAuthorDto>>(authors, jsonSerializerOptions)!.First().Id;

        // Act:
        httpResponseMessage = await HttpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            $"{AuthorBaseUri}/{existingAuthorWithBooksId}"));

        // Assert:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, httpResponseMessage.StatusCode);
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Contains("This author has books. Please, delete the books before deleting the author.", responseContent);
    }

    [Fact]
    public async Task GetAuthorOptions_WithoutParameters_returnsOkWithHeaders()
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

    private async Task<Guid> GetAuthorIdOrderedByFullNameAsync(int authorIndex)
    {
        return (await GetAsync<List<GetAuthorDto>>(MediaTypeNames.Application.Json, AuthorBaseUri))
            .OrderBy(getAuthorDto => getAuthorDto.FullName)
            .ElementAt(authorIndex).Id;
    }
}
