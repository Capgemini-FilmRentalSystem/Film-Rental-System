using FilmRentalStore.MVC.DTOs.Language;

namespace FilmRentalStore.MVC.ViewModels.Language
{
    public class LanguageIndexViewModel
    {
        public List<LanguageResponseDto> Languages { get; set; } = new List<LanguageResponseDto>();
    }
}
