using FilmRentalStore.MVC.DTOs.Customers;
using FilmRentalStore.MVC.Filters;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Customer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.Controllers
{
    [TokenRequired]
    public class CustomersController : Controller
    {
        private readonly ICustomerApiService _customerService;
        private readonly IStoreApiService _storeService;

        public CustomersController(ICustomerApiService customerService, IStoreApiService storeService)
        {
            _customerService = customerService;
            _storeService = storeService;
        }

        // GET: Customers
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? searchName = null, string? searchEmail = null)
        {
            try
            {
                List<CustomerResponseDto> customers;

                if (!string.IsNullOrEmpty(searchName) || !string.IsNullOrEmpty(searchEmail))
                {
                    customers = await _customerService.SearchAsync(searchName, searchEmail);
                }
                else
                {
                    customers = await _customerService.GetAllAsync(page, pageSize);
                }

                var viewModel = new CustomerIndexViewModel
                {
                    Customers = customers,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCustomers = customers.Count,
                    SearchName = searchName,
                    SearchEmail = searchEmail
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading customers: {ex.Message}");
                return View(new CustomerIndexViewModel());
            }
        }

        // GET: Customers/Details/5
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                if (customer == null)
                    return NotFound();

                var viewModel = new CustomerDetailsViewModel { Customer = customer };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading customer: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        [RoleAuthorize(RoleConstants.Customer)]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var customer = await _customerService.GetMeAsync();
                var viewModel = new CustomerDetailsViewModel { Customer = customer };
                ViewBag.IsProfile = true;

                return View("Details", viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading profile: {ex.Message}");
                return RedirectToAction("Customer", "Dashboard");
            }
        }

        // GET: Customers/Create
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Create()
        {
            try
            {
                var viewModel = new CustomerCreateEditViewModel();
                await PopulateStores(viewModel);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading form: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Create(CustomerCreateEditViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await PopulateStores(viewModel);
                    return View(viewModel);
                }

                if (string.IsNullOrWhiteSpace(viewModel.Password))
                {
                    ModelState.AddModelError(nameof(viewModel.Password), "Password is required.");
                    await PopulateStores(viewModel);
                    return View(viewModel);
                }

                var dto = viewModel.ToRequestDto();
                var result = await _customerService.CreateAsync(dto);
                TempData["Success"] = "Customer created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating customer: {ex.Message}");
                await PopulateStores(viewModel);
                return View(viewModel);
            }
        }

        // GET: Customers/Edit/5
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                if (customer == null)
                    return NotFound();

                var viewModel = CustomerCreateEditViewModel.FromResponseDto(customer);
                await PopulateStores(viewModel);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading customer: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Edit(int id, CustomerCreateEditViewModel viewModel)
        {
            if (id != viewModel.CustomerId)
                return BadRequest();

            try
            {
                if (!ModelState.IsValid)
                {
                    await PopulateStores(viewModel);
                    return View(viewModel);
                }

                var dto = viewModel.ToRequestDto();
                var result = await _customerService.UpdateAsync(id, dto);
                TempData["Success"] = "Customer updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating customer: {ex.Message}");
                await PopulateStores(viewModel);
                return View(viewModel);
            }
        }

        // POST: Customers/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                await _customerService.ActivateAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error activating customer: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Customers/Deactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                await _customerService.DeactivateAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error deactivating customer: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task PopulateStores(CustomerCreateEditViewModel viewModel)
        {
            var stores = await _storeService.GetAllAsync();
            viewModel.Stores = stores.Select(store => new SelectListItem
            {
                Value = store.StoreId.ToString(),
                Text = FormatStoreOption(store),
                Selected = store.StoreId == viewModel.StoreId
            }).ToList();
        }

        private static string FormatStoreOption(FilmRentalStore.MVC.DTOs.Store.StoreResponseDto store)
        {
            var location = store.Address == null
                ? string.Empty
                : string.Join(", ", new[] { store.Address.City, store.Address.Country }
                    .Where(part => !string.IsNullOrWhiteSpace(part)));

            return string.IsNullOrWhiteSpace(location)
                ? $"Store #{store.StoreId}"
                : $"Store #{store.StoreId} - {location}";
        }
    }
}
