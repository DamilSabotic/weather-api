using System.Globalization;
using CodeAssignmentTemplate.Clients;
using CodeAssignmentTemplate.Exceptions;
using CodeAssignmentTemplate.Models.Weather;

namespace CodeAssignmentTemplate.Services;

public class WeatherService : IWeatherService
{
    private readonly ISmhiClient _smhiClient;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(ISmhiClient smhiClient, ILogger<WeatherService> logger)
    {
        _smhiClient = smhiClient;
        _logger = logger;
    }

    public async Task<AverageTemperatureResponse> GetAverageTemperatureAsync(CancellationToken ct)
    {
        var data = await _smhiClient.GetLatestHourTemperaturesAsync(ct);

        var temperatures = new List<double>();
        foreach (var station in data.Station)
        {
            var reading = station.Value.FirstOrDefault();
            if (reading is null)
                continue;

            if (double.TryParse(reading.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var temperature))
                temperatures.Add(temperature);
            else
                _logger.LogWarning(
                    "Station {Key} returned non-numeric temperature value: {Value}",
                    station.Key, reading.Value);
        }

        if (temperatures.Count == 0)
            throw new SmhiUnavailableException(
                "SMHI returned no usable temperature observations for the latest hour.");

        return new AverageTemperatureResponse
        {
            AverageTemperatureInCelsius = temperatures.Average()
        };
    }

    public Task<RainfallResponse> GetRainfallAsync(string city, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
