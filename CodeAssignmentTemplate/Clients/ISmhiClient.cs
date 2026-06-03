using CodeAssignmentTemplate.Models.Smhi;

namespace CodeAssignmentTemplate.Clients;

public interface ISmhiClient
{
    Task<SmhiStationSetData> GetLatestHourTemperaturesAsync(CancellationToken ct);
}
