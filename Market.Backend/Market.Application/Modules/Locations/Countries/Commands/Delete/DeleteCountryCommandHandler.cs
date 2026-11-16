namespace Market.Application.Modules.Locations.Countries.Commands.Delete;

public sealed class DeleteCountryCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DeleteCountryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCountryCommand request, CancellationToken ct)
    {
        var country = await ctx.Countries
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (country is null)
            throw new MarketNotFoundException($"Država (ID={request.Id}) nije pronađena.");

        // Soft delete never reaches the database as DELETE, so the FK (Restrict) cannot protect us here.
        // The rule has to be checked in code: a country that still has (non-deleted) cities cannot be deleted.
        var hasCities = await ctx.Cities
            .AnyAsync(c => c.CountryId == country.Id, ct);

        if (hasCities)
            throw new MarketConflictException(
                $"Država {country.Name} se ne može obrisati jer ima gradove. Prvo obrišite njene gradove.");

        // Remove → DatabaseContext turns it into a soft delete (IsDeleted = true).
        ctx.Countries.Remove(country);
        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
