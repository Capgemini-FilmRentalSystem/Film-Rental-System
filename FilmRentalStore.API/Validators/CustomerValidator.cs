using FilmRentalStore.API.DTOs.Customers;
using FluentValidation;

namespace FilmRentalStore.API.Validations
{
    public class CustomerRequestDtoValidator : AbstractValidator<CustomerRequestDto>
    {
        public CustomerRequestDtoValidator()
        {
            RuleFor(x => x.StoreId)
                .GreaterThan(0)
                .WithMessage("Store id must be greater than 0");

            RuleFor(x => x.Username)
                .MaximumLength(50)
                .WithMessage("Username cannot exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Username));

            RuleFor(x => x.Password)
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long")
                .When(x => !string.IsNullOrWhiteSpace(x.Password));

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required")
                .MaximumLength(45)
                .WithMessage("First name cannot exceed 45 characters");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required")
                .MaximumLength(45)
                .WithMessage("Last name cannot exceed 45 characters");

            RuleFor(x => x.Email)
                .MaximumLength(50)
                .WithMessage("Email cannot exceed 50 characters")
                .EmailAddress()
                .WithMessage("Invalid email format")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            When(x => x.Address != null, () =>
            {
                RuleFor(x => x.Address!.AddressLine)
                    .MaximumLength(50)
                    .WithMessage("Address line cannot exceed 50 characters");

                RuleFor(x => x.Address!.District)
                    .MaximumLength(20)
                    .WithMessage("District cannot exceed 20 characters");

                RuleFor(x => x.Address!.PostalCode)
                    .MaximumLength(10)
                    .WithMessage("Postal code cannot exceed 10 characters");

                RuleFor(x => x.Address!.Phone)
                    .MaximumLength(20)
                    .WithMessage("Phone cannot exceed 20 characters");
            });
        }
    }

    public class CustomerRegisterRequestDtoValidator : AbstractValidator<CustomerRegisterRequestDto>
    {
        public CustomerRegisterRequestDtoValidator()
        {
            RuleFor(x => x.StoreId)
                .GreaterThan(0)
                .WithMessage("Store id must be greater than 0");

            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required")
                .MaximumLength(50)
                .WithMessage("Username cannot exceed 50 characters");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required")
                .MaximumLength(45)
                .WithMessage("First name cannot exceed 45 characters");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required")
                .MaximumLength(45)
                .WithMessage("Last name cannot exceed 45 characters");

            RuleFor(x => x.Email)
                .MaximumLength(50)
                .WithMessage("Email cannot exceed 50 characters")
                .EmailAddress()
                .WithMessage("Invalid email format")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Address)
                .NotNull()
                .WithMessage("Address is required");

            When(x => x.Address != null, () =>
            {
                RuleFor(x => x.Address.AddressLine)
                    .NotEmpty()
                    .WithMessage("Address line is required")
                    .MaximumLength(50)
                    .WithMessage("Address line cannot exceed 50 characters");

                RuleFor(x => x.Address.District)
                    .NotEmpty()
                    .WithMessage("District is required")
                    .MaximumLength(20)
                    .WithMessage("District cannot exceed 20 characters");

                RuleFor(x => x.Address.PostalCode)
                    .MaximumLength(10)
                    .WithMessage("Postal code cannot exceed 10 characters");

                RuleFor(x => x.Address.Phone)
                    .NotEmpty()
                    .WithMessage("Phone is required")
                    .MaximumLength(20)
                    .WithMessage("Phone cannot exceed 20 characters");

                RuleFor(x => x.Address.CityName)
                    .NotEmpty()
                    .WithMessage("City name is required")
                    .MaximumLength(50)
                    .WithMessage("City name cannot exceed 50 characters");

                RuleFor(x => x.Address.CountryName)
                    .NotEmpty()
                    .WithMessage("Country name is required")
                    .MaximumLength(50)
                    .WithMessage("Country name cannot exceed 50 characters");
            });
        }
    }
}
