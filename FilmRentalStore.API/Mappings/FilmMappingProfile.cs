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

            CreateMap<Film, FilmResponseDto>()
                .ForMember(dest => dest.CategoryIds,
                    opt => opt.MapFrom(src => src.FilmCategories.Select(fc => fc.CategoryId)))
                .ForMember(dest => dest.ActorIds,
                    opt => opt.MapFrom(src => src.FilmActors.Select(fa => fa.ActorId)))
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.FilmCategories.Select(fc => fc.Category)))
                .ForMember(dest => dest.Actors,
                    opt => opt.MapFrom(src => src.FilmActors.Select(fa => fa.Actor)));
        }
    }
}
