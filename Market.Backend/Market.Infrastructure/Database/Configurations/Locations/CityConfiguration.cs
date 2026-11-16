using Market.Domain.Entities.Locations;

namespace Market.Infrastructure.Database.Configurations.Locations;

public sealed class CityConfiguration : IEntityTypeConfiguration<CityEntity>
{
    public void Configure(EntityTypeBuilder<CityEntity> builder)
    {
        builder
            .ToTable("Cities");

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(CityEntity.Constraints.NameMaxLength);

        builder
            .Property(x => x.PostalCode)
            .HasMaxLength(CityEntity.Constraints.PostalCodeMaxLength);

        // Two countries can have a city with the same name, but one country cannot have it twice.
        builder
            .HasIndex(x => new { x.CountryId, x.Name })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder
            .HasOne(x => x.Country)
            .WithMany(x => x.Cities)
            .HasForeignKey(x => x.CountryId)
            // Restrict — the database rejects a hard DELETE of a country that still has cities.
            // Soft delete (IsDeleted = true) is an UPDATE and never reaches this FK rule,
            // so the same rule is also checked in DeleteCountryCommandHandler.
            .OnDelete(DeleteBehavior.Restrict);
    }
}
