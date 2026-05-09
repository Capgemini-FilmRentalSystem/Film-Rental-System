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

            CreateMap<CustomerCreateDto, Customer>()
                .ForMember(dest => dest.AddressId, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore());

            CreateMap<CustomerUpdateDto, Customer>()
                .ForMember(dest => dest.AddressId, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Customer, CustomerAddressDto>()
                .ForMember(dest => dest.CustomerName,
                           opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.AddressLine,
                           opt => opt.MapFrom(src => src.Address.Address1))
                .ForMember(dest => dest.Address2,
                           opt => opt.MapFrom(src => src.Address.Address2))
                .ForMember(dest => dest.District,
                           opt => opt.MapFrom(src => src.Address.District))
                .ForMember(dest => dest.PostalCode,
                           opt => opt.MapFrom(src => src.Address.PostalCode))
                .ForMember(dest => dest.Phone,
                           opt => opt.MapFrom(src => src.Address.Phone))
                .ForMember(dest => dest.City,
                           opt => opt.MapFrom(src => src.Address.City.City1))
                .ForMember(dest => dest.Country,
                           opt => opt.MapFrom(src => src.Address.City.Country.Country1));
        }
    }
}