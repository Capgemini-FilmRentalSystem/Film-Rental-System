using FilmRentalStore.API.DTOs.Auth;
using FluentValidation;

namespace FilmRentalStore.API.Validators
{
    public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required.")
                .MaximumLength(16)
                .WithMessage("Username cannot exceed 16 characters.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.");
        }
    }
}