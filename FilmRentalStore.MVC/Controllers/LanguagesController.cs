using FilmRentalStore.MVC.DTOs.Language;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Language;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.MVC.Controllers
{
    public class LanguagesController : Controller
    {
        private readonly ILanguageApiService _languageService;

        public LanguagesController(ILanguageApiService languageService)
        {
            _languageService = languageService;
        }

        // GET: Languages
        public async Task<IActionResult> Index()
        {
            try
            {
                var languages = await _languageService.GetAllAsync();
                var viewModel = new LanguageIndexViewModel { Languages = languages };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading languages: {ex.Message}");
                return View(new LanguageIndexViewModel());
            }
        }

        // GET: Languages/Details/5
        public async Task<IActionResult> Details(byte id)
        {
            try
            {
                var language = await _languageService.GetByIdAsync(id);
                if (language == null)
                    return NotFound();

                var viewModel = new LanguageDetailsViewModel { Language = language };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading language: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Languages/Create
        public IActionResult Create()
        {
            return View(new LanguageCreateEditViewModel());
        }

        // POST: Languages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LanguageCreateEditViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                var dto = viewModel.ToRequestDto();
                var result = await _languageService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating language: {ex.Message}");
                return View(viewModel);
            }
        }

        // GET: Languages/Edit/5
        public async Task<IActionResult> Edit(byte id)
        {
            try
            {
                var language = await _languageService.GetByIdAsync(id);
                if (language == null)
                    return NotFound();

                var viewModel = LanguageCreateEditViewModel.FromResponseDto(language);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading language: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Languages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, LanguageCreateEditViewModel viewModel)
        {
            if (id != viewModel.LanguageId)
                return BadRequest();

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                var dto = viewModel.ToRequestDto();
                var result = await _languageService.UpdateAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating language: {ex.Message}");
                return View(viewModel);
            }
        }
    }
}
