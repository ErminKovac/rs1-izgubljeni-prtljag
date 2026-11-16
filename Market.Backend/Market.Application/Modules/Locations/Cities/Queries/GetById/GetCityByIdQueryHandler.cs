namespace Market.Application.Modules.Locations.Cities.Queries.GetById;

public sealed class GetCityByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetCityByIdQuery, GetCityByIdQueryDto>
{
    public async Task<GetCityByIdQueryDto> Handle(GetCityByIdQuery request, CancellationToken ct)
    {
        var city = await ctx.Cities
            .Where(x => x.Id == request.Id)
            .Select(x => new GetCityByIdQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                PostalCode = x.PostalCode,
                CountryId = x.CountryId,
                // navigation in Select → EF Core generates a JOIN, no Include needed
                CountryName = x.Country!.Name
            })
            .FirstOrDefaultAsync(ct);

        if (city is null)
            throw new MarketNotFoundException($"Grad (ID={request.Id}) nije pronađen.");

        return city;
    }
}
