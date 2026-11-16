namespace Market.Application.Modules.Locations.Cities.Commands.Update;

public sealed class UpdateCityCommand : IRequest<Unit>
{
    /// <summary>Taken from the route (PUT /Cities/{id}), not from the body.</summary>
    [JsonIgnore]
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? PostalCode { get; set; }

    public required int CountryId { get; set; }
}
