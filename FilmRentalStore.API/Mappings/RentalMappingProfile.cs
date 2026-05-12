using AutoMapper;
using FilmRentalStore.API.DTOs.Rental;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Mappings
{
    public class RentalMappingProfile : Profile
    {
        public RentalMappingProfile()
        {
            CreateMap<RentalRequestDto, Rental>()
                .ForMember(dest => dest.RentalId, opt => opt.Ignore())
                .ForMember(dest => dest.RentalDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.ReturnDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Inventory, opt => opt.Ignore())
                .ForMember(dest => dest.Staff, opt => opt.Ignore())
                .ForMember(dest => dest.Payments, opt => opt.Ignore());

            CreateMap<Rental, RentalResponseDto>();
        }
    }
}
