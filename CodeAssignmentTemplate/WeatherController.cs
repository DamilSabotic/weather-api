using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CodeAssignmentTemplate;

[Route("weather-service")]
[ApiController]
public static class WeatherController
{
	public static RouteGroupBuilder RegisterWeatherApi(this RouteGroupBuilder routeGroup)
	{
		var weatherEndpoint = routeGroup.MapGroup("weather-service").WithTags("Weather Service");

		weatherEndpoint
			.MapGet("lund/average-temperature", GetAverageTemperature)
			.WithName("GetAverageTemperature")
			.WithSummary("Get the average temperature for Lund")
			.WithDescription("Get the average temperature for Lund for the last 24 hours");

		weatherEndpoint
			.MapGet("lund/rainfall", GetCityRainfall)
			.WithName("GetCityRainfall")
			.WithSummary("Get the rainfall for Lund")
			.WithDescription("Get the rainfall for Lund for the last 24 hours");

		return weatherEndpoint;
	}

	private static async Task<Results<StatusCodeHttpResult, Ok<double>>> GetAverageTemperature(IWeatherService weatherService)
	{
		try
		{
			return TypedResults.Ok(await weatherService.GetAverageTemperatureAsync("lund"));

		}
		catch (NotImplementedException)
		{
			return TypedResults.StatusCode(501);
		}
	}

	private static async Task<Results<StatusCodeHttpResult, Ok<double>>> GetCityRainfall(
		IWeatherService weatherService)
	{
		try
		{
			return TypedResults.Ok(await weatherService.GetRainFallAsync("lund"));
		}
		catch (NotImplementedException)
		{
			return TypedResults.StatusCode(501);
		}
	}
}