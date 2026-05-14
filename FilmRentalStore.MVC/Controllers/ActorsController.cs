using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Actor;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.MVC.Controllers
{
    public class ActorsController : Controller
    {
        private readonly IActorApiService _actorApiService;

        public ActorsController(IActorApiService actorApiService)
        {
            _actorApiService = actorApiService;
        }

        public async Task<IActionResult> Index()
        {
            var actors = await _actorApiService.GetAllActorsAsync();

            return View(actors);
        }

        public async Task<IActionResult> Details(int id)
        {
            var actor = await _actorApiService.GetActorByIdAsync(id);

            if(actor == null)
                return NotFound();
            return View(actor);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ActorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _actorApiService.CreateActorAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _actorApiService.GetActorByIdAsync(id);
            
            if(actor == null)
                return NotFound();

            return View(actor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ActorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _actorApiService.UpdateActorAsync(id, model);

            return RedirectToAction(nameof(Index));
        }
    }
}