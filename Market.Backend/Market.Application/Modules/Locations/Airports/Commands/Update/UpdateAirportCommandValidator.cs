namespace Market.Application.Modules.Locations.Airports.Commands.Update;

public sealed class UpdateAirportCommandValidator : AbstractValidator<UpdateAirportCommand>
{
    public UpdateAirportCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

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
