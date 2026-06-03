using System.ComponentModel.DataAnnotations;

namespace CodeAssignmentTemplate.Configuration;

public class WeatherApiOptions
{
    [Required]
    public string SmhiBaseUrl { get; set; } = string.Empty;
}
