using System.Text.Json.Serialization;

namespace CodeAssignmentTemplate.Models.Smhi;

public class SmhiRainfallStations
{
    [JsonPropertyName("station")]
    public List<SmhiRainfallStation> Station { get; init; } = [];
}

public class SmhiRainfallStation
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("active")]
    public bool Active { get; init; }
}
