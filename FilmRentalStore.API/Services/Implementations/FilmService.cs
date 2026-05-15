using AutoMapper;
using FilmRentalStore.API.DTOs.Film;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;

namespace FilmRentalStore.API.Services.Implementations
{
    public class FilmService : IFilmService
    {
        private readonly IFilmRepository _filmRepository;
        private readonly IActorRepository _actorRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IMapper _mapper;

        public FilmService(
            IFilmRepository filmRepository,
            IActorRepository actorRepository,
            ICategoryRepository categoryRepository,
            ILanguageRepository languageRepository,
            IMapper mapper)
        {
            _filmRepository = filmRepository;
            _actorRepository = actorRepository;
            _categoryRepository = categoryRepository;
            _languageRepository = languageRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FilmResponseDto>> GetAllFilmsAsync(int page, int pageSize)
        {
            if (page <= 0)
                throw new BadRequestException("Page must be greater than zero.");

            if (pageSize <= 0)
                throw new BadRequestException("Page size must be greater than zero.");

            if (pageSize > IFilmService.MaxPageSize)
                throw new BadRequestException($"Page size cannot be greater than {IFilmService.MaxPageSize}.");

            var (films, _) = await _filmRepository.GetAllAsync(page, pageSize);

            if (films is null || !films.Any())
                throw new NotFoundException("No films found");

            return _mapper.Map<IEnumerable<FilmResponseDto>>(films);
        }

        public async Task<IEnumerable<FilmResponseDto>> GetAllFilmsAsync()
        {
            var films = await _filmRepository.GetAllAsync();

            if (films is null || !films.Any())
                throw new NotFoundException("No films found");

            return _mapper.Map<IEnumerable<FilmResponseDto>>(films);
        }

        public async Task<FilmResponseDto> GetFilmByIdAsync(int filmId)
        {
            var film = await _filmRepository.GetByIdAsync(filmId);

            if (film == null)
                throw new NotFoundException("Film not found");

            return _mapper.Map<FilmResponseDto>(film);
        }

        public async Task<FilmResponseDto> CreateFilmAsync(FilmRequestDto filmDto)
        {
            if (filmDto == null)
                throw new BadRequestException("Film data is required");

            var languageExists = await _languageRepository.LanguageExistsAsync(filmDto.LanguageId);

            if (!languageExists)
                throw new BadRequestException("Invalid language id");

            if (filmDto.OriginalLanguageId.HasValue)
            {
                var originalLanguageExists =
                    await _languageRepository.LanguageExistsAsync(filmDto.OriginalLanguageId.Value);

                if (!originalLanguageExists)
                    throw new BadRequestException("Invalid original language id");
            }

            await ValidateActorIdsAsync(filmDto.ActorIds);
            await ValidateCategoryIdsAsync(filmDto.CategoryIds);

            var film = _mapper.Map<Film>(filmDto);

            film.LastUpdate = DateTime.Now;

            await _filmRepository.AddAsync(film);
            await _filmRepository.SaveChangesAsync();

            if (filmDto.ActorIds != null)
            {
                await _filmRepository.ReplaceActorsAsync(film.FilmId, filmDto.ActorIds);
            }

            if (filmDto.CategoryIds != null)
            {
                await _filmRepository.ReplaceCategoriesAsync(film.FilmId, filmDto.CategoryIds);
            }

            if (filmDto.ActorIds != null || filmDto.CategoryIds != null)
            {
                await _filmRepository.SaveChangesAsync();
            }

            var createdFilm = await _filmRepository.GetByIdAsync(film.FilmId);

            if (createdFilm == null)
                throw new NotFoundException("Created film record not found");

            return _mapper.Map<FilmResponseDto>(createdFilm);
        }

        public async Task<FilmResponseDto> UpdateFilmAsync(int filmId, FilmRequestDto filmDto)
        {
            if (filmDto == null)
                throw new BadRequestException("Film data is required");

            var film = await _filmRepository.GetEntityByIdAsync(filmId);

            if (film == null)
                throw new NotFoundException("Film not found");

            var languageExists = await _languageRepository.LanguageExistsAsync(filmDto.LanguageId);

            if (!languageExists)
                throw new BadRequestException("Invalid language id");

            if (filmDto.OriginalLanguageId.HasValue)
            {
                var originalLanguageExists =
                    await _languageRepository.LanguageExistsAsync(filmDto.OriginalLanguageId.Value);

                if (!originalLanguageExists)
                    throw new BadRequestException("Invalid original language id");
            }

            await ValidateActorIdsAsync(filmDto.ActorIds);
            await ValidateCategoryIdsAsync(filmDto.CategoryIds);

            _mapper.Map(filmDto, film);

            film.LastUpdate = DateTime.Now;

            _filmRepository.Update(film);

            if (filmDto.ActorIds != null)
            {
                await _filmRepository.ReplaceActorsAsync(filmId, filmDto.ActorIds);
            }

            if (filmDto.CategoryIds != null)
            {
                await _filmRepository.ReplaceCategoriesAsync(filmId, filmDto.CategoryIds);
            }

            await _filmRepository.SaveChangesAsync();

            var updatedFilm = await _filmRepository.GetByIdAsync(filmId);

            if (updatedFilm == null)
                throw new NotFoundException("Updated film record not found");

            return _mapper.Map<FilmResponseDto>(updatedFilm);
        }

        public async Task AssignActorToFilmAsync(int filmId, FilmActorAssignRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Actor assignment data is required");

            var filmExists = await _filmRepository.FilmExistsAsync(filmId);

            if (!filmExists)
                throw new NotFoundException("Film not found");

            var actorExists = await _actorRepository.ActorExistsAsync(dto.ActorId);

            if (!actorExists)
                throw new NotFoundException("Actor not found");

            var alreadyAssigned = await _filmRepository.IsActorAssignedAsync(filmId, dto.ActorId);

            if (alreadyAssigned)
                throw new ConflictException("Actor is already assigned to this film");

            await _filmRepository.AssignActorAsync(filmId, dto.ActorId);
            await _filmRepository.SaveChangesAsync();
        }

        public async Task RemoveActorFromFilmAsync(int filmId, int actorId)
        {
            var filmExists = await _filmRepository.FilmExistsAsync(filmId);

            if (!filmExists)
                throw new NotFoundException("Film not found");

            var actorExists = await _actorRepository.ActorExistsAsync(actorId);

            if (!actorExists)
                throw new NotFoundException("Actor not found");

            var assigned = await _filmRepository.IsActorAssignedAsync(filmId, actorId);

            if (!assigned)
                throw new NotFoundException("Actor is not assigned to this film");

            await _filmRepository.RemoveActorAsync(filmId, actorId);
            await _filmRepository.SaveChangesAsync();
        }

        public async Task AssignCategoryToFilmAsync(int filmId, FilmCategoryAssignRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Category assignment data is required");

            var filmExists = await _filmRepository.FilmExistsAsync(filmId);

            if (!filmExists)
                throw new NotFoundException("Film not found");

            var categoryExists = await _categoryRepository.ExistsAsync(dto.CategoryId);

            if (!categoryExists)
                throw new NotFoundException("Category not found");

            var alreadyAssigned = await _filmRepository.IsCategoryAssignedAsync(filmId, dto.CategoryId);

            if (alreadyAssigned)
                throw new ConflictException("Category is already assigned to this film");

            await _filmRepository.AssignCategoryAsync(filmId, dto.CategoryId);
            await _filmRepository.SaveChangesAsync();
        }

        public async Task RemoveCategoryFromFilmAsync(int filmId, byte categoryId)
        {
            var filmExists = await _filmRepository.FilmExistsAsync(filmId);

            if (!filmExists)
                throw new NotFoundException("Film not found");

            var categoryExists = await _categoryRepository.ExistsAsync(categoryId);

            if (!categoryExists)
                throw new NotFoundException("Category not found");

            var assigned = await _filmRepository.IsCategoryAssignedAsync(filmId, categoryId);

            if (!assigned)
                throw new NotFoundException("Category is not assigned to this film");

            await _filmRepository.RemoveCategoryAsync(filmId, categoryId);
            await _filmRepository.SaveChangesAsync();
        }

        private async Task ValidateActorIdsAsync(IEnumerable<int>? actorIds)
        {
            if (actorIds == null)
                return;

            foreach (var actorId in actorIds.Distinct())
            {
                var actorExists = await _actorRepository.ActorExistsAsync(actorId);

                if (!actorExists)
                    throw new NotFoundException($"Actor with ID {actorId} was not found");
            }
        }

        private async Task ValidateCategoryIdsAsync(IEnumerable<byte>? categoryIds)
        {
            if (categoryIds == null)
                return;

            foreach (var categoryId in categoryIds.Distinct())
            {
                var categoryExists = await _categoryRepository.ExistsAsync(categoryId);

                if (!categoryExists)
                    throw new NotFoundException($"Category with ID {categoryId} was not found");
            }
        }
    }
}
