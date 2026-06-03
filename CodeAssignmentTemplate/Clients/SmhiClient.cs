using System.Net.Http.Json;
using CodeAssignmentTemplate.Models.Smhi;

namespace CodeAssignmentTemplate.Clients;

public class SmhiClient : ISmhiClient
{
    // parameter/1 = air temperature; station-set/all = latest reading from every station.
    private const string LatestHourTemperaturesResource =
        "parameter/1/station-set/all/period/latest-hour/data.json";

    private readonly HttpClient _httpClient;

    public SmhiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SmhiStationSetData> GetLatestHourTemperaturesAsync(CancellationToken ct)
    {
        var data = await _httpClient.GetFromJsonAsync<SmhiStationSetData>(
            LatestHourTemperaturesResource, ct);

        return data ?? new SmhiStationSetData();
    }
}
