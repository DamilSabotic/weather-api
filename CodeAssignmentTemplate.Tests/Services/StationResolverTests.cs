using CodeAssignmentTemplate.Exceptions;
using CodeAssignmentTemplate.Models.Smhi;
using CodeAssignmentTemplate.Services;
using Xunit;

namespace CodeAssignmentTemplate.Tests.Services;

public class StationResolverTests
{
    [Fact]
    public void Resolve_PrefersActivePartialOverInactiveExact()
    {
        var catalogue = Catalogue(
            Station(72630, "Göteborg", active: false),
            Station(71420, "Göteborg A", active: true));

        var station = StationResolver.Resolve(catalogue, "Göteborg");

        Assert.Equal(71420, station.Id);
    }

    [Fact]
    public void Resolve_PrefersExactOverPartialAmongActiveStations()
    {
        var catalogue = Catalogue(
            Station(100, "Lund Test", active: true),
            Station(200, "Lund", active: true));

        var station = StationResolver.Resolve(catalogue, "Lund");

        Assert.Equal(200, station.Id);
    }

    [Fact]
    public void Resolve_BreaksRemainingTiesByLowestId()
    {
        var catalogue = Catalogue(
            Station(900, "Boras", active: true),
            Station(300, "Boras", active: true));

        var station = StationResolver.Resolve(catalogue, "Boras");

        Assert.Equal(300, station.Id);
    }

    [Fact]
    public void Resolve_FoldsDiacriticsWhenMatching()
    {
        var catalogue = Catalogue(Station(71420, "Göteborg A", active: true));

        var station = StationResolver.Resolve(catalogue, "Goteborg");

        Assert.Equal(71420, station.Id);
    }

    [Fact]
    public void Resolve_ThrowsCityNotFoundWhenNoStationMatches()
    {
        var catalogue = Catalogue(Station(53430, "Lund", active: true));

        Assert.Throws<CityNotFoundException>(() => StationResolver.Resolve(catalogue, "Atlantis"));
    }

    [Theory]
    [InlineData("Boda", 1, "Boda", 2, "Långsboda")]
    [InlineData("Berg", 1, "Berg", 2, "Degeberga D")]
    [InlineData("Ed", 1, "Ed", 2, "Louisefred")]
    public void Resolve_IgnoresInteriorSubstringMatches(
        string query, int expectedId, string exactName, int decoyId, string decoyName)
    {
        var catalogue = Catalogue(
            Station(decoyId, decoyName, active: true),
            Station(expectedId, exactName, active: false));

        var station = StationResolver.Resolve(catalogue, query);

        Assert.Equal(expectedId, station.Id);
    }

    [Fact]
    public void Resolve_ThrowsWhenOnlyInteriorSubstringMatchExists()
    {
        var catalogue = Catalogue(Station(100, "Långsboda", active: true));

        Assert.Throws<CityNotFoundException>(() => StationResolver.Resolve(catalogue, "Boda"));
    }

    [Fact]
    public void Resolve_MatchesHyphenatedSuccessorSiteNames()
    {
        var catalogue = Catalogue(Station(53300, "Malmö-Sturup Flygplats", active: true));

        var station = StationResolver.Resolve(catalogue, "Malmö");

        Assert.Equal(53300, station.Id);
    }

    private static SmhiRainfallStations Catalogue(params SmhiRainfallStation[] stations) =>
        new() { Station = stations.ToList() };

    private static SmhiRainfallStation Station(int id, string name, bool active) =>
        new() { Id = id, Name = name, Active = active };
}
