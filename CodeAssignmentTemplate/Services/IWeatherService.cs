using CodeAssignmentTemplate.Models.Weather;

namespace CodeAssignmentTemplate.Services;

public interface IWeatherService
{
    /// <summary>
    /// Returns the unweighted arithmetic mean of all SMHI station air temperatures for the latest hour.
    /// Stations with missing or non-numeric values are excluded from the average.
    /// </summary>
    Task<AverageTemperatureResponse> GetAverageTemperatureAsync(CancellationToken ct);

    Task<RainfallResponse> GetRainfallAsync(string city, CancellationToken ct);
}
