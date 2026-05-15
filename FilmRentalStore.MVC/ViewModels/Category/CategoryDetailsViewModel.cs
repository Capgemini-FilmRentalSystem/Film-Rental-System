using FilmRentalStore.MVC.DTOs.Category;
using FilmRentalStore.MVC.DTOs.Film;

namespace FilmRentalStore.MVC.ViewModels.Category
{
    public class CategoryDetailsViewModel
    {
        public CategoryResponseDto Category { get; set; } = null!;
        public List<FilmResponseDto> Films { get; set; } = new();
    }
}
