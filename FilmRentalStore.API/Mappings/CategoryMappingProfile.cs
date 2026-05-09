using AutoMapper;
using FilmRentalStore.API.DTOs.Category;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Mappings
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.FilmCategories, opt => opt.Ignore());

            CreateMap<Category, CategoryResponseDto>();
        }
    }
}
