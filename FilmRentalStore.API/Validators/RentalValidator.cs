using FilmRentalStore.API.Models;
using FluentValidation;

namespace FilmRentalStore.API.Validators
{
    public class RentalValidator : AbstractValidator<Rental>
    {
        public RentalValidator()
        {
            // CustomerId Validation
            RuleFor(r => r.CustomerId)
                .NotEmpty()
                .WithMessage("CustomerId is required")
                .GreaterThan((short)0)
                .WithMessage("CustomerId must be greater than 0");

            // InventoryId Validation
            RuleFor(r => r.InventoryId)
                .NotEmpty()
                .WithMessage("InventoryId is required")
                .GreaterThan(0)
                .WithMessage("InventoryId must be greater than 0");

            // StaffId Validation
            RuleFor(r => r.StaffId)
                .NotEmpty()
                .WithMessage("StaffId is required")
                .GreaterThan((byte)0)
                .WithMessage("StaffId must be greater than 0");

            // RentalDate Validation
            RuleFor(r => r.RentalDate)
                .NotEmpty()
                .WithMessage("RentalDate is required");

            RuleFor(r => r.RentalDate)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("RentalDate cannot be in future");

            RuleFor(r => r.RentalDate)
                .GreaterThan(DateTime.Now.AddYears(-10))
                .WithMessage("RentalDate is too old");

            // ReturnDate Validation
            RuleFor(r => r.ReturnDate)
                .GreaterThan(r => r.RentalDate)
                .When(r => r.ReturnDate.HasValue)
                .WithMessage("ReturnDate must be after RentalDate");

            RuleFor(r => r.ReturnDate)
                .LessThanOrEqualTo(DateTime.Now)
                .When(r => r.ReturnDate.HasValue)
                .WithMessage("ReturnDate cannot be in future");

            // Rental Duration Validation
            RuleFor(r => r)
                .Must(r =>
                {
                    if (!r.ReturnDate.HasValue)
                        return true;

                    return (r.ReturnDate.Value - r.RentalDate).Days <= 30;
                })
                .WithMessage("Rental duration cannot exceed 30 days");

            // Prevent same-day invalid return
            RuleFor(r => r)
                .Must(r =>
                {
                    if (!r.ReturnDate.HasValue)
                        return true;

                    return r.ReturnDate.Value >= r.RentalDate;
                })
                .WithMessage("ReturnDate cannot be before RentalDate");
        }
    }
}