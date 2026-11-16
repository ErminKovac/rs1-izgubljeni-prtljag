namespace Market.Application.Modules.Locations.Airports.Queries.List;

public sealed class ListAirportsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListAirportsQuery, PageResult<ListAirportsQueryDto>>
{
    public async Task<PageResult<ListAirportsQueryDto>> Handle(ListAirportsQuery request, CancellationToken ct)
    {
        var q = ctx.Airports.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            q = q.Where(x => x.Name.ToLower().Contains(term)
                             || x.IataCode.ToLower().Contains(term)
                             || x.City!.Name.ToLower().Contains(term));
        }

        if (request.CountryId is not null)
        {
            q = q.Where(x => x.City!.CountryId == request.CountryId);
        }

        if (request.CityId is not null)
        {
            q = q.Where(x => x.CityId == request.CityId);
        }

        // Sorted by IATA code (unique), so the order is stable for paging.
        var projectedQuery = q.OrderBy(x => x.IataCode)
            .ThenBy(x => x.Id)
            .Select(x => new ListAirportsQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                IataCode = x.IataCode,
                CityId = x.CityId,
                CityName = x.City!.Name,
                CountryId = x.City!.CountryId,
                CountryName = x.City!.Country!.Name
            });

        return await PageResult<ListAirportsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
