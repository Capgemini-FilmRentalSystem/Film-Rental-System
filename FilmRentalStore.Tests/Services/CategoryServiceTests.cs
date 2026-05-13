using AutoMapper;
using FilmRentalStore.API.DTOs.Category;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Implementations;
using Moq;
using Xunit;

namespace FilmRentalStore.API.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _service = new CategoryService(_categoryRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenCategoriesExist_ReturnsMappedCategories()
    {
        var categories = new List<Category> { new() { CategoryId = 1, Name = "Action" } };
        var response = new List<CategoryResponseDto> { new() { CategoryId = 1, Name = "Action" } };

        _categoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);
        _mapper.Setup(m => m.Map<IEnumerable<CategoryResponseDto>>(categories)).Returns(response);

        var result = await _service.GetAllAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ReturnsMappedCategory()
    {
        var category = new Category { CategoryId = 2, Name = "Comedy" };
        var response = new CategoryResponseDto { CategoryId = 2, Name = "Comedy" };

        _categoryRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(category);
        _mapper.Setup(m => m.Map<CategoryResponseDto>(category)).Returns(response);

        var result = await _service.GetByIdAsync(2);

        Assert.Equal("Comedy", result.Name);
    }

    [Fact]
    public async Task CreateAsync_WhenValidDto_AddsCategoryAndReturnsCreatedCategory()
    {
        var dto = new CategoryRequestDto { Name = "Drama" };
        var category = new Category { CategoryId = 3, Name = "Drama" };
        var response = new CategoryResponseDto { CategoryId = 3, Name = "Drama" };

        _categoryRepository.Setup(r => r.NameExistsAsync("Drama")).ReturnsAsync(false);
        _mapper.Setup(m => m.Map<Category>(dto)).Returns(category);
        _categoryRepository.Setup(r => r.AddAsync(category)).ReturnsAsync(category);
        _categoryRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _categoryRepository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(category);
        _mapper.Setup(m => m.Map<CategoryResponseDto>(category)).Returns(response);

        var result = await _service.CreateAsync(dto);

        Assert.Equal(3, result.CategoryId);
        _categoryRepository.Verify(r => r.AddAsync(category), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenValidDto_UpdatesCategoryAndReturnsUpdatedCategory()
    {
        var dto = new CategoryRequestDto { Name = "Sci-Fi" };
        var category = new Category { CategoryId = 4, Name = "Old" };
        var response = new CategoryResponseDto { CategoryId = 4, Name = "Sci-Fi" };

        _categoryRepository.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(category);
        _mapper.Setup(m => m.Map<CategoryRequestDto, Category>(dto, category)).Returns(category);
        _categoryRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mapper.Setup(m => m.Map<CategoryResponseDto>(category)).Returns(response);

        var result = await _service.UpdateAsync(4, dto);

        Assert.Equal(4, result.CategoryId);
        _categoryRepository.Verify(r => r.Update(category), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryReturnsEmpty_ThrowsNotFoundException()
    {
        _categoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category>());

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAllAsync());
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ThrowsNotFoundException()
    {
        _categoryRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));
    }

    [Fact]
    public async Task CreateAsync_WhenDtoIsNull_ThrowsBadRequestException()
    {
        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateAsync(null!));
    }

    [Fact]
    public async Task CreateAsync_WhenNameAlreadyExists_ThrowsConflictException()
    {
        var dto = new CategoryRequestDto { Name = "Action" };
        _categoryRepository.Setup(r => r.NameExistsAsync("Action")).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync(dto));
    }
}
