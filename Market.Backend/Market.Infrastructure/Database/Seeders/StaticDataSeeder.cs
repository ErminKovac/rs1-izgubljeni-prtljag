using Market.Domain.Entities.Finance;
using Market.Domain.Entities.Locations;

namespace Market.Infrastructure.Database.Seeders;

/// <summary>
/// Static (reference) data that every environment needs, including production.
/// It is written into the migration with <c>HasData</c>, so it needs fixed Ids and fixed dates
/// (a value such as <c>DateTime.UtcNow</c> would change the model and create a new migration every time).
/// Demo data for development belongs in <see cref="DynamicDataSeeder"/>.
/// </summary>
public static class StaticDataSeeder
{
    private static readonly DateTime SeedDateUtc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedCurrencies(modelBuilder);
        SeedLocations(modelBuilder);
    }

    /// <summary>
    /// Currencies and the fixed EUR rate. Rates of the other currencies change every day,
    /// so they are not reference data: in development they come from <see cref="DynamicDataSeeder"/>,
    /// otherwise the administrator enters them (or enables the exchange rate provider).
    /// </summary>
    private static void SeedCurrencies(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CurrencyEntity>().HasData(
            new CurrencyEntity { Id = 1, Code = "BAM", Name = "Konvertibilna marka", Symbol = "KM", DecimalPlaces = 2, IsBase = true, IsEnabled = true, CreatedAtUtc = SeedDateUtc },
            new CurrencyEntity { Id = 2, Code = "EUR", Name = "Euro", Symbol = "€", DecimalPlaces = 2, IsBase = false, IsEnabled = true, CreatedAtUtc = SeedDateUtc },
            new CurrencyEntity { Id = 3, Code = "USD", Name = "Američki dolar", Symbol = "$", DecimalPlaces = 2, IsBase = false, IsEnabled = true, CreatedAtUtc = SeedDateUtc },
            new CurrencyEntity { Id = 4, Code = "CHF", Name = "Švicarski franak", Symbol = "CHF", DecimalPlaces = 2, IsBase = false, IsEnabled = true, CreatedAtUtc = SeedDateUtc },
            new CurrencyEntity { Id = 5, Code = "GBP", Name = "Britanska funta", Symbol = "£", DecimalPlaces = 2, IsBase = false, IsEnabled = true, CreatedAtUtc = SeedDateUtc },
            new CurrencyEntity { Id = 6, Code = "RSD", Name = "Srpski dinar", Symbol = "RSD", DecimalPlaces = 2, IsBase = false, IsEnabled = true, CreatedAtUtc = SeedDateUtc });

        // The KM has been pegged to the euro since 1999 (currency board): 1 EUR = 1.95583 KM.
        modelBuilder.Entity<ExchangeRateEntity>().HasData(
            new ExchangeRateEntity
            {
                Id = 1,
                CurrencyId = 2,
                RateToBase = ExchangeRateEntity.EurRateToBam,
                ValidFromUtc = new DateTime(1999, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Source = "CBBiH (fiksni kurs)",
                CreatedAtUtc = SeedDateUtc
            });
    }

    /// <summary>
    /// Countries, cities and airports (issue #2). They are reference data like currencies:
    /// lost baggage reports and flights need airports in every environment, also in production,
    /// so they go into the migration. The administrator adds more through the admin pages.
    /// Order matters only for readability: EF inserts countries before cities and cities before airports (FK).
    /// </summary>
    private static void SeedLocations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CountryEntity>().HasData(
            new CountryEntity { Id = 1, Name = "Bosna i Hercegovina", IsoCode = "BA", CreatedAtUtc = SeedDateUtc },
            new CountryEntity { Id = 2, Name = "Hrvatska", IsoCode = "HR", CreatedAtUtc = SeedDateUtc },
            new CountryEntity { Id = 3, Name = "Srbija", IsoCode = "RS", CreatedAtUtc = SeedDateUtc },
            new CountryEntity { Id = 4, Name = "Austrija", IsoCode = "AT", CreatedAtUtc = SeedDateUtc },
            new CountryEntity { Id = 5, Name = "Njemačka", IsoCode = "DE", CreatedAtUtc = SeedDateUtc },
            new CountryEntity { Id = 6, Name = "Turska", IsoCode = "TR", CreatedAtUtc = SeedDateUtc });

        modelBuilder.Entity<CityEntity>().HasData(
            new CityEntity { Id = 1, Name = "Sarajevo", PostalCode = "71000", CountryId = 1, CreatedAtUtc = SeedDateUtc },
            new CityEntity { Id = 2, Name = "Tuzla", PostalCode = "75000", CountryId = 1, CreatedAtUtc = SeedDateUtc },
            new CityEntity { Id = 3, Name = "Mostar", PostalCode = "88000", CountryId = 1, CreatedAtUtc = SeedDateUtc },
            new CityEntity { Id = 4, Name = "Banja Luka", PostalCode = "78000", CountryId = 1, CreatedAtUtc = SeedDateUtc },
            new CityEntity { Id = 5, Name = "Zagreb", PostalCode = "10000", CountryId = 2, CreatedAtUtc = SeedDateUtc },
            new CityEntity { Id = 6, Name = "Beograd", PostalCode = "11000", CountryId = 3, CreatedAtUtc = SeedDateUtc },
            new CityEntity { Id = 7, Name = "Beč", PostalCode = "1010", CountryId = 4, CreatedAtUtc = SeedDateUtc },
            new CityEntity { Id = 8, Name = "Frankfurt na Majni", PostalCode = "60311", CountryId = 5, CreatedAtUtc = SeedDateUtc },
            new CityEntity { Id = 9, Name = "Istanbul", PostalCode = "34000", CountryId = 6, CreatedAtUtc = SeedDateUtc });

        modelBuilder.Entity<AirportEntity>().HasData(
            new AirportEntity { Id = 1, IataCode = "SJJ", Name = "Međunarodni aerodrom Sarajevo", CityId = 1, CreatedAtUtc = SeedDateUtc },
            new AirportEntity { Id = 2, IataCode = "TZL", Name = "Međunarodni aerodrom Tuzla", CityId = 2, CreatedAtUtc = SeedDateUtc },
            new AirportEntity { Id = 3, IataCode = "OMO", Name = "Međunarodni aerodrom Mostar", CityId = 3, CreatedAtUtc = SeedDateUtc },
            new AirportEntity { Id = 4, IataCode = "BNX", Name = "Međunarodni aerodrom Banja Luka", CityId = 4, CreatedAtUtc = SeedDateUtc },
            new AirportEntity { Id = 5, IataCode = "ZAG", Name = "Zračna luka Franjo Tuđman", CityId = 5, CreatedAtUtc = SeedDateUtc },
            new AirportEntity { Id = 6, IataCode = "BEG", Name = "Aerodrom Nikola Tesla", CityId = 6, CreatedAtUtc = SeedDateUtc },
            new AirportEntity { Id = 7, IataCode = "VIE", Name = "Aerodrom Beč-Schwechat", CityId = 7, CreatedAtUtc = SeedDateUtc },
            new AirportEntity { Id = 8, IataCode = "FRA", Name = "Aerodrom Frankfurt", CityId = 8, CreatedAtUtc = SeedDateUtc },
            new AirportEntity { Id = 9, IataCode = "IST", Name = "Aerodrom Istanbul", CityId = 9, CreatedAtUtc = SeedDateUtc });
    }
}
