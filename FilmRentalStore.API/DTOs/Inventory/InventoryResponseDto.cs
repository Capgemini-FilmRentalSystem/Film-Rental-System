namespace FilmRentalStore.API.DTOs.Inventory
{
    public class InventoryResponseDto
    {
        public int InventoryId { get; set; }

        public int FilmId { get; set; }

        public string FilmTitle { get; set; } = null!;

        public int StoreId { get; set; }

        public DateTime LastUpdate { get; set; }

        public bool IsAvailable { get; set; }
    }
}