using AutoMapper;
using FilmRentalStore.API.DTOs;
using FilmRentalStore.API.DTOs.Address;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Mappings
{
    public class CommonMappingProfile : Profile
    {
        public CommonMappingProfile()
        {
            CreateMap<Address, AddressResponseDto>()
                .ForMember(dest => dest.AddressLine,
                    opt => opt.MapFrom(src => src.Address1))
                .ForMember(dest => dest.City,
                    opt => opt.MapFrom(src => src.City != null ? src.City.City1 : string.Empty))
                .ForMember(dest => dest.Country,
                    opt => opt.MapFrom(src => src.City != null && src.City.Country != null
                        ? src.City.Country.Country1
                        : string.Empty));

            CreateMap<Role, RoleSummaryDto>();

            CreateMap<Staff, StaffSummaryDto>()
                .ForMember(dest => dest.RoleTitle,
                    opt => opt.MapFrom(src => src.Role != null ? src.Role.RoleTitle : string.Empty));

            CreateMap<Store, StoreSummaryDto>();

            CreateMap<Customer, CustomerSummaryDto>()
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom(src => src.Active == "Y"));

            CreateMap<Language, LanguageSummaryDto>();
            CreateMap<Film, FilmSummaryDto>();
            CreateMap<Actor, ActorSummaryDto>();
            CreateMap<Category, CategorySummaryDto>();
            CreateMap<Inventory, InventorySummaryDto>();

            CreateMap<Rental, RentalSummaryDto>()
                .ForMember(dest => dest.Film,
                    opt => opt.MapFrom(src => src.Inventory.Film));
        }
    }
}
