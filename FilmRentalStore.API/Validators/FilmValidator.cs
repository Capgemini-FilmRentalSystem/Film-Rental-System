using FilmRentalStore.API.DTOs.Film;
using FluentValidation;

namespace FilmRentalStore.API.Validators
{
    public class FilmRequestDtoValidator : AbstractValidator<FilmRequestDto>
    {
        public FilmRequestDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(255).WithMessage("Title cannot exceed 255 characters.");

            RuleFor(x => x.LanguageId)
                .GreaterThan((byte)0).WithMessage("Language ID must be greater than 0.");

            RuleFor(x => x.RentalDuration)
                .GreaterThan((byte)0).WithMessage("Rental duration must be at least 1 day.");

            RuleFor(x => x.RentalRate)
                .GreaterThanOrEqualTo(0).WithMessage("Rental rate cannot be negative.")
                .LessThanOrEqualTo(99.99m).WithMessage("Rental rate cannot exceed 99.99.");

            RuleFor(x => x.ReplacementCost)
                .GreaterThanOrEqualTo(0).WithMessage("Replacement cost cannot be negative.")
                .LessThanOrEqualTo(999.99m).WithMessage("Replacement cost cannot exceed 999.99.");

            RuleFor(x => x.Rating)
                .Must(r => r == "G" || r == "PG" || r == "PG-13" || r == "R" || r == "NC-17")
                .WithMessage("Rating must be one of: G, PG, PG-13, R, NC-17.")
                .When(x => x.Rating != null);

            RuleFor(x => x.Length)
                .GreaterThan((short)0).WithMessage("Length must be greater than 0.")
                .When(x => x.Length.HasValue);

            RuleFor(x => x.ReleaseYear)
                .Length(4).WithMessage("Release year must be 4 characters.")
                .When(x => x.ReleaseYear != null);
        }
    }

    public class FilmActorAssignRequestDtoValidator : AbstractValidator<FilmActorAssignRequestDto>
    {
        public FilmActorAssignRequestDtoValidator()
        {
            RuleFor(x => x.ActorId)
                .GreaterThan(0).WithMessage("Actor ID must be greater than 0.");
        }
    }

    public class FilmCategoryAssignRequestDtoValidator : AbstractValidator<FilmCategoryAssignRequestDto>
    {
        public FilmCategoryAssignRequestDtoValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan((byte)0).WithMessage("Category ID must be greater than 0.");
        }
    }
}
