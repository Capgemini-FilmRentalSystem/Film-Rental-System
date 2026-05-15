using FilmRentalStore.MVC.DTOs.Film;

namespace FilmRentalStore.MVC.ViewModels.Film
{
    public class FilmIndexViewModel
    {
        public List<FilmResponseDto> Films { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage { get; set; }
        public string? SearchTerm { get; set; }
    }
}
