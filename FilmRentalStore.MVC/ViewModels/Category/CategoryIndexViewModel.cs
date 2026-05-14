using FilmRentalStore.MVC.DTOs.Category;

namespace FilmRentalStore.MVC.ViewModels.Category
{
    public class CategoryIndexViewModel
    {
        public List<CategoryResponseDto> Categories { get; set; } = new();
    }
}