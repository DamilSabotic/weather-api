using System.Globalization;
using CodeAssignmentTemplate.Clients;
using CodeAssignmentTemplate.Exceptions;
using CodeAssignmentTemplate.Models.Smhi;
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
        var temperatureData = await _smhiClient.GetLatestHourTemperaturesAsync(ct);

        var temperatures = new List<double>(temperatureData.Station.Count);
        foreach (var station in temperatureData.Station)
        {
            var reading = station.Value.MaxBy(v => v.Date);
            if (reading is null)
                continue;

            if (TryParseSmhiValue(reading.Value, out var temperature))
                temperatures.Add(temperature);
            else
                _logger.LogWarning(
                    "Station {Key} returned non-numeric temperature value: {Value}",
                    station.Key, reading.Value);
        }

        // A 200 response with no usable data is treated as an unusable upstream response -> 502.
        if (temperatures.Count == 0)
            throw new SmhiUnavailableException(
                "SMHI returned no usable temperature observations for the latest hour.");

        return new AverageTemperatureResponse
        {
            AverageTemperatureInCelsius = temperatures.Average()
        };
    }

    public async Task<RainfallResponse> GetRainfallAsync(string city, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City must not be empty.", nameof(city));

        var trimmedCity = city.Trim();
        var catalogue = await _smhiClient.GetRainfallStationsAsync(ct);
        if (catalogue.Station.Count == 0)
            throw new SmhiUnavailableException("SMHI returned an empty precipitation station catalogue.");

        var station = StationResolver.Resolve(catalogue, trimmedCity);

        _logger.LogInformation(
            "Resolved city '{City}' to SMHI station {StationName} (id {StationId}).",
            trimmedCity, station.Name, station.Id);

        var rainfallData = await _smhiClient.GetLatestMonthsRainfallAsync(station.Id, ct);
        var observations = ExtractValidObservations(rainfallData);

        if (observations.Count == 0)
            throw new SmhiUnavailableException(
                $"SMHI returned no usable rainfall data for '{trimmedCity}' in the requested period.");

        return new RainfallResponse
        {
            RainfallInMillimeters = observations.Sum(o => o.Millimeters),
            StartDate = observations.Min(o => o.Day).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            EndDate = observations.Max(o => o.Day).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        };
    }

    // Values arrive as InvariantCulture strings; we keep numeric ones and skip null/empty/non-numeric.
    // The quality code is intentionally not filtered (see doc/DECISIONS.md).
    private static List<(DateOnly Day, double Millimeters)> ExtractValidObservations(SmhiStationData rainfallData)
    {
        var observations = new List<(DateOnly Day, double Millimeters)>(rainfallData.Value.Count);
        foreach (var observation in rainfallData.Value)
        {
            if (!TryParseSmhiValue(observation.Value, out var millimeters))
                continue;

            if (!TryParseSmhiDateRef(observation.Ref, out var day))
                continue;

            observations.Add((day, millimeters));
        }

        return observations;
    }

    private static bool TryParseSmhiValue(string value, out double parsed) =>
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed);

    private static bool TryParseSmhiDateRef(string dateRef, out DateOnly day) =>
        DateOnly.TryParseExact(dateRef, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out day);
}
