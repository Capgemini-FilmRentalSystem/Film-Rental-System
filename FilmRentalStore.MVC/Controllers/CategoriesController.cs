using FilmRentalStore.MVC.DTOs.Category;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.MVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryApiService _categoryService;

        public CategoriesController(ICategoryApiService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: /Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var vm = new CategoryIndexViewModel { Categories = categories };
            return View(vm);
        }

        // GET: /Categories/Details/1
        public async Task<IActionResult> Details(byte id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // GET: /Categories/Create
        public IActionResult Create()
        {
            return View(new CategoryFormViewModel());
        }

        // POST: /Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            await _categoryService.CreateCategoryAsync(vm.Category);
            TempData["Success"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Categories/Edit/1
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

        // POST: /Categories/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, CategoryFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            await _categoryService.UpdateCategoryAsync(id, vm.Category);
            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}