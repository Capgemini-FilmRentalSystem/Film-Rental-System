using FilmRentalStore.API.Models;
using FluentValidation;

namespace FilmRentalStore.API.Validators
{
    public class InventoryValidator : AbstractValidator<Inventory>
    {
        public InventoryValidator()
        {
            // inventory_id validation
            RuleFor(i => i.InventoryId)
                .GreaterThan(0)
                .WithMessage("InventoryId must be greater than 0");

            // film_id validation
            RuleFor(i => i.FilmId)
                .GreaterThan((short)0)
                .WithMessage("FilmId must be greater than 0");

            // store_id validation
            RuleFor(i => i.StoreId)
                .GreaterThan((byte)0)
                .WithMessage("StoreId must be greater than 0");

            // last_update validation
            RuleFor(i => i.LastUpdate)
                .NotEmpty()
                .WithMessage("LastUpdate is required");

            RuleFor(i => i.LastUpdate)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("LastUpdate cannot be in future");
        }
    }
}