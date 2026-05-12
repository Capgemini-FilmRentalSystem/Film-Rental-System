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

        public async Task<(IEnumerable<Payment> Payments, int TotalCount)> GetAllAsync(int page, int pageSize)
        {
            var totalCount = await _context.Payments.CountAsync();

            var payments = await _context.Payments
                .AsNoTracking()
                .Include(p => p.Customer)
                .Include(p => p.Staff)
                    .ThenInclude(s => s.Role)
                .Include(p => p.Rental)
                    .ThenInclude(r => r!.Inventory)
                        .ThenInclude(i => i.Film)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (payments, totalCount);
        }

        public async Task<Payment?> GetByIdAsync(int paymentId)
        {
            return await _context.Payments
                .AsNoTracking()
                .Include(p => p.Customer)
                .Include(p => p.Staff)
                    .ThenInclude(s => s.Role)
                .Include(p => p.Rental)
                    .ThenInclude(r => r!.Inventory)
                        .ThenInclude(i => i.Film)
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
