using CodeAssignmentTemplate.Models.Weather;
using CodeAssignmentTemplate.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeAssignmentTemplate.Controllers;

[ApiController]
[Route("api/weather")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    /// <summary>
    /// Returns the unweighted average air temperature across all Swedish SMHI stations
    /// for the latest hour.
    /// </summary>
    [HttpGet("temperature")]
    [ProducesResponseType(typeof(AverageTemperatureResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<AverageTemperatureResponse>> GetAverageTemperature(
        CancellationToken ct)
    {
        return Ok(await _weatherService.GetAverageTemperatureAsync(ct));
    }

    /// <summary>
    /// Returns total rainfall in millimetres for the given Swedish city over the last months,
    /// along with the start and end dates of the measured period.
    /// Returns 404 if no SMHI station matches the city name.
    /// </summary>
    [HttpGet("rainfall/{city}")]
    [ProducesResponseType(typeof(RainfallResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status501NotImplemented)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<RainfallResponse>> GetCityRainfall(
        string city, CancellationToken ct)
    {
        return Ok(await _weatherService.GetRainfallAsync(city, ct));
    }
}
