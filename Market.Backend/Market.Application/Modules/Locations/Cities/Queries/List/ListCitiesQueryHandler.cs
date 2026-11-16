namespace Market.Application.Modules.Locations.Cities.Queries.List;

public sealed class ListCitiesQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListCitiesQuery, PageResult<ListCitiesQueryDto>>
{
    public async Task<PageResult<ListCitiesQueryDto>> Handle(ListCitiesQuery request, CancellationToken ct)
    {
        var q = ctx.Cities.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            q = q.Where(x => x.Name.ToLower().Contains(term)
                             || (x.PostalCode != null && x.PostalCode.ToLower().Contains(term)));
        }

        if (request.CountryId is not null)
        {
            q = q.Where(x => x.CountryId == request.CountryId);
        }

        // ThenBy(Id) makes the order stable (Name is not unique across countries), so paging never skips or repeats rows.
        var projectedQuery = q.OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(x => new ListCitiesQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                PostalCode = x.PostalCode,
                CountryId = x.CountryId,
                CountryName = x.Country!.Name,
                CountryIsoCode = x.Country!.IsoCode,
                AirportsCount = x.Airports.Count()
            });

        return await PageResult<ListCitiesQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
