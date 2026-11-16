namespace Market.Application.Modules.Locations.Countries.Queries.GetById;

public sealed class GetCountryByIdQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string IsoCode { get; init; }
}
