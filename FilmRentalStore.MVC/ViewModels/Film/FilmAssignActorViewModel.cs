using FilmRentalStore.MVC.DTOs.Actor;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.ViewModels.Film
{
    public class FilmAssignActorViewModel
    {
        public int FilmId { get; set; }
        public string FilmTitle { get; set; } = null!;
        public int SelectedActorId { get; set; }
        public List<SelectListItem> Actors { get; set; } = new();
        public List<ActorResponseDto> AssignedActors { get; set; } = new();
    }
}