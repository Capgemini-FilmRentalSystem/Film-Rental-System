using FilmRentalStore.API.DTOs.Category;
using FluentValidation;

namespace FilmRentalStore.API.Validators
{
    public class CategoryDtoValidator : AbstractValidator<CategoryDto>
    {
        public CategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(25).WithMessage("Category name cannot exceed 25 characters.")
                .Matches("^[a-zA-Z ]+$").WithMessage("Category name can only contain letters and spaces.")
                .Must(n => n.Trim().Length > 0).WithMessage("Category name cannot be only whitespace.");
        }
    }
}
