using System.Threading.Tasks;

namespace CodeAssignmentTemplate;

public interface IWeatherService
{
	Task<double> GetAverageTemperatureAsync(string city);

	Task<double> GetRainFallAsync(string city);
}