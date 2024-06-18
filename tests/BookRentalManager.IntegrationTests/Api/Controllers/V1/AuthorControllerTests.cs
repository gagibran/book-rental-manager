using System.Net.Http.Json;
using System.Net.Mime;
using BookRentalManager.Api.Constants;
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
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync("api/v1/author");

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        CollectionWithLinks collectionWithLinks = GetValuesWithLinksFromHateoasCollectionResponse(responseContent);
        Assert.Contains(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            httpResponseMessage.Content.Headers.ContentType!.ToString());
        Assert.NotEqual(string.Empty, ((dynamic)collectionWithLinks.Collection.ElementAt(0)).id);
        Assert.Equal("Erich Gamma", ((dynamic)collectionWithLinks.Collection.ElementAt(0)).fullName);
        Assert.NotEmpty(((dynamic)collectionWithLinks.Collection.ElementAt(0)).books);
        Assert.NotEmpty(((dynamic)collectionWithLinks.Collection.ElementAt(0)).links);
        Assert.Empty(collectionWithLinks.Links);
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
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/v1/author{queryParameters}");

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
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync("api/v1/author?pageSize=notANumber");

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
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync("api/v1/author?sortBy=notAValidParameter");

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
            $"api/v1/author{queryParameters}"));

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
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/v1/author/{expectedId}");

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        AuthorWithLinks authorWithLinks = GetVAuthorWithLinksFromHateoasAuthorResponse(responseContent);
        Assert.Contains(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            httpResponseMessage.Content.Headers.ContentType!.ToString());
        Assert.Equal(s_expectedAuthors.ElementAt(currentAuthorIndex).FullName, authorWithLinks.FullName);
        Assert.True(s_expectedAuthors.ElementAt(currentAuthorIndex).Books.SequenceEqual(
            s_expectedAuthors.ElementAt(currentAuthorIndex).Books.OrderBy(book => book.BookTitle)));
        Assert.Equal("self", ((dynamic)authorWithLinks.Links.ElementAt(0)).rel);
        Assert.Equal("add_existing_books_to_author", ((dynamic)authorWithLinks.Links.ElementAt(1)).rel);
        Assert.Equal("delete_author", ((dynamic)authorWithLinks.Links.ElementAt(2)).rel);
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
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/v1/author/{expectedId}");

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
    public async Task GetAuthorByIdAsync_WithHeadAndHateoas_Returns200OkWithContentTypeHeaders(int currentAuthorIndex)
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
            $"api/v1/author/{id}"));

        // Assert:
        httpResponseMessage.EnsureSuccessStatusCode();
        IEnumerable<string> actualContentTypeHeaders = httpResponseMessage.Content.Headers.GetValues("content-type");
        Assert.Equal(expectedContentTypeHeaders, actualContentTypeHeaders);
    }

    private async Task<Guid> GetAuthorIdOrderedByFullNameAsync(int authorIndex)
    {
        HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync("api/v1/author");
        HttpClient.DefaultRequestHeaders.Clear();
        return (await httpResponseMessage.Content.ReadFromJsonAsync<List<GetAuthorDto>>())!
            .OrderBy(getAuthorDto => getAuthorDto.FullName)
            .ElementAt(authorIndex).Id;
    }
}
