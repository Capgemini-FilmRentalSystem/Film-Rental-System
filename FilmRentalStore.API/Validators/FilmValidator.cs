using FilmRentalStore.API.DTOs.Film;
using FluentValidation;

namespace FilmRentalStore.API.Validators
{
    public class CreateFilmValidator : AbstractValidator<CreateFilmDto>
    {
        private static readonly string[] ValidRatings =
            { "G", "PG", "PG-13", "R", "NC-17" };

        public CreateFilmValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(255).WithMessage("Title cannot exceed 255 characters.");

            RuleFor(x => x.LanguageId)
                .GreaterThan((byte)0).WithMessage("A valid Language ID is required.");

            RuleFor(x => x.RentalDuration)
                .GreaterThan((byte)0).WithMessage("Rental duration must be at least 1 day.");

            RuleFor(x => x.RentalRate)
                .GreaterThanOrEqualTo(0).WithMessage("Rental rate cannot be negative.")
                .LessThanOrEqualTo(99.99m).WithMessage("Rental rate cannot exceed 99.99.");

            RuleFor(x => x.ReplacementCost)
                .GreaterThanOrEqualTo(0).WithMessage("Replacement cost cannot be negative.")
                .LessThanOrEqualTo(999.99m).WithMessage("Replacement cost cannot exceed 999.99.");

            RuleFor(x => x.Rating)
                .Must(r => r == null || ValidRatings.Contains(r))
                .WithMessage($"Rating must be one of: {string.Join(", ", ValidRatings)}");

            RuleFor(x => x.ReleaseYear)
                .Matches(@"^\d{4}$").WithMessage("Release year must be a 4-digit number.")
                .When(x => x.ReleaseYear != null);

            RuleFor(x => x.Length)
                .GreaterThan((short)0).WithMessage("Film length must be greater than 0.")
                .When(x => x.Length.HasValue);
        }
    }

    public class UpdateFilmValidator : AbstractValidator<UpdateFilmDto>
    {
        private static readonly string[] ValidRatings =
            { "G", "PG", "PG-13", "R", "NC-17" };

        public UpdateFilmValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(255).WithMessage("Title cannot exceed 255 characters.");

            RuleFor(x => x.LanguageId)
                .GreaterThan((byte)0).WithMessage("A valid Language ID is required.");

            RuleFor(x => x.RentalDuration)
                .GreaterThan((byte)0).WithMessage("Rental duration must be at least 1 day.");

            RuleFor(x => x.RentalRate)
                .GreaterThanOrEqualTo(0).WithMessage("Rental rate cannot be negative.")
                .LessThanOrEqualTo(99.99m).WithMessage("Rental rate cannot exceed 99.99.");

            RuleFor(x => x.ReplacementCost)
                .GreaterThanOrEqualTo(0).WithMessage("Replacement cost cannot be negative.")
                .LessThanOrEqualTo(999.99m).WithMessage("Replacement cost cannot exceed 999.99.");

            RuleFor(x => x.Rating)
                .Must(r => r == null || ValidRatings.Contains(r))
                .WithMessage($"Rating must be one of: {string.Join(", ", ValidRatings)}");

            RuleFor(x => x.ReleaseYear)
                .Matches(@"^\d{4}$").WithMessage("Release year must be a 4-digit number.")
                .When(x => x.ReleaseYear != null);

            RuleFor(x => x.Length)
                .GreaterThan((short)0).WithMessage("Film length must be greater than 0.")
                .When(x => x.Length.HasValue);
        }
    }

    public class UpdateFilmRateValidator : AbstractValidator<UpdateFilmRateDto>
    {
        public UpdateFilmRateValidator()
        {
            RuleFor(x => x.RentalRate)
                .GreaterThanOrEqualTo(0).WithMessage("Rental rate cannot be negative.")
                .LessThanOrEqualTo(99.99m).WithMessage("Rental rate cannot exceed 99.99.");
        }
    }
}
