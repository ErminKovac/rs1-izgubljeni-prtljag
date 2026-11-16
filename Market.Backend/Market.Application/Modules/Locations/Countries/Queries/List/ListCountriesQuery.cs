namespace Market.Application.Modules.Locations.Countries.Queries.List;

public sealed class ListCountriesQuery : BasePagedQuery<ListCountriesQueryDto>
{
    /// <summary>Search by name or ISO code (contains, case-insensitive). (optional)</summary>
    public string? Search { get; init; }
}
