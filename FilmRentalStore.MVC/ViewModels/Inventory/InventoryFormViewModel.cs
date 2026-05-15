using FilmRentalStore.MVC.DTOs.Inventory;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FilmRentalStore.MVC.ViewModels.Inventory
{
    public class InventoryFormViewModel
    {
        public InventoryRequestDto Inventory { get; set; } = new();
        public List<SelectListItem> Films { get; set; } = new();
        public List<SelectListItem> Stores { get; set; } = new();
    }
}
