using FilmRentalStore.MVC.DTOs.Film;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Film;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.Controllers
{
    public class FilmsController : Controller
    {
        private readonly IFilmApiService _filmService;

        public FilmsController(IFilmApiService filmService)
        {
            _filmService = filmService;
        }

        // GET: /Films?page=1&pageSize=10
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var films = await _filmService.GetAllFilmsAsync(page, pageSize);
            var vm = new FilmIndexViewModel
            {
                Films = films,
                CurrentPage = page,
                PageSize = pageSize,
                HasNextPage = films.Count == pageSize
            };
            return View(vm);
        }

        // GET: /Films/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var film = await _filmService.GetFilmByIdAsync(id);
            if (film == null) return NotFound();

            var vm = new FilmDetailViewModel { Film = film };
            return View(vm);
        }

        // GET: /Films/Create
        public async Task<IActionResult> Create()
        {
            var vm = await BuildFilmFormViewModel();
            return View(vm);
        }

        // POST: /Films/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FilmFormViewModel vm)
        {
            BuildSpecialFeaturesString(vm);

            if (!ModelState.IsValid)
            {
                await RepopulateDropdowns(vm);
                return View(vm);
            }

            await _filmService.CreateFilmAsync(vm.Film);
            TempData["Success"] = "Film created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Films/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var film = await _filmService.GetFilmByIdAsync(id);
            if (film == null) return NotFound();

            var vm = await BuildFilmFormViewModel();
            vm.Film = new FilmRequestDto
            {
                Title = film.Title,
                Description = film.Description,
                ReleaseYear = film.ReleaseYear,
                LanguageId = film.Language?.LanguageId ?? 0,
                OriginalLanguageId = film.OriginalLanguage?.LanguageId,
                RentalDuration = film.RentalDuration,
                RentalRate = film.RentalRate,
                Length = film.Length,
                ReplacementCost = film.ReplacementCost,
                Rating = film.Rating,
                SpecialFeatures = film.SpecialFeatures
            };

            if (!string.IsNullOrEmpty(film.SpecialFeatures))
                vm.SelectedSpecialFeatures = film.SpecialFeatures
                    .Split(',').Select(s => s.Trim()).ToList();

            ViewBag.FilmId = id;
            return View(vm);
        }

        // POST: /Films/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FilmFormViewModel vm)
        {
            BuildSpecialFeaturesString(vm);

            if (!ModelState.IsValid)
            {
                await RepopulateDropdowns(vm);
                ViewBag.FilmId = id;
                return View(vm);
            }

            await _filmService.UpdateFilmAsync(id, vm.Film);
            TempData["Success"] = "Film updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: /Films/AssignActor/5
        public async Task<IActionResult> AssignActor(int id)
        {
            var film = await _filmService.GetFilmByIdAsync(id);
            if (film == null) return NotFound();

            var allActors = await _filmService.GetActorsAsync();

            var vm = new FilmAssignActorViewModel
            {
                FilmId = id,
                FilmTitle = film.Title,
                Actors = allActors.Select(a => new SelectListItem
                {
                    Value = a.ActorId.ToString(),
                    Text = a.FullName
                }).ToList()
            };

            return View(vm);
        }

        // POST: /Films/AssignActor/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignActor(int id, FilmAssignActorViewModel vm)
        {
            await _filmService.AssignActorAsync(id, new FilmActorAssignRequestDto
            {
                ActorId = vm.SelectedActorId
            });
            TempData["Success"] = "Actor assigned successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: /Films/RemoveActor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveActor(int filmId, int actorId)
        {
            await _filmService.RemoveActorAsync(filmId, actorId);
            TempData["Success"] = "Actor removed successfully.";
            return RedirectToAction(nameof(Details), new { id = filmId });
        }

        // GET: /Films/AssignCategory/5
        public async Task<IActionResult> AssignCategory(int id)
        {
            var film = await _filmService.GetFilmByIdAsync(id);
            if (film == null) return NotFound();

            var allCategories = await _filmService.GetCategoriesAsync();

            var vm = new FilmAssignCategoryViewModel
            {
                FilmId = id,
                FilmTitle = film.Title,
                Categories = allCategories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(vm);
        }

        // POST: /Films/AssignCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignCategory(int id, FilmAssignCategoryViewModel vm)
        {
            await _filmService.AssignCategoryAsync(id, new FilmCategoryAssignRequestDto
            {
                CategoryId = vm.SelectedCategoryId
            });
            TempData["Success"] = "Category assigned successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: /Films/RemoveCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCategory(int filmId, byte categoryId)
        {
            await _filmService.RemoveCategoryAsync(filmId, categoryId);
            TempData["Success"] = "Category removed successfully.";
            return RedirectToAction(nameof(Details), new { id = filmId });
        }

        // ── Private Helpers ───────────────────────────────────────────────────

        private async Task<FilmFormViewModel> BuildFilmFormViewModel()
        {
            var languages = await _filmService.GetLanguagesAsync();
            var langItems = languages.Select(l => new SelectListItem
            {
                Value = l.LanguageId.ToString(),
                Text = l.Name
            }).ToList();

            return new FilmFormViewModel
            {
                Languages = langItems,
                OriginalLanguages = new List<SelectListItem>(langItems)
            };
        }

        private async Task RepopulateDropdowns(FilmFormViewModel vm)
        {
            var languages = await _filmService.GetLanguagesAsync();
            vm.Languages = languages.Select(l => new SelectListItem
            {
                Value = l.LanguageId.ToString(),
                Text = l.Name
            }).ToList();
            vm.OriginalLanguages = new List<SelectListItem>(vm.Languages);
        }

        private static void BuildSpecialFeaturesString(FilmFormViewModel vm)
        {
            if (vm.SelectedSpecialFeatures != null && vm.SelectedSpecialFeatures.Any())
                vm.Film.SpecialFeatures = string.Join(", ", vm.SelectedSpecialFeatures);
            else
                vm.Film.SpecialFeatures = null;
        }
    }
}