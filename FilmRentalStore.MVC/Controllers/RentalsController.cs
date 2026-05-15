using FilmRentalStore.MVC.DTOs.Rental;
using FilmRentalStore.MVC.Filters;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Rental;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.Controllers
{
    [TokenRequired]
    [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff, RoleConstants.Customer)]
    public class RentalsController : Controller
    {
        private readonly IRentalApiService _rentalService;
        private readonly IInventoryApiService _inventoryService;
        private readonly ICustomerApiService _customerService;
        private readonly IStaffApiService _staffService;
        private const int RentalLookupPageSize = 100;

        public RentalsController(
            IRentalApiService rentalService,
            IInventoryApiService inventoryService,
            ICustomerApiService customerService,
            IStaffApiService staffService)
        {
            _rentalService = rentalService;
            _inventoryService = inventoryService;
            _customerService = customerService;
            _staffService = staffService;
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string sortOrder = "date_desc")
        {
            ViewBag.IsMine = false;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.SortOrder = sortOrder;

            var allRentals = FilterByStore(await GetAllRentalsAsync(false));
            allRentals = SortRentals(allRentals, sortOrder).ToList();
            var rentals = allRentals
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.HasNextPage = page * pageSize < allRentals.Count;

            return View(rentals);
        }

        [RoleAuthorize(RoleConstants.Customer)]
        public async Task<IActionResult> MyRentals(int page = 1, int pageSize = 10, string sortOrder = "date_desc")
        {
            ViewBag.IsMine = true;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.SortOrder = sortOrder;

            var allRentals = SortRentals(await GetAllRentalsAsync(true), sortOrder).ToList();
            var rentals = allRentals
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.HasNextPage = page * pageSize < allRentals.Count;

            return View("Index", rentals);
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Details(int id)
        {
            var rental = await _rentalService.GetByIdAsync(id);
            if (rental == null) return NotFound();
            if (!CanAccessStore(rental.Inventory?.StoreId)) return NotFound();

            ViewBag.IsMine = false;
            return View(rental);
        }

        [RoleAuthorize(RoleConstants.Customer)]
        public async Task<IActionResult> MyDetails(int id)
        {
            var rental = await _rentalService.GetMineByIdAsync(id);
            if (rental == null) return NotFound();

            ViewBag.IsMine = true;
            return View("Details", rental);
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Create()
        {
            var vm = new RentalFormViewModel();
            await PopulateRentalFormOptions(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Create(RentalFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateRentalFormOptions(vm);
                return View(vm);
            }

            if (!await RentalRequestBelongsToScopedStoreAsync(vm.Rental))
            {
                ModelState.AddModelError("", "Selected rental data is not available for your store.");
                await PopulateRentalFormOptions(vm);
                return View(vm);
            }

            var created = await _rentalService.CreateAsync(vm.Rental);
            TempData["Success"] = "Rental created successfully.";
            return created == null
                ? RedirectToAction(nameof(Index))
                : RedirectToAction(nameof(Details), new { id = created.RentalId });
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Return(int id)
        {
            var rental = await _rentalService.GetByIdAsync(id);
            if (rental == null) return NotFound();
            if (!CanAccessStore(rental.Inventory?.StoreId)) return NotFound();

            ViewBag.RentalId = id;
            return View(new RentalReturnRequestDto { ReturnDate = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Return(int id, RentalReturnRequestDto dto)
        {
            var rental = await _rentalService.GetByIdAsync(id);
            if (rental == null) return NotFound();
            if (!CanAccessStore(rental.Inventory?.StoreId)) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.RentalId = id;
                return View(dto);
            }

            await _rentalService.ReturnAsync(id, dto);
            TempData["Success"] = "Rental returned successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task PopulateRentalFormOptions(RentalFormViewModel vm)
        {
            var inventory = await _inventoryService.GetAllAsync(1, 100);
            inventory = FilterByStore(inventory).ToList();
            vm.InventoryItems = inventory.Where(item => item.IsAvailable).Select(item => new SelectListItem
            {
                Value = item.InventoryId.ToString(),
                Text = $"#{item.InventoryId} - {item.Film?.Title ?? "Unknown film"}",
                Selected = item.InventoryId == vm.Rental.InventoryId
            }).ToList();

            var customers = await _customerService.GetActiveAsync();
            customers = FilterByStore(customers).ToList();
            vm.Customers = customers.Select(customer => new SelectListItem
            {
                Value = customer.CustomerId.ToString(),
                Text = $"{customer.FullName} ({customer.Username})",
                Selected = customer.CustomerId == vm.Rental.CustomerId
            }).ToList();

            var staff = await _staffService.GetAllAsync();
            staff = FilterByStore(staff).ToList();
            vm.Staff = staff.Where(member => member.Active).Select(member => new SelectListItem
            {
                Value = member.StaffId.ToString(),
                Text = $"{member.FirstName} {member.LastName} ({member.Username})",
                Selected = member.StaffId == vm.Rental.StaffId
            }).ToList();
        }

        private async Task<List<RentalResponseDto>> GetAllRentalsAsync(bool mine)
        {
            var rentals = new List<RentalResponseDto>();

            for (var page = 1; ; page++)
            {
                var batch = mine
                    ? await _rentalService.GetMineAsync(page, RentalLookupPageSize)
                    : await _rentalService.GetAllAsync(page, RentalLookupPageSize);

                if (!batch.Any())
                {
                    break;
                }

                rentals.AddRange(batch);

                if (batch.Count < RentalLookupPageSize)
                {
                    break;
                }
            }

            return rentals;
        }

        private List<RentalResponseDto> FilterByStore(IEnumerable<RentalResponseDto> rentals)
        {
            var scopedStoreId = GetScopedStoreId();
            return scopedStoreId.HasValue
                ? rentals.Where(rental => rental.Inventory?.StoreId == scopedStoreId.Value).ToList()
                : rentals.ToList();
        }

        private IEnumerable<FilmRentalStore.MVC.DTOs.Inventory.InventoryResponseDto> FilterByStore(
            IEnumerable<FilmRentalStore.MVC.DTOs.Inventory.InventoryResponseDto> inventory)
        {
            var scopedStoreId = GetScopedStoreId();
            return scopedStoreId.HasValue
                ? inventory.Where(item => item.StoreId == scopedStoreId.Value)
                : inventory;
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

        private async Task<bool> RentalRequestBelongsToScopedStoreAsync(RentalRequestDto rental)
        {
            var scopedStoreId = GetScopedStoreId();
            if (!scopedStoreId.HasValue)
            {
                return true;
            }

            var inventory = await _inventoryService.GetByIdAsync(rental.InventoryId);
            if (inventory?.StoreId != scopedStoreId.Value)
            {
                return false;
            }

            var customer = await _customerService.GetByIdAsync(rental.CustomerId);
            if (customer?.StoreId != scopedStoreId.Value)
            {
                return false;
            }

            var staff = await _staffService.GetAllAsync();
            return staff.Any(member => member.StaffId == rental.StaffId && member.StoreId == scopedStoreId.Value);
        }

        private int? GetScopedStoreId()
        {
            var role = HttpContext.Session.GetString(SessionKeys.Role);
            return role == RoleConstants.Admin ? null : HttpContext.Session.GetInt32(SessionKeys.StoreId);
        }

        private static IEnumerable<RentalResponseDto> SortRentals(IEnumerable<RentalResponseDto> rentals, string sortOrder)
        {
            return sortOrder == "date_asc"
                ? rentals.OrderBy(rental => rental.RentalDate)
                : rentals.OrderByDescending(rental => rental.RentalDate);
        }
    }
}
