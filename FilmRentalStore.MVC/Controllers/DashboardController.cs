using FilmRentalStore.MVC.DTOs.Dashboard;
using FilmRentalStore.MVC.Filters;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.MVC.Controllers
{
    [TokenRequired]
    public class DashboardController : Controller
    {
        private readonly IApiClient _apiClient;

        public DashboardController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString(SessionKeys.Role);

            return role switch
            {
                RoleConstants.Admin => RedirectToAction("Admin"),
                RoleConstants.Manager => RedirectToAction("Manager"),
                RoleConstants.Staff => RedirectToAction("Staff"),
                RoleConstants.Customer => RedirectToAction("Customer"),
                _ => RedirectToAction("AccessDenied", "Error")
            };
        }

        [RoleAuthorize(RoleConstants.Admin)]
        public async Task<IActionResult> Admin()
        {
            var stats = await GetStatsAsync();
            var vm = new DashboardViewModel
            {
                Title = "Admin Dashboard",
                Summary = BuildSummary(),
                Cards = new()
                {
                    Card("Films", stats.FilmCount, "Total catalogue titles"),
                    Card("Stores", stats.StoreCount, "Configured locations"),
                    Card("Available Copies", stats.AvailableCopyCount, "Across all stores"),
                    Card("Active Rentals", stats.ActiveRentalCount, "Not returned")
                },
                Links = new()
                {
                    Link("Films", "Films"),
                    Link("Inventory", "Inventory"),
                    Link("Rentals", "Rentals"),
                    Link("Staff", "Staff"),
                    Link("Stores", "Stores")
                }
            };

            return View(vm);
        }

        [RoleAuthorize(RoleConstants.Manager)]
        public async Task<IActionResult> Manager()
        {
            var stats = await GetStatsAsync();
            var vm = new DashboardViewModel
            {
                Title = "Manager Dashboard",
                Summary = BuildSummary(),
                Cards = new()
                {
                    Card("Catalogue Films", stats.FilmCount, "In your store"),
                    Card("Available Copies", stats.AvailableCopyCount, "Ready to rent"),
                    Card("Active Rentals", stats.ActiveRentalCount, "For your store"),
                    Card("Active Staff", stats.ActiveStaffCount, "Assigned staff")
                },
                Links = new()
                {
                    Link("Films", "Films"),
                    Link("Inventory", "Inventory"),
                    Link("Rentals", "Rentals"),
                    Link("Customers", "Customers"),
                    Link("Staff", "Staff")
                }
            };

            return View(vm);
        }

        [RoleAuthorize(RoleConstants.Staff)]
        public async Task<IActionResult> Staff()
        {
            var stats = await GetStatsAsync();
            var vm = new DashboardViewModel
            {
                Title = "Staff Dashboard",
                Summary = BuildSummary(),
                Cards = new()
                {
                    Card("Catalogue Films", stats.FilmCount, "In your store"),
                    Card("Available Copies", stats.AvailableCopyCount, "Ready to rent"),
                    Card("Active Rentals", stats.ActiveRentalCount, "Needs follow-up"),
                    Card("Active Customers", stats.ActiveCustomerCount, "In your store")
                },
                Links = new()
                {
                    Link("Rent Out Film", "Rentals", "Create"),
                    Link("Rentals", "Rentals"),
                    Link("Inventory", "Inventory"),
                    Link("Customers", "Customers"),
                    Link("Payments", "Payments")
                }
            };

            return View(vm);
        }

        [RoleAuthorize(RoleConstants.Customer)]
        public async Task<IActionResult> Customer()
        {
            var stats = await GetStatsAsync();
            var vm = new DashboardViewModel
            {
                Title = "Customer Dashboard",
                Summary = BuildSummary(),
                Cards = new()
                {
                    Card("Available Films", stats.FilmCount, "In your store"),
                    Card("Available Copies", stats.AvailableCopyCount, "Ready to rent"),
                    Card("Current Rentals", stats.ActiveRentalCount, "Not returned"),
                    Card("Total Paid", stats.PaymentAmount.ToString("N2"), $"{stats.PaymentCount} payments")
                },
                Links = new()
                {
                    Link("Browse Films", "Films"),
                    Link("My Rentals", "Rentals", "MyRentals"),
                    Link("My Payments", "Payments", "MyPayments"),
                    Link("My Profile", "Customers", "Profile")
                }
            };

            return View(vm);
        }

        private async Task<DashboardStatsDto> GetStatsAsync()
        {
            try
            {
                return await _apiClient.GetAsync<DashboardStatsDto>(ApiRoutes.Dashboard) ?? new DashboardStatsDto();
            }
            catch
            {
                return new DashboardStatsDto();
            }
        }

        private string? BuildSummary()
        {
            var username = HttpContext.Session.GetString(SessionKeys.Username);
            return string.IsNullOrWhiteSpace(username) ? null : $"Signed in as {username}";
        }

        private static DashboardCardViewModel Card(string label, int value, string? note = null)
        {
            return Card(label, value.ToString(), note);
        }

        private static DashboardCardViewModel Card(string label, string value, string? note = null)
        {
            return new DashboardCardViewModel
            {
                Label = label,
                Value = value,
                Note = note
            };
        }

        private static DashboardLinkViewModel Link(string text, string controller, string action = "Index")
        {
            return new DashboardLinkViewModel
            {
                Text = text,
                Controller = controller,
                Action = action
            };
        }
    }
}
