using AutoMapper;
using FilmRentalStore.API.DTOs.Film;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Mappings
{
    public class FilmMappingProfile : Profile
    {
        public FilmMappingProfile()
        {
            CreateMap<FilmDto, Film>()
                .ForMember(dest => dest.FilmId, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Language, opt => opt.Ignore())
                .ForMember(dest => dest.OriginalLanguage, opt => opt.Ignore())
                .ForMember(dest => dest.FilmActors, opt => opt.Ignore())
                .ForMember(dest => dest.FilmCategories, opt => opt.Ignore())
                .ForMember(dest => dest.Inventories, opt => opt.Ignore());

            CreateMap<Film, FilmResponseDto>()
                .ForMember(dest => dest.LanguageName,
                    opt => opt.MapFrom(src => src.Language.Name))
                .ForMember(dest => dest.OriginalLanguageName,
                    opt => opt.MapFrom(src =>
                        src.OriginalLanguage != null
                            ? src.OriginalLanguage.Name
                            : null));
        }
    }
}