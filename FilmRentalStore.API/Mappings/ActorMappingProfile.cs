using AutoMapper;
using FilmRentalStore.API.DTOs.Actor;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Mappings
{
    public class ActorMappingProfile : Profile
    {
        public ActorMappingProfile()
        {
            CreateMap<ActorRequestDto, Actor>()
                .ForMember(dest => dest.ActorId, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.FilmActors, opt => opt.Ignore());

            CreateMap<Actor, ActorResponseDto>();
        }
    }
}
