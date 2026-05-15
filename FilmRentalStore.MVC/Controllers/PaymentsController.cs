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
        private const int PaymentLookupPageSize = 100;

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

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            ViewBag.IsMine = false;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            var scopedStoreId = GetScopedStoreId();
            var allPayments = scopedStoreId.HasValue
                ? FilterByStore(await GetAllPaymentsForLookupAsync()).ToList()
                : await _paymentService.GetAllAsync(page, pageSize);

            var payments = scopedStoreId.HasValue
                ? allPayments.Skip((page - 1) * pageSize).Take(pageSize).ToList()
                : allPayments;

            ViewBag.HasNextPage = scopedStoreId.HasValue
                ? page * pageSize < allPayments.Count
                : payments.Count == pageSize;

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
            if (!CanAccessStore(payment.Rental?.StoreId)) return NotFound();

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

            if (!await PaymentRequestBelongsToScopedStoreAsync(vm.Payment))
            {
                ModelState.AddModelError("", "Selected payment data is not available for your store.");
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
            customers = FilterByStore(customers).ToList();
            vm.Customers = customers.Select(customer => new SelectListItem
            {
                Value = customer.CustomerId.ToString(),
                Text = $"{customer.FullName} ({customer.Username})",
                Selected = customer.CustomerId == vm.Payment.CustomerId
            }).ToList();

            var staff = await _staffService.GetAllAsync();
            staff = FilterByStore(staff).ToList();
            vm.Staff = staff.Where(member => member.Active).Select(member => new SelectListItem
            {
                Value = member.StaffId.ToString(),
                Text = $"{member.FirstName} {member.LastName} ({member.Username})",
                Selected = member.StaffId == vm.Payment.StaffId
            }).ToList();

            var rentals = await _rentalService.GetAllAsync(1, 100);
            rentals = FilterByStore(rentals).ToList();
            vm.Rentals = rentals.Select(rental => new SelectListItem
            {
                Value = rental.RentalId.ToString(),
                Text = $"#{rental.RentalId} - {rental.Customer?.FullName ?? "Unknown customer"}",
                Selected = rental.RentalId == vm.Payment.RentalId
            }).ToList();
        }

        private IEnumerable<PaymentResponseDto> FilterByStore(IEnumerable<PaymentResponseDto> payments)
        {
            var scopedStoreId = GetScopedStoreId();
            return scopedStoreId.HasValue
                ? payments.Where(payment => payment.Rental?.StoreId == scopedStoreId.Value)
                : payments;
        }

        private async Task<List<PaymentResponseDto>> GetAllPaymentsForLookupAsync()
        {
            var payments = new List<PaymentResponseDto>();

            for (var page = 1; ; page++)
            {
                var batch = await _paymentService.GetAllAsync(page, PaymentLookupPageSize);
                if (!batch.Any())
                {
                    break;
                }

                payments.AddRange(batch);

                if (batch.Count < PaymentLookupPageSize)
                {
                    break;
                }
            }

            return payments;
        }

        private IEnumerable<FilmRentalStore.MVC.DTOs.Rental.RentalResponseDto> FilterByStore(
            IEnumerable<FilmRentalStore.MVC.DTOs.Rental.RentalResponseDto> rentals)
        {
            var scopedStoreId = GetScopedStoreId();
            return scopedStoreId.HasValue
                ? rentals.Where(rental => rental.Inventory?.StoreId == scopedStoreId.Value)
                : rentals;
        }

        private IEnumerable<FilmRentalStore.MVC.DTOs.Customers.CustomerResponseDto> FilterByStore(
            IEnumerable<FilmRentalStore.MVC.DTOs.Customers.CustomerResponseDto> customers)
        {
            var scopedStoreId = GetScopedStoreId();
            return scopedStoreId.HasValue
                ? customers.Where(customer => customer.StoreId == scopedStoreId.Value)
                : customers;
        }

        private IEnumerable<FilmRentalStore.MVC.DTOs.Staff.StaffResponseDto> FilterByStore(
            IEnumerable<FilmRentalStore.MVC.DTOs.Staff.StaffResponseDto> staff)
        {
            var scopedStoreId = GetScopedStoreId();
            return scopedStoreId.HasValue
                ? staff.Where(member => member.StoreId == scopedStoreId.Value)
                : staff;
        }

        private bool CanAccessStore(int? storeId)
        {
            var scopedStoreId = GetScopedStoreId();
            return !scopedStoreId.HasValue || storeId == scopedStoreId.Value;
        }

        private async Task<bool> PaymentRequestBelongsToScopedStoreAsync(PaymentRequestDto payment)
        {
            var scopedStoreId = GetScopedStoreId();
            if (!scopedStoreId.HasValue)
            {
                return true;
            }

            var rental = await _rentalService.GetByIdAsync(payment.RentalId);
            if (rental?.Inventory?.StoreId != scopedStoreId.Value)
            {
                return false;
            }

            var customer = await _customerService.GetByIdAsync(payment.CustomerId);
            if (customer?.StoreId != scopedStoreId.Value)
            {
                return false;
            }

            var staff = await _staffService.GetAllAsync();
            return staff.Any(member => member.StaffId == payment.StaffId && member.StoreId == scopedStoreId.Value);
        }

        private int? GetScopedStoreId()
        {
            var role = HttpContext.Session.GetString(SessionKeys.Role);
            return role == RoleConstants.Admin ? null : HttpContext.Session.GetInt32(SessionKeys.StoreId);
        }
    }
}
