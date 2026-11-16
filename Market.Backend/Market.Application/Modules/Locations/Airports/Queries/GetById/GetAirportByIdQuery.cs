namespace Market.Application.Modules.Locations.Airports.Queries.GetById;

public sealed class GetAirportByIdQuery : IRequest<GetAirportByIdQueryDto>
{
    public int Id { get; set; }
}
