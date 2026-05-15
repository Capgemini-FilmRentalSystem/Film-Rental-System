using FilmRentalStore.MVC.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.ViewModels.Film
{
    public class FilmAssignCategoryViewModel
    {
        public int FilmId { get; set; }
        public string FilmTitle { get; set; } = null!;
        public List<byte> SelectedCategoryIds { get; set; } = new();
        public List<SelectListItem> Categories { get; set; } = new();
        public List<CategorySummaryDto> AssignedCategories { get; set; } = new();
    }
}
