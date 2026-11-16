namespace Market.Application.Modules.Locations.Cities.Commands.Update;

public sealed class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
{
    public UpdateCityCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Naziv je obavezan.")
            .MaximumLength(CityEntity.Constraints.NameMaxLength).WithMessage($"Naziv može imati najviše {CityEntity.Constraints.NameMaxLength} znakova.");

        RuleFor(x => x.PostalCode)
            .MaximumLength(CityEntity.Constraints.PostalCodeMaxLength).WithMessage($"Poštanski broj može imati najviše {CityEntity.Constraints.PostalCodeMaxLength} znakova.");

        RuleFor(x => x.CountryId)
            .GreaterThan(0).WithMessage("Država je obavezna.");
    }
}
