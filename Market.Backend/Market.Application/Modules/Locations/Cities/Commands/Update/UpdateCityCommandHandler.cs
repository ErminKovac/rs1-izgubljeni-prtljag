namespace Market.Application.Modules.Locations.Cities.Commands.Update;

public sealed class UpdateCityCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateCityCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCityCommand request, CancellationToken ct)
    {
        var city = await ctx.Cities
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (city is null)
            throw new MarketNotFoundException($"Grad (ID={request.Id}) nije pronađen.");

        // The city may be moved to another country, so the (new) country must exist.
        var countryExists = await ctx.Countries.AnyAsync(x => x.Id == request.CountryId, ct);
        if (!countryExists)
            throw new MarketNotFoundException($"Država (ID={request.CountryId}) nije pronađena.");

        var name = request.Name.Trim();
        var normalizedName = name.ToLower();

        var exists = await ctx.Cities
            .AnyAsync(x => x.Id != request.Id
                           && x.CountryId == request.CountryId
                           && x.Name.ToLower() == normalizedName, ct);

        if (exists)
            throw new MarketConflictException($"Grad {name} već postoji u odabranoj državi.");

        city.Name = name;
        city.PostalCode = request.PostalCode.TrimToNull();
        city.CountryId = request.CountryId;

        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
