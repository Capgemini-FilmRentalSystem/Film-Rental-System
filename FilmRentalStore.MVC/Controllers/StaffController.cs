using FilmRentalStore.MVC.DTOs.Staff;
using FilmRentalStore.MVC.Filters;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Staff;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.Controllers
{
    [TokenRequired]
    [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
    public class StaffController : Controller
    {
        private readonly IStaffApiService _staffService;
        private readonly IStoreApiService _storeService;

        public StaffController(IStaffApiService staffService, IStoreApiService storeService)
        {
            _staffService = staffService;
            _storeService = storeService;
        }

        public async Task<IActionResult> Index(byte? id)
        {
            var staff = new List<StaffResponseDto>();

            if (id.HasValue)
            {
                var result = await _staffService.GetByIdAsync(id.Value);
                if (result != null)
                {
                    staff.Add(result);
                }
            }
            else
            {
                staff = await _staffService.GetAllAsync();
            }

            ViewBag.SearchId = id;
            return View(staff);
        }

        public async Task<IActionResult> Details(byte id)
        {
            var staff = await _staffService.GetByIdAsync(id);
            if (staff == null) return NotFound();

            return View(staff);
        }

        [RoleAuthorize(RoleConstants.Admin)]
        public async Task<IActionResult> Create()
        {
            var vm = new StaffFormViewModel
            {
                Active = true,
                RoleId = 3
            };

            await PopulateStaffFormOptions(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin)]
        public async Task<IActionResult> Create(StaffFormViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Password))
            {
                ModelState.AddModelError(nameof(vm.Password), "Password is required.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateStaffFormOptions(vm);
                return View(vm);
            }

            await _staffService.CreateAsync(vm.ToCreateDto());
            TempData["Success"] = "Staff member created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [RoleAuthorize(RoleConstants.Admin)]
        public async Task<IActionResult> Edit(byte id)
        {
            var staff = await _staffService.GetByIdAsync(id);
            if (staff == null) return NotFound();

            var vm = StaffFormViewModel.FromResponseDto(staff);
            await PopulateStaffFormOptions(vm);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin)]
        public async Task<IActionResult> Edit(byte id, StaffFormViewModel vm)
        {
            if (id != vm.StaffId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await PopulateStaffFormOptions(vm);
                return View(vm);
            }

            await _staffService.UpdateAsync(id, vm.ToUpdateDto());
            TempData["Success"] = "Staff member updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin)]
        public async Task<IActionResult> Deactivate(byte id)
        {
            await _staffService.DeactivateAsync(id);
            TempData["Success"] = "Staff member deactivated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateStaffFormOptions(StaffFormViewModel vm)
        {
            vm.Roles = BuildRoleItems(vm.RoleId);
            var stores = await _storeService.GetAllAsync();
            vm.Stores = stores.Select(store => new SelectListItem
            {
                Value = store.StoreId.ToString(),
                Text = FormatStoreOption(store),
                Selected = store.StoreId == vm.StoreId
            }).ToList();
        }

        private static List<SelectListItem> BuildRoleItems(int selectedRoleId)
        {
            return new List<SelectListItem>
            {
                new() { Value = "1", Text = RoleConstants.Admin, Selected = selectedRoleId == 1 },
                new() { Value = "2", Text = RoleConstants.Manager, Selected = selectedRoleId == 2 },
                new() { Value = "3", Text = RoleConstants.Staff, Selected = selectedRoleId == 3 }
            };
        }

        private static string FormatStoreOption(FilmRentalStore.MVC.DTOs.Store.StoreResponseDto store)
        {
            var location = store.Address == null
                ? string.Empty
                : string.Join(", ", new[] { store.Address.City, store.Address.Country }
                    .Where(part => !string.IsNullOrWhiteSpace(part)));

            return string.IsNullOrWhiteSpace(location)
                ? "Store"
                : location;
        }
    }
}
