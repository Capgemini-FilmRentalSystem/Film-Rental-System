using AutoMapper;
using FilmRentalStore.API.DTOs.Film;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Implementations;
using Moq;
using Xunit;

namespace FilmRentalStore.API.Tests.Services;

public class FilmServiceTests
{
    private readonly Mock<IFilmRepository> _filmRepository = new();
    private readonly Mock<IActorRepository> _actorRepository = new();
    private readonly Mock<ICategoryRepository> _categoryRepository = new();
    private readonly Mock<ILanguageRepository> _languageRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly FilmService _service;

    public FilmServiceTests()
    {
        _service = new FilmService(
            _filmRepository.Object,
            _actorRepository.Object,
            _categoryRepository.Object,
            _languageRepository.Object,
            _mapper.Object);
    }

    [Fact]
    public async Task GetAllFilmsAsync_WhenFilmsExist_ReturnsMappedFilms()
    {
        var films = new List<Film> { new() { FilmId = 1, Title = "Film A", LanguageId = 1 } };
        var response = new List<FilmResponseDto> { new() { FilmId = 1, Title = "Film A" } };

        _filmRepository.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync((films, films.Count));
        _mapper.Setup(m => m.Map<IEnumerable<FilmResponseDto>>(films)).Returns(response);

        var result = await _service.GetAllFilmsAsync(1, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetFilmByIdAsync_WhenFilmExists_ReturnsMappedFilm()
    {
        var film = new Film { FilmId = 2, Title = "Film B", LanguageId = 1 };
        var response = new FilmResponseDto { FilmId = 2, Title = "Film B" };

        _filmRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(film);
        _mapper.Setup(m => m.Map<FilmResponseDto>(film)).Returns(response);

        var result = await _service.GetFilmByIdAsync(2);

        Assert.Equal(2, result.FilmId);
    }

    [Fact]
    public async Task CreateFilmAsync_WhenValidDto_AddsFilmAndReturnsCreatedFilm()
    {
        var dto = CreateFilmRequestDto();
        var film = new Film { FilmId = 3, Title = dto.Title, LanguageId = dto.LanguageId };
        var response = new FilmResponseDto { FilmId = 3, Title = dto.Title };

        _languageRepository.Setup(r => r.LanguageExistsAsync(dto.LanguageId)).ReturnsAsync(true);
        _mapper.Setup(m => m.Map<Film>(dto)).Returns(film);
        _filmRepository.Setup(r => r.AddAsync(film)).Returns(Task.CompletedTask);
        _filmRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _filmRepository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(film);
        _mapper.Setup(m => m.Map<FilmResponseDto>(film)).Returns(response);

        var result = await _service.CreateFilmAsync(dto);

        Assert.Equal(3, result.FilmId);
        _filmRepository.Verify(r => r.AddAsync(film), Times.Once);
    }

    [Fact]
    public async Task AssignActorToFilmAsync_WhenValidRequest_AssignsActor()
    {
        var dto = new FilmActorAssignRequestDto { ActorId = 5 };

        _filmRepository.Setup(r => r.FilmExistsAsync(10)).ReturnsAsync(true);
        _actorRepository.Setup(r => r.ActorExistsAsync(5)).ReturnsAsync(true);
        _filmRepository.Setup(r => r.IsActorAssignedAsync(10, 5)).ReturnsAsync(false);
        _filmRepository.Setup(r => r.AssignActorAsync(10, 5)).Returns(Task.CompletedTask);
        _filmRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        await _service.AssignActorToFilmAsync(10, dto);

        _filmRepository.Verify(r => r.AssignActorAsync(10, 5), Times.Once);
        _filmRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllFilmsAsync_WhenPageIsInvalid_ThrowsBadRequestException()
    {
        await Assert.ThrowsAsync<BadRequestException>(() => _service.GetAllFilmsAsync(0, 10));
    }

    [Fact]
    public async Task GetFilmByIdAsync_WhenFilmDoesNotExist_ThrowsNotFoundException()
    {
        _filmRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Film?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetFilmByIdAsync(99));
    }

    [Fact]
    public async Task CreateFilmAsync_WhenLanguageDoesNotExist_ThrowsBadRequestException()
    {
        var dto = CreateFilmRequestDto();
        _languageRepository.Setup(r => r.LanguageExistsAsync(dto.LanguageId)).ReturnsAsync(false);

        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateFilmAsync(dto));
    }

    [Fact]
    public async Task AssignActorToFilmAsync_WhenActorAlreadyAssigned_ThrowsConflictException()
    {
        var dto = new FilmActorAssignRequestDto { ActorId = 5 };

        _filmRepository.Setup(r => r.FilmExistsAsync(10)).ReturnsAsync(true);
        _actorRepository.Setup(r => r.ActorExistsAsync(5)).ReturnsAsync(true);
        _filmRepository.Setup(r => r.IsActorAssignedAsync(10, 5)).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() => _service.AssignActorToFilmAsync(10, dto));
    }

    private static FilmRequestDto CreateFilmRequestDto()
    {
        return new FilmRequestDto
        {
            Title = "Test Film",
            Description = "Test Description",
            ReleaseYear = "2024",
            LanguageId = 1,
            RentalDuration = 3,
            RentalRate = 4.99m,
            ReplacementCost = 19.99m,
            Rating = "PG"
        };
    }
}
