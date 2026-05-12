namespace FilmRentalStore.API.Validators
{
    using FluentValidation;
    using FilmRentalStore.API.DTOs.Actor;

    public class ActorRequestDtoValidator : AbstractValidator<ActorRequestDto>
    {
        public ActorRequestDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(45).WithMessage("First name must be at most 45 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(45).WithMessage("Last name must be at most 45 characters.");
        }
    }
}
