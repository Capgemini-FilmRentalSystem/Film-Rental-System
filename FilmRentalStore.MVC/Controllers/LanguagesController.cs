using FilmRentalStore.MVC.Filters;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Language;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.MVC.Controllers
{
    [TokenRequired]
    [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff, RoleConstants.Customer)]
    public class LanguagesController : Controller
    {
        private readonly ILanguageApiService _languageService;
        private readonly IFilmApiService _filmService;

        public LanguagesController(ILanguageApiService languageService, IFilmApiService filmService)
        {
            _languageService = languageService;
            _filmService = filmService;
        }

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

        public async Task<IActionResult> Details(byte id)
        {
            try
            {
                var language = await _languageService.GetByIdAsync(id);
                if (language == null)
                {
                    return NotFound();
                }

                var films = await GetAllFilmsForLookupAsync();
                var viewModel = new LanguageDetailsViewModel
                {
                    Language = language,
                    Films = films
                        .Where(film => film.Language?.LanguageId == id || film.OriginalLanguage?.LanguageId == id)
                        .OrderBy(film => film.Title)
                        .ToList()
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading language: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public IActionResult Create()
        {
            return View(new LanguageCreateEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Create(LanguageCreateEditViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                var dto = viewModel.ToRequestDto();
                await _languageService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating language: {ex.Message}");
                return View(viewModel);
            }
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Edit(byte id)
        {
            try
            {
                var language = await _languageService.GetByIdAsync(id);
                if (language == null)
                {
                    return NotFound();
                }

                var viewModel = LanguageCreateEditViewModel.FromResponseDto(language);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading language: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Edit(byte id, LanguageCreateEditViewModel viewModel)
        {
            if (id != viewModel.LanguageId)
            {
                return BadRequest();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                var dto = viewModel.ToRequestDto();
                await _languageService.UpdateAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating language: {ex.Message}");
                return View(viewModel);
            }
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
