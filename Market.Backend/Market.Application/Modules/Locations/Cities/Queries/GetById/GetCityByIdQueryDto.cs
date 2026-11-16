namespace Market.Application.Modules.Locations.Cities.Queries.GetById;

public sealed class GetCityByIdQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? PostalCode { get; init; }
    public required int CountryId { get; init; }
    public required string CountryName { get; init; }
}
