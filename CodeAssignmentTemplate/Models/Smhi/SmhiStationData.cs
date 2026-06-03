using System.Text.Json.Serialization;

namespace CodeAssignmentTemplate.Models.Smhi;

public class SmhiStationData
{
    [JsonPropertyName("value")]
    public List<SmhiRainfallValue> Value { get; init; } = [];
}

public class SmhiRainfallValue
{
    [JsonPropertyName("ref")]
    public string Ref { get; init; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;

}
