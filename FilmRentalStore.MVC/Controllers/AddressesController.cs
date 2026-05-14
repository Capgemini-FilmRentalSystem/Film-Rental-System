using FilmRentalStore.MVC.DTOs.Address;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Address;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.MVC.Controllers
{
    public class AddressesController : Controller
    {
        private readonly IAddressApiService _addressService;

        public AddressesController(IAddressApiService addressService)
        {
            _addressService = addressService;
        }

        // GET: Addresses
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                var addresses = await _addressService.GetAllAsync(page, pageSize);
                var viewModel = new AddressIndexViewModel
                {
                    Addresses = addresses,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalAddresses = addresses.Count
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading addresses: {ex.Message}");
                return View(new AddressIndexViewModel());
            }
        }

        // GET: Addresses/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var address = await _addressService.GetByIdAsync(id);
                if (address == null)
                    return NotFound();

                var viewModel = new AddressDetailsViewModel { Address = address };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading address: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Addresses/Create
        public IActionResult Create()
        {
            return View(new AddressCreateEditViewModel());
        }

        // POST: Addresses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddressCreateEditViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                var dto = viewModel.ToRequestDto();
                var result = await _addressService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating address: {ex.Message}");
                return View(viewModel);
            }
        }

        // GET: Addresses/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var address = await _addressService.GetByIdAsync(id);
                if (address == null)
                    return NotFound();

                var viewModel = AddressCreateEditViewModel.FromResponseDto(address);
                viewModel.AddressId = id;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading address: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Addresses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AddressCreateEditViewModel viewModel)
        {
            if (id != viewModel.AddressId)
                return BadRequest();

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                var dto = viewModel.ToRequestDto();
                var result = await _addressService.UpdateAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating address: {ex.Message}");
                return View(viewModel);
            }
        }

        // POST: Addresses/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _addressService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error deleting address: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
