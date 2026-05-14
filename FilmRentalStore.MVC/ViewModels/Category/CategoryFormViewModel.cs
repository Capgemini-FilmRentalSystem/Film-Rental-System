using FilmRentalStore.MVC.DTOs.Category;

namespace FilmRentalStore.MVC.ViewModels.Category
{
    public class CategoryFormViewModel
    {
        public byte CategoryId { get; set; }  // 0 = create
        public CategoryRequestDto Category { get; set; } = new();
    }
}