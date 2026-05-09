using FilmRentalStore.API.Data;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmRentalStore.API.Repositories.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Customer)
                .Include(p => p.Staff)
                .Include(p => p.Rental)
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(int paymentId)
        {
            return await _context.Payments
                .Include(p => p.Customer)
                .Include(p => p.Staff)
                .Include(p => p.Rental)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task AddAsync(Payment payment)
        {
            payment.PaymentDate = DateTime.Now;
            payment.LastUpdate = DateTime.Now;

            await _context.Payments.AddAsync(payment);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}