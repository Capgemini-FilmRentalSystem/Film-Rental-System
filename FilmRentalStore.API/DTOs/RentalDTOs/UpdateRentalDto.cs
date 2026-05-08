namespace FilmRentalStore.API.DTOs.RentalDTOs
{
    public class UpdateRentalDto
    {
        public DateTime RentalDate { get; set; }
        public int InventoryId { get; set; }
        public int CustomerId { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int StaffId { get; set; }
    }
}