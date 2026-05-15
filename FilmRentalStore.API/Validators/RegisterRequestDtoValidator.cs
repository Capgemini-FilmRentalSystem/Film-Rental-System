using FilmRentalStore.API.DTOs.Auth;
using FluentValidation;

namespace FilmRentalStore.API.Validators
{
    public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name cannot be empty")
                .MaximumLength(45)
                .WithMessage("First name cannot exceed 45 characters");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name cannot be empty")
                .MaximumLength(45)
                .WithMessage("Last name cannot exceed 45 characters");

            RuleFor(x => x.AddressId)
                .GreaterThan(0)
                .WithMessage("AddressId cannot be less than zero")
                .When(x => x.Address == null);

            RuleFor(x => x.Email)
                .MaximumLength(50)
                .WithMessage("Email cannot exceed 50 characters")
                .EmailAddress()
                .WithMessage("Invalid email format")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.StoreId)
                .GreaterThan(0)
                .WithMessage("StoreId cannot be less than zero");

            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username cannot be empty")
                .MaximumLength(16)
                .WithMessage("Username cannot exceed 16 characters");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password cannot be empty")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long");

            RuleFor(x => x.RoleId)
                .InclusiveBetween(1, 3)
                .WithMessage("RoleId must be 1, 2 or 3");

            RuleFor(x => x.Address)
                .NotNull()
                .WithMessage("Address details are required")
                .When(x => x.AddressId <= 0);

            When(x => x.AddressId <= 0 && x.Address != null, () =>
            {
                RuleFor(x => x.Address!.AddressLine)
                    .NotEmpty()
                    .WithMessage("Address line is required");

                RuleFor(x => x.Address!.District)
                    .NotEmpty()
                    .WithMessage("District is required");

                RuleFor(x => x.Address!.Phone)
                    .NotEmpty()
                    .WithMessage("Phone is required");

                RuleFor(x => x.Address!.City)
                    .NotEmpty()
                    .WithMessage("City is required");

                RuleFor(x => x.Address!.Country)
                    .NotEmpty()
                    .WithMessage("Country is required");
            });
        }
    }
}
