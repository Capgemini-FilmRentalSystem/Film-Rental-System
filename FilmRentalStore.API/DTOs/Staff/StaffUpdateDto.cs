namespace FilmRentalStore.API.DTOs.Staff
{
    public class StaffUpdateDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int AddressId { get; set; }
        public string? Email { get; set; }
        public bool Active { get; set; } = true;
        public int RoleId { get; set; }
    }
}
