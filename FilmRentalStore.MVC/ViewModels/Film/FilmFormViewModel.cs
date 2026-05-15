using FilmRentalStore.MVC.DTOs.Film;
using FilmRentalStore.MVC.DTOs.Language;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.ViewModels.Film
{
    public class FilmFormViewModel
    {
        public FilmRequestDto Film { get; set; } = new();

        // Dropdowns
        public List<SelectListItem> Languages { get; set; } = new();
        public List<SelectListItem> OriginalLanguages { get; set; } = new();
        public List<SelectListItem> Actors { get; set; } = new();
        public List<SelectListItem> Categories { get; set; } = new();

        public List<string> RatingOptions { get; set; } = new()
        {
            "G", "PG", "PG-13", "R", "NC-17"
        };

        public List<string> SpecialFeatureOptions { get; set; } = new()
        {
            "Trailers", "Commentaries", "Deleted Scenes", "Behind the Scenes"
        };

        // Selected special features (multi-select helper)
        public List<string> SelectedSpecialFeatures { get; set; } = new();
        public List<int> SelectedActorIds { get; set; } = new();
        public List<byte> SelectedCategoryIds { get; set; } = new();
    }
}
