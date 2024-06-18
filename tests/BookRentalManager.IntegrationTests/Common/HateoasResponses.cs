namespace BookRentalManager.IntegrationTests.Common;

public record CollectionWithLinks(List<object> Collection, List<object> Links);
public record AuthorWithLinks(string FullName, List<object> Books, List<object> Links);
