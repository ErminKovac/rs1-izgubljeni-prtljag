using Market.Application.Modules.Locations.Countries.Commands.Create;
using Market.Application.Modules.Locations.Countries.Commands.Delete;
using Market.Application.Modules.Locations.Countries.Commands.Update;
using Market.Application.Modules.Locations.Countries.Queries.GetById;
using Market.Application.Modules.Locations.Countries.Queries.List;
using Market.Shared.Constants;

namespace Market.API.Controllers;

/// <summary>
/// Countries (reference data). Every logged-in user can read them (fallback policy),
/// only the administrator can change them.
/// </summary>
[ApiController]
[Route("[controller]")]
public class CountriesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = PolicyNames.Admin)]
    public async Task<ActionResult<int>> Create(CreateCountryCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = PolicyNames.Admin)]
    public async Task Update(int id, UpdateCountryCommand command, CancellationToken ct)
    {
        // ID from the route takes precedence
        command.Id = id;
        await sender.Send(command, ct);
        // no return -> 204 No Content
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = PolicyNames.Admin)]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteCountryCommand { Id = id }, ct);
        // no return -> 204 No Content
    }

    [HttpGet("{id:int}")]
    public async Task<GetCountryByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var country = await sender.Send(new GetCountryByIdQuery { Id = id }, ct);
        return country; // if NotFoundException -> 404 via middleware
    }

    [HttpGet]
    public async Task<PageResult<ListCountriesQueryDto>> List([FromQuery] ListCountriesQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }
}
