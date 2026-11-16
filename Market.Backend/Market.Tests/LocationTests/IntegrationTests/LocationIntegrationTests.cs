using Market.Application.Common;
using Market.Application.Modules.Locations.Airports.Queries.List;
using System.Net;
using System.Net.Http.Json;

namespace Market.Tests.LocationTests.IntegrationTests;

/// <summary>
/// Countries, cities and airports through the API (issue #2): authorization, seeded data and delete rules.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class LocationIntegrationTests
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public LocationIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Airports_LoggedInEmployee_ReturnsSeededAirportsOfBosnia()
    {
        // Arrange: every logged-in user can read airports (not only the administrator)
        var client = await _factory.GetAuthenticatedClientAsync(
            CustomWebApplicationFactory<Program>.EmployeeEmail,
            CustomWebApplicationFactory<Program>.EmployeePassword);

        // Act: countryId=1 is "Bosna i Hercegovina" (StaticDataSeeder)
        var result = await client.GetFromJsonAsync<PageResult<ListAirportsQueryDto>>("/Airports?countryId=1&paging.pageSize=50");

        // Assert
        Assert.NotNull(result);
        var codes = result.Items.Select(a => a.IataCode).ToList();
        Assert.Contains("SJJ", codes);
        Assert.Contains("TZL", codes);
        Assert.Contains("OMO", codes);
        Assert.Contains("BNX", codes);
        Assert.All(result.Items, a => Assert.Equal("Bosna i Hercegovina", a.CountryName));
    }

    [Fact]
    public async Task Get_Airports_Anonymous_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient(); // no Bearer token

        // Act
        var response = await client.GetAsync("/Airports");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Country_NonAdmin_ShouldReturnForbidden()
    {
        // Arrange
        var client = await _factory.GetAuthenticatedClientAsync(
            CustomWebApplicationFactory<Program>.EmployeeEmail,
            CustomWebApplicationFactory<Program>.EmployeePassword);

        // Act
        var response = await client.PostAsJsonAsync("/Countries", new { Name = "Crna Gora", IsoCode = "ME" });

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Post_Airport_InvalidIataCode_ShouldReturnBadRequest()
    {
        // Arrange
        var client = await _factory.GetAuthenticatedClientAsync(); // admin

        // Act
        var response = await client.PostAsJsonAsync("/Airports", new { Name = "Aerodrom", IataCode = "S1", CityId = 1 });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Admin_CreatesCountryCityAirport_AndCannotDeleteCountryThatHasCities()
    {
        // Arrange
        var client = await _factory.GetAuthenticatedClientAsync(); // admin

        // Act: country → city → airport
        var countryResponse = await client.PostAsJsonAsync("/Countries", new { Name = "Italija", IsoCode = "it" });
        Assert.Equal(HttpStatusCode.Created, countryResponse.StatusCode);
        var countryId = (await countryResponse.Content.ReadFromJsonAsync<Dictionary<string, int>>())!["id"];

        var cityResponse = await client.PostAsJsonAsync("/Cities", new { Name = "Rim", PostalCode = "00100", CountryId = countryId });
        Assert.Equal(HttpStatusCode.Created, cityResponse.StatusCode);
        var cityId = (await cityResponse.Content.ReadFromJsonAsync<Dictionary<string, int>>())!["id"];

        var airportResponse = await client.PostAsJsonAsync("/Airports", new { Name = "Aerodrom Fiumicino", IataCode = "fco", CityId = cityId });
        Assert.Equal(HttpStatusCode.Created, airportResponse.StatusCode);

        // the same IATA code again → 409
        var duplicateResponse = await client.PostAsJsonAsync("/Airports", new { Name = "Drugi", IataCode = "FCO", CityId = cityId });
        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);

        // Assert: a country that has cities cannot be deleted
        var deleteResponse = await client.DeleteAsync($"/Countries/{countryId}");
        Assert.Equal(HttpStatusCode.Conflict, deleteResponse.StatusCode);
    }
}
