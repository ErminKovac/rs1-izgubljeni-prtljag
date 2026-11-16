using Market.Application.Modules.Locations.Airports.Commands.Create;
using Market.Application.Modules.Locations.Airports.Commands.Delete;
using Market.Application.Modules.Locations.Airports.Commands.Update;
using Market.Application.Modules.Locations.Airports.Queries.GetById;
using Market.Application.Modules.Locations.Airports.Queries.List;
using Market.Shared.Constants;

namespace Market.API.Controllers;

/// <summary>
/// Airports (reference data). Every logged-in user can read them (e.g. a passenger picks an airport
/// in a lost baggage report), only the administrator can change them.
/// </summary>
[ApiController]
[Route("[controller]")]
public class AirportsController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = PolicyNames.Admin)]
    public async Task<ActionResult<int>> Create(CreateAirportCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = PolicyNames.Admin)]
    public async Task Update(int id, UpdateAirportCommand command, CancellationToken ct)
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
        await sender.Send(new DeleteAirportCommand { Id = id }, ct);
        // no return -> 204 No Content
    }

    [HttpGet("{id:int}")]
    public async Task<GetAirportByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var airport = await sender.Send(new GetAirportByIdQuery { Id = id }, ct);
        return airport; // if NotFoundException -> 404 via middleware
    }

    [HttpGet]
    public async Task<PageResult<ListAirportsQueryDto>> List([FromQuery] ListAirportsQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }
}
