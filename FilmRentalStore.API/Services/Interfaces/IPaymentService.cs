using FilmRentalStore.API.DTOs.Payment;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IPaymentService
    {
        const int DefaultPage = 1;
        const int DefaultPageSize = 10;
        const int MaxPageSize = 100;

        /// <summary>Returns a paged payment result; throws NotFoundException when no payments exist.</summary>
        Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync(int page, int pageSize);

        Task<PaymentResponseDto> GetPaymentByIdAsync(int paymentId);

        Task<IEnumerable<PaymentResponseDto>> GetPaymentsByCustomerIdAsync(int customerId, int page, int pageSize);

        Task<PaymentResponseDto> GetCustomerPaymentByIdAsync(int customerId, int paymentId);

        Task<PaymentResponseDto> CreatePaymentAsync(PaymentRequestDto paymentDto);
    }
}
