using FilmRentalStore.MVC.DTOs;

namespace FilmRentalStore.MVC.DTOs.Inventory
{
    public class InventoryResponseDto
    {
        public int InventoryId { get; set; }

        public FilmSummaryDto Film { get; set; } = null!;

        public DateTime LastUpdate { get; set; }

        public bool IsAvailable { get; set; }
    }
}
