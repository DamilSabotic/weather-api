using System.Text.Json.Serialization;

namespace CodeAssignmentTemplate.Models.Smhi;

public class SmhiStationSetData
{
    [JsonPropertyName("station")]
    public List<SmhiStation> Station { get; init; } = [];
}

public class SmhiStation
{
    [JsonPropertyName("key")]
    public string Key { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("value")]
    public List<SmhiStationValue> Value { get; init; } = [];
}

public class SmhiStationValue
{
    /// <summary>Observation timestamp in epoch milliseconds.</summary>
    [JsonPropertyName("date")]
    public long Date { get; init; }

    /// <summary>Temperature as a string, e.g. "10.9".</summary>
    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;

    [JsonPropertyName("quality")]
    public string Quality { get; init; } = string.Empty;
}
