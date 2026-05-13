using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<(IEnumerable<Payment> Payments, int TotalCount)> GetAllAsync(int page, int pageSize);

        Task<Payment?> GetByIdAsync(int paymentId);

        Task<IEnumerable<Payment>> GetByCustomerIdAsync(int customerId, int page, int pageSize);

        Task<Payment?> GetByIdForCustomerAsync(int paymentId, int customerId);

        Task AddAsync(Payment payment);

        Task<bool> SaveChangesAsync();
    }
}
