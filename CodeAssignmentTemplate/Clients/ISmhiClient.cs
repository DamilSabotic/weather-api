using CodeAssignmentTemplate.Models.Smhi;

namespace CodeAssignmentTemplate.Clients;

public interface ISmhiClient
{
    Task<SmhiStationSetData> GetLatestHourTemperaturesAsync(CancellationToken ct);

    Task<SmhiRainfallStations> GetRainfallStationsAsync(CancellationToken ct);

    Task<SmhiStationData> GetLatestMonthsRainfallAsync(int stationId, CancellationToken ct);
}
