using System.Dynamic;
using Newtonsoft.Json;

namespace BookRentalManager.IntegrationTests;

public abstract class IntegrationTest(IntegrationTestsWebApplicationFactory integrationTestsWebbApplicationFactory)
    : IClassFixture<IntegrationTestsWebApplicationFactory>
{
    protected HttpClient HttpClient { get; } = integrationTestsWebbApplicationFactory.CreateClient();

    protected static (List<object> values, List<object> links) GetValuesAndLinksFromHateoasResponse(string responseContent)
    {
        ExpandoObject? responseBody = JsonConvert.DeserializeObject<ExpandoObject>(responseContent);
        var values = (List<object>?)responseBody!.ElementAt(0).Value;
        var links = (List<object>?)responseBody!.ElementAt(1).Value;
        return (values!, links!);
    }
}
