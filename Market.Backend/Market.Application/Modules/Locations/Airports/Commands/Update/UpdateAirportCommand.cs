namespace Market.Application.Modules.Locations.Airports.Commands.Update;

public sealed class UpdateAirportCommand : IRequest<Unit>
{
    /// <summary>Taken from the route (PUT /Airports/{id}), not from the body.</summary>
    [JsonIgnore]
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string IataCode { get; set; }

    public required int CityId { get; set; }
}
