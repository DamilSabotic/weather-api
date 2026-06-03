using CodeAssignmentTemplate.Clients;
using CodeAssignmentTemplate.Exceptions;
using CodeAssignmentTemplate.Models.Smhi;
using CodeAssignmentTemplate.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CodeAssignmentTemplate.Tests.Services;

public class WeatherServiceRainfallTests
{
    [Fact]
    public async Task GetRainfallAsync_SumsAndReportsSpanFromTheData()
    {
        var service = CreateService(
            Catalogue(Station(53430, "Lund", active: true)),
            Series(53430,
                ("2026-03-10", "1.5"),
                ("2026-02-01", "0.5"),
                ("2026-04-20", "2.0")));

        var response = await service.GetRainfallAsync("Lund", CancellationToken.None);

        Assert.Equal(4.0, response.RainfallInMillimeters);
        Assert.Equal("2026-02-01", response.StartDate);
        Assert.Equal("2026-04-20", response.EndDate);
    }

    [Fact]
    public async Task GetRainfallAsync_ExcludesEmptyAndNonNumericValues()
    {
        var service = CreateService(
            Catalogue(Station(53430, "Lund", active: true)),
            Series(53430,
                ("2026-03-01", "1.0"),
                ("2026-03-02", ""),
                ("2026-03-03", "not-a-number"),
                ("2026-03-04", "2.0")));

        var response = await service.GetRainfallAsync("Lund", CancellationToken.None);

        Assert.Equal(3.0, response.RainfallInMillimeters);
        Assert.Equal("2026-03-01", response.StartDate);
        Assert.Equal("2026-03-04", response.EndDate);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetRainfallAsync_ThrowsArgumentExceptionForBlankCity(string? city)
    {
        var service = CreateService(
            Catalogue(Station(53430, "Lund", active: true)),
            data: new Dictionary<int, SmhiStationData>());

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetRainfallAsync(city!, CancellationToken.None));
    }

    [Fact]
    public async Task GetRainfallAsync_ThrowsSmhiUnavailableWhenMatchedStationHasNoData()
    {
        var service = CreateService(
            Catalogue(Station(72630, "Göteborg", active: false)),
            data: new Dictionary<int, SmhiStationData>());

        await Assert.ThrowsAsync<SmhiUnavailableException>(
            () => service.GetRainfallAsync("Göteborg", CancellationToken.None));
    }

    [Fact]
    public async Task GetRainfallAsync_ResolvesGoteborgToActiveGoteborgA()
    {
        var service = CreateService(
            Catalogue(
                Station(72630, "Göteborg", active: false),
                Station(71420, "Göteborg A", active: true)),
            Series(71420,
                ("2026-03-01", "1.0"),
                ("2026-03-02", "2.0")));

        var response = await service.GetRainfallAsync("Göteborg", CancellationToken.None);

        Assert.Equal(3.0, response.RainfallInMillimeters);
        Assert.Equal("2026-03-01", response.StartDate);
        Assert.Equal("2026-03-02", response.EndDate);
    }

    private static WeatherService CreateService(
        SmhiRainfallStations catalogue,
        Dictionary<int, SmhiStationData> data)
    {
        return new WeatherService(
            new StubSmhiClient(catalogue, data),
            NullLogger<WeatherService>.Instance);
    }

    private static SmhiRainfallStations Catalogue(params SmhiRainfallStation[] stations) =>
        new() { Station = stations.ToList() };

    private static SmhiRainfallStation Station(int id, string name, bool active) =>
        new() { Id = id, Name = name, Active = active };

    private static Dictionary<int, SmhiStationData> Series(
        int stationId, params (string Ref, string Value)[] values)
    {
        var data = new SmhiStationData
        {
            Value = values
                .Select(v => new SmhiRainfallValue { Ref = v.Ref, Value = v.Value })
                .ToList()
        };

        return new Dictionary<int, SmhiStationData> { [stationId] = data };
    }

    private sealed class StubSmhiClient : ISmhiClient
    {
        private readonly SmhiRainfallStations _catalogue;
        private readonly Dictionary<int, SmhiStationData> _data;

        public StubSmhiClient(SmhiRainfallStations catalogue, Dictionary<int, SmhiStationData> data)
        {
            _catalogue = catalogue;
            _data = data;
        }

        public Task<SmhiStationSetData> GetLatestHourTemperaturesAsync(CancellationToken ct) =>
            throw new NotSupportedException();

        public Task<SmhiRainfallStations> GetRainfallStationsAsync(CancellationToken ct) =>
            Task.FromResult(_catalogue);

        public Task<SmhiStationData> GetLatestMonthsRainfallAsync(int stationId, CancellationToken ct) =>
            Task.FromResult(_data.TryGetValue(stationId, out var data) ? data : new SmhiStationData());
    }
}
