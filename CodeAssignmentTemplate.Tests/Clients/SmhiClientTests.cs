using System.Net;
using CodeAssignmentTemplate.Clients;
using Xunit;

namespace CodeAssignmentTemplate.Tests.Clients;

public class SmhiClientTests
{
    [Fact]
    public async Task GetLatestMonthsRainfallAsync_ReturnsEmptyOnNotFound()
    {
        var client = CreateClient(new HttpResponseMessage(HttpStatusCode.NotFound), out _);

        var data = await client.GetLatestMonthsRainfallAsync(53430, CancellationToken.None);

        Assert.Empty(data.Value);
    }

    [Fact]
    public async Task GetLatestMonthsRainfallAsync_ThrowsOnNonNotFoundError()
    {
        var client = CreateClient(new HttpResponseMessage(HttpStatusCode.InternalServerError), out _);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => client.GetLatestMonthsRainfallAsync(53430, CancellationToken.None));
    }

    [Fact]
    public async Task GetLatestMonthsRainfallAsync_DeserializesValuesAndRequestsExpectedPath()
    {
        const string json = """
            {
              "value": [
                { "from": 1769061601000, "to": 1769148000000, "ref": "2026-01-22", "value": "0.0", "quality": "G" },
                { "from": 1769148001000, "to": 1769234400000, "ref": "2026-01-23", "value": "1.5", "quality": "Y" }
              ]
            }
            """;
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };
        var client = CreateClient(response, out var handler);

        var data = await client.GetLatestMonthsRainfallAsync(53430, CancellationToken.None);

        Assert.Equal(2, data.Value.Count);
        Assert.Equal("2026-01-22", data.Value[0].Ref);
        Assert.Equal("0.0", data.Value[0].Value);
        Assert.Equal("1.5", data.Value[1].Value);
        Assert.Equal(
            "https://opendata-download-metobs.smhi.se/api/version/latest/parameter/5/station/53430/period/latest-months/data.json",
            handler.LastRequestUri?.ToString());
    }

    private static SmhiClient CreateClient(HttpResponseMessage response, out StubHttpMessageHandler handler)
    {
        handler = new StubHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            // Mirrors the trailing-slash base address configured in Program.cs.
            BaseAddress = new Uri("https://opendata-download-metobs.smhi.se/api/version/latest/")
        };

        return new SmhiClient(httpClient);
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public StubHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        public Uri? LastRequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri;
            return Task.FromResult(_response);
        }
    }
}
