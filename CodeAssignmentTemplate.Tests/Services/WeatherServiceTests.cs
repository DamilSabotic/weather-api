using System.Globalization;
using CodeAssignmentTemplate.Clients;
using CodeAssignmentTemplate.Exceptions;
using CodeAssignmentTemplate.Models.Smhi;
using CodeAssignmentTemplate.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CodeAssignmentTemplate.Tests.Services;

public class WeatherServiceTests
{
    [Fact]
    public async Task GetAverageTemperatureAsync_ReturnsAverageOfValidStationReadings()
    {
        var service = CreateService(Data(
            Station("station-1", "10.5"),
            Station("station-2", "20.5"),
            Station("station-3", "14.0")));

        var response = await service.GetAverageTemperatureAsync(CancellationToken.None);

        Assert.Equal(15.0, response.AverageTemperatureInCelsius);
    }

    [Fact]
    public async Task GetAverageTemperatureAsync_UsesFirstReadingPerStation()
    {
        var service = CreateService(Data(
            Station("station-1", "10.0", "999.0"),
            Station("station-2", "20.0", "999.0")));

        var response = await service.GetAverageTemperatureAsync(CancellationToken.None);

        Assert.Equal(15.0, response.AverageTemperatureInCelsius);
    }

    [Fact]
    public async Task GetAverageTemperatureAsync_IgnoresEmptyAndNonNumericReadings()
    {
        var service = CreateService(Data(
            Station("station-1", "10.0"),
            Station("station-2"),
            Station("station-3", "not-a-number"),
            Station("station-4", "20.0")));

        var response = await service.GetAverageTemperatureAsync(CancellationToken.None);

        Assert.Equal(15.0, response.AverageTemperatureInCelsius);
    }

    [Fact]
    public async Task GetAverageTemperatureAsync_ThrowsWhenNoUsableReadingsExist()
    {
        var service = CreateService(Data(
            Station("station-1"),
            Station("station-2", "not-a-number")));

        await Assert.ThrowsAsync<SmhiUnavailableException>(
            () => service.GetAverageTemperatureAsync(CancellationToken.None));
    }

    [Fact]
    public async Task GetAverageTemperatureAsync_ThrowsWhenStationListIsEmpty()
    {
        var service = CreateService(Data());

        await Assert.ThrowsAsync<SmhiUnavailableException>(
            () => service.GetAverageTemperatureAsync(CancellationToken.None));
    }

    [Fact]
    public async Task GetAverageTemperatureAsync_ParsesDecimalsUnderCommaDecimalCulture()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("sv-SE");
            var service = CreateService(Data(
                Station("station-1", "10.5"),
                Station("station-2", "20.5")));

            var response = await service.GetAverageTemperatureAsync(CancellationToken.None);

            Assert.Equal(15.5, response.AverageTemperatureInCelsius);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    private static WeatherService CreateService(SmhiStationSetData data)
    {
        return new WeatherService(
            new FakeSmhiClient(data),
            NullLogger<WeatherService>.Instance);
    }

    private static SmhiStationSetData Data(params SmhiStation[] stations)
    {
        return new SmhiStationSetData
        {
            Station = stations.ToList()
        };
    }

    private static SmhiStation Station(string key, params string[] values)
    {
        return new SmhiStation
        {
            Key = key,
            Value = values
                .Select(value => new SmhiStationValue { Value = value })
                .ToList()
        };
    }

    private sealed class FakeSmhiClient : ISmhiClient
    {
        private readonly SmhiStationSetData _data;

        public FakeSmhiClient(SmhiStationSetData data)
        {
            _data = data;
        }

        public Task<SmhiStationSetData> GetLatestHourTemperaturesAsync(CancellationToken ct)
        {
            return Task.FromResult(_data);
        }
    }
}
