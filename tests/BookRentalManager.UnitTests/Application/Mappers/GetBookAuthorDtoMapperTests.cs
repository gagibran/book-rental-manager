namespace BookRentalManager.UnitTests.Application.Mappers;

public sealed class GetBookAuthorDtoMapperTests
{
    [Fact]
    public void Map_WithValidBookAuthor_ReturnsValidGetBookAuthorDto()
    {
        // Arrange:
        BookAuthor customer = TestFixtures.CreateDummyBookAuthor();
        var expectedGetBookAuthorDto = new GetBookAuthorDto(customer.Id, customer.FullName);
        var getBookAuthorDtoMapper = new GetBookAuthorDtoMapper();

        // Act:
        GetBookAuthorDto getBookAuthorDto = getBookAuthorDtoMapper.Map(customer);

        // Assert (maybe refactor this using FluentAssertions):
        Assert.Equal(expectedGetBookAuthorDto.Id, getBookAuthorDto.Id);
        Assert.Equal(expectedGetBookAuthorDto.FullName, getBookAuthorDto.FullName);
    }
}
