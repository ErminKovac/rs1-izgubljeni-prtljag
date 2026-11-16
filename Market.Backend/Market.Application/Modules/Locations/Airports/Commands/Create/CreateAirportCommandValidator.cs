namespace Market.Application.Modules.Locations.Airports.Commands.Create;

public sealed class CreateAirportCommandValidator : AbstractValidator<CreateAirportCommand>
{
    public CreateAirportCommandValidator()
    {
        // Only the request itself is checked here (no database access, see ValidationBehavior).
        // Whether the city exists and the IATA code is free is checked by the handler (404 / 409).
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Naziv je obavezan.")
            .MaximumLength(AirportEntity.Constraints.NameMaxLength).WithMessage($"Naziv može imati najviše {AirportEntity.Constraints.NameMaxLength} znakova.");

        RuleFor(x => x.IataCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("IATA oznaka je obavezna.")
            .Matches("^[A-Za-z]{3}$").WithMessage("IATA oznaka mora imati tačno 3 slova (npr. SJJ).");

        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage("Grad je obavezan.");
    }
}
