namespace Market.Application.Modules.Locations.Countries.Queries.GetById;

public sealed class GetCountryByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetCountryByIdQuery, GetCountryByIdQueryDto>
{
    public async Task<GetCountryByIdQueryDto> Handle(GetCountryByIdQuery request, CancellationToken ct)
    {
        // Select → DTO: the entity itself is never returned through the API.
        var country = await ctx.Countries
            .Where(x => x.Id == request.Id)
            .Select(x => new GetCountryByIdQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                IsoCode = x.IsoCode
            })
            .FirstOrDefaultAsync(ct);

        if (country is null)
            throw new MarketNotFoundException($"Država (ID={request.Id}) nije pronađena.");

        return country;
    }
}
