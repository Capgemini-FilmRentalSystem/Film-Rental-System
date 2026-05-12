using AutoMapper;
using FilmRentalStore.API.DTOs.Film;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Mappings
{
    public class FilmMappingProfile : Profile
    {
        public FilmMappingProfile()
        {
            CreateMap<FilmRequestDto, Film>()
                .ForMember(dest => dest.FilmId, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Language, opt => opt.Ignore())
                .ForMember(dest => dest.OriginalLanguage, opt => opt.Ignore())
                .ForMember(dest => dest.FilmActors, opt => opt.Ignore())
                .ForMember(dest => dest.FilmCategories, opt => opt.Ignore())
                .ForMember(dest => dest.Inventories, opt => opt.Ignore());

            CreateMap<Film, FilmResponseDto>();
        }
    }
}
