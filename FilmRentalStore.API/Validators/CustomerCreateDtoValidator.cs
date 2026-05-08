using FilmRentalStore.API.DTOs.Customers;
using FluentValidation;

namespace FilmRentalStore.API.Validators
{
    public class CustomerCreateDtoValidator : AbstractValidator<CustomerCreateDto>
    {
        public CustomerCreateDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(45).WithMessage("First name cannot exceed 45 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(45).WithMessage("Last name cannot exceed 45 characters.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(50).WithMessage("Email cannot exceed 50 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.StoreId)
                .GreaterThan(0).WithMessage("A valid Store ID is required.");

            RuleFor(x => x.AddressId)
                .GreaterThan(0).WithMessage("A valid Address ID is required.");
        }
    }
}