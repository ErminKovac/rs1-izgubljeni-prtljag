using Market.Domain.Entities.Locations;

namespace Market.Infrastructure.Database.Configurations.Locations;

public sealed class CountryConfiguration : IEntityTypeConfiguration<CountryEntity>
{
    public void Configure(EntityTypeBuilder<CountryEntity> builder)
    {
        builder
            .ToTable("Countries");

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(CountryEntity.Constraints.NameMaxLength);

        builder
            .Property(x => x.IsoCode)
            .IsRequired()
            .HasMaxLength(CountryEntity.Constraints.IsoCodeLength)
            .IsFixedLength();

        // The create/update handlers check duplicates first and return 409;
        // the unique indexes are the last guard for two simultaneous requests.
        // Unique only among non-deleted rows: a soft-deleted country keeps its row,
        // and without the filter its code could never be used again.
        builder
            .HasIndex(x => x.IsoCode)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder
            .HasIndex(x => x.Name)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
