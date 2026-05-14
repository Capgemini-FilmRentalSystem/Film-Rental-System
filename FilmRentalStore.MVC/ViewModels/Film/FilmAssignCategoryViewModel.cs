using FilmRentalStore.MVC.DTOs.Category;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.ViewModels.Film
{
    public class FilmAssignCategoryViewModel
    {
        public int FilmId { get; set; }
        public string FilmTitle { get; set; } = null!;
        public byte SelectedCategoryId { get; set; }
        public List<SelectListItem> Categories { get; set; } = new();
        public List<CategoryResponseDto> AssignedCategories { get; set; } = new();
    }
}