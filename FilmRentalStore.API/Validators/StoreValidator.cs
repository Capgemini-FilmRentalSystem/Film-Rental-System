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
                .WithMessage("Address id must be greater than 0");
        }
    }
}
