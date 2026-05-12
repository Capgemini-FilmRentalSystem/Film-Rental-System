using AutoMapper;
using FilmRentalStore.API.DTOs.Payment;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Mappings
{
    public class PaymentMappingProfile : Profile
    {
        public PaymentMappingProfile()
        {
            CreateMap<PaymentRequestDto, Payment>()
                .ForMember(dest => dest.PaymentId, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.LastUpdate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Staff, opt => opt.Ignore())
                .ForMember(dest => dest.Rental, opt => opt.Ignore());

            CreateMap<Payment, PaymentResponseDto>();
        }
    }
}
