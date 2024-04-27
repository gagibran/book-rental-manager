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
    public static TheoryData<string, Func<GetAuthorDto, string>, List<GetAuthorDto>, IEnumerable<string>> GetAuthorsByQueryParametersAsyncTestData()
    {
        var expectedAuthors = new List<GetAuthorDto>
        {
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
        };
        return new()
        {
            {
                "",
                getAuthorDto => getAuthorDto.FullName,
                expectedAuthors,
                new List<string> { "{\"totalAmountOfItems\":7,\"pageIndex\":1,\"pageSize\":50,\"totalAmountOfPages\":1}" }
            },
            {
                "?sortBy=FullName",
                getAuthorDto => getAuthorDto.FullName,
                expectedAuthors,
                new List<string> { "{\"totalAmountOfItems\":7,\"pageIndex\":1,\"pageSize\":50,\"totalAmountOfPages\":1}" }
            },
            {
                "?sortBy=FullName&pageSize=1&pageIndex=2",
                getAuthorDto => getAuthorDto.FullName,
                expectedAuthors.Skip(1).Take(1).ToList(),
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
        (List<object> values, List<object> links) = GetValuesAndLinksFromHateoasResponse(responseContent);
        Assert.Contains(
            CustomMediaTypeNames.Application.VendorBookRentalManagerHateoasJson,
            httpResponseMessage.Content.Headers.ContentType!.ToString());
        Assert.NotEqual(string.Empty, ((dynamic)values.ElementAt(0)).id);
        Assert.Equal("Erich Gamma", ((dynamic)values.ElementAt(0)).fullName);
        Assert.NotEmpty(((dynamic)values.ElementAt(0)).books);
        Assert.NotEmpty(((dynamic)values.ElementAt(0)).links);
        Assert.Empty(links);
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
}
