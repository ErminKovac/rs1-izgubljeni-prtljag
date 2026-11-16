using Market.Domain.Entities.Locations;

namespace Market.Infrastructure.Database.Configurations.Locations;

public sealed class AirportConfiguration : IEntityTypeConfiguration<AirportEntity>
{
    public void Configure(EntityTypeBuilder<AirportEntity> builder)
    {
        builder
            .ToTable("Airports");

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(AirportEntity.Constraints.NameMaxLength);

        builder
            .Property(x => x.IataCode)
            .IsRequired()
            .HasMaxLength(AirportEntity.Constraints.IataCodeLength)
            .IsFixedLength();

        // The IATA code identifies an airport worldwide, so it is unique (among non-deleted airports).
        builder
            .HasIndex(x => x.IataCode)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder
            .HasOne(x => x.City)
            .WithMany(x => x.Airports)
            .HasForeignKey(x => x.CityId)
            // Restrict — the database rejects a hard DELETE of a city that still has airports
            // (the same rule is checked in DeleteCityCommandHandler, because soft delete is an UPDATE).
            .OnDelete(DeleteBehavior.Restrict);
    }
}
