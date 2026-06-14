using Members.Api.IntegrationTests.Testing;

namespace Members.Api.IntegrationTests;

[Collection("Api")]
public sealed class HealthEndpointsTests
{
    private readonly HttpClient _client;

    public HealthEndpointsTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_health_live_returns_200()
    {
        var response = await _client.GetAsync("/health/live");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_health_ready_returns_200_when_db_is_available()
    {
        var response = await _client.GetAsync("/health/ready");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}
