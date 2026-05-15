using FilmRentalStore.API.Data;
using FilmRentalStore.API.DTOs.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager,Staff,Customer")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardStatsDto>> GetStats()
        {
            if (User.IsInRole("Admin"))
            {
                return Ok(await BuildAdminStatsAsync());
            }

            var storeId = GetIntClaim("store_id");
            if (!storeId.HasValue)
            {
                return Unauthorized("Invalid store claim.");
            }

            if (User.IsInRole("Customer"))
            {
                var customerId = GetIntClaim("customer_id");
                if (!customerId.HasValue)
                {
                    return Unauthorized("Invalid customer claim.");
                }

                return Ok(await BuildCustomerStatsAsync(storeId.Value, customerId.Value));
            }

            return Ok(await BuildStoreStatsAsync(storeId.Value, includeStaff: User.IsInRole("Manager")));
        }

        private async Task<DashboardStatsDto> BuildAdminStatsAsync()
        {
            var filmCount = await _context.Films.AsNoTracking().CountAsync();
            var storeCount = await _context.Stores.AsNoTracking().CountAsync();
            var availableCopyCount = await _context.Inventories
                .AsNoTracking()
                .CountAsync(item => !item.Rentals.Any(rental => rental.ReturnDate == null));
            var activeRentalCount = await _context.Rentals
                .AsNoTracking()
                .CountAsync(rental => rental.ReturnDate == null);

            return new DashboardStatsDto
            {
                FilmCount = filmCount,
                StoreCount = storeCount,
                AvailableCopyCount = availableCopyCount,
                ActiveRentalCount = activeRentalCount
            };
        }

        private async Task<DashboardStatsDto> BuildStoreStatsAsync(int storeId, bool includeStaff)
        {
            var inventory = _context.Inventories
                .AsNoTracking()
                .Where(item => item.StoreId == storeId);

            var filmCount = await inventory
                .Select(item => item.FilmId)
                .Distinct()
                .CountAsync();
            var availableCopyCount = await inventory
                .CountAsync(item => !item.Rentals.Any(rental => rental.ReturnDate == null));
            var activeRentalCount = await _context.Rentals
                .AsNoTracking()
                .CountAsync(rental => rental.ReturnDate == null && rental.Inventory.StoreId == storeId);
            var activeCustomerCount = await _context.Customers
                .AsNoTracking()
                .CountAsync(customer => customer.StoreId == storeId && customer.Active == "Y");
            var activeStaffCount = includeStaff
                ? await _context.Staff
                    .AsNoTracking()
                    .CountAsync(staff => staff.StoreId == storeId && staff.Active && staff.Role.RoleTitle == "Staff")
                : 0;

            return new DashboardStatsDto
            {
                FilmCount = filmCount,
                AvailableCopyCount = availableCopyCount,
                ActiveRentalCount = activeRentalCount,
                ActiveCustomerCount = activeCustomerCount,
                ActiveStaffCount = activeStaffCount
            };
        }

        private async Task<DashboardStatsDto> BuildCustomerStatsAsync(int storeId, int customerId)
        {
            var inventory = _context.Inventories
                .AsNoTracking()
                .Where(item => item.StoreId == storeId);

            var filmCount = await inventory
                .Where(item => !item.Rentals.Any(rental => rental.ReturnDate == null))
                .Select(item => item.FilmId)
                .Distinct()
                .CountAsync();
            var availableCopyCount = await inventory
                .CountAsync(item => !item.Rentals.Any(rental => rental.ReturnDate == null));
            var activeRentalCount = await _context.Rentals
                .AsNoTracking()
                .CountAsync(rental => rental.CustomerId == customerId && rental.ReturnDate == null);
            var paymentCount = await _context.Payments
                .AsNoTracking()
                .CountAsync(payment => payment.CustomerId == customerId);
            var paymentAmount = await _context.Payments
                .AsNoTracking()
                .Where(payment => payment.CustomerId == customerId)
                .SumAsync(payment => (decimal?)payment.Amount);

            return new DashboardStatsDto
            {
                FilmCount = filmCount,
                AvailableCopyCount = availableCopyCount,
                ActiveRentalCount = activeRentalCount,
                PaymentCount = paymentCount,
                PaymentAmount = paymentAmount ?? 0m
            };
        }

        private int? GetIntClaim(string claimType)
        {
            var value = User.FindFirstValue(claimType);
            return int.TryParse(value, out var parsed) ? parsed : null;
        }
    }
}
