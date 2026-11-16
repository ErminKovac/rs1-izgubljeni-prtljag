using Market.Domain.Entities.Finance;
using Market.Domain.Entities.Locations;


namespace Market.Application.Abstractions;

// Application layer
public interface IAppDbContext
{
    DbSet<MarketUserEntity> Users { get; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; }
    DbSet<PasswordResetTokenEntity> PasswordResetTokens { get; }

    DbSet<CurrencyEntity> Currencies { get; }
    DbSet<ExchangeRateEntity> ExchangeRates { get; }

    DbSet<CountryEntity> Countries { get; }
    DbSet<CityEntity> Cities { get; }
    DbSet<AirportEntity> Airports { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}
