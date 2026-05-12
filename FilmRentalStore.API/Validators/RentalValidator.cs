using FilmRentalStore.API.DTOs.Rental;
using FluentValidation;

namespace FilmRentalStore.API.Validators
{
    public class RentalRequestDtoValidator : AbstractValidator<RentalRequestDto>
    {
        public RentalRequestDtoValidator()
        {
            RuleFor(r => r.CustomerId)
                .GreaterThan(0)
                .WithMessage("CustomerId must be greater than 0");

            RuleFor(r => r.InventoryId)
                .GreaterThan(0)
                .WithMessage("InventoryId must be greater than 0");

            RuleFor(r => r.StaffId)
                .GreaterThan((byte)0)
                .WithMessage("StaffId must be greater than 0");
        }
    }

    public class RentalReturnRequestDtoValidator : AbstractValidator<RentalReturnRequestDto>
    {
        public RentalReturnRequestDtoValidator()
        {
            RuleFor(r => r.ReturnDate)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("ReturnDate cannot be in future")
                .When(r => r.ReturnDate.HasValue);
        }
    }
}
