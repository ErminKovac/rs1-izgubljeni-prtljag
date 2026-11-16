namespace Market.Application.Modules.Locations.Airports.Queries.List;

public sealed class ListAirportsQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string IataCode { get; init; }
    public required int CityId { get; init; }
    public required string CityName { get; init; }
    public required int CountryId { get; init; }
    public required string CountryName { get; init; }
}
