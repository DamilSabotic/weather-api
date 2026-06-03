namespace CodeAssignmentTemplate.Models.Weather;

public class RainfallResponse
{
    public double RainfallInMillimeters { get; init; }

    public string StartDate { get; init; } = string.Empty;

    public string EndDate { get; init; } = string.Empty;
}
