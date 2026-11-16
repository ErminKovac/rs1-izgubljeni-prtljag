namespace Market.Application.Modules.Locations.Airports.Commands.Delete;

public sealed class DeleteAirportCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
