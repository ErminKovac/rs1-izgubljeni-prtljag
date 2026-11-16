namespace Market.Application.Modules.Locations.Countries.Commands.Update;

public sealed class UpdateCountryCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateCountryCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCountryCommand request, CancellationToken ct)
    {
        var country = await ctx.Countries
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (country is null)
            throw new MarketNotFoundException($"Država (ID={request.Id}) nije pronađena.");

        var name = request.Name.Trim();
        var isoCode = request.IsoCode.Trim().ToUpperInvariant();

        // Duplicates are checked against all other countries (x.Id != request.Id),
        // so saving a country without changes is not reported as a duplicate of itself.
        if (await ctx.Countries.AnyAsync(x => x.Id != request.Id && x.IsoCode == isoCode, ct))
            throw new MarketConflictException($"Država sa ISO oznakom {isoCode} već postoji.");

        var normalizedName = name.ToLower();
        if (await ctx.Countries.AnyAsync(x => x.Id != request.Id && x.Name.ToLower() == normalizedName, ct))
            throw new MarketConflictException($"Država sa nazivom {name} već postoji.");

        country.Name = name;
        country.IsoCode = isoCode;

        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
