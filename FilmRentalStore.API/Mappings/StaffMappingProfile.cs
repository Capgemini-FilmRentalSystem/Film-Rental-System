using AutoMapper;
using FilmRentalStore.API.DTOs.Staff;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Mappings
{
    public class StaffMappingProfile : Profile
    {
        public StaffMappingProfile()
        {
            CreateMap<StaffCreateDto, Staff>()
                .ForMember(dest => dest.StaffId, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Picture, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Store, opt => opt.Ignore())
                .ForMember(dest => dest.StoreNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.Payments, opt => opt.Ignore())
                .ForMember(dest => dest.Rentals, opt => opt.Ignore());

            CreateMap<StaffUpdateDto, Staff>()
                .ForMember(dest => dest.StaffId, opt => opt.Ignore())
                .ForMember(dest => dest.Username, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Picture, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Store, opt => opt.Ignore())
                .ForMember(dest => dest.StoreNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.Payments, opt => opt.Ignore())
                .ForMember(dest => dest.Rentals, opt => opt.Ignore());

            CreateMap<Staff, StaffResponseDto>()
                .ForMember(dest => dest.RoleTitle,
                    opt => opt.MapFrom(src => src.Role.RoleTitle));
        }
    }
}
