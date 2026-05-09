using AutoMapper;
using FilmRentalStore.API.DTOs.Inventory;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Mappings
{
    public class InventoryMappingProfile : Profile
    {
        public InventoryMappingProfile()
        {
            CreateMap<InventoryDto, Inventory>()
                .ForMember(dest => dest.InventoryId, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Film, opt => opt.Ignore())
                .ForMember(dest => dest.Store, opt => opt.Ignore())
                .ForMember(dest => dest.Rentals, opt => opt.Ignore());

            CreateMap<Inventory, InventoryResponseDto>()
                .ForMember(dest => dest.FilmTitle,
                    opt => opt.MapFrom(src => src.Film.Title))
                .ForMember(dest => dest.IsAvailable,
                    opt => opt.MapFrom(src => !src.Rentals.Any(r => r.ReturnDate == null)));
        }
    }
}