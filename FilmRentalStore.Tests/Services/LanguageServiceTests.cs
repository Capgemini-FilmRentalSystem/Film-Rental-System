using AutoMapper;
using FilmRentalStore.API.DTOs.Language;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Implementations;
using Moq;
using Xunit;

namespace FilmRentalStore.API.Tests.Services;

public class LanguageServiceTests
{
    private readonly Mock<ILanguageRepository> _languageRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly LanguageService _service;

    public LanguageServiceTests()
    {
        _service = new LanguageService(_languageRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetAllLanguagesAsync_WhenLanguagesExist_ReturnsMappedLanguages()
    {
        var languages = new List<Language> { new() { LanguageId = 1, Name = "English" } };
        var response = new List<LanguageResponseDto> { new() { LanguageId = 1, Name = "English" } };

        _languageRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(languages);
        _mapper.Setup(m => m.Map<IEnumerable<LanguageResponseDto>>(languages)).Returns(response);

        var result = await _service.GetAllLanguagesAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task GetLanguageByIdAsync_WhenLanguageExists_ReturnsMappedLanguage()
    {
        var language = new Language { LanguageId = 2, Name = "Hindi" };
        var response = new LanguageResponseDto { LanguageId = 2, Name = "Hindi" };

        _languageRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(language);
        _mapper.Setup(m => m.Map<LanguageResponseDto>(language)).Returns(response);

        var result = await _service.GetLanguageByIdAsync(2);

        Assert.Equal("Hindi", result.Name);
    }

    [Fact]
    public async Task CreateLanguageAsync_WhenValidDto_AddsLanguageAndReturnsCreatedLanguage()
    {
        var dto = new LanguageRequestDto { Name = "French" };
        var language = new Language { LanguageId = 3, Name = "French" };
        var response = new LanguageResponseDto { LanguageId = 3, Name = "French" };

        _languageRepository.Setup(r => r.LanguageNameExistsAsync("French")).ReturnsAsync(false);
        _mapper.Setup(m => m.Map<Language>(dto)).Returns(language);
        _languageRepository.Setup(r => r.AddAsync(language)).Returns(Task.CompletedTask);
        _languageRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _languageRepository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(language);
        _mapper.Setup(m => m.Map<LanguageResponseDto>(language)).Returns(response);

        var result = await _service.CreateLanguageAsync(dto);

        Assert.Equal(3, result.LanguageId);
        _languageRepository.Verify(r => r.AddAsync(language), Times.Once);
    }

    [Fact]
    public async Task UpdateLanguageAsync_WhenValidDto_UpdatesLanguageAndReturnsUpdatedLanguage()
    {
        var dto = new LanguageRequestDto { Name = "Spanish" };
        var language = new Language { LanguageId = 4, Name = "Old" };
        var response = new LanguageResponseDto { LanguageId = 4, Name = "Spanish" };

        _languageRepository.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(language);
        _languageRepository.Setup(r => r.LanguageNameExistsAsync("Spanish")).ReturnsAsync(false);
        _mapper.Setup(m => m.Map<LanguageRequestDto, Language>(dto, language)).Returns(language);
        _languageRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mapper.Setup(m => m.Map<LanguageResponseDto>(language)).Returns(response);

        var result = await _service.UpdateLanguageAsync(4, dto);

        Assert.Equal(4, result.LanguageId);
        _languageRepository.Verify(r => r.Update(language), Times.Once);
    }

    [Fact]
    public async Task GetAllLanguagesAsync_WhenRepositoryReturnsEmpty_ThrowsNotFoundException()
    {
        _languageRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Language>());

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAllLanguagesAsync());
    }

    [Fact]
    public async Task GetLanguageByIdAsync_WhenLanguageDoesNotExist_ThrowsNotFoundException()
    {
        _languageRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Language?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetLanguageByIdAsync(99));
    }

    [Fact]
    public async Task CreateLanguageAsync_WhenNameIsEmpty_ThrowsBadRequestException()
    {
        var dto = new LanguageRequestDto { Name = " " };

        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateLanguageAsync(dto));
    }

    [Fact]
    public async Task UpdateLanguageAsync_WhenNewNameAlreadyExists_ThrowsConflictException()
    {
        var dto = new LanguageRequestDto { Name = "French" };
        var language = new Language { LanguageId = 5, Name = "German" };

        _languageRepository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(language);
        _languageRepository.Setup(r => r.LanguageNameExistsAsync("French")).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() => _service.UpdateLanguageAsync(5, dto));
    }
}
