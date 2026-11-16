namespace Market.Application.Modules.Locations.Cities.Queries.GetById;

public sealed class GetCityByIdQuery : IRequest<GetCityByIdQueryDto>
{
    public int Id { get; set; }
}
