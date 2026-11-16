namespace Market.Application.Modules.Locations.Cities.Commands.Create;

public sealed class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator()
    {
        // Only the request itself is checked here (no database access, see ValidationBehavior).
        // Whether the country exists and the name is free is checked by the handler (404 / 409).
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Naziv je obavezan.")
            .MaximumLength(CityEntity.Constraints.NameMaxLength).WithMessage($"Naziv može imati najviše {CityEntity.Constraints.NameMaxLength} znakova.");

        RuleFor(x => x.PostalCode)
            .MaximumLength(CityEntity.Constraints.PostalCodeMaxLength).WithMessage($"Poštanski broj može imati najviše {CityEntity.Constraints.PostalCodeMaxLength} znakova.");

        RuleFor(x => x.CountryId)
            .GreaterThan(0).WithMessage("Država je obavezna.");
    }
}
