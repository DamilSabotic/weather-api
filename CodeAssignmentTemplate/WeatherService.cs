using System;
using System.Threading.Tasks;

namespace CodeAssignmentTemplate;

public class WeatherService : IWeatherService
{
	// You can use this class to implement the IWeatherService interface and provide weather-related functionality.
	// Learn more about dependency injection and how to register services in the Program.cs file of an ASP.NET Core application.
	// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0
	public async Task<double> GetAverageTemperatureAsync(string city)
	{
		// TODO Your code here!
		throw new NotImplementedException();
	}

	public async Task<double> GetRainFallAsync(string city)
	{
		// TODO Your code here!
		throw new NotImplementedException();
	}
}