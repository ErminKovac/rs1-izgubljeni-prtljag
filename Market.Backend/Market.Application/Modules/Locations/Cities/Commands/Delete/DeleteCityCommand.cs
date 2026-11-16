namespace Market.Application.Modules.Locations.Cities.Commands.Delete;

public sealed class DeleteCityCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
