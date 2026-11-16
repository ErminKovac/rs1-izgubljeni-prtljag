namespace Market.Application.Modules.Locations.Countries.Commands.Create;

public sealed class CreateCountryCommand : IRequest<int>
{
    /// <summary>Display name, e.g. "Bosna i Hercegovina".</summary>
    public required string Name { get; set; }

    /// <summary>ISO 3166-1 alpha-2 code, e.g. "BA" (case-insensitive, stored in upper case).</summary>
    public required string IsoCode { get; set; }
}
