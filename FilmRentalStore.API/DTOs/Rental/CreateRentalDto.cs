namespace FilmRentalStore.API.DTOs.Rental
{
    public class CreateRentalDto
    {
        public int InventoryId { get; set; }
        public int CustomerId { get; set; }
        public byte StaffId { get; set; }
    }
}