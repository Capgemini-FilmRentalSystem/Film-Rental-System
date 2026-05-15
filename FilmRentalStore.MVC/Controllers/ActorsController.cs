using FilmRentalStore.MVC.DTOs.Actor;
using FilmRentalStore.MVC.Filters;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.MVC.Controllers
{
    [TokenRequired]
    [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Staff)]
    public class ActorsController : Controller
    {
        private readonly IActorApiService _actorService;
        private readonly IFilmApiService _filmService;

        public ActorsController(IActorApiService actorService, IFilmApiService filmService)
        {
            _actorService = actorService;
            _filmService = filmService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _actorService.GetAllAsync());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading actors: {ex.Message}";
                return View(new List<ActorResponseDto>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var actor = await _actorService.GetByIdAsync(id);
            if (actor == null) return NotFound();

            var films = await GetAllFilmsForLookupAsync();
            ViewBag.Films = films
                .Where(film => film.ActorIds.Contains(id) || film.Actors.Any(a => a.ActorId == id))
                .OrderBy(film => film.Title)
                .ToList();

            return View(actor);
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public IActionResult Create()
        {
            return View(new ActorRequestDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Create(ActorRequestDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            await _actorService.CreateAsync(dto);
            TempData["Success"] = "Actor created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _actorService.GetByIdAsync(id);
            if (actor == null) return NotFound();

            ViewBag.ActorId = id;
            return View(new ActorRequestDto
            {
                FirstName = actor.FirstName,
                LastName = actor.LastName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(RoleConstants.Admin, RoleConstants.Manager)]
        public async Task<IActionResult> Edit(int id, ActorRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ActorId = id;
                return View(dto);
            }

            await _actorService.UpdateAsync(id, dto);
            TempData["Success"] = "Actor updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
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
