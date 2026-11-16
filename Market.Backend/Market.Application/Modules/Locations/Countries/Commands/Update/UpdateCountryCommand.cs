namespace Market.Application.Modules.Locations.Countries.Commands.Update;

public sealed class UpdateCountryCommand : IRequest<Unit>
{
    /// <summary>Taken from the route (PUT /Countries/{id}), not from the body.</summary>
    [JsonIgnore]
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string IsoCode { get; set; }
}
