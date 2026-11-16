namespace Market.Application.Modules.Locations.Airports.Commands.Update;

public sealed class UpdateAirportCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateAirportCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAirportCommand request, CancellationToken ct)
    {
        var airport = await ctx.Airports
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (airport is null)
            throw new MarketNotFoundException($"Aerodrom (ID={request.Id}) nije pronađen.");

        var cityExists = await ctx.Cities.AnyAsync(x => x.Id == request.CityId, ct);
        if (!cityExists)
            throw new MarketNotFoundException($"Grad (ID={request.CityId}) nije pronađen.");

        var iataCode = request.IataCode.Trim().ToUpperInvariant();

        if (await ctx.Airports.AnyAsync(x => x.Id != request.Id && x.IataCode == iataCode, ct))
            throw new MarketConflictException($"Aerodrom sa IATA oznakom {iataCode} već postoji.");

        airport.Name = request.Name.Trim();
        airport.IataCode = iataCode;
        airport.CityId = request.CityId;

        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
