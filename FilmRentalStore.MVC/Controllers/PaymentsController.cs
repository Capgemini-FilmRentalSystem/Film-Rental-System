using FilmRentalStore.MVC.DTOs.Payment;
using FilmRentalStore.MVC.Filters;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.Controllers
{
    [TokenRequired]
    [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff, RoleConstants.Customer)]
    public class PaymentsController : Controller
    {
        private readonly IPaymentApiService _paymentService;
        private readonly ICustomerApiService _customerService;
        private readonly IStaffApiService _staffService;
        private readonly IRentalApiService _rentalService;

        public PaymentsController(
            IPaymentApiService paymentService,
            ICustomerApiService customerService,
            IStaffApiService staffService,
            IRentalApiService rentalService)
        {
            _paymentService = paymentService;
            _customerService = customerService;
            _staffService = staffService;
            _rentalService = rentalService;
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            ViewBag.IsMine = false;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            var payments = await _paymentService.GetAllAsync(page, pageSize);
            ViewBag.HasNextPage = payments.Count == pageSize;

            return View(payments);
        }

        [RoleAuthorize(RoleConstants.Customer)]
        public async Task<IActionResult> MyPayments(int page = 1, int pageSize = 10)
        {
            ViewBag.IsMine = true;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            var payments = await _paymentService.GetMineAsync(page, pageSize);
            ViewBag.HasNextPage = payments.Count == pageSize;

            return View("Index", payments);
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Details(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null) return NotFound();

            ViewBag.IsMine = false;
            return View(payment);
        }

        [RoleAuthorize(RoleConstants.Customer)]
        public async Task<IActionResult> MyDetails(int id)
        {
            var payment = await _paymentService.GetMineByIdAsync(id);
            if (payment == null) return NotFound();

            ViewBag.IsMine = true;
            return View("Details", payment);
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Create()
        {
            var vm = new PaymentFormViewModel();
            await PopulatePaymentFormOptions(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Create(PaymentFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulatePaymentFormOptions(vm);
                return View(vm);
            }

            var created = await _paymentService.CreateAsync(vm.Payment);
            TempData["Success"] = "Payment created successfully.";
            return created == null
                ? RedirectToAction(nameof(Index))
                : RedirectToAction(nameof(Details), new { id = created.PaymentId });
        }

        private async Task PopulatePaymentFormOptions(PaymentFormViewModel vm)
        {
            var customers = await _customerService.GetActiveAsync();
            vm.Customers = customers.Select(customer => new SelectListItem
            {
                Value = customer.CustomerId.ToString(),
                Text = $"{customer.FullName} ({customer.Username})",
                Selected = customer.CustomerId == vm.Payment.CustomerId
            }).ToList();

            var staff = await _staffService.GetAllAsync();
            vm.Staff = staff.Where(member => member.Active).Select(member => new SelectListItem
            {
                Value = member.StaffId.ToString(),
                Text = $"{member.FirstName} {member.LastName} ({member.Username})",
                Selected = member.StaffId == vm.Payment.StaffId
            }).ToList();

            var rentals = await _rentalService.GetAllAsync(1, 100);
            vm.Rentals = rentals.Select(rental => new SelectListItem
            {
                Value = rental.RentalId.ToString(),
                Text = $"#{rental.RentalId} - {rental.Customer?.FullName ?? "Unknown customer"}",
                Selected = rental.RentalId == vm.Payment.RentalId
            }).ToList();
        }
    }
}
