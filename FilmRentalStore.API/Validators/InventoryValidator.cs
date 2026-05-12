using FilmRentalStore.API.DTOs.Inventory;
using FluentValidation;

namespace FilmRentalStore.API.Validators
{
    public class InventoryRequestDtoValidator : AbstractValidator<InventoryRequestDto>
    {
        public InventoryRequestDtoValidator()
        {
            RuleFor(i => i.FilmId)
                .GreaterThan(0)
                .WithMessage("FilmId must be greater than 0");

            RuleFor(i => i.StoreId)
                .GreaterThan(0)
                .WithMessage("StoreId must be greater than 0");
        }
    }
}
