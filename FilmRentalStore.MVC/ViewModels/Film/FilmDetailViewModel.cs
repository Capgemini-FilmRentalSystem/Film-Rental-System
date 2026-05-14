using FilmRentalStore.MVC.DTOs;
using FilmRentalStore.MVC.DTOs.Film;

namespace FilmRentalStore.MVC.ViewModels.Film
{
    public class FilmDetailViewModel
    {
        public FilmResponseDto Film { get; set; } = null!;
        public List<ActorSummaryDto> Actors { get; set; } = new();
        public List<CategorySummaryDto> Categories { get; set; } = new();
    }
}