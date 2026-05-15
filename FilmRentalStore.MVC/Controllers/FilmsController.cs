using FilmRentalStore.MVC.DTOs.Film;
using FilmRentalStore.MVC.Filters;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Film;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.Controllers
{
    [TokenRequired]
    [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff, RoleConstants.Customer)]
    public class FilmsController : Controller
    {
        private readonly IFilmApiService _filmService;
        private readonly IInventoryApiService _inventoryService;
        private const int LookupPageSize = 100;

        public FilmsController(IFilmApiService filmService, IInventoryApiService inventoryService)
        {
            _filmService = filmService;
            _inventoryService = inventoryService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? searchTerm = null)
        {
            var scopedStoreId = GetScopedStoreId();
            var needsLocalFilter = scopedStoreId.HasValue || !string.IsNullOrWhiteSpace(searchTerm);

            var allFilms = needsLocalFilter
                ? await GetAllFilmsForLookupAsync()
                : await _filmService.GetAllFilmsAsync(page, pageSize);

            if (scopedStoreId.HasValue)
            {
                var filmIds = await GetFilmIdsForStoreAsync(scopedStoreId.Value);
                allFilms = allFilms.Where(film => filmIds.Contains(film.FilmId)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                allFilms = FilterFilms(allFilms, searchTerm).ToList();
            }

            var films = needsLocalFilter
                ? allFilms
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
                : allFilms;

            var vm = new FilmIndexViewModel
            {
                Films = films,
                CurrentPage = page,
                PageSize = pageSize,
                HasNextPage = needsLocalFilter ? page * pageSize < allFilms.Count : films.Count == pageSize,
                SearchTerm = searchTerm
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var film = await _filmService.GetFilmByIdAsync(id);
            if (film == null) return NotFound();

            var scopedStoreId = GetScopedStoreId();
            if (scopedStoreId.HasValue)
            {
                var filmIds = await GetFilmIdsForStoreAsync(scopedStoreId.Value);
                if (!filmIds.Contains(id)) return NotFound();
            }

            var vm = new FilmDetailViewModel
            {
                Film = film,
                Actors = film.Actors,
                Categories = film.Categories
            };
            return View(vm);
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Create()
        {
            var vm = await BuildFilmFormViewModel();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Create(FilmFormViewModel vm)
        {
            BuildSpecialFeaturesString(vm);
            ApplySelectedRelationships(vm);

            if (!ModelState.IsValid)
            {
                await RepopulateDropdowns(vm);
                return View(vm);
            }

            await _filmService.CreateFilmAsync(vm.Film);
            TempData["Success"] = "Film created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Edit(int id)
        {
            var film = await _filmService.GetFilmByIdAsync(id);
            if (film == null) return NotFound();
            if (!await CanAccessFilmAsync(id)) return NotFound();

            var vm = await BuildFilmFormViewModel();
            vm.Film = new FilmRequestDto
            {
                Title = film.Title,
                Description = film.Description,
                ReleaseYear = film.ReleaseYear,
                LanguageId = FindLanguageId(vm.Languages, film.Language?.Name),
                OriginalLanguageId = FindOptionalLanguageId(vm.OriginalLanguages, film.OriginalLanguage?.Name),
                RentalDuration = film.RentalDuration,
                RentalRate = film.RentalRate,
                Length = film.Length,
                ReplacementCost = film.ReplacementCost,
                Rating = film.Rating,
                SpecialFeatures = film.SpecialFeatures,
                ActorIds = film.ActorIds,
                CategoryIds = film.CategoryIds
            };
            vm.SelectedActorIds = film.ActorIds.ToList();
            vm.SelectedCategoryIds = film.CategoryIds.ToList();

            if (!string.IsNullOrEmpty(film.SpecialFeatures))
            {
                vm.SelectedSpecialFeatures = film.SpecialFeatures
                    .Split(',')
                    .Select(feature => feature.Trim())
                    .ToList();
            }

            ViewBag.FilmId = id;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Edit(int id, FilmFormViewModel vm)
        {
            if (!await CanAccessFilmAsync(id)) return NotFound();

            BuildSpecialFeaturesString(vm);
            ApplySelectedRelationships(vm);

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

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> AssignActor(int id)
        {
            var film = await _filmService.GetFilmByIdAsync(id);
            if (film == null) return NotFound();
            if (!await CanAccessFilmAsync(id)) return NotFound();

            var allActors = await _filmService.GetActorsAsync();
            var assignedActorIds = film.ActorIds.ToHashSet();

            var vm = new FilmAssignActorViewModel
            {
                FilmId = id,
                FilmTitle = film.Title,
                AssignedActors = film.Actors,
                Actors = allActors
                    .Where(actor => !assignedActorIds.Contains(actor.ActorId))
                    .Select(actor => new SelectListItem
                    {
                        Value = actor.ActorId.ToString(),
                        Text = actor.FullName
                    }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> AssignActor(int id, FilmAssignActorViewModel vm)
        {
            var film = await _filmService.GetFilmByIdAsync(id);
            if (film == null) return NotFound();
            if (!await CanAccessFilmAsync(id)) return NotFound();

            var assignedActorIds = film.ActorIds.ToHashSet();
            foreach (var actorId in vm.SelectedActorIds.Distinct().Where(actorId => !assignedActorIds.Contains(actorId)))
            {
                await _filmService.AssignActorAsync(id, new FilmActorAssignRequestDto
                {
                    ActorId = actorId
                });
            }

            TempData["Success"] = "Actors assigned successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> RemoveActor(int filmId, int actorId)
        {
            if (!await CanAccessFilmAsync(filmId)) return NotFound();

            await _filmService.RemoveActorAsync(filmId, actorId);
            TempData["Success"] = "Actor removed successfully.";
            return RedirectToAction(nameof(Details), new { id = filmId });
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> AssignCategory(int id)
        {
            var film = await _filmService.GetFilmByIdAsync(id);
            if (film == null) return NotFound();
            if (!await CanAccessFilmAsync(id)) return NotFound();

            var allCategories = await _filmService.GetCategoriesAsync();
            var assignedCategoryIds = film.CategoryIds.ToHashSet();

            var vm = new FilmAssignCategoryViewModel
            {
                FilmId = id,
                FilmTitle = film.Title,
                AssignedCategories = film.Categories,
                Categories = allCategories
                    .Where(category => !assignedCategoryIds.Contains(category.CategoryId))
                    .Select(category => new SelectListItem
                    {
                        Value = category.CategoryId.ToString(),
                        Text = category.Name
                    }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> AssignCategory(int id, FilmAssignCategoryViewModel vm)
        {
            var film = await _filmService.GetFilmByIdAsync(id);
            if (film == null) return NotFound();
            if (!await CanAccessFilmAsync(id)) return NotFound();

            var assignedCategoryIds = film.CategoryIds.ToHashSet();
            foreach (var categoryId in vm.SelectedCategoryIds.Distinct().Where(categoryId => !assignedCategoryIds.Contains(categoryId)))
            {
                await _filmService.AssignCategoryAsync(id, new FilmCategoryAssignRequestDto
                {
                    CategoryId = categoryId
                });
            }

            TempData["Success"] = "Categories assigned successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> RemoveCategory(int filmId, byte categoryId)
        {
            if (!await CanAccessFilmAsync(filmId)) return NotFound();

            await _filmService.RemoveCategoryAsync(filmId, categoryId);
            TempData["Success"] = "Category removed successfully.";
            return RedirectToAction(nameof(Details), new { id = filmId });
        }

        private async Task<FilmFormViewModel> BuildFilmFormViewModel()
        {
            var languages = await _filmService.GetLanguagesAsync();
            var languageItems = languages.Select(language => new SelectListItem
            {
                Value = language.LanguageId.ToString(),
                Text = language.Name
            }).ToList();

            var actors = await _filmService.GetActorsAsync();
            var categories = await _filmService.GetCategoriesAsync();

            return new FilmFormViewModel
            {
                Languages = languageItems,
                OriginalLanguages = new List<SelectListItem>(languageItems),
                Actors = actors.Select(actor => new SelectListItem
                {
                    Value = actor.ActorId.ToString(),
                    Text = actor.FullName
                }).ToList(),
                Categories = categories.Select(category => new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.Name
                }).ToList()
            };
        }

        private async Task RepopulateDropdowns(FilmFormViewModel vm)
        {
            var languages = await _filmService.GetLanguagesAsync();
            vm.Languages = languages.Select(language => new SelectListItem
            {
                Value = language.LanguageId.ToString(),
                Text = language.Name
            }).ToList();
            vm.OriginalLanguages = new List<SelectListItem>(vm.Languages);

            var actors = await _filmService.GetActorsAsync();
            vm.Actors = actors.Select(actor => new SelectListItem
            {
                Value = actor.ActorId.ToString(),
                Text = actor.FullName,
                Selected = vm.SelectedActorIds.Contains(actor.ActorId)
            }).ToList();

            var categories = await _filmService.GetCategoriesAsync();
            vm.Categories = categories.Select(category => new SelectListItem
            {
                Value = category.CategoryId.ToString(),
                Text = category.Name,
                Selected = vm.SelectedCategoryIds.Contains(category.CategoryId)
            }).ToList();
        }

        private static void BuildSpecialFeaturesString(FilmFormViewModel vm)
        {
            vm.Film.SpecialFeatures = vm.SelectedSpecialFeatures != null && vm.SelectedSpecialFeatures.Any()
                ? string.Join(", ", vm.SelectedSpecialFeatures)
                : null;
        }

        private static void ApplySelectedRelationships(FilmFormViewModel vm)
        {
            vm.Film.ActorIds = vm.SelectedActorIds?.Distinct().ToList() ?? new List<int>();
            vm.Film.CategoryIds = vm.SelectedCategoryIds?.Distinct().ToList() ?? new List<byte>();
        }

        private static byte FindLanguageId(IEnumerable<SelectListItem> languages, string? name)
        {
            return FindOptionalLanguageId(languages, name) ?? 0;
        }

        private static byte? FindOptionalLanguageId(IEnumerable<SelectListItem> languages, string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var match = languages.FirstOrDefault(language =>
                string.Equals(language.Text, name, StringComparison.OrdinalIgnoreCase));

            return byte.TryParse(match?.Value, out var id) ? id : null;
        }

        private async Task<List<FilmResponseDto>> GetAllFilmsForLookupAsync()
        {
            var films = new List<FilmResponseDto>();

            for (var page = 1; ; page++)
            {
                var batch = await _filmService.GetAllFilmsAsync(page, LookupPageSize);
                if (!batch.Any())
                {
                    break;
                }

                films.AddRange(batch);

                if (batch.Count < LookupPageSize)
                {
                    break;
                }
            }

            return films;
        }

        private async Task<HashSet<int>> GetFilmIdsForStoreAsync(int storeId)
        {
            var filmIds = new HashSet<int>();

            for (var page = 1; ; page++)
            {
                var batch = await _inventoryService.GetAllAsync(page, LookupPageSize);
                if (!batch.Any())
                {
                    break;
                }

                foreach (var item in batch.Where(item => item.StoreId == storeId))
                {
                    filmIds.Add(item.FilmId);
                }

                if (batch.Count < LookupPageSize)
                {
                    break;
                }
            }

            return filmIds;
        }

        private int? GetScopedStoreId()
        {
            var role = HttpContext.Session.GetString(SessionKeys.Role);
            return role == RoleConstants.Admin ? null : HttpContext.Session.GetInt32(SessionKeys.StoreId);
        }

        private async Task<bool> CanAccessFilmAsync(int filmId)
        {
            var scopedStoreId = GetScopedStoreId();
            if (!scopedStoreId.HasValue)
            {
                return true;
            }

            var filmIds = await GetFilmIdsForStoreAsync(scopedStoreId.Value);
            return filmIds.Contains(filmId);
        }

        private static IEnumerable<FilmResponseDto> FilterFilms(IEnumerable<FilmResponseDto> films, string searchTerm)
        {
            var term = searchTerm.Trim();

            return films.Where(film =>
                Contains(film.Title, term)
                || Contains(film.Description, term)
                || Contains(film.Rating, term)
                || Contains(film.Language?.Name, term)
                || Contains(film.OriginalLanguage?.Name, term)
                || film.Actors.Any(actor => Contains(actor.FullName, term))
                || film.Categories.Any(category => Contains(category.Name, term)));
        }

        private static bool Contains(string? value, string searchTerm)
        {
            return !string.IsNullOrWhiteSpace(value)
                && value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        }
    }
}
