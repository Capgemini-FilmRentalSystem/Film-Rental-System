using FilmRentalStore.API.DTOs.Payment;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync();

        Task<PaymentResponseDto> GetPaymentByIdAsync(int paymentId);

        Task<PaymentResponseDto> CreatePaymentAsync(PaymentCreateDto paymentDto);
    }
}