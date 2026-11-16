namespace Market.Application.Modules.Locations.Countries.Queries.List;

public sealed class ListCountriesQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string IsoCode { get; init; }

    /// <summary>Number of (non-deleted) cities; a country with cities cannot be deleted.</summary>
    public required int CitiesCount { get; init; }
}
