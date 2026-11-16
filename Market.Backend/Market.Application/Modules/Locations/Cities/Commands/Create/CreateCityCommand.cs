namespace Market.Application.Modules.Locations.Cities.Commands.Create;

public sealed class CreateCityCommand : IRequest<int>
{
    /// <summary>Display name, e.g. "Sarajevo".</summary>
    public required string Name { get; set; }

    /// <summary>Postal code, e.g. "71000". (optional)</summary>
    public string? PostalCode { get; set; }

    /// <summary>Country the city belongs to.</summary>
    public required int CountryId { get; set; }
}
