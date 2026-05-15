using FilmRentalStore.MVC.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.ViewModels.Film
{
    public class FilmAssignActorViewModel
    {
        public int FilmId { get; set; }
        public string FilmTitle { get; set; } = null!;
        public List<int> SelectedActorIds { get; set; } = new();
        public List<SelectListItem> Actors { get; set; } = new();
        public List<ActorSummaryDto> AssignedActors { get; set; } = new();
    }
}
