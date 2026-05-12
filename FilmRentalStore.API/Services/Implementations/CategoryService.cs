using AutoMapper;
using FilmRentalStore.API.DTOs.Category;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;

namespace FilmRentalStore.API.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepo, IMapper mapper)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            var categories = await _categoryRepo.GetAllAsync();

            if (categories is null || !categories.Any())
                throw new NotFoundException("No categories found.");

            return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        }

        public async Task<CategoryResponseDto> GetByIdAsync(byte id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);

            if (category == null) throw new NotFoundException("Category not found");

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<CategoryResponseDto> CreateAsync(CategoryRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Category data is required.");

            var nameExists = await _categoryRepo.NameExistsAsync(dto.Name);

            if (nameExists)
                throw new ConflictException($"Category '{dto.Name}' already exists.");

            var category = _mapper.Map<Category>(dto);

            category.LastUpdate = DateTime.Now;

            await _categoryRepo.AddAsync(category);
            await _categoryRepo.SaveChangesAsync();

            var createdCategory = await _categoryRepo.GetByIdAsync(category.CategoryId);

            if (createdCategory == null)
                throw new NotFoundException("Created category record not found.");

            return _mapper.Map<CategoryResponseDto>(createdCategory);
        }

        public async Task<CategoryResponseDto> UpdateAsync(byte id, CategoryRequestDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Category data is required.");

            var category = await _categoryRepo.GetByIdAsync(id);

            if (category == null)
                throw new NotFoundException("Category not found.");

            _mapper.Map(dto, category);

            category.LastUpdate = DateTime.Now;

            _categoryRepo.Update(category);
            await _categoryRepo.SaveChangesAsync();

            var updatedCategory = await _categoryRepo.GetByIdAsync(id);

            if (updatedCategory == null)
                throw new NotFoundException("Updated category record not found.");

            return _mapper.Map<CategoryResponseDto>(updatedCategory);
        }
    }
}
