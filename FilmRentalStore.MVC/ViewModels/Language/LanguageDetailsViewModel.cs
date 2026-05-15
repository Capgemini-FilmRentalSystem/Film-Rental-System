using FilmRentalStore.MVC.DTOs.Language;
using FilmRentalStore.MVC.DTOs.Film;

namespace FilmRentalStore.MVC.ViewModels.Language
{
    public class LanguageDetailsViewModel
    {
        public LanguageResponseDto? Language { get; set; }
        public List<FilmResponseDto> Films { get; set; } = new();
    }
}
