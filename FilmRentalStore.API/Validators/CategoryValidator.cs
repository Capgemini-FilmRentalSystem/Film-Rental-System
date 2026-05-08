using FilmRentalStore.API.DTOs.Category;
using FluentValidation;

namespace FilmRentalStore.API.Validators
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(25).WithMessage("Category name cannot exceed 25 characters.")
                .Matches("^[a-zA-Z ]+$").WithMessage("Category name can only contain letters and spaces.");
        }
    }

    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(25).WithMessage("Category name cannot exceed 25 characters.")
                .Matches("^[a-zA-Z ]+$").WithMessage("Category name can only contain letters and spaces.");
        }
    }
}
