using System.Text;
using CodeAssignmentTemplate.Exceptions;
using CodeAssignmentTemplate.Models.Smhi;

namespace CodeAssignmentTemplate.Services;

public static class StationResolver
{
    public static SmhiRainfallStation Resolve(SmhiRainfallStations catalogue, string city)
    {
        var normalizedQuery = NormalizeForComparison(city);

        // Active-first: SMHI keeps the live automatic station under a " A" suffix while the bare-name
        // predecessor is inactive with no recent data, so an active partial match beats an inactive exact.
        var station = catalogue.Station
            .Select(s => new { Station = s, NormalizedName = NormalizeForComparison(s.Name) })
            .Where(candidate => IsWordBoundaryMatch(candidate.NormalizedName, normalizedQuery))
            .OrderByDescending(candidate => candidate.Station.Active)
            .ThenByDescending(candidate => candidate.NormalizedName == normalizedQuery)
            .ThenBy(candidate => candidate.Station.Id)
            .Select(candidate => candidate.Station)
            .FirstOrDefault();

        if (station is null)
            throw new CityNotFoundException(city,
                $"No SMHI station found for '{city}'. Provide a Swedish city name that has an SMHI " +
                "rainfall station (e.g. Göteborg, Malmö, Stockholm).");

        return station;
    }

    private static bool IsWordBoundaryMatch(string normalizedName, string normalizedQuery)
    {
        if (normalizedName == normalizedQuery)
            return true;

        if (!normalizedName.StartsWith(normalizedQuery, StringComparison.Ordinal))
            return false;

        var boundary = normalizedName[normalizedQuery.Length];
        return boundary is ' ' or '-';
    }

    private static string NormalizeForComparison(string value)
    {
        var lowered = value.ToLowerInvariant();
        var builder = new StringBuilder(lowered.Length);
        foreach (var c in lowered)
        {
            builder.Append(c switch
            {
                'å' or 'ä' => 'a',
                'ö' => 'o',
                'é' => 'e',
                _ => c
            });
        }

        return builder.ToString();
    }
}
