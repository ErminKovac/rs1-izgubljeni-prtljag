namespace Market.Application.Modules.Locations.Airports.Commands.Delete;

public sealed class DeleteAirportCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DeleteAirportCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAirportCommand request, CancellationToken ct)
    {
        var airport = await ctx.Airports
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (airport is null)
            throw new MarketNotFoundException($"Aerodrom (ID={request.Id}) nije pronađen.");

        // Nothing references airports yet. When flights (#4) and airport employees (#5) are added,
        // this handler gets the same check as DeleteCityCommandHandler (airport in use → 409).
        ctx.Airports.Remove(airport);
        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
