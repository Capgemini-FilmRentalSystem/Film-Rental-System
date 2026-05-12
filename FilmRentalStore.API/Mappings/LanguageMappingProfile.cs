using AutoMapper;
using FilmRentalStore.API.DTOs.Language;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Mappings
{
    public class LanguageMappingProfile : Profile
    {
        public LanguageMappingProfile()
        {
            CreateMap<LanguageRequestDto, Language>()
                .ForMember(dest => dest.LanguageId, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.FilmLanguages, opt => opt.Ignore())
                .ForMember(dest => dest.FilmOriginalLanguages, opt => opt.Ignore());

            CreateMap<Language, LanguageResponseDto>();
        }
    }
}
