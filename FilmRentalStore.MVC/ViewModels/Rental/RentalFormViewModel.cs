using FilmRentalStore.MVC.DTOs.Rental;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.ViewModels.Rental
{
    public class RentalFormViewModel
    {
        public RentalRequestDto Rental { get; set; } = new();
        public List<SelectListItem> InventoryItems { get; set; } = new();
        public List<SelectListItem> Customers { get; set; } = new();
        public List<SelectListItem> Staff { get; set; } = new();
    }
}
