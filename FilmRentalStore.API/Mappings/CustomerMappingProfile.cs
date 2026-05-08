using AutoMapper;
using FilmRentalStore.API.DTOs.Customers;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Mappings
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            // ── Customer → CustomerResponseDto ────────────────────────────────
            CreateMap<Customer, CustomerResponseDto>()
                .ForMember(dest => dest.IsActive,
                           opt => opt.MapFrom(src => src.Active == "Y"));

            // ── CustomerCreateDto → Customer ──────────────────────────────────
            // Address properties are handled separately in the service
            CreateMap<CustomerCreateDto, Customer>()
                .ForMember(dest => dest.AddressId, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore());

            // ── CustomerUpdateDto → Customer (patch existing entity) ──────────
            // Ignores null source values so we never accidentally blank a field
            // Address properties are handled separately in the service
            CreateMap<CustomerUpdateDto, Customer>()
                .ForMember(dest => dest.AddressId, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, srcMember) => srcMember != null));

            // ── Customer → CustomerAddressDto ─────────────────────────────────
            // Uses scaffolded property names: Address1, City1, Country1
            CreateMap<Customer, CustomerAddressDto>()
                .ForMember(dest => dest.CustomerName,
                           opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.AddressLine,
                           opt => opt.MapFrom(src => src.Address.Address1))       // scaffolded name
                .ForMember(dest => dest.Address2,
                           opt => opt.MapFrom(src => src.Address.Address2))
                .ForMember(dest => dest.District,
                           opt => opt.MapFrom(src => src.Address.District))
                .ForMember(dest => dest.PostalCode,
                           opt => opt.MapFrom(src => src.Address.PostalCode))
                .ForMember(dest => dest.Phone,
                           opt => opt.MapFrom(src => src.Address.Phone))
                .ForMember(dest => dest.City,
                           opt => opt.MapFrom(src => src.Address.City.City1))     // scaffolded name
                .ForMember(dest => dest.Country,
                           opt => opt.MapFrom(src => src.Address.City.Country.Country1)); // scaffolded name
        }
    }
}