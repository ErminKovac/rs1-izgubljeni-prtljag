namespace Market.Application.Modules.Locations.Airports.Queries.List;

public sealed class ListAirportsQuery : BasePagedQuery<ListAirportsQueryDto>
{
    /// <summary>Search by airport name, IATA code or city name (contains, case-insensitive). (optional)</summary>
    public string? Search { get; init; }

    /// <summary>Only airports in this country. (optional)</summary>
    public int? CountryId { get; init; }

    /// <summary>Only airports in this city. (optional)</summary>
    public int? CityId { get; init; }
}
