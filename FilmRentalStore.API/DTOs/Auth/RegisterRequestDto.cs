namespace FilmRentalStore.API.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int AddressId { get; set; }
        public string? Email { get; set; }
        public int StoreId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; } = 3;
    }
}
