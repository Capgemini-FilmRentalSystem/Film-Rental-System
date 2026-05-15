using FilmRentalStore.MVC.DTOs.Payment;

namespace FilmRentalStore.MVC.Services.Interfaces
{
    public interface IPaymentApiService
    {
        Task<List<PaymentResponseDto>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<List<PaymentResponseDto>> GetMineAsync(int page = 1, int pageSize = 10);
        Task<PaymentResponseDto?> GetByIdAsync(int id);
        Task<PaymentResponseDto?> GetMineByIdAsync(int id);
        Task<PaymentResponseDto?> CreateAsync(PaymentRequestDto dto);
    }
}
