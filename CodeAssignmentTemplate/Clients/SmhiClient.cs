using System.Net;
using System.Net.Http.Json;
using CodeAssignmentTemplate.Models.Smhi;

namespace CodeAssignmentTemplate.Clients;

public class SmhiClient : ISmhiClient
{
    private const string LatestHourTemperaturesResource =
        "parameter/1/station-set/all/period/latest-hour/data.json";

    private const string RainfallStationsResource = "parameter/5.json";

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

    public async Task<SmhiRainfallStations> GetRainfallStationsAsync(CancellationToken ct)
    {
        var data = await _httpClient.GetFromJsonAsync<SmhiRainfallStations>(
            RainfallStationsResource, ct);

        return data ?? new SmhiRainfallStations();
    }

    public async Task<SmhiStationData> GetLatestMonthsRainfallAsync(int stationId, CancellationToken ct)
    {
        var resource = $"parameter/5/station/{stationId}/period/latest-months/data.json";

        try
        {
            var data = await _httpClient.GetFromJsonAsync<SmhiStationData>(resource, ct);
            return data ?? new SmhiStationData();
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            // A known station can still lack data for this period.
            return new SmhiStationData();
        }
    }
}
