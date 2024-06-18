using System.Dynamic;
using Newtonsoft.Json;

namespace BookRentalManager.IntegrationTests.Common;

public abstract class IntegrationTest(IntegrationTestsWebApplicationFactory integrationTestsWebbApplicationFactory)
    : IClassFixture<IntegrationTestsWebApplicationFactory>
{
    protected HttpClient HttpClient { get; } = integrationTestsWebbApplicationFactory.CreateClient();

    protected static CollectionWithLinks GetValuesWithLinksFromHateoasCollectionResponse(string responseContent)
    {
        ExpandoObject? responseBody = JsonConvert.DeserializeObject<ExpandoObject>(responseContent);
        return new(
            (List<object>?)responseBody!.ElementAt(0).Value!,
            (List<object>?)responseBody!.ElementAt(1).Value!);
    }

    protected static AuthorWithLinks GetVAuthorWithLinksFromHateoasAuthorResponse(string responseContent)
    {
        ExpandoObject? responseBody = JsonConvert.DeserializeObject<ExpandoObject>(responseContent);
        return new(
            (string)responseBody!.ElementAt(0).Value!,
            (List<object>)responseBody!.ElementAt(1).Value!,
            (List<object>)responseBody!.ElementAt(3).Value!);
    }
}
