using FilmRentalStore.MVC.ViewModels.Payment;

namespace FilmRentalStore.MVC.Services.Interfaces
{
    public interface IPaymentApiService
    {
        Task<IEnumerable<PaymentViewModel>> GetAllPaymentsAsync(int page, int pageSize);

        Task<IEnumerable<PaymentViewModel>> GetMyPaymentsAsync(int page, int pageSize);

        Task<PaymentViewModel?> GetPaymentByIdAsync(int paymentId);

        Task CreatePaymentAsync(PaymentViewModel model);
    }
}