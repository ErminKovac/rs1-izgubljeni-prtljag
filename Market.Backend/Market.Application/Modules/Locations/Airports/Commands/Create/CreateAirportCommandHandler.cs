namespace Market.Application.Modules.Locations.Airports.Commands.Create;

public sealed class CreateAirportCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateAirportCommand, int>
{
    public async Task<int> Handle(CreateAirportCommand request, CancellationToken ct)
    {
        // Rules that need the database: missing city → 404, IATA code already used → 409.
        var cityExists = await ctx.Cities.AnyAsync(x => x.Id == request.CityId, ct);
        if (!cityExists)
            throw new MarketNotFoundException($"Grad (ID={request.CityId}) nije pronađen.");

        var iataCode = request.IataCode.Trim().ToUpperInvariant();

        if (await ctx.Airports.AnyAsync(x => x.IataCode == iataCode, ct))
            throw new MarketConflictException($"Aerodrom sa IATA oznakom {iataCode} već postoji.");

        var airport = new AirportEntity
        {
            Name = request.Name.Trim(),
            IataCode = iataCode,
            CityId = request.CityId
        };

        ctx.Airports.Add(airport);
        await ctx.SaveChangesAsync(ct);

        return airport.Id;
    }
}
