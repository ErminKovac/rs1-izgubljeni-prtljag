namespace Market.Application.Modules.Locations.Cities.Commands.Create;

public sealed class CreateCityCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateCityCommand, int>
{
    public async Task<int> Handle(CreateCityCommand request, CancellationToken ct)
    {
        // Rules that need the database: missing country → 404, same name in the same country → 409.
        var countryExists = await ctx.Countries.AnyAsync(x => x.Id == request.CountryId, ct);
        if (!countryExists)
            throw new MarketNotFoundException($"Država (ID={request.CountryId}) nije pronađena.");

        var name = request.Name.Trim();
        var normalizedName = name.ToLower();

        var exists = await ctx.Cities
            .AnyAsync(x => x.CountryId == request.CountryId && x.Name.ToLower() == normalizedName, ct);

        if (exists)
            throw new MarketConflictException($"Grad {name} već postoji u odabranoj državi.");

        var city = new CityEntity
        {
            Name = name,
            // an empty form field is stored as null (no value), not as ""
            PostalCode = request.PostalCode.TrimToNull(),
            CountryId = request.CountryId
        };

        ctx.Cities.Add(city);
        await ctx.SaveChangesAsync(ct);

        return city.Id;
    }
}
