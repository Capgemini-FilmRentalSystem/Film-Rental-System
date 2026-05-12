using FilmRentalStore.API.DTOs.Staff;
using FluentValidation;

namespace FilmRentalStore.API.Validators
{
    public class StaffCreateRequestDtoValidator : AbstractValidator<StaffCreateRequestDto>
    {
        public StaffCreateRequestDtoValidator()
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
                .WithMessage("AddressId cannot be less than zero");

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

            RuleFor(x => x.RoleId)
                .InclusiveBetween(1, 3)
                .WithMessage("RoleId must be 1, 2 or 3");
        }
    }

    public class StaffUpdateRequestDtoValidator : AbstractValidator<StaffUpdateRequestDto>
    {
        public StaffUpdateRequestDtoValidator()
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
                .WithMessage("AddressId cannot be less than zero");

            RuleFor(x => x.Email)
                .MaximumLength(50)
                .WithMessage("Email cannot exceed 50 characters")
                .EmailAddress()
                .WithMessage("Invalid email format")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.RoleId)
                .InclusiveBetween(1, 3)
                .WithMessage("RoleId must be 1, 2 or 3");
        }
    }
}
