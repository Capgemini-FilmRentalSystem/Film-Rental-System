using FilmRentalStore.API.DTOs.Store;
using FluentValidation;

namespace FilmRentalStore.API.Validations
{
    public class StoreRequestDtoValidator : AbstractValidator<StoreRequestDto>
    {
        public StoreRequestDtoValidator()
        {
            RuleFor(x => x.ManagerStaffId)
                .GreaterThan((byte)0)
                .WithMessage("Manager staff id must be greater than 0");

            RuleFor(x => x.AddressId)
                .GreaterThan(0)
                .WithMessage("Address id must be greater than 0")
                .When(x => x.Address == null);

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
