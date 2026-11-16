namespace Market.Application.Modules.Locations.Cities.Queries.List;

public sealed class ListCitiesQuery : BasePagedQuery<ListCitiesQueryDto>
{
    /// <summary>Search by name or postal code (contains, case-insensitive). (optional)</summary>
    public string? Search { get; init; }

    /// <summary>Only cities of this country. (optional)</summary>
    public int? CountryId { get; init; }
}
