using FilmRentalStore.API.DTOs.Film;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;
using static FilmRentalStore.API.Exceptions.Exceptions;

namespace FilmRentalStore.API.Services.Implementations
{
    public class FilmService : IFilmService
    {
        private readonly IFilmRepository _filmRepo;

        private static readonly string[] ValidRatings =
            { "G", "PG", "PG-13", "R", "NC-17" };

        private static readonly string[] ValidSpecialFeatures =
            { "Trailers", "Commentaries", "Deleted Scenes", "Behind the Scenes" };

        public FilmService(IFilmRepository filmRepo)
        {
            _filmRepo = filmRepo;
        }

        public async Task<IEnumerable<FilmResponseDto>> GetAllAsync()
        {
            var films = await _filmRepo.GetAllAsync();
            return films.Select(MapToResponse);
        }

        public async Task<FilmDetailResponseDto> GetByIdAsync(int id)
        {
            var film = await _filmRepo.GetByIdWithDetailsAsync(id)
                ?? throw new NotFoundException("Film", id);
            return MapToDetailResponse(film);
        }

        public async Task<IEnumerable<FilmResponseDto>> SearchByTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new BadRequestException("Search title cannot be empty.");
            var films = await _filmRepo.SearchByTitleAsync(title);
            return films.Select(MapToResponse);
        }

        public async Task<IEnumerable<FilmResponseDto>> GetByRatingAsync(string rating)
        {
            if (!ValidRatings.Contains(rating))
                throw new BadRequestException($"Invalid rating. Allowed: {string.Join(", ", ValidRatings)}");
            var films = await _filmRepo.GetByRatingAsync(rating);
            return films.Select(MapToResponse);
        }

        public async Task<IEnumerable<FilmResponseDto>> GetByCategoryAsync(byte categoryId)
        {
            var films = await _filmRepo.GetByCategoryAsync(categoryId);
            return films.Select(MapToResponse);
        }

        public async Task<IEnumerable<FilmResponseDto>> GetByActorAsync(int actorId)
        {
            var films = await _filmRepo.GetByActorAsync(actorId);
            return films.Select(MapToResponse);
        }

        public async Task<IEnumerable<FilmResponseDto>> GetByLanguageAsync(byte languageId)
        {
            var films = await _filmRepo.GetByLanguageAsync(languageId);
            return films.Select(MapToResponse);
        }

        public async Task<IEnumerable<FilmResponseDto>> GetByReleaseYearAsync(string year)
        {
            if (year.Length != 4 || !year.All(char.IsDigit))
                throw new BadRequestException("Release year must be a 4-digit number.");
            var films = await _filmRepo.GetByReleaseYearAsync(year);
            return films.Select(MapToResponse);
        }

        public async Task<FilmResponseDto> CreateAsync(CreateFilmDto dto)
        {
            ValidateRatingAndFeatures(dto.Rating, dto.SpecialFeatures);

            var film = new Film
            {
                Title = dto.Title,
                Description = dto.Description,
                ReleaseYear = dto.ReleaseYear,
                LanguageId = dto.LanguageId,
                OriginalLanguageId = dto.OriginalLanguageId,
                RentalDuration = dto.RentalDuration,
                RentalRate = dto.RentalRate,
                Length = dto.Length,
                ReplacementCost = dto.ReplacementCost,
                Rating = dto.Rating,
                SpecialFeatures = dto.SpecialFeatures,
                LastUpdate = DateTime.UtcNow
            };

            var created = await _filmRepo.CreateAsync(film);
            return MapToResponse(created);
        }

        public async Task<FilmResponseDto> UpdateAsync(int id, UpdateFilmDto dto)
        {
            var film = await _filmRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Film", id);

            ValidateRatingAndFeatures(dto.Rating, dto.SpecialFeatures);

            film.Title = dto.Title;
            film.Description = dto.Description;
            film.ReleaseYear = dto.ReleaseYear;
            film.LanguageId = dto.LanguageId;
            film.OriginalLanguageId = dto.OriginalLanguageId;
            film.RentalDuration = dto.RentalDuration;
            film.RentalRate = dto.RentalRate;
            film.Length = dto.Length;
            film.ReplacementCost = dto.ReplacementCost;
            film.Rating = dto.Rating;
            film.SpecialFeatures = dto.SpecialFeatures;

            var updated = await _filmRepo.UpdateAsync(film);
            return MapToResponse(updated);
        }

        public async Task DeleteAsync(int id)
        {
            var film = await _filmRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Film", id);

            var inventoryCount = await _filmRepo.GetInventoryCountAsync(id);
            if (inventoryCount > 0)
                throw new ConflictException(
                    $"Cannot delete film '{film.Title}': it has {inventoryCount} inventory record(s). Remove inventory first.");

            await _filmRepo.DeleteAsync(film);
        }

        public async Task UpdateRentalRateAsync(int id, UpdateFilmRateDto dto)
        {
            if (!await _filmRepo.ExistsAsync(id))
                throw new NotFoundException("Film", id);

            await _filmRepo.UpdateRentalRateAsync(id, dto.RentalRate);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static void ValidateRatingAndFeatures(string? rating, string? specialFeatures)
        {
            if (rating != null && !ValidRatings.Contains(rating))
                throw new BadRequestException(
                    $"Invalid rating '{rating}'. Allowed values: {string.Join(", ", ValidRatings)}");

            if (specialFeatures != null)
            {
                var parts = specialFeatures.Split(',').Select(p => p.Trim());
                var invalid = parts.Where(p => !ValidSpecialFeatures.Contains(p)).ToList();
                if (invalid.Any())
                    throw new BadRequestException(
                        $"Invalid special feature(s): {string.Join(", ", invalid)}. " +
                        $"Allowed: {string.Join(", ", ValidSpecialFeatures)}");
            }
        }

        private static FilmResponseDto MapToResponse(Film f) => new()
        {
            FilmId = f.FilmId,
            Title = f.Title,
            Description = f.Description,
            ReleaseYear = f.ReleaseYear,
            LanguageId = f.LanguageId,
            LanguageName = f.Language?.Name?.Trim(),
            OriginalLanguageId = f.OriginalLanguageId,
            RentalDuration = f.RentalDuration,
            RentalRate = f.RentalRate,
            Length = f.Length,
            ReplacementCost = f.ReplacementCost,
            Rating = f.Rating,
            SpecialFeatures = f.SpecialFeatures,
            LastUpdate = f.LastUpdate
        };

        private static FilmDetailResponseDto MapToDetailResponse(Film f) => new()
        {
            FilmId = f.FilmId,
            Title = f.Title,
            Description = f.Description,
            ReleaseYear = f.ReleaseYear,
            LanguageId = f.LanguageId,
            LanguageName = f.Language?.Name?.Trim(),
            OriginalLanguageId = f.OriginalLanguageId,
            OriginalLanguageName = f.OriginalLanguage?.Name?.Trim(),
            RentalDuration = f.RentalDuration,
            RentalRate = f.RentalRate,
            Length = f.Length,
            ReplacementCost = f.ReplacementCost,
            Rating = f.Rating,
            SpecialFeatures = f.SpecialFeatures,
            LastUpdate = f.LastUpdate,
            InventoryCount = f.Inventories?.Count ?? 0,
            Actors = f.FilmActors?.Select(fa => new FilmActorDto
            {
                ActorId = fa.ActorId,
                FullName = $"{fa.Actor.FirstName} {fa.Actor.LastName}"
            }).ToList() ?? new(),
            Categories = f.FilmCategories?.Select(fc => new FilmCategoryDto
            {
                CategoryId = fc.CategoryId,
                Name = fc.Category.Name
            }).ToList() ?? new()
        };
    }
}
