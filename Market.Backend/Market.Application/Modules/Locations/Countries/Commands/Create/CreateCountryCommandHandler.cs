namespace Market.Application.Modules.Locations.Countries.Commands.Create;

public sealed class CreateCountryCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateCountryCommand, int>
{
    public async Task<int> Handle(CreateCountryCommand request, CancellationToken ct)
    {
        // Input format (required name, 2-letter code) is checked by CreateCountryCommandValidator.
        // Rules that need the database are checked here: duplicate code or name → 409.
        var name = request.Name.Trim();
        var isoCode = request.IsoCode.Trim().ToUpperInvariant();

        if (await ctx.Countries.AnyAsync(x => x.IsoCode == isoCode, ct))
            throw new MarketConflictException($"Država sa ISO oznakom {isoCode} već postoji.");

        // Case-insensitive, like the unique index (default SQL Server collation).
        var normalizedName = name.ToLower();
        if (await ctx.Countries.AnyAsync(x => x.Name.ToLower() == normalizedName, ct))
            throw new MarketConflictException($"Država sa nazivom {name} već postoji.");

        var country = new CountryEntity
        {
            Name = name,
            IsoCode = isoCode
        };

        ctx.Countries.Add(country);
        await ctx.SaveChangesAsync(ct);

        return country.Id;
    }
}
