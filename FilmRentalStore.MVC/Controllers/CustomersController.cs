using FilmRentalStore.MVC.DTOs.Customers;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Customer;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.MVC.Controllers
{
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

        // GET: Customers/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var viewModel = new CustomerCreateEditViewModel();
                // TODO: Populate stores from IStoreApiService
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
        public async Task<IActionResult> Create(CustomerCreateEditViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                var dto = viewModel.ToRequestDto();
                var result = await _customerService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating customer: {ex.Message}");
                return View(viewModel);
            }
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                if (customer == null)
                    return NotFound();

                var viewModel = CustomerCreateEditViewModel.FromResponseDto(customer);
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
        public async Task<IActionResult> Edit(int id, CustomerCreateEditViewModel viewModel)
        {
            if (id != viewModel.CustomerId)
                return BadRequest();

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                var dto = viewModel.ToRequestDto();
                var result = await _customerService.UpdateAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating customer: {ex.Message}");
                return View(viewModel);
            }
        }

        // POST: Customers/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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
    }
}
