using FilmRentalStore.API.DTOs.Address;

namespace FilmRentalStore.API.DTOs.Customers
{
    public class CustomerRegisterRequestDto
    {
        public int StoreId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public AddressRequestDto Address { get; set; } = null!;
    }
}
