namespace CodeAssignmentTemplate.Models.Weather;

public class RainfallResponse
{
    public double RainfallInMillimeters { get; init; }

    /// <summary>First day of the summed period, formatted yyyy-MM-dd.</summary>
    public string StartDate { get; init; } = string.Empty;

    /// <summary>Last day of the summed period, formatted yyyy-MM-dd.</summary>
    public string EndDate { get; init; } = string.Empty;
}
