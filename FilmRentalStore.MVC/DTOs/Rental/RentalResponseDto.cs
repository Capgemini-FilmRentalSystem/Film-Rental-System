using FilmRentalStore.MVC.DTOs;

namespace FilmRentalStore.MVC.DTOs.Rental
{
    public class RentalResponseDto
    {
        public int RentalId { get; set; }

        public DateTime RentalDate { get; set; }

        public InventorySummaryDto Inventory { get; set; } = null!;

        public CustomerSummaryDto Customer { get; set; } = null!;

        public DateTime? ReturnDate { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
