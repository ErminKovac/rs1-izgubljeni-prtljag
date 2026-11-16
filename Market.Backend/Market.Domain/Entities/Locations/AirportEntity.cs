using Market.Domain.Common;

namespace Market.Domain.Entities.Locations;

/// <summary>
/// Represents an airport in a city, e.g. "Međunarodni aerodrom Sarajevo" (SJJ).
/// </summary>
public class AirportEntity : BaseEntity
{
    /// <summary>
    /// Display name, e.g. "Međunarodni aerodrom Sarajevo".
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// IATA airport code in upper case, e.g. "SJJ", "TZL".
    /// </summary>
    public string IataCode { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the city the airport is in.
    /// </summary>
    public int CityId { get; set; }

    /// <summary>
    /// Navigation reference to the airport's city.
    /// </summary>
    public CityEntity? City { get; set; }

    /// <summary>
    /// Single source of truth for technical/business constraints.
    /// Used in validators and EF configuration.
    /// </summary>
    public static class Constraints
    {
        public const int NameMaxLength = 150;

        public const int IataCodeLength = 3;
    }
}
