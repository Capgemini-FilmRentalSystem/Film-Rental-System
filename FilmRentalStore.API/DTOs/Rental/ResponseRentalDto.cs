namespace FilmRentalStore.API.DTOs.Rental
{
    public class RentalResponseDto
    {
        public int RentalId { get; set; }

        public DateTime RentalDate { get; set; }

        public int InventoryId { get; set; }

        public string FilmTitle { get; set; } = null!;

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = null!;

        public byte StaffId { get; set; }

        public string StaffName { get; set; } = null!;

        public DateTime? ReturnDate { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}