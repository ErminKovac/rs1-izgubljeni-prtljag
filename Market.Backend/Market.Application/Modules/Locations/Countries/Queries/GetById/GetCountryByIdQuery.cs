namespace Market.Application.Modules.Locations.Countries.Queries.GetById;

public sealed class GetCountryByIdQuery : IRequest<GetCountryByIdQueryDto>
{
    public int Id { get; set; }
}
