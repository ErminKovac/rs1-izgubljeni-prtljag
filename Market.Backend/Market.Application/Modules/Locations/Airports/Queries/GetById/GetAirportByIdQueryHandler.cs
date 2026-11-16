namespace Market.Application.Modules.Locations.Airports.Queries.GetById;

public sealed class GetAirportByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetAirportByIdQuery, GetAirportByIdQueryDto>
{
    public async Task<GetAirportByIdQueryDto> Handle(GetAirportByIdQuery request, CancellationToken ct)
    {
        var airport = await ctx.Airports
            .Where(x => x.Id == request.Id)
            .Select(x => new GetAirportByIdQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                IataCode = x.IataCode,
                CityId = x.CityId,
                CityName = x.City!.Name,
                CountryId = x.City!.CountryId,
                CountryName = x.City!.Country!.Name
            })
            .FirstOrDefaultAsync(ct);

        if (airport is null)
            throw new MarketNotFoundException($"Aerodrom (ID={request.Id}) nije pronađen.");

        return airport;
    }
}
