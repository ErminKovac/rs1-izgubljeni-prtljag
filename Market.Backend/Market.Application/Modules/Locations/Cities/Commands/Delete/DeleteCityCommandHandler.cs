namespace Market.Application.Modules.Locations.Cities.Commands.Delete;

public sealed class DeleteCityCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DeleteCityCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCityCommand request, CancellationToken ct)
    {
        var city = await ctx.Cities
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (city is null)
            throw new MarketNotFoundException($"Grad (ID={request.Id}) nije pronađen.");

        // Same rule as for countries: soft delete is an UPDATE, so the FK (Restrict) does not run.
        var hasAirports = await ctx.Airports
            .AnyAsync(a => a.CityId == city.Id, ct);

        if (hasAirports)
            throw new MarketConflictException(
                $"Grad {city.Name} se ne može obrisati jer ima aerodrome. Prvo obrišite njegove aerodrome.");

        ctx.Cities.Remove(city);
        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
