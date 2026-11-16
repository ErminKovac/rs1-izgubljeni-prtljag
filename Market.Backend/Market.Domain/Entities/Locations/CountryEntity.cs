using Market.Domain.Common;

namespace Market.Domain.Entities.Locations;

/// <summary>
/// Represents a country, e.g. "Bosna i Hercegovina" (BA).
/// </summary>
public class CountryEntity : BaseEntity
{
    /// <summary>
    /// Display name, e.g. "Bosna i Hercegovina".
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ISO 3166-1 alpha-2 code in upper case, e.g. "BA", "HR".
    /// </summary>
    public string IsoCode { get; set; } = string.Empty;

    /// <summary>
    /// Cities of this country (read-only navigation, cities are added through <see cref="CityEntity"/>).
    /// </summary>
    public IReadOnlyCollection<CityEntity> Cities { get; private set; } = new List<CityEntity>();

    /// <summary>
    /// Single source of truth for technical/business constraints.
    /// Used in validators and EF configuration.
    /// </summary>
    public static class Constraints
    {
        public const int NameMaxLength = 100;

        public const int IsoCodeLength = 2;
    }
}
