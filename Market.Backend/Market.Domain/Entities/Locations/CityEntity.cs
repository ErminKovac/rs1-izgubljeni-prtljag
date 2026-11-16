using Market.Domain.Common;

namespace Market.Domain.Entities.Locations;

/// <summary>
/// Represents a city in a country, e.g. "Sarajevo" (Bosna i Hercegovina).
/// </summary>
public class CityEntity : BaseEntity
{
    /// <summary>
    /// Display name, e.g. "Sarajevo".
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Postal code, e.g. "71000". (optional)
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Identifier of the country the city belongs to.
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// Navigation reference to the city's country.
    /// </summary>
    public CountryEntity? Country { get; set; }

    /// <summary>
    /// Airports in this city (read-only navigation, airports are added through <see cref="AirportEntity"/>).
    /// </summary>
    public IReadOnlyCollection<AirportEntity> Airports { get; private set; } = new List<AirportEntity>();

    /// <summary>
    /// Single source of truth for technical/business constraints.
    /// Used in validators and EF configuration.
    /// </summary>
    public static class Constraints
    {
        public const int NameMaxLength = 100;

        public const int PostalCodeMaxLength = 20;
    }
}
