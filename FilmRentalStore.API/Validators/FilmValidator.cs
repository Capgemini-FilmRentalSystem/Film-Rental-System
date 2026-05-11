using FilmRentalStore.API.DTOs.Film;
using FluentValidation;

namespace FilmRentalStore.API.Validators
{
    public class FilmDtoValidator : AbstractValidator<FilmDto>
    {
        private static readonly string[] ValidRatings =
            { "G", "PG", "PG-13", "R", "NC-17" };

        private static readonly string[] ValidSpecialFeatures =
            { "Trailers", "Commentaries", "Deleted Scenes", "Behind the Scenes" };

        public FilmDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(255).WithMessage("Title cannot exceed 255 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(65535).WithMessage("Description is too long.")
                .When(x => x.Description != null);

            RuleFor(x => x.ReleaseYear)
                .Matches(@"^\d{4}$").WithMessage("Release year must be exactly 4 digits (e.g. 2005).")
                .Must(y => {
                    int year = int.Parse(y!);
                    return year >= 1888 && year <= DateTime.UtcNow.Year;
                })
                .WithMessage($"Release year must be between 1888 and {DateTime.UtcNow.Year}.")
                .When(x => x.ReleaseYear != null);

            RuleFor(x => x.LanguageId)
                .GreaterThan((byte)0).WithMessage("Language ID is required and must be greater than 0.");

            RuleFor(x => x.OriginalLanguageId)
                .GreaterThan((byte)0).WithMessage("Original Language ID must be greater than 0.")
                .When(x => x.OriginalLanguageId.HasValue);

            RuleFor(x => x.RentalDuration)
                .GreaterThan((byte)0).WithMessage("Rental duration must be at least 1 day.")
                .LessThanOrEqualTo((byte)100).WithMessage("Rental duration cannot exceed 100 days.");

            RuleFor(x => x.RentalRate)
                .GreaterThanOrEqualTo(0).WithMessage("Rental rate cannot be negative.")
                .LessThanOrEqualTo(99.99m).WithMessage("Rental rate cannot exceed 99.99.")
                .PrecisionScale(4, 2, true)
                .WithMessage("Rental rate must have at most 2 decimal places.");

            RuleFor(x => x.Length)
                .GreaterThan((short)0).WithMessage("Film length must be greater than 0 minutes.")
                .LessThanOrEqualTo((short)600).WithMessage("Film length cannot exceed 600 minutes.")
                .When(x => x.Length.HasValue);

            RuleFor(x => x.ReplacementCost)
                .GreaterThanOrEqualTo(0).WithMessage("Replacement cost cannot be negative.")
                .LessThanOrEqualTo(999.99m).WithMessage("Replacement cost cannot exceed 999.99.")
                .PrecisionScale(5, 2, true)
                .WithMessage("Replacement cost must have at most 2 decimal places.");

            RuleFor(x => x.Rating)
                .Must(r => ValidRatings.Contains(r))
                .WithMessage($"Rating must be one of: {string.Join(", ", ValidRatings)}.")
                .When(x => x.Rating != null);

            RuleFor(x => x.SpecialFeatures)
                .Must(sf => {
                    var parts = sf!.Split(',').Select(p => p.Trim()).ToList();
                    return parts.All(p => ValidSpecialFeatures.Contains(p));
                })
                .WithMessage($"Each special feature must be one of: {string.Join(", ", ValidSpecialFeatures)}. " +
                             "Separate multiple values with a comma.")
                .Must(sf => {
                    var parts = sf!.Split(',').Select(p => p.Trim()).ToList();
                    return parts.Count == parts.Distinct().Count();
                })
                .WithMessage("Special features cannot contain duplicate values.")
                .When(x => x.SpecialFeatures != null);
        }
    }

    public class FilmActorAssignDtoValidator : AbstractValidator<FilmActorAssignDto>
    {
        public FilmActorAssignDtoValidator()
        {
            RuleFor(x => x.ActorId)
                .GreaterThan(0).WithMessage("Actor ID is required and must be greater than 0.");
        }
    }

    public class FilmCategoryAssignDtoValidator : AbstractValidator<FilmCategoryAssignDto>
    {
        public FilmCategoryAssignDtoValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan((byte)0).WithMessage("Category ID is required and must be greater than 0.");
        }
    }
}
