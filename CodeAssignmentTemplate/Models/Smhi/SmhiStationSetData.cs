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
    [JsonPropertyName("date")]
    public long Date { get; init; }

    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;

    [JsonPropertyName("quality")]
    public string Quality { get; init; } = string.Empty;
}
