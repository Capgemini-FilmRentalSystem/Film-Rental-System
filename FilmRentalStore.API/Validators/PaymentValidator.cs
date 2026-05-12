using FilmRentalStore.API.DTOs.Payment;
using FluentValidation;

namespace FilmRentalStore.API.Validators
{
    public class PaymentRequestDtoValidator : AbstractValidator<PaymentRequestDto>
    {
        public PaymentRequestDtoValidator()
        {
            RuleFor(p => p.CustomerId)
                .GreaterThan(0)
                .WithMessage("Customer ID must be greater than 0.");

            RuleFor(p => p.StaffId)
                .GreaterThan((byte)0)
                .WithMessage("Staff ID must be greater than 0.");

            RuleFor(p => p.RentalId)
                .GreaterThan(0)
                .WithMessage("Rental ID must be greater than 0.");

            RuleFor(p => p.Amount)

                .GreaterThan(0).WithMessage("Payment amount must be greater than zero.")
                .Must(amount => decimal.Round(amount, 2) == amount)
                .WithMessage("Amount must have at most 2 decimal places.");
        }
    }
}
