using FilmRentalStore.MVC.DTOs.Category;
using FilmRentalStore.MVC.Filters;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.MVC.Controllers
{
    [TokenRequired]
    [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff, RoleConstants.Customer)]
    public class CategoriesController : Controller
    {
        private readonly ICategoryApiService _categoryService;
        private readonly IFilmApiService _filmService;

        public CategoriesController(ICategoryApiService categoryService, IFilmApiService filmService)
        {
            _categoryService = categoryService;
            _filmService = filmService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var vm = new CategoryIndexViewModel { Categories = categories };
            return View(vm);
        }

        public async Task<IActionResult> Details(byte id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            var films = await GetAllFilmsForLookupAsync();
            var vm = new CategoryDetailsViewModel
            {
                Category = category,
                Films = films
                    .Where(film => film.CategoryIds.Contains(id) || film.Categories.Any(c => c.CategoryId == id))
                    .OrderBy(film => film.Title)
                    .ToList()
            };

            return View(vm);
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public IActionResult Create()
        {
            return View(new CategoryFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Create(CategoryFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            await _categoryService.CreateCategoryAsync(vm.Category);
            TempData["Success"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Edit(byte id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            var vm = new CategoryFormViewModel
            {
                CategoryId = id,
                Category = new CategoryRequestDto { Name = category.Name }
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Edit(byte id, CategoryFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            await _categoryService.UpdateCategoryAsync(id, vm.Category);
            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<FilmRentalStore.MVC.DTOs.Film.FilmResponseDto>> GetAllFilmsForLookupAsync()
        {
            const int pageSize = 100;
            var page = 1;
            var films = new List<FilmRentalStore.MVC.DTOs.Film.FilmResponseDto>();

            while (true)
            {
                var batch = await _filmService.GetAllFilmsAsync(page, pageSize);
                films.AddRange(batch);

                if (batch.Count < pageSize)
                {
                    return films;
                }

                page++;
            }
        }
    }
}
