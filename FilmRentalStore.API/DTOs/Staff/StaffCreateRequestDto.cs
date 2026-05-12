namespace FilmRentalStore.API.DTOs.Staff
{
    public class StaffCreateRequestDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int AddressId { get; set; }
        public string? Email { get; set; }
        public int StoreId { get; set; }
        public bool Active { get; set; } = true;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }
    }
}
