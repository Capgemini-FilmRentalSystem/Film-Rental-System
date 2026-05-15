using FilmRentalStore.MVC.DTOs.Inventory;
using FilmRentalStore.MVC.Filters;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Inventory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.Controllers
{
    [TokenRequired]
    [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff, RoleConstants.Customer)]
    public class InventoryController : Controller
    {
        private readonly IInventoryApiService _inventoryService;
        private readonly IFilmApiService _filmService;
        private readonly IStoreApiService _storeService;
        private const int InventoryFetchPageSize = 100;

        public InventoryController(
            IInventoryApiService inventoryService,
            IFilmApiService filmService,
            IStoreApiService storeService)
        {
            _inventoryService = inventoryService;
            _filmService = filmService;
            _storeService = storeService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var inventory = await GetAllInventoryForSummaryAsync();
            var groupedInventory = inventory
                .GroupBy(item => new { item.FilmId, item.StoreId })
                .Select(group =>
                {
                    var representative = group
                        .OrderByDescending(item => item.IsAvailable)
                        .ThenBy(item => item.InventoryId)
                        .First();

                    return new InventoryResponseDto
                    {
                        InventoryId = representative.InventoryId,
                        FilmId = group.Key.FilmId,
                        StoreId = group.Key.StoreId,
                        Film = representative.Film,
                        LastUpdate = group.Max(item => item.LastUpdate),
                        IsAvailable = group.Any(item => item.IsAvailable),
                        AvailableCopies = group.Count(item => item.IsAvailable),
                        TotalCopies = group.Count()
                    };
                })
                .OrderBy(item => item.Film?.Title)
                .ThenBy(item => item.StoreId)
                .ToList();

            var items = groupedInventory
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.HasNextPage = page * pageSize < groupedInventory.Count;

            return View(items);
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Create()
        {
            var vm = new InventoryFormViewModel();
            await PopulateInventoryFormOptions(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Create(InventoryFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateInventoryFormOptions(vm);
                return View(vm);
            }

            await _inventoryService.CreateAsync(vm.Inventory);
            TempData["Success"] = "Inventory item created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);
            if (item == null) return NotFound();

            ViewBag.InventoryId = id;
            var vm = new InventoryFormViewModel
            {
                Inventory = new InventoryRequestDto
                {
                    FilmId = item.FilmId,
                    StoreId = item.StoreId
                }
            };
            await PopulateInventoryFormOptions(vm);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Edit(int id, InventoryFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.InventoryId = id;
                await PopulateInventoryFormOptions(vm);
                return View(vm);
            }

            await _inventoryService.UpdateAsync(id, vm.Inventory);
            TempData["Success"] = "Inventory item updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task PopulateInventoryFormOptions(InventoryFormViewModel vm)
        {
            var films = await _filmService.GetAllFilmsAsync(1, 100);
            vm.Films = films.Select(film => new SelectListItem
            {
                Value = film.FilmId.ToString(),
                Text = film.Title,
                Selected = film.FilmId == vm.Inventory.FilmId
            }).ToList();

            var stores = await _storeService.GetAllAsync();
            vm.Stores = stores.Select(store => new SelectListItem
            {
                Value = store.StoreId.ToString(),
                Text = FormatStoreOption(store),
                Selected = store.StoreId == vm.Inventory.StoreId
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

        private async Task<List<InventoryResponseDto>> GetAllInventoryForSummaryAsync()
        {
            var items = new List<InventoryResponseDto>();

            for (var page = 1; ; page++)
            {
                var batch = await _inventoryService.GetAllAsync(page, InventoryFetchPageSize);
                if (!batch.Any())
                {
                    break;
                }

                items.AddRange(batch);

                if (batch.Count < InventoryFetchPageSize)
                {
                    break;
                }
            }

            return items;
        }
    }
}
