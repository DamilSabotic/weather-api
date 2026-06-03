using CodeAssignmentTemplate.Models.Weather;

namespace CodeAssignmentTemplate.Services;

public interface IWeatherService
{
    Task<AverageTemperatureResponse> GetAverageTemperatureAsync(CancellationToken ct);

    Task<RainfallResponse> GetRainfallAsync(string city, CancellationToken ct);
}
