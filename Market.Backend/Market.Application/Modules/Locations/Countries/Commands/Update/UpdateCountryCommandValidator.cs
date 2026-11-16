namespace Market.Application.Modules.Locations.Countries.Commands.Update;

public sealed class UpdateCountryCommandValidator : AbstractValidator<UpdateCountryCommand>
{
    public UpdateCountryCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Naziv je obavezan.")
            .MaximumLength(CountryEntity.Constraints.NameMaxLength).WithMessage($"Naziv može imati najviše {CountryEntity.Constraints.NameMaxLength} znakova.");

        RuleFor(x => x.IsoCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("ISO oznaka je obavezna.")
            .Matches("^[A-Za-z]{2}$").WithMessage("ISO oznaka mora imati tačno 2 slova (npr. BA).");
    }
}
