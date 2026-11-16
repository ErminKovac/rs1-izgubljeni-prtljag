using Market.Application.Common.Exceptions;
using Market.Application.Modules.Locations.Airports.Commands.Create;
using Market.Application.Modules.Locations.Cities.Commands.Create;
using Market.Application.Modules.Locations.Countries.Commands.Create;
using Market.Application.Modules.Locations.Countries.Commands.Delete;
using Market.Domain.Entities.Locations;
using Microsoft.Extensions.Time.Testing;

namespace Market.Tests.LocationTests.UnitTests;

/// <summary>
/// Rules of the country / city / airport handlers and validators (issue #2), on an empty InMemory database.
/// </summary>
public class LocationUnitTests
{
    private static DatabaseContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Each test gets a new (empty) database
            .Options;

        return new DatabaseContext(options, new FakeTimeProvider());
    }

    private static async Task<CountryEntity> AddCountryAsync(DatabaseContext context, string name, string isoCode)
    {
        var country = new CountryEntity { Name = name, IsoCode = isoCode };
        context.Countries.Add(country);
        await context.SaveChangesAsync();
        return country;
    }

    [Fact]
    public async Task CreateCountry_TrimsNameAndStoresIsoCodeInUpperCase()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var handler = new CreateCountryCommandHandler(context);

        // Act
        var id = await handler.Handle(new CreateCountryCommand { Name = "  Slovenija ", IsoCode = "si" }, CancellationToken.None);

        // Assert
        var country = await context.Countries.FindAsync(id);
        Assert.NotNull(country);
        Assert.Equal("Slovenija", country.Name);
        Assert.Equal("SI", country.IsoCode);
    }

    [Fact]
    public async Task CreateCountry_DuplicateIsoCodeInAnotherCase_ThrowsConflict()
    {
        // Arrange: "ba" and "BA" are the same code
        using var context = GetInMemoryDbContext();
        await AddCountryAsync(context, "Bosna i Hercegovina", "BA");
        var handler = new CreateCountryCommandHandler(context);

        // Act + Assert
        await Assert.ThrowsAsync<MarketConflictException>(() =>
            handler.Handle(new CreateCountryCommand { Name = "Druga država", IsoCode = "ba" }, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteCountry_WithCities_ThrowsConflictAndKeepsCountry()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var country = await AddCountryAsync(context, "Bosna i Hercegovina", "BA");
        context.Cities.Add(new CityEntity { Name = "Sarajevo", CountryId = country.Id });
        await context.SaveChangesAsync();

        var handler = new DeleteCountryCommandHandler(context);

        // Act + Assert
        await Assert.ThrowsAsync<MarketConflictException>(() =>
            handler.Handle(new DeleteCountryCommand { Id = country.Id }, CancellationToken.None));
        Assert.False(country.IsDeleted);
    }

    [Fact]
    public async Task DeleteCountry_WithoutCities_SoftDeletes()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var country = await AddCountryAsync(context, "Slovenija", "SI");
        var handler = new DeleteCountryCommandHandler(context);

        // Act
        await handler.Handle(new DeleteCountryCommand { Id = country.Id }, CancellationToken.None);

        // Assert: the row stays (IsDeleted = true), but the global query filter hides it
        Assert.False(await context.Countries.AnyAsync(x => x.Id == country.Id));
        var deleted = await context.Countries.IgnoreQueryFilters().SingleAsync(x => x.Id == country.Id);
        Assert.True(deleted.IsDeleted);
    }

    [Fact]
    public async Task CreateCity_SameNameInSameCountry_ThrowsConflict_ButOtherCountryIsAllowed()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var bih = await AddCountryAsync(context, "Bosna i Hercegovina", "BA");
        var serbia = await AddCountryAsync(context, "Srbija", "RS");
        var handler = new CreateCityCommandHandler(context);
        await handler.Handle(new CreateCityCommand { Name = "Gradiška", CountryId = bih.Id }, CancellationToken.None);

        // Act + Assert: same name in the same country (other case) → 409
        await Assert.ThrowsAsync<MarketConflictException>(() =>
            handler.Handle(new CreateCityCommand { Name = "GRADIŠKA", CountryId = bih.Id }, CancellationToken.None));

        // the same name in another country is a different city
        var otherId = await handler.Handle(new CreateCityCommand { Name = "Gradiška", CountryId = serbia.Id }, CancellationToken.None);
        Assert.True(otherId > 0);
    }

    [Fact]
    public async Task CreateAirport_UnknownCity_ThrowsNotFound()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var handler = new CreateAirportCommandHandler(context);

        // Act + Assert
        await Assert.ThrowsAsync<MarketNotFoundException>(() =>
            handler.Handle(new CreateAirportCommand { Name = "Aerodrom", IataCode = "XYZ", CityId = 999 }, CancellationToken.None));
    }

    [Theory]
    [InlineData("SJJ", true)]
    [InlineData("sjj", true)]   // case-insensitive, the handler stores it in upper case
    [InlineData("SJ", false)]   // too short
    [InlineData("SJJA", false)] // too long
    [InlineData("S1J", false)]  // letters only
    [InlineData("", false)]
    public void CreateAirportValidator_ChecksIataCodeFormat(string iataCode, bool expectedValid)
    {
        // Arrange
        var validator = new CreateAirportCommandValidator();
        var command = new CreateAirportCommand { Name = "Aerodrom", IataCode = iataCode, CityId = 1 };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Equal(expectedValid, result.IsValid);
    }
}
