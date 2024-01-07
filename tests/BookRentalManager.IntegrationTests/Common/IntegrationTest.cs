﻿namespace BookRentalManager.IntegrationTests;

public abstract class IntegrationTest(IntegrationTestsWebApplicationFactory integrationTestsWebbApplicationFactory)
    : IClassFixture<IntegrationTestsWebApplicationFactory>
{
    protected HttpClient HttpClient { get; } = integrationTestsWebbApplicationFactory.CreateClient();
}