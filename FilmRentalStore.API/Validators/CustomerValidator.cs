using FilmRentalStore.API.DTOs.Customers;
using FluentValidation;

namespace FilmRentalStore.API.Validations
{
    public class CustomerCreateDtoValidator : AbstractValidator<CustomerCreateDto>
    {
        public CustomerCreateDtoValidator()
        {
            RuleFor(x => x.StoreId)
                .GreaterThan(0)
                .WithMessage("Store id must be greater than 0");

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

            RuleFor(x => x.AddressId)
                .GreaterThan(0)
                .WithMessage("Address id must be greater than 0");
        }
    }

    public class CustomerUpdateDtoValidator : AbstractValidator<CustomerUpdateDto>
    {
        public CustomerUpdateDtoValidator()
        {
            RuleFor(x => x.StoreId)
                .GreaterThan(0)
                .WithMessage("Store id must be greater than 0");

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

            RuleFor(x => x.AddressId)
                .GreaterThan(0)
                .WithMessage("Address id must be greater than 0");
        }
    }
}