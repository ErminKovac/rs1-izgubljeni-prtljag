namespace Market.Application.Modules.Locations.Cities.Queries.List;

public sealed class ListCitiesQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? PostalCode { get; init; }
    public required int CountryId { get; init; }
    public required string CountryName { get; init; }
    public required string CountryIsoCode { get; init; }

    /// <summary>Number of (non-deleted) airports; a city with airports cannot be deleted.</summary>
    public required int AirportsCount { get; init; }
}
