namespace Market.Application.Modules.Locations.Airports.Queries.GetById;

public sealed class GetAirportByIdQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string IataCode { get; init; }
    public required int CityId { get; init; }
    public required string CityName { get; init; }
    public required int CountryId { get; init; }
    public required string CountryName { get; init; }
}
