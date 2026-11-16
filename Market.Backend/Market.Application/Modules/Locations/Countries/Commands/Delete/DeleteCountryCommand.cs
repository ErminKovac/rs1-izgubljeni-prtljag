namespace Market.Application.Modules.Locations.Countries.Commands.Delete;

public sealed class DeleteCountryCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
