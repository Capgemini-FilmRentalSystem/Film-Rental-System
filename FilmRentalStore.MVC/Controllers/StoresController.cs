using FilmRentalStore.MVC.DTOs.Store;
using FilmRentalStore.MVC.Filters;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.Controllers
{
    [TokenRequired]
    [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
    public class StoresController : Controller
    {
        private readonly IStoreApiService _storeService;
        private readonly IStaffApiService _staffService;

        public StoresController(IStoreApiService storeService, IStaffApiService staffService)
        {
            _storeService = storeService;
            _staffService = staffService;
        }

        public async Task<IActionResult> Index(int? id)
        {
            var stores = new List<StoreResponseDto>();
            var scopedStoreId = GetScopedStoreId();

            if (scopedStoreId.HasValue)
            {
                var result = await _storeService.GetByIdAsync(scopedStoreId.Value);
                if (result != null)
                {
                    stores.Add(result);
                }
            }
            else if (id.HasValue)
            {
                var result = await _storeService.GetByIdAsync(id.Value);
                if (result != null)
                {
                    stores.Add(result);
                }
            }
            else
            {
                stores = await _storeService.GetAllAsync();
            }

            ViewBag.SearchId = id;
            return View(stores);
        }

        public async Task<IActionResult> Details(int id)
        {
            var scopedStoreId = GetScopedStoreId();
            if (scopedStoreId.HasValue && id != scopedStoreId.Value) return NotFound();

            var store = await _storeService.GetByIdAsync(id);
            if (store == null) return NotFound();

            return View(store);
        }

        [RoleAuthorize(RoleConstants.Admin)]
        public async Task<IActionResult> Create()
        {
            var vm = new StoreFormViewModel();
            await PopulateStoreFormOptions(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin)]
        public async Task<IActionResult> Create(StoreFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateStoreFormOptions(vm);
                return View(vm);
            }

            await _storeService.CreateAsync(vm.ToRequestDto());
            TempData["Success"] = "Store created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [RoleAuthorize(RoleConstants.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var store = await _storeService.GetByIdAsync(id);
            if (store == null) return NotFound();

            var vm = StoreFormViewModel.FromResponseDto(store);
            await PopulateStoreFormOptions(vm);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin)]
        public async Task<IActionResult> Edit(int id, StoreFormViewModel vm)
        {
            if (id != vm.StoreId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await PopulateStoreFormOptions(vm);
                return View(vm);
            }

            await _storeService.UpdateAsync(id, vm.ToRequestDto());
            TempData["Success"] = "Store updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task PopulateStoreFormOptions(StoreFormViewModel vm)
        {
            var staff = await _staffService.GetAllAsync();
            vm.Managers = staff
                .Where(member => member.Active && (member.RoleId == 2 || member.Role?.RoleTitle == RoleConstants.Manager))
                .Select(member => new SelectListItem
                {
                    Value = member.StaffId.ToString(),
                    Text = $"{member.FirstName} {member.LastName} ({member.Username})",
                    Selected = member.StaffId == vm.ManagerStaffId
                })
                .ToList();
        }

        private int? GetScopedStoreId()
        {
            var role = HttpContext.Session.GetString(SessionKeys.Role);
            return role == RoleConstants.Admin ? null : HttpContext.Session.GetInt32(SessionKeys.StoreId);
        }
    }
}
