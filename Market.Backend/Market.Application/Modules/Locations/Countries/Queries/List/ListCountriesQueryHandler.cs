namespace Market.Application.Modules.Locations.Countries.Queries.List;

public sealed class ListCountriesQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListCountriesQuery, PageResult<ListCountriesQueryDto>>
{
    public async Task<PageResult<ListCountriesQueryDto>> Handle(ListCountriesQuery request, CancellationToken ct)
    {
        var q = ctx.Countries.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            q = q.Where(x => x.Name.ToLower().Contains(term) || x.IsoCode.ToLower().Contains(term));
        }

        // ThenBy(Id) makes the order stable, so paging never skips or repeats rows.
        var projectedQuery = q.OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(x => new ListCountriesQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                IsoCode = x.IsoCode,
                // the global query filter applies here too, so soft-deleted cities are not counted
                CitiesCount = x.Cities.Count()
            });

        return await PageResult<ListCountriesQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
