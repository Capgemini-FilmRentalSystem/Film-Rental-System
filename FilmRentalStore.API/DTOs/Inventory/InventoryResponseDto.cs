using FilmRentalStore.API.DTOs;

namespace FilmRentalStore.API.DTOs.Inventory
{
    public class InventoryResponseDto
    {
        public int InventoryId { get; set; }

        public int FilmId { get; set; }

        public int StoreId { get; set; }

        public FilmSummaryDto Film { get; set; } = null!;

        public DateTime LastUpdate { get; set; }

        public bool IsAvailable { get; set; }
    }
}
