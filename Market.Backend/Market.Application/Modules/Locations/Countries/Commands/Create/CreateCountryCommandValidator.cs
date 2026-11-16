namespace Market.Application.Modules.Locations.Countries.Commands.Create;

public sealed class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
{
    public CreateCountryCommandValidator()
    {
        // Only the request itself is checked here (no database access, see ValidationBehavior).
        // A duplicate name or code needs the database, so it is checked by the handler (409).
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Naziv je obavezan.")
            .MaximumLength(CountryEntity.Constraints.NameMaxLength).WithMessage($"Naziv može imati najviše {CountryEntity.Constraints.NameMaxLength} znakova.");

        // Cascade.Stop: an empty code is reported only as "obavezna", not also as a wrong format.
        RuleFor(x => x.IsoCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("ISO oznaka je obavezna.")
            .Matches("^[A-Za-z]{2}$").WithMessage("ISO oznaka mora imati tačno 2 slova (npr. BA).");
    }
}
