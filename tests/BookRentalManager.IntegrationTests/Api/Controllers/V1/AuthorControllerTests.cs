using System.Net.Http.Json;
using System.Net.Mime;
using BookRentalManager.Api.Constants;
using BookRentalManager.Application.Dtos;
using BookRentalManager.Domain.ValueObjects;
using Moq;

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

    [Fact]
    public async Task GetAuthorsByQueryParametersAsync_WithMediaTypeNotVendorSpecific_Returns200WithObject()
    {
        // Arrange:
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
        HttpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);

        // Act:
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync("api/v1/author");

        // Assert:
        IOrderedEnumerable<GetAuthorDto> actualAuthors = (await httpResponseMessage.Content.ReadFromJsonAsync<List<GetAuthorDto>>())!
            .OrderBy(actualAuthor => actualAuthor.FullName);
        for (int authorIndex = 0; authorIndex < expectedAuthors.Count; authorIndex++)
        {
            Assert.Equal(expectedAuthors.ElementAt(authorIndex).FullName, actualAuthors.ElementAt(authorIndex).FullName);
            Assert.True(expectedAuthors.ElementAt(authorIndex).Books.SequenceEqual(
                actualAuthors.ElementAt(authorIndex).Books.OrderBy(book => book.BookTitle)));
        }
    }
}
