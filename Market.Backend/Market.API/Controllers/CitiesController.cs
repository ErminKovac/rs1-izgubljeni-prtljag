using Market.Application.Modules.Locations.Cities.Commands.Create;
using Market.Application.Modules.Locations.Cities.Commands.Delete;
using Market.Application.Modules.Locations.Cities.Commands.Update;
using Market.Application.Modules.Locations.Cities.Queries.GetById;
using Market.Application.Modules.Locations.Cities.Queries.List;
using Market.Shared.Constants;

namespace Market.API.Controllers;

/// <summary>
/// Cities (reference data). Every logged-in user can read them (fallback policy),
/// only the administrator can change them.
/// </summary>
[ApiController]
[Route("[controller]")]
public class CitiesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = PolicyNames.Admin)]
    public async Task<ActionResult<int>> Create(CreateCityCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = PolicyNames.Admin)]
    public async Task Update(int id, UpdateCityCommand command, CancellationToken ct)
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
        await sender.Send(new DeleteCityCommand { Id = id }, ct);
        // no return -> 204 No Content
    }

    [HttpGet("{id:int}")]
    public async Task<GetCityByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var city = await sender.Send(new GetCityByIdQuery { Id = id }, ct);
        return city; // if NotFoundException -> 404 via middleware
    }

    [HttpGet]
    public async Task<PageResult<ListCitiesQueryDto>> List([FromQuery] ListCitiesQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }
}
