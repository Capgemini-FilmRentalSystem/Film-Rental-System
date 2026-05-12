using AutoMapper;
using FilmRentalStore.API.DTOs.Store;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Mappings
{
    public class StoreMappingProfile : Profile
    {
        public StoreMappingProfile() 
        {
            CreateMap<StoreRequestDto, Store>()
                .ForMember(dest => dest.StoreId, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.ManagerStaff, opt => opt.Ignore())
                .ForMember(dest => dest.Customers, opt => opt.Ignore())
                .ForMember(dest => dest.Inventories, opt => opt.Ignore())
                .ForMember(dest => dest.Staff, opt => opt.Ignore());

            CreateMap<Store, StoreResponseDto>();

        }
    }
}
