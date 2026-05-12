using AutoMapper;
using FilmRentalStore.API.DTOs.Customers;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Mappings
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<Customer, CustomerResponseDto>()
                .ForMember(dest => dest.IsActive,
                           opt => opt.MapFrom(src => src.Active == "Y"));

            CreateMap<CustomerRequestDto, Customer>()
                .ForMember(dest => dest.AddressId, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
