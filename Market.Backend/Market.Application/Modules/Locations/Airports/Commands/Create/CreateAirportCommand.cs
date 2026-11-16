namespace Market.Application.Modules.Locations.Airports.Commands.Create;

public sealed class CreateAirportCommand : IRequest<int>
{
    /// <summary>Display name, e.g. "Međunarodni aerodrom Sarajevo".</summary>
    public required string Name { get; set; }

    /// <summary>IATA code, e.g. "SJJ" (case-insensitive, stored in upper case).</summary>
    public required string IataCode { get; set; }

    /// <summary>City the airport is in.</summary>
    public required int CityId { get; set; }
}
